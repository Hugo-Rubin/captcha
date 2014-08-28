using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using Accord;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using System.Configuration;
using Library;

namespace BLL
{
    /// <summary>
    /// Classe de predição do captcha do Sintegra RJ
    /// </summary>
    public class PredictCaptchaRJ : Predict
    {
        public override char Recognize(ImgArray Caracter)
        {
            //TODO: Remover ToBitmap()
            int[] p = this.predict(Caracter.ToBitmap().CortarECentralizar(60, 60).InserirFundoBranco().PixelIntensity());
            return dicio[p[0]];
        }

        [Obsolete("Usar ImgArray")]
        public override string Recognize(Bitmap[] Caracteres)
        {
            String Result = String.Empty;

            foreach (var caracter in Caracteres)
            {
                int[] p = this.predict(caracter.CortarECentralizar(60, 60).InserirFundoBranco().PixelIntensity());
                foreach (var letraIdx in p)
                {
                    Result += this.dicio[letraIdx];
                }
            }
            return Result;

        }

        public string Recognize(ImgArray[] CaracteresCortadosECentralizados)
        {
            String Result = String.Empty;
            foreach (var caracter in CaracteresCortadosECentralizados)
            {
                //TODO: Remover ToBitmap()
                int[] p = this.predict(caracter.ToBitmap().PixelIntensity());
                foreach (var letraIdx in p)
                {
                    Result += this.dicio[letraIdx];
                }
            }
            return Result;

        }

        public PredictCaptchaRJ()
        {
            this.dicio = new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'b', 'c', 'd',
                                      'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'p', 'q',
                                      'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };

            this.HiddenUnits = 135;
            this.Saidas = this.dicio.Count();
            this.TamanhoCaracter = 3600;
            double[,] theta1 = CarregarTheta(ConfigurationManager.AppSettings["ArquivoTheta1_C4"], HiddenUnits, TamanhoCaracter);
            double[,] theta2 = CarregarTheta(ConfigurationManager.AppSettings["ArquivoTheta2_C4"], Saidas, HiddenUnits);
            CarregarMatrixTransposta(theta1, theta2);
        }
    }
}