using System;
using System.Collections.Generic;
using System.Drawing;

namespace PalleteQuantizer.PathProviders
{
    public class StandardPathProvider : IPathProvider
    {
        public IList<Point> GetPointPath(Int32 width, Int32 height)
        {
            var result = new List<Point>(width*height);

            for (Int32 y = 0; y < height; y++)
            {
                for (Int32 x = 0; x < width; x++)
                {
                    var point = new Point(x, y);
                    result.Add(point);
                }
            }

            return result;
        }
    }
}