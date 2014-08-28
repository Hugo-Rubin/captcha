using System;
using System.Collections.Generic;
using Core.Logic.Filtros;
using Core.Logic.Predict;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    public class ReconhecimentoDePadraoNFE
    {
        /// <summary>
        ///   Retorna padrões de efeitos encontrados na imagem, podendo ser: Segmantado, Distorção, Bandeira ou Bandeira segmentada
        /// </summary>
        /// <param name="src"> </param>
        /// <returns> </returns>
        public HashSet<TipoPadrao> GetPadrao(ImgArray src)
        {
            var result = new HashSet<TipoPadrao>();
            if (ImagemEstaSegmentada(src))
            {
                result.Add(TipoPadrao.Segmentada);
            }

            if (ContemBandeiraOuDistorcao(src))
            {
                var imgPadrao = PintarPadrao(src);
                switch (PredictPadraoCaptchaNFE.Instance.Recognize(imgPadrao))
                {
                    case 'b':
                        result.Add(TipoPadrao.Bandeira);
                        break;
                    case 'd':
                        result.Add(TipoPadrao.Distorcida);
                        break;
                }
            }
            return result;
        }

        private bool ImagemEstaSegmentada(ImgArray imagem)
        {
            for (var y = 0; y < imagem.Height - 2; y++)
            {
                for (var x = 0; x < imagem.Width - 2; x++)
                {
                    if (imagem.GetPixel(x, y).IsBlackPixel())
                    {
                        try
                        {
                            if (!(!imagem.GetPixel(x + 1, y).IsBlackPixel()
                                  && !imagem.GetPixel(x, y - 1).IsBlackPixel()
                                  && !imagem.GetPixel(x, y + 1).IsBlackPixel())
                                )
                            {
                                return (!(imagem.GetPixel(x + 2, y).IsBlackPixel()
                                          || imagem.GetPixel(x, y + 2).IsBlackPixel()));
                            }
                        }
                        catch (Exception e)
                        {
                            ServerLog.AppendErrorLog(
                                String.Format("{0} \t {1} \t {2} \t {3}", e.Message, e.InnerException, e.HelpLink,
                                              e.StackTrace), imagem);
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///   Verifica se a imagem está segmentada e reta
        /// </summary>
        /// <param name="imagem"> </param>
        /// <returns> </returns>
        private bool ContemBandeiraOuDistorcao(ImgArray imagem)
        {
            uint top = (uint)imagem.Height, bottom = 0;
            var foraDoPadrao = false;

            var y = 0;
            while (!foraDoPadrao && y < imagem.Height - 2)
            {
                for (var x = 0; x < imagem.Width - 2; x++)
                {
                    if (imagem.GetPixel(x, y).IsBlackPixel())
                    {
                        top = (uint)Math.Min(top, y);
                        bottom = (uint)Math.Max(bottom, y);

                        var c1 = imagem.GetPixel(x - 1, y);
                        var c2 = imagem.GetPixel(x + 1, y);
                        var c3 = imagem.GetPixel(x, y + 1);
                        var c4 = imagem.GetPixel(x + 1, y + 1);
                        if (c2.IsBlackPixel()
                            && c3.IsBlackPixel()
                            && !(!c1.IsBlackPixel() && c4.IsBlackPixel()))
                        {
                            foraDoPadrao = true;
                            break;
                        }
                    }
                }
                y++;
            }
            return (foraDoPadrao || bottom - top >= 41);
        }

        private ImgArray PintarPadrao(ImgArray imagem)
        {
            // Não dá pra evitar ToBitMap e new ImgArray pq GaussianBlur e PaintBlur estao usando LockBits
            var gb = new GaussianBlur();
            var blurredImg = gb.ProcessImage(imagem.ToBitmap());
            return new ImgArray(blurredImg.PaintBlur());
        }
    }
}