using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.SpectralCluster
{
    public class SCPoint
    {
        private double X;
        private double Y;

        public SCPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double distance(SCPoint testPoint)
        {
            return Math.Sqrt(Math.Pow((this.X - testPoint.X), 2) + Math.Pow((this.Y - testPoint.Y), 2));
        }

        public double squaredDistance(SCPoint testPoint)
        {
            return Math.Pow((this.X - testPoint.X), 2) + Math.Pow((this.Y - testPoint.Y), 2);
        }

        public bool equals(SCPoint test)
        {
            bool result = false;
            if (test.X == this.X && test.Y == this.Y)
            {
                result = true;
            }
            return result;
        }
    }
}
