using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Captchas.Abstract;
using Core.Logic.RemocaoFundo;
using Core.Logic.Separacao;
using Core.Logic.Tratamento;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sistema de Consignações da Aeronautica
    /// </summary>
    public class CaptchaCA : Captcha
    {
        public CaptchaCA(String fileName)
            : base(fileName)
        {
        }

        public CaptchaCA(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaCA(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(45, 45); }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            var cfs = new ColorFillingSegmentation2(ImgArray, 8, 2, true, true, 120);
            var sp = new SeparacaoPadrao(this, cfs);
            
            return sp.ColorFillingSegmentation2AndCorteCego(18);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var icm = new ICMBasic();
            var img = icm.Apply(new ImgArray(source), 10, 20, 1);
            var fd = new ForwardDerivative();

            //for (int i = 0; i < 3; i++)
            //{
            img = fd.Apply(img, true, false);
            //}

            img = LimparParteEsquerdaDaImagem(RemoverLinhaEsquerda(img.RemoverRuidos(25)));
            var er = new Erosion(img.InvertImageColors());

            return er.Result.InvertImageColors().PreencherPixels().RemoverRuidos(5).ToBitmap();
        }

        protected ImgArray RemoverLinhaEsquerda(ImgArray source)
        {
            for (var i = 0; i < 2; i++)
            {
                var minX = source.GetMinXPoint();
                var alturaTopoRuido = source.GetAlturaETopoLinha(minX);

                var limiteAlturaRuido = alturaTopoRuido.Chave + 3;

                while (alturaTopoRuido.Chave <= limiteAlturaRuido &&
                       source.GetPixel(alturaTopoRuido.Valor.X, alturaTopoRuido.Valor.Y).IsBlackPixel())
                {
                    source = source.PintarPixelsAbaixo(alturaTopoRuido.Valor, alturaTopoRuido.Chave, Color.White);
                    alturaTopoRuido =
                        source.GetAlturaETopoLinha(new Point(alturaTopoRuido.Valor.X + 1, alturaTopoRuido.Valor.Y));
                }

                if (source.GetMinXPoint().X - minX.X > 2)
                {
                    break;
                }
            }
            return source;
        }

        // TODO: Antes de apagar no pixel 29, verificar a quantidade de pixels pretos ali. Caso tenha mais do que X (como quando a imagem começa com um 'm'), ir andando colunas
        // TODO: para a esquerda até que o número de pixels seja menor que X.
        private ImgArray LimparParteEsquerdaDaImagem(ImgArray source)
        {
            const int col = 29;
            /*int nPixels = source.GetNumberOfBlackPixelsCol(col);
            while (nPixels > 3)
            {
                nPixels = source.GetNumberOfBlackPixelsCol(--col);
            }*/

            var r = new Rectangle(col, 0, source.Width - col, source.Height);
            return source.GetSegment(r).CortarECentralizar(source.Width, source.Height);
        }
    }
}