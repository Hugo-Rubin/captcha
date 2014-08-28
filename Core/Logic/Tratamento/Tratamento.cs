using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Tratamento
{
    public static class Tratamento
    {
        public static ImgArray TratarImagem(this ImgArray imagem)
        {
            var img = imagem.Clone();
            var preenchido = false;
            var padroes = new ReconhecimentoDePadraoNFE();
            var padrao = padroes.GetPadrao(img);
            img = ImgArray.RemoverFundo(img);

            if (padrao.Contains(TipoPadrao.Segmentada) && padrao.Contains(TipoPadrao.Bandeira))
            {
                img = img.PreencherPixels(2);
                //img = img.PreencherPixels();
                preenchido = true;
            }

            if (padrao.Contains(TipoPadrao.Distorcida))
                img = LensDistortionReturn.Calculate(img);
            else if (padrao.Contains(TipoPadrao.Bandeira))
            {
                var e = new Endireitamento(true, true);
                img = e.ApplyTo(img);
            }

            if ((!padrao.Contains(TipoPadrao.Segmentada)
                 && padrao.Contains(TipoPadrao.Bandeira))
                || padrao.Contains(TipoPadrao.Distorcida))
                img = img.PreencherPixels();
            else
            {
                if (!preenchido)
                {
                    img = img.PreencherPixels(2);
                    //img = img.PreencherPixels();
                }
            }

            return img;
        }
    }
}