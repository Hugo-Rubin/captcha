using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.ImageQuantizer.Helpers;

namespace Core.Logic.ImageQuantizer.Quantizers
{
    public abstract class BaseColorCacheQuantizer : BaseColorQuantizer
    {
        private IColorCache colorCache;
        
        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseColorCacheQuantizer" /> class.
        /// </summary>
        protected BaseColorCacheQuantizer()
        {
            colorCache = null;
        }
        
        /// <summary>
        ///   Changes the cache provider.
        /// </summary>
        /// <param name="colorCache"> The color cache. </param>
        public void ChangeCacheProvider(IColorCache colorCache)
        {
            this.colorCache = colorCache;
        }

        /// <summary>
        ///   Caches the palette.
        /// </summary>
        /// <param name="palette"> The palette. </param>
        public void CachePalette(IList<Color> palette)
        {
            GetColorCache().CachePalette(palette);
        }
        
        private IColorCache GetColorCache()
        {
            // if there is no cache, it attempts to create a default cache; integrated in the quantizer
            var result = colorCache ?? (colorCache = OnCreateDefaultCache());

            // if the cache exists; or default one was created for these purposes.. use it
            if (result == null)
            {
                var message =
                    string.Format("The color cache is not initialized! Please use SetColorCache() method on quantizer.");
                throw new ArgumentNullException(message);
            }

            // cache is fine, return it
            return result;
        }

        /// <summary>
        ///   Called when it is needed to create default cache (no cache is supplied from outside).
        /// </summary>
        /// <returns> </returns>
        protected abstract IColorCache OnCreateDefaultCache();

        /// <summary>
        ///   Redirection to retrieve palette to be cached, if palette is not available yet.
        /// </summary>
        protected abstract List<Color> OnGetPaletteToCache(Int32 colorCount);

        /// <summary>
        ///   See <see cref="BaseColorQuantizer.OnPrepare" /> for more details.
        /// </summary>
        protected override void OnPrepare(ImageBuffer image)
        {
            base.OnPrepare(image);

            GetColorCache().Prepare();
        }

        /// <summary>
        ///   See <see cref="BaseColorQuantizer.OnGetPalette" /> for more details.
        /// </summary>
        protected override sealed List<Color> OnGetPalette(Int32 colorCount)
        {
            // use optimization, or calculate new palette if color count is lower than unique color count
            var palette = base.OnGetPalette(colorCount) ?? OnGetPaletteToCache(colorCount);
            GetColorCache().CachePalette(palette);
            return palette;
        }

        /// <summary>
        ///   See <see cref="BaseColorQuantizer.OnGetPaletteIndex" /> for more details.
        /// </summary>
        protected override void OnGetPaletteIndex(Color color, Int32 key, Int32 x, Int32 y, out int paletteIndex)
        {
            base.OnGetPaletteIndex(color, key, x, y, out paletteIndex);

            // if not determined, use cache to determine the index
            if (paletteIndex == InvalidIndex)
            {
                GetColorCache().GetColorPaletteIndex(color, out paletteIndex);
            }
        }
    }
}