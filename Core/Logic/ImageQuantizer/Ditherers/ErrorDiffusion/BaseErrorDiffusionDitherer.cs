using System;
using System.Drawing;
using Core.Logic.ImageQuantizer.Helpers;

namespace Core.Logic.ImageQuantizer.Ditherers.ErrorDiffusion
{
    public abstract class BaseErrorDistributionDitherer : BaseColorDitherer
    {
        #region | Properties |

        ///<summary>
        ///  Gets the width of the matrix side.
        ///
        ///  center
        ///  v --------> side width = 2
        ///  | 0 | 1 | 2 | 3 | 4 |
        ///</summary>
        protected abstract Int32 MatrixSideWidth { get; }

        /// <summary>
        ///   Gets the height of the matrix side.
        /// 
        ///   --- 
        ///   0  
        ///   --- 
        ///   1  &lt;- center
        ///   --- | 
        ///   2  | side height = 1
        ///   --- v
        /// </summary>
        protected abstract Int32 MatrixSideHeight { get; }

        #endregion

        #region | Methods |

        private void ProcessNeighbor(Pixel targetPixel, Int32 x, Int32 y, Single factor, Int32 redError,
                                     Int32 greenError, Int32 blueError)
        {
            var oldColor = TargetBuffer.ReadColorUsingPixelFrom(targetPixel, x, y);
            oldColor = QuantizationHelper.ConvertAlpha(oldColor);
            var red = GetClampedColorElementWithError(oldColor.R, factor, redError);
            var green = GetClampedColorElementWithError(oldColor.G, factor, greenError);
            var blue = GetClampedColorElementWithError(oldColor.B, factor, blueError);
            var newColor = Color.FromArgb(255, red, green, blue);
            TargetBuffer.WriteColorUsingPixelAt(targetPixel, x, y, newColor, Quantizer);
        }

        #endregion

        #region << BaseColorDitherer >>

        /// <summary>
        ///   See <see cref="BaseColorDitherer.OnProcessPixel" /> for more details.
        /// </summary>
        protected override Boolean OnProcessPixel(Pixel sourcePixel, Pixel targetPixel)
        {
            // only process dithering when reducing from truecolor to indexed
            if (!TargetBuffer.IsIndexed) return false;

            // retrieves the colors
            var sourceColor = SourceBuffer.GetColorFromPixel(sourcePixel);
            var targetColor = TargetBuffer.GetColorFromPixel(targetPixel);

            // converts alpha to solid color
            sourceColor = QuantizationHelper.ConvertAlpha(sourceColor);

            // calculates the difference (error)
            var redError = sourceColor.R - targetColor.R;
            var greenError = sourceColor.G - targetColor.G;
            var blueError = sourceColor.B - targetColor.B;

            // only propagate non-zero error
            if (redError != 0 || greenError != 0 || blueError != 0)
            {
                // processes the matrix
                for (var shiftY = -MatrixSideHeight; shiftY <= MatrixSideHeight; shiftY++)
                    for (var shiftX = -MatrixSideWidth; shiftX <= MatrixSideWidth; shiftX++)
                    {
                        var targetX = sourcePixel.X + shiftX;
                        var targetY = sourcePixel.Y + shiftY;
                        var coeficient = CachedMatrix[shiftY + MatrixSideHeight, shiftX + MatrixSideWidth];
                        var coeficientSummed =
                            CachedSummedMatrix[shiftY + MatrixSideHeight, shiftX + MatrixSideWidth];

                        if (coeficient != 0 &&
                            targetX >= 0 && targetX < TargetBuffer.Width &&
                            targetY >= 0 && targetY < TargetBuffer.Height)
                        {
                            ProcessNeighbor(targetPixel, targetX, targetY, coeficientSummed, redError, greenError,
                                            blueError);
                        }
                    }
            }

            // pixels are not processed, only neighbors are
            return false;
        }

        #endregion

        #region << IColorDitherer >>

        /// <summary>
        ///   See <see cref="IColorDitherer.IsInplace" /> for more details.
        /// </summary>
        public override Boolean IsInplace
        {
            get { return false; }
        }

        #endregion
    }
}