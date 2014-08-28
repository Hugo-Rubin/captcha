using System.Drawing;

namespace Core.Logic.Tratamento
{
// ReSharper disable once InconsistentNaming
    public interface ICM
    {
        Bitmap Apply(Bitmap img, double maxDiff, double weightDiff, int iterations, double covar = 0);
    }
}