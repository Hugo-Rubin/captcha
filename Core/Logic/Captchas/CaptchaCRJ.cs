using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Extensions;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Separacao;
using Core.Logic.SeparacaoADAP_CRJ_Cinza;
using Core.Logic.Tratamento;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sistema de Consignações do Rio de Janeiro
    /// </summary>
    public class CaptchaCRJ : Captcha
    {
        public CaptchaCRJ(String fileName)
            : base(fileName)
        {
        }

        public CaptchaCRJ(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaCRJ(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 4; }
        }

        public TipoPadraoConsigRJ Padrao { get; protected set; }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(60, 60); }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            switch (Padrao)
            {
                case TipoPadraoConsigRJ.Cinza:
                    {
                        var separacao = new SeparacaoPadraoADAP(this);
                        return separacao.ColorFillingSegmentation2AndSeamCarving2().PreencherPixelEmTodos();
                    }

                case TipoPadraoConsigRJ.Verde:
                    {
                        var sc = new SeamCarving2(4, (int)Math.Round((double)ImgArray.Width / 4));
                        var imgs = sc.GetClusters(ImgArray);

                        var dil = new ManhattanDilation();
                        var index = 0;

                        foreach (var image in imgs)
                        {
                            imgs[index++] = dil.Apply(new ImgArray(image), 1).RemoverRuidos(20);
                        }

                        return imgs.CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
                    }

                case TipoPadraoConsigRJ.Azul:
                    {
                        var sc = new SeamCarving2(4, (int)Math.Round((double)ImgArray.Width / 4));
                        var imgs = sc.GetClusters(ImgArray);

                        var dil = new ManhattanDilation();
                        var index = 0;

                        foreach (var image in imgs)
                        {
                            imgs[index++] = dil.Apply(new ImgArray(image), 1);
                        }

                        return imgs.CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
                    }
            }

            return new ImgArray[4];
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var id = new IdentificaPadraoConsigRJ();
            Padrao = id.GetPadrao(source);

            switch (Padrao)
            {
                case TipoPadraoConsigRJ.Cinza:
                    {
                        source = source.TransformToGrayscale();

                        const int threshold = 150;

                        var result = new Bitmap(source.Width, source.Height).InserirFundoBranco();

                        for (var y = 0; y < source.Height - 1; y++)
                        {
                            for (var x = 0; x < source.Width; x++)
                            {
                                if (source.GetPixel(x, y).R < threshold)
                                {
                                    result.SetPixel(x, y, Color.Black);
                                }
                            }
                        }

                        var img = new ImgArray(result);
                        img = img.RemoverRuidos(7);

                        return img.ToBitmap();
                    }

                case TipoPadraoConsigRJ.Verde:
                    {
                        //source = source.MakeGrayscale();

                        var img = new ImgArray(source.Width, source.Height);

                        for (var y = 0; y < source.Height; y++)
                        {
                            for (var x = 0; x < source.Width; x++)
                            {
                                if (source.GetPixel(x, y).G < 110)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                }
                                else if (y > 0 && y < source.Height - 1 && source.GetPixel(x, y + 1).G < 110 &&
                                         source.GetPixel(x, y - 1).G < 110)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                }
                                else if (x > 0 && x < source.Width - 1 && source.GetPixel(x + 1, y).G < 110 &&
                                         source.GetPixel(x - 1, y).G < 110)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                }
                            }
                        }

                        var fd = new ForwardDerivative();

                        return fd.Apply(img, true, false).RemoverRuidos(10).ToBitmap();
                    }

                case TipoPadraoConsigRJ.Azul:
                    {
                        source = source.TransformToGrayscale();

                        var img = new ImgArray(source.Width, source.Height);

                        for (var y = 0; y < source.Height; y++)
                        {
                            for (var x = 0; x < source.Width; x++)
                            {
                                if (source.GetPixel(x, y).R < 220)
                                {
                                    img.SetPixel(x, y, Color.Black);
                                }
                            }
                        }

                        var fd = new ForwardDerivative();
                        var di = new ManhattanDilation();

                        return di.Apply(fd.Apply(img), 1).RemoverRuidos(8).InserirBordaX(1).InserirBordaY(1).ToBitmap();
                    }
            }

            return new Bitmap(source.Width, source.Height).InserirFundoBranco();
        }
    }
}