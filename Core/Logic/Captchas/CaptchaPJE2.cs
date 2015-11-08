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
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Core.Logic.Captchas
{
    public class CaptchaPJE2 : Captcha
    {
        public CaptchaPJE2(String fileName)
            : base(fileName)
        {
        }

        public CaptchaPJE2(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaPJE2(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 6; }
        }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(25, 25); }
        }


        public override IEnumerable<ImgArray> GetCaracteres()
        {
            // Como as imagens possuem tamanho padronizado, podemos fazer a separação dos caracteres com cortes retos entre as letras
            /*List<ImgArray> letras = new List<ImgArray>();
            letras.Add(this.ImgArray.GetSegment(new Rectangle(0, 0, 15, this.ImgArray.height)));
            letras.Add(this.ImgArray.GetSegment(new Rectangle(17, 0, 15, this.ImgArray.height)));
            letras.Add(this.ImgArray.GetSegment(new Rectangle(33, 0, 15, this.ImgArray.height)));
            letras.Add(this.ImgArray.GetSegment(new Rectangle(49, 0, 15, this.ImgArray.height)));
            letras.Add(this.ImgArray.GetSegment(new Rectangle(65, 0, 15, this.ImgArray.height)));
            letras.Add(this.ImgArray.GetSegment(new Rectangle(81, 0, 15, this.ImgArray.height)));

            return letras.CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);*/
            return null;
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            ImgArray img = new ImgArray(source, false, 215);

            /*ForwardDerivative fd = new ForwardDerivative();
            fd.Apply(img);
            fd.Apply(img);

            AForge.Imaging.Filters.GaussianBlur gs = new AForge.Imaging.Filters.GaussianBlur();*/

            /*// create grayscale filter (BT709)
            Grayscale filter = new Grayscale(1, 1, 1);
            // apply the filter
            Bitmap bitmap = filter.Apply(img.ToBitmap());

            HoughCircleTransformation circleTransform = new HoughCircleTransformation(3);
            // apply Hough circle transform
            circleTransform.ProcessImage(bitmap);
            Bitmap houghCircleImage = circleTransform.ToBitmap();
            // get circles using relative intensity
            HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity(0.1);*/

            int[] tst1 = img.ToIntVector();
            ImgArray tst2 = new ImgArray(tst1, img.Width, img.Height);

            var circleHoughObject = new CircleHough();
            int radius = 3;
            int lines = 1;
            int width = img.Width;
            int height = img.Height;
            int[] orig = img.ToIntVector();
            circleHoughObject.Init(orig, width, height, radius);
            circleHoughObject.SetLines(lines);
            orig = (int[]) (object) circleHoughObject.Process();
 
            var overlayImage = new ImgArray(OverlayImage(orig, width, height), width, height);

            int rmax = (int) Math.Sqrt(width * width + height * height);
            int[] acc = new int[width * height];
            acc = (int[]) (object) circleHoughObject.GetAcc();

            var houghAccImage = new ImgArray(acc, width, height);

            var linesImage = new ImgArray(orig, width, height);

            return img.RemoverRuidos(30).ToBitmap();
        }

        private int[] OverlayImage(int[] input, int width, int height)
        {
            uint[] imgUint = new uint[width * height];
            ImgArray.ToIntVector().CopyTo(imgUint, 0);

            for (int y = 0; y < height; y++)
            {
                for(int x = 0; x < width;x++)
                {
                    if ((input[y * width + x] & 0xff) > 0)
                    {
                        imgUint[y * width + x] = 0xffff0000;
                    }
			    }
		    }

            return (int[]) (object) imgUint;
    	}


    }
}