using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    public interface IDilation
    {
        ImgArray Apply(ImgArray img, int k);
    }
}