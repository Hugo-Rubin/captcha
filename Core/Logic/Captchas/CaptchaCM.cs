using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Captchas.Abstract;
using Core.Logic.RemocaoFundo;
using Core.Logic.Separacao;
using Core.Logic.Tratamento;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sistema de Consignações da Marinha
    /// </summary>
    public class CaptchaCM : Captcha //CaptchaCA
    {
#region codigo com problemas em 30/10/2014
        //public CaptchaCM(String fileName)
        //    : base(fileName)
        //{
        //}

        //public CaptchaCM(Bitmap bmpSource)
        //    : base(bmpSource)
        //{
        //}

        //public CaptchaCM(NanoArray nanoArraySource)
        //    : base(nanoArraySource)
        //{
        //}

        //private const int EspessuraMaximaDoRisco = 5;
        //private readonly List<Point> colToErase = new List<Point>();

        //public override Bitmap RemoverFundo(Bitmap source)
        //{
        //    var icm = new ICMBasic();
        //    var img = icm.Apply(new ImgArray(source), 10, 20, 1).RemoverRuidos(5);

        //    img = LimparParteDireitaDaImagem(img);

        //    img = LimparParteEsquerdaDaImagem(img);

        //    return LimparParteSuperiorDaImagem(img).ToBitmap();
        //}

        //private ImgArray LimparParteSuperiorDaImagem(ImgArray img)
        //{
        //    var flipped = img.Flip(RotateFlipType.Rotate270FlipNone);
        //    flipped = RemoverLinhas(flipped);
        //    return flipped.Flip(RotateFlipType.Rotate90FlipNone).RemoverRuidos(26);
        //}

        //private ImgArray LimparParteDireitaDaImagem(ImgArray img)
        //{
        //    var x = PosXEndOfWord(img);

        //    img = img.GetSegment(new Rectangle(0, 0, x + 3, img.Height));
        //    return RemoverLinhas(img.InvertHorizontally()).InvertHorizontally().RemoverRuidos(EspessuraMaximaDoRisco + 1);
        //}

        //private int PosXEndOfWord(ImgArray img)
        //{
        //    int x;
        //    for (x = img.Width - 2; x > 0; x--)
        //    {
        //        if (img.GetNumberOfBlackPixelsCol(x) > EspessuraMaximaDoRisco)
        //        {
        //            break;
        //        }
        //    }
        //    return x;
        //}

        //// TODO: >>> Talvez seja necessário gerar grafos a partir de outros pontos iniciais e não apenas do pixel preto mais à esquerda <<<
        //private ImgArray RemoverLinhas(ImgArray img)
        //{
        //    var thin = new ThinningZhangSuen();
        //    var thinImg = thin.Apply(img.RemoverRuidos(6));

        //    var gs = new GraphSearch(thinImg);
        //    gs.FindGraphs();
        //    var paths = gs.GetLongestPaths();

        //    var img1 = img;
        //    var cols = (from graph in paths
        //               from point in graph
        //               where GetColThickness(img1, point) < EspessuraMaximaDoRisco
        //               select point).Count();

        //    if (cols > 0)
        //    {
        //        return EraseCol(img);
        //    }

        //    return img;
        //}

        //private ImgArray EraseCol(ImgArray img)
        //{
        //    foreach (var p in colToErase)
        //    {
        //        img.SetPixel(p.X, p.Y, Color.White);
        //    }

        //    colToErase.Clear();
        //    return img;
        //}

        //private int GetColThickness(ImgArray img, Point p)
        //{
        //    colToErase.Clear();
        //    var c = img.GetPixel(p.X, p.Y);
        //    var count = 0;
        //    var point = p;

        //    while (c.IsBlackPixel())
        //    {
        //        count++;
        //        colToErase.Add(point);
        //        c = img.GetPixel(point.X, ++point.Y);
        //    }

        //    point = new Point(p.X, p.Y - 1);
        //    c = img.GetPixel(point.X, point.Y);

        //    while (c.IsBlackPixel())
        //    {
        //        count++;
        //        colToErase.Add(point);
        //        c = img.GetPixel(point.X, --point.Y);
        //    }

        //    return count;
        //}

        //private ImgArray LimparParteEsquerdaDaImagem(ImgArray source)
        //{
        //    source = RemoverLinhas(source);

        //    const int col = 29;
        //    /*int nPixels = source.GetNumberOfBlackPixelsCol(col);
        //    while (nPixels > 3)
        //    {
        //        nPixels = source.GetNumberOfBlackPixelsCol(--col);
        //    }*/

        //    var r = new Rectangle(col, 0, source.Width - col, source.Height);
        //    return source.GetSegment(r).CortarECentralizar(source.Width, source.Height);
        //}

        //public override IEnumerable<ImgArray> GetCaracteres()
        //{
        //    var cfs = new ColorFillingSegmentation2(
        //        imgBlackAndWhite: ImgArray,
        //        minClusterLength: 120,
        //        mesclarClusterAbaixo: false,
        //        mesclarClusterAcima: true, 
        //        tamanhoMinimoLetra: 120);
        //    var sp = new SeparacaoPadrao(this, cfs);
        //    return sp.ColorFillingSegmentation2AndCorteCego(18);
        //}
#endregion
    
    public CaptchaCM(String fileName)
            : base(fileName)
        {
        }

        public CaptchaCM(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaCM(NanoArray nanoArraySource)
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
            var cfs = new ColorFillingSegmentation2(
                imgBlackAndWhite: ImgArray,
                minClusterLength: 120,
                mesclarClusterAbaixo: false,
                mesclarClusterAcima: true,
                tamanhoMinimoLetra: 120);
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