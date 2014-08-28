using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.ImageQuantizer.ColorCaches.Common;
using Core.Logic.ImageQuantizer.Helpers;

namespace Core.Logic.ImageQuantizer.ColorCaches.LocalitySensitiveHash
{
    public class LshColorCache : BaseColorCache
    {
        #region | Constants |

        private const Byte DefaultQuality = 16; // 16
        private const Int64 MaximalDistance = 4096;

        private const Single NormalizedDistanceRgb = 1.0f / 196608.0f; // 256*256*3 (RGB) = 196608 / 768.0f
        private const Single NormalizedDistanceHsl = 1.0f / 260672.0f; // 360*360 (H) + 256*256*2 (SL) = 260672 / 872.0f
        private const Single NormalizedDistanceLab = 1.0f / 507.0f; // 13*13*3 = 507 / 300.0f

        #endregion

        #region | Fields |

        private Int64 bucketSize;
        private BucketInfo[] buckets;
        private Int64 maxBucketIndex;
        private Int64 minBucketIndex;
        private Byte quality;

        #endregion

        #region | Properties |

        /// <summary>
        ///   Gets or sets the quality.
        /// </summary>
        /// <value> The quality. </value>
        public Byte Quality
        {
            get { return quality; }
            set
            {
                quality = value;

                bucketSize = MaximalDistance / quality;
                minBucketIndex = quality;
                maxBucketIndex = 0;

                buckets = new BucketInfo[quality];
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is color model supported.
        /// </summary>
        /// <value> <c>true</c> if this instance is color model supported; otherwise, <c>false</c> . </value>
        public override Boolean IsColorModelSupported
        {
            get { return true; }
        }

        #endregion

        #region | Constructors |

        /// <summary>
        ///   Initializes a new instance of the <see cref="LshColorCache" /> class.
        /// </summary>
        public LshColorCache()
        {
            ColorModel = ColorModel.RedGreenBlue;
            Quality = DefaultQuality;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LshColorCache" /> class.
        /// </summary>
        /// <param name="colorModel"> The color model. </param>
        /// <param name="quality"> The quality. </param>
        public LshColorCache(ColorModel colorModel, Byte quality)
        {
            ColorModel = colorModel;
            Quality = quality;
        }

        #endregion

        #region | Helper methods |

        private Int64 GetColorBucketIndex(Color color)
        {
            var normalizedDistance = 0.0f;
            Single componentA, componentB, componentC;

            switch (ColorModel)
            {
                case ColorModel.RedGreenBlue:
                    normalizedDistance = NormalizedDistanceRgb;
                    break;
                case ColorModel.HueSaturationLuminance:
                    normalizedDistance = NormalizedDistanceHsl;
                    break;
                case ColorModel.LabColorSpace:
                    normalizedDistance = NormalizedDistanceLab;
                    break;
            }

            ColorModelHelper.GetColorComponents(ColorModel, color, out componentA, out componentB, out componentC);
            var distance = componentA * componentA + componentB * componentB + componentC * componentC;
            var normalized = distance * normalizedDistance * MaximalDistance;
            var resultHash = (Int64)normalized / bucketSize;

            return resultHash;
        }

        private BucketInfo GetBucket(Color color)
        {
            var bucketIndex = GetColorBucketIndex(color);

            if (bucketIndex < minBucketIndex)
            {
                bucketIndex = minBucketIndex;
            }
            else if (bucketIndex > maxBucketIndex)
            {
                bucketIndex = maxBucketIndex;
            }
            else if (buckets[bucketIndex] == null)
            {
                var bottomFound = false;
                var topFound = false;
                var bottomBucketIndex = bucketIndex;
                var topBucketIndex = bucketIndex;

                while (!bottomFound && !topFound)
                {
                    bottomBucketIndex--;
                    topBucketIndex++;
                    bottomFound = bottomBucketIndex > 0 && buckets[bottomBucketIndex] != null;
                    topFound = topBucketIndex < quality && buckets[topBucketIndex] != null;
                }

                bucketIndex = bottomFound ? bottomBucketIndex : topBucketIndex;
            }

            return buckets[bucketIndex];
        }

        #endregion

        #region << BaseColorCache >>

        /// <summary>
        ///   See <see cref="BaseColorCache.Prepare" /> for more details.
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();
            buckets = new BucketInfo[quality];
        }

        /// <summary>
        ///   See <see cref="BaseColorCache.OnCachePalette" /> for more details.
        /// </summary>
        protected override void OnCachePalette(IList<Color> palette)
        {
            var paletteIndex = 0;
            minBucketIndex = quality;
            maxBucketIndex = 0;

            foreach (var color in palette)
            {
                var bucketIndex = GetColorBucketIndex(color);
                var bucket = buckets[bucketIndex] ?? new BucketInfo();
                bucket.AddColor(paletteIndex++, color);
                buckets[bucketIndex] = bucket;

                if (bucketIndex < minBucketIndex) minBucketIndex = bucketIndex;
                if (bucketIndex > maxBucketIndex) maxBucketIndex = bucketIndex;
            }
        }

        /// <summary>
        ///   See <see cref="BaseColorCache.OnGetColorPaletteIndex" /> for more details.
        /// </summary>
        protected override void OnGetColorPaletteIndex(Color color, out Int32 paletteIndex)
        {
            var bucket = GetBucket(color);
            var colorCount = bucket.Colors.Count();
            paletteIndex = 0;

            if (colorCount == 1)
            {
                paletteIndex = bucket.Colors.First().Key;
            }
            else
            {
                var index = 0;
                var colorIndex = ColorModelHelper.GetEuclideanDistance(color, ColorModel,
                                                                         bucket.Colors.Values.ToList());

                foreach (var colorPaletteIndex in bucket.Colors.Keys)
                {
                    if (index == colorIndex)
                    {
                        paletteIndex = colorPaletteIndex;
                        break;
                    }

                    index++;
                }
            }
        }

        #endregion
    }
}