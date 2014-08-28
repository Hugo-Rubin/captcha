using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Logic.ImageQuantizer.PathProviders
{
    public class SerpentinePathProvider : IPathProvider
    {
        #region IPathProvider Members

        public IList<Point> GetPointPath(Int32 width, Int32 height)
        {
            var leftToRight = true;
            var result = new List<Point>(width * height);

            for (var y = 0; y < height; y++)
            {
                for (var x = leftToRight ? 0 : width - 1; leftToRight ? x < width : x >= 0; x += leftToRight ? 1 : -1)
                {
                    var point = new Point(x, y);
                    result.Add(point);
                }

                leftToRight = !leftToRight;
            }

            return result;
        }

        #endregion
    }
}