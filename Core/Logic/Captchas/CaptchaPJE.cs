using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;
using Core.Logic.Filtros;
using System.Drawing.Imaging;
using Core.Common.Extensions;
using Core.Logic.RemocaoFundo;
using Core.Logic.Separacao;
using Core.Logic.Tratamento;

namespace Core.Logic.Captchas
{
    public class CaptchaPJE : Captcha
    {
        public CaptchaPJE(String fileName)
            : base(fileName)
        {
        }

        public CaptchaPJE(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaPJE(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(25, 25); }
        }


        public override IEnumerable<ImgArray> GetCaracteres()
        {
            SeparacaoPadrao sp = new SeparacaoPadrao(this);
            return sp.ColorFillingSegmentation2AndSeamCarving2(null, false).CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            ImgArray img = new ImgArray(source, false, 212);
            
            // Máscaras [0, 1, 0] e [0] são usadas para remover os quadriláteros da imagem. Eles tem 1 pixel de espessura e a máscara precisa ser aplicada na vertical e na horizontal.
            //                      [1]
            //                      [0]

            // Máscara horizontal:
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 1; x < img.Width - 1; x++)
                {
                    if (!img.GetPixel(x - 1, y).IsBlackPixel() && img.GetPixel(x, y).IsBlackPixel() && !img.GetPixel(x + 1, y).IsBlackPixel())
                    {
                        img.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            }

            // Máscara vertical:
            for (int y = 1; y < img.Height - 1; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (!img.GetPixel(x, y - 1).IsBlackPixel() && img.GetPixel(x, y).IsBlackPixel() && !img.GetPixel(x, y + 1).IsBlackPixel())
                    {
                        img.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            }

            return img.ApagarMargem().ToBitmap();
        }


    }
}