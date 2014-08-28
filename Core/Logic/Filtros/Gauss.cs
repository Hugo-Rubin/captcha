using System;

namespace Core.Logic.Filtros
{
    public static class Gauss
    {
        public static double Calculate(double b, double c, double x)
        {
            var exp1 = (x - b) * (x - b);
            var exp2 = (2 * (c * c));
            var result = (Math.Exp(-(exp1 / exp2)));
            return result;
        }

        public static double NormalDistribution(double x, double mean, double variance)
        {
            return ((1 / (Math.Sqrt(2 * Math.PI * variance)) * Math.Pow(Math.E, -(Math.Exp(x - mean) / 2 * variance))));
        }
    }
}