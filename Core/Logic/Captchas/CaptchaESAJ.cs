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
    public class CaptchaESAJ : Captcha
    {
        public CaptchaESAJ(String fileName)
            : base(fileName)
        {
        }

        public CaptchaESAJ(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaESAJ(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(20, 20); }
        }


        public override IEnumerable<ImgArray> GetCaracteres()
        {
            SeparacaoPadrao sp = new SeparacaoPadrao(this);
            return sp.ColorFillingSegmentation2AndSeamCarving2(null, false).CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            Color corLetras;
            bool posTratamento = false;

            if (source.CountPixelsWithColor(Color.FromArgb(255, 255, 0, 0)) > 0)
            {
                corLetras = Color.FromArgb(255, 255, 0, 0);
            }
            else
            {
                if (source.CountPixelsWithColor(Color.FromArgb(255, 0, 0, 255)) > 0)
                {
                    corLetras = Color.FromArgb(255, 0, 0, 255);
                }
                else
                {
                    corLetras = Color.FromArgb(255, 0, 0, 0);
                    posTratamento = true; // verificar bordas procurando traços pretos na imagem e removendo-os caso existam
                }
            }

            ImgArray img = source.KeepColor(corLetras);

            if (posTratamento)
            {
                img = LocalizarERemoverRiscos(img);
            }

            return img.ToBitmap();
        }

        private ImgArray LocalizarERemoverRiscos(ImgArray source)
        {
            List<Point> pontosPretos = VarrerBordas(source);

            if (pontosPretos.Count == 0)
            {
                return source;
            }

            ImgArray img = new ImgArray(source.Width, source.Height);

            foreach (var p in pontosPretos)
            {
                int x = p.X;
                int y = p.Y;

                if (source.GetPixel(x, y).IsBlackPixel())
                {
                    Neighbors n = new Neighbors(source, p);
                    // Usar Graph para percorrer o caminho do traço e apagar os pixels
                }
            }

            return img;
        }

        /// <summary>
        /// Varre as bordas da imagem procurando por pixels pretos
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private List<Point> VarrerBordas(ImgArray img)
        {
            List<Point> pontos = new List<Point>();

            for (int y = 0; y < img.Height; y += img.Height - 1)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        pontos.Add(new Point(x, y));
                    }
                }
            }

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x += img.Width - 1)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        pontos.Add(new Point(x, y));
                    }
                }
            }

            return pontos;
        }


    }
}