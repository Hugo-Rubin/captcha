using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.ImageQuantizer.ColorCaches.Common;

namespace Core.Logic.ImageQuantizer.ColorCaches
{
    public abstract class BaseColorCache : ImageQuantizer.IColorCache
    {
        private readonly ConcurrentDictionary<Int32, Int32> cache;

        /// <summary>
        ///   Gets or sets the color model.
        /// </summary>
        /// <value> The color model. </value>
        protected ColorModel ColorModel { get; set; }

        /// <summary>
        ///   Gets a value indicating whether this instance is color model supported.
        /// </summary>
        /// <value> <c>true</c> if this instance is color model supported; otherwise, <c>false</c> . </value>
        public abstract Boolean IsColorModelSupported { get; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseColorCache" /> class.
        /// </summary>
        protected BaseColorCache()
        {
            cache = new ConcurrentDictionary<Int32, Int32>();
        }

        /// <summary>
        ///   Changes the color model.
        /// </summary>
        /// <param name="colorModel"> The color model. </param>
        public void ChangeColorModel(ColorModel colorModel)
        {
            ColorModel = colorModel;
        }

        /// <summary>
        ///   Called when a palette is about to be cached, or precached.
        /// </summary>
        /// <param name="palette"> The palette. </param>
        protected abstract void OnCachePalette(IList<Color> palette);

        /// <summary>
        ///   Called when palette index is about to be retrieve for a given color.
        /// </summary>
        /// <param name="color"> The color. </param>
        /// <param name="paletteIndex"> Index of the palette. </param>
        protected abstract void OnGetColorPaletteIndex(Color color, out Int32 paletteIndex);

        /// <summary>
        ///   See <see cref="IColorCache.Prepare" /> for more details.
        /// </summary>
        public virtual void Prepare()
        {
            cache.Clear();
        }

        /// <summary>
        ///   See <see cref="IColorCache.CachePalette" /> for more details.
        /// </summary>
        public void CachePalette(IList<Color> palette)
        {
            OnCachePalette(palette);
        }

        /// <summary>
        ///   See <see cref="IColorCache.GetColorPaletteIndex" /> for more details.
        /// </summary>
        public void GetColorPaletteIndex(Color color, out Int32 paletteIndex)
        {
            var key = color.R << 16 | color.G << 8 | color.B;

            paletteIndex = cache.AddOrUpdate(key,
                                             colorKey =>
                                             {
                                                 Int32 paletteIndexInside;
                                                 OnGetColorPaletteIndex(color, out paletteIndexInside);
                                                 return paletteIndexInside;
                                             },
                                             (colorKey, inputIndex) => inputIndex);
        }
    }
}