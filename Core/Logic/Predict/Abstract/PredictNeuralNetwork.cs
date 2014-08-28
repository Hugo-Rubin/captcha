using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Accord.Math;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Predict.Abstract
{
    public abstract class PredictNeuralNetwork<T> : Predict<T> where T: new()
    {
        private double[,] th1;
        private double[,] th2;
        protected abstract int HiddenUnits { get; }

        /// <summary>
        ///   Quantidade de pixels do caracter, definida por Width x Height da imagem
        /// </summary>
        protected abstract int TamanhoCaracter { get; }

        protected abstract char[] Dicionario { get; }

        protected virtual String ChaveTheta1
        {
            get { return "theta1"; }
        }

        protected virtual String ChaveTheta2
        {
            get { return "theta2"; }
        }

        public String ArquivoTheta1
        {
            get
            {
                var chave = ResourceFromConfigKey(ChaveTheta1);
                var constante = ServerUtil.ResourcesDir + "theta1.csv";
                return chave ?? constante;
            }
        }

        public String ArquivoTheta2
        {
            get { return ResourceFromConfigKey(ChaveTheta2) ?? ServerUtil.ResourcesDir + "theta2.csv"; }
        }

        protected int Saidas
        {
            get { return Dicionario.Length; }
        }

        /// <summary>
        ///   Determina qual arquivo determinara renovacao de cache
        /// </summary>
        public override string CacheDependencyFile
        {
            get
            {
                ServerLog.Append("CacheDependencyFile = " + ArquivoTheta1);
                return ArquivoTheta1;
            }
        }

        private String ResourceFromConfigKey(string key)
        {
            var chave = ConfigurationManager.AppSettings[key];
            if (chave != null)
                return ServerUtil.ResourcesDir + chave;
            return null;
        }

        private void CarregarThetasUsandoAppConfig()
        {
            var theta1 = CarregarTheta(ArquivoTheta1, HiddenUnits, TamanhoCaracter);
            var theta2 = CarregarTheta(ArquivoTheta2, Saidas, HiddenUnits);
            CarregarMatrixTransposta(theta1, theta2);
        }

        private double[,] CarregarTheta(String arquivoTheta, int tamX, int tamY)
        {
            var result = new double[tamX, tamY + 1];
            using (TextReader tr = new StreamReader(arquivoTheta))
            {
                var i = 0;
                string line;

                var format = CultureInfo.CurrentCulture.NumberFormat;
                var separadorDecimal = format.NumberDecimalSeparator.First();

                while ((line = tr.ReadLine()) != null)
                {
                    var temp = line.Split(';');
                    for (var j = 0; j < temp.Length; j++)
                    {
                        temp[j] = temp[j].Replace(',', separadorDecimal);
                        result[i, j] = double.Parse(temp[j], NumberStyles.Any);
                    }
                    i++;
                }
            }
            return result;
        }

        private void CarregarMatrixTransposta(double[,] theta1, double[,] theta2)
        {
            th1 = theta1.Transpose();
            th2 = theta2.Transpose();
        }

        protected int Predict(double[,] x)
        {
            if (th1 == null || th2 == null)
            {
                CarregarThetasUsandoAppConfig();
            }

            if (th1 == null)
            {
                throw new Exception("Erro ao carregar th1");
            }

            var m = x.GetLength(0);
            int[] p;

            try
            {
                var colX = new double[m];
                colX.Init(1);

                var x1 = x.InsertColumn(colX, 0);

                if (x1.GetLength(1) != th1.GetLength(0))
                {
                    ServerLog.AppendErrorLog("\nX1 não pode ser multiplicado por Theta1. As dimensões não batem",
                                             new Bitmap(1, 1));
                    return 0;
                }

                var mult1 = x1.Multiply(th1);
                var sig = Sigmoid(mult1);

                var colH1 = new double[sig.GetLength(0)];
                colH1.Init(1);

                var h1 = sig.InsertColumn(colH1, 0);

                var mult2 = h1.Multiply(th2);
                var h2 = Sigmoid(mult2);

                h2.Max(1, out p);
            }
            catch (Exception exception)
            {
                ServerLog.AppendErrorLog(exception.Message, new Bitmap(1, 1));
                throw;
            }
            return p[0];
        }

        private double[,] Sigmoid(double[,] z)
        {
            var g = new double[z.GetLength(0), z.GetLength(1)];
            for (var i = 0; i < z.GetLength(0); i++)
            {
                for (var j = 0; j < z.GetLength(1); j++)
                {
                    g[i, j] = Sigmoid(z[i, j]);
                }
            }
            return g;
        }

        private double Sigmoid(double z)
        {
            return 1.0 / (1 + Math.Exp(-z));
        }

        public override char Recognize(ImgArray caracter)
        {
            var idx = Predict(caracter.PixelIntensity());
            return Dicionario[idx];
        }
    }
}