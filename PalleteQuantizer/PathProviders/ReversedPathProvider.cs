using System;
using System.Collections.Generic;
using System.Drawing;

namespace PalleteQuantizer.PathProviders
{
    public class ReversedPathProvider : IPathProvider
    {
        public IList<Point> GetPointPath(Int32 width, Int32 height)
        {
            var result = new List<Point>(width*height);

            for (Int32 y = height - 1; y >= 0; y--)
            {
                for (Int32 x = width - 1; x >= 0; x--)
                {
                    var point = new Point(x, y);
                    result.Add(point);
                }
            }

            return result;
        }
    }
}