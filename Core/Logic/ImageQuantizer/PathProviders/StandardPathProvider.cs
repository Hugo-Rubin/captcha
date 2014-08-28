using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Logic.ImageQuantizer.PathProviders
{
    public class StandardPathProvider : IPathProvider
    {
        #region IPathProvider Members

        public IList<Point> GetPointPath(Int32 width, Int32 height)
        {
            var result = new List<Point>(width * height);

            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    var point = new Point(x, y);
                    result.Add(point);
                }

            return result;
        }

        #endregion
    }
}