using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Logic.ImageQuantizer.PathProviders
{
    public class ReversedPathProvider : IPathProvider
    {
        #region IPathProvider Members

        public IList<Point> GetPointPath(Int32 width, Int32 height)
        {
            var result = new List<Point>(width * height);

            for (var y = height - 1; y >= 0; y--)
                for (var x = width - 1; x >= 0; x--)
                {
                    var point = new Point(x, y);
                    result.Add(point);
                }

            return result;
        }

        #endregion
    }
}