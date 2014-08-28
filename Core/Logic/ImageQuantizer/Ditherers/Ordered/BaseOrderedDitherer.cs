using System;
using System.Drawing;
using Core.Logic.ImageQuantizer.Helpers;

namespace Core.Logic.ImageQuantizer.Ditherers.Ordered
{
    public abstract class BaseOrderedDitherer : BaseColorDitherer
    {
        #region | Properties |

        /// <summary>
        ///   Gets the width of the matrix.
        /// </summary>
        protected abstract Byte MatrixWidth { get; }

        /// <summary>
        ///   Gets the height of the matrix.
        /// </summary>
        protected abstract Byte MatrixHeight { get; }

        #endregion

        #region << BaseColorDitherer >>

        /// <summary>
        ///   See <see cref="BaseColorDitherer.OnProcessPixel" /> for more details.
        /// </summary>
        protected override Boolean OnProcessPixel(Pixel sourcePixel, Pixel targetPixel)
        {
            // reads the source pixel
            var oldColor = SourceBuffer.GetColorFromPixel(sourcePixel);

            // converts alpha to solid color
            oldColor = QuantizationHelper.ConvertAlpha(oldColor);

            // retrieves matrix coordinates
            var x = targetPixel.X % MatrixWidth;
            var y = targetPixel.Y % MatrixHeight;

            // determines the threshold
            var threshold = Convert.ToInt32(CachedMatrix[x, y]);

            // only process dithering if threshold is substantial
            if (threshold > 0)
            {
                var red = GetClampedColorElement(oldColor.R + threshold);
                var green = GetClampedColorElement(oldColor.G + threshold);
                var blue = GetClampedColorElement(oldColor.B + threshold);

                var newColor = Color.FromArgb(255, red, green, blue);

                if (TargetBuffer.IsIndexed)
                {
                    var newPixelIndex = (Byte)Quantizer.GetPaletteIndex(newColor, targetPixel.X, targetPixel.Y);
                    targetPixel.Index = newPixelIndex;
                }
                else
                {
                    targetPixel.Color = newColor;
                }
            }

            // writes the process pixel
            return true;
        }

        #endregion

        #region << IColorDitherer >>

        /// <summary>
        ///   See <see cref="IColorDitherer.IsInplace" /> for more details.
        /// </summary>
        public override Boolean IsInplace
        {
            get { return true; }
        }

        #endregion
    }
}