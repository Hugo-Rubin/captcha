using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.ImageQuantizer.ColorCaches.Common;
using Core.Logic.ImageQuantizer.Helpers;

namespace Core.Logic.ImageQuantizer.ColorCaches.Octree
{
    public class OctreeColorCache : BaseColorCache
    {
        private OctreeCacheNode root;

        /// <summary>
        ///   Gets a value indicating whether this instance is color model supported.
        /// </summary>
        /// <value> <c>true</c> if this instance is color model supported; otherwise, <c>false</c> . </value>
        public override Boolean IsColorModelSupported
        {
            get { return false; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="OctreeColorCache" /> class.
        /// </summary>
        public OctreeColorCache()
        {
            ColorModel = ColorModel.RedGreenBlue;
            root = new OctreeCacheNode();
        }

        /// <summary>
        ///   See <see cref="BaseColorCache.Prepare" /> for more details.
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();
            root = new OctreeCacheNode();
        }

        /// <summary>
        ///   See <see cref="BaseColorCache.OnCachePalette" /> for more details.
        /// </summary>
        protected override void OnCachePalette(IList<Color> palette)
        {
            var index = 0;

            foreach (var color in palette)
            {
                root.AddColor(color, index++, 0);
            }
        }

        /// <summary>
        ///   See <see cref="BaseColorCache.OnGetColorPaletteIndex" /> for more details.
        /// </summary>
        protected override void OnGetColorPaletteIndex(Color color, out Int32 paletteIndex)
        {
            var candidates = root.GetPaletteIndex(color, 0);

            paletteIndex = 0;
            var index = 0;
            var colorIndex = ColorModelHelper.GetEuclideanDistance(color, ColorModel, candidates.Values.ToList());

            foreach (var colorPaletteIndex in candidates.Keys)
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
}