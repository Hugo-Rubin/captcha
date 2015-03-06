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
        private const int EspessuraMaximaDoRisco = 1;
        private readonly List<Point> colToErase = new List<Point>();

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
            ColorFillingSegmentation2 cfs = new ColorFillingSegmentation2(this.ImgArray, 8, 18, true, false, 22);
            return cfs.GetCaracteres().CortarECentralizarTodos(this.TamanhoImagemLetra.X, this.TamanhoImagemLetra.Y);
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
                img = RemoverRiscos(img);
            }

            return img.ToBitmap();
        }

        private ImgArray RemoverRiscos(ImgArray source)
        {
            List<Point> pontosPretos = VarrerBordas(source);

            // Caso a imagem não tenha riscos pretos esse método é chamado pois as letras são pretas, mas não há o que remover.
            if (pontosPretos.Count == 0)
            {
                return source;
            }

            ImgArray img = source.Clone();

            //foreach (var pInicio in pontosPretos)
            //{
            while (pontosPretos.Count > 1)
            {
                var pInicio = pontosPretos.FirstOrDefault();

                int x = pInicio.X;
                int y = pInicio.Y;

                // Pegar o angulo da reta entre o ponto p com todos os outros pontos pi em relação ao eixo de origem
                List<ChaveValor<Point, double>> angulos = new List<ChaveValor<Point, double>>();
                var vertices = pontosPretos.SkipWhile(p => p == pInicio); // retorna uma lista com todos os pontos com exceção do ponto atual

                #region Pego o ângulo da reta com seu eixo
                Point p2 = Ponto2ParaMedirAngulo(img, pInicio);
                var eixo = EixoDoPonto(pInicio, img.Width, img.Height);
                var sentido = eixo == 'x' ? new Point(x + 1, y) : new Point(x, y + 1);

                double anguloComEixo = AnguloEntreDuasRetas(pInicio, p2, pInicio, sentido);

                /*if (anguloComEixo > 90.0)
                {
                    anguloComEixo = Math.Abs(180.0 - anguloComEixo);
                }*/

                #endregion

                #region Pego o ângulo da reta formado pelo ponto atual e cada um dos outros pontos em relação ao eixo X

                foreach (var v in vertices)
                {
                    // Retorna uma lista com o ângulo entre o ponto P e todos os outros pontos das bordas em relação ao eixo do ponto de origem
                    var angV = AnguloEntreDuasRetas(pInicio, v, pInicio, sentido);

                    // Retorna uma lista com o ângulo entre o ponto P e todos os outros pontos das bordas em relação ao eixo X
                    // var angV = Math.Abs(Math.Atan2(v.Y - y, v.X - x) * ((double)180 / Math.PI));

                    /*if (angV > 90.0)
                    {
                        angV = Math.Abs(180.0 - angV);
                    }*/

                    angulos.Add(new ChaveValor<Point, double> { Chave = v, Valor = angV });
                    // Para medir em relação à outra reta (como o eixo Y por exemplo), fazer:
                    // Double Angle = Math.Atan2(y2 - y1, x2 - x1) - Math.Atan2(y4 - y3, x4 - x3);
                }

                #endregion

                Point pFim;
                ImgArray linhaImg;
                ChaveValor<List<Point>, bool> linhaValidada;

                do
                {
                    pFim = CompararAngulos(angulos, anguloComEixo);
                    linhaImg = PintarLinha(img.Width, img.Height, pInicio, pFim);
                    linhaValidada = VerificarEReposicionarLinha(linhaImg.ToList(), img); // Apagar linha na imagem original seguindo os pontos pretos em linhaImg, não apagar caso na imagem original os vizinhos de cima ou de baixo também forem pretos
                    
                    if (angulos.Count == 1)
                    {
                        linhaValidada.Valor = true;
                    }                    
                    if (!linhaValidada.Valor)
                    {
                        angulos.Remove(angulos.Where(c => c.Chave == pFim).FirstOrDefault());
                    }
                } while (!linhaValidada.Valor);

                List<Point> linhaImgLista = linhaValidada.Chave;
                img = ApagarRisco(img, linhaImgLista);
                pontosPretos.Remove(pInicio);
                pontosPretos.Remove(pFim);
                //RemoverLinhas(source); // Provavelmente preciso girar a imagem para que o pixel q é passado fique sempre à esquerda. Fazer testes para descobrir.
            }

            return LimparBordas(img).RemoverRuidos(4);
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

            return EliminarPontosDesnecessarios(pontos, img);
        }

        /// <summary>
        /// Remove os pontos com mais de um vizinho para que fiquem apenas os pontos das extremidades da retas
        /// </summary>
        /// <param name="pontos"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        private List<Point> EliminarPontosDesnecessarios(List<Point> pontos, ImgArray img)
        {
            List<Point> cPontos = new List<Point>();

            foreach (var p in pontos)
            {
                Neighbors n = new Neighbors(img, p);
                if (n.ToStack().Sum() == 1)
                {
                    cPontos.Add(p);
                }
            }

            return cPontos;
        }

        private ImgArray ApagarRisco(ImgArray source, List<Point> linha)
        {
            ImgArray img = source.Clone();

            foreach (var ponto in linha)
            {
                int x = ponto.X;
                int y = ponto.Y;

                if (y > 0 && y < img.Height - 1)
                {
                    if (!img.GetPixel(x, y - 1).IsBlackPixel() && !img.GetPixel(x, y + 1).IsBlackPixel())
                    {
                        img.SetPixel(x, y, Color.White);
                    }
                }
                else
                {
                    img.SetPixel(x, y, Color.White);
                }
            }

            return img;
        }

        private Point CompararAngulos(List<ChaveValor<Point, double>> angulos, double anguloReta)
        {
            double anguloMaisProximo = angulos[0].Valor;
            Point pixel = angulos[0].Chave;

            //int closest = list.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);

            for (int i = 0; i < angulos.Count; ++i)
            {
                if (Math.Abs(angulos[i].Valor - anguloReta) < Math.Abs(anguloMaisProximo - anguloReta))
                {
                    anguloMaisProximo = angulos[i].Valor;
                    pixel = angulos[i].Chave;
                }
            }

            return pixel;
        }

        private Point Ponto2ParaMedirAngulo(ImgArray img, Point origem)
        {
            int nivelMaximo = 2; // numero de linhas do traço que desejamos subir/descer até chegarmos na posição desejada para pedir o ângulo em relação à um dos eixos.
            int count = 0;
            bool direcaoDireita; // Se a linha estiver indo para a esquerda = false, se estiver indo para a direita = true

            Neighbors n = new Neighbors(img, origem);

            if (n.P3 == 1 || n.P4 == 1 || n.P5 == 1)
            {
                direcaoDireita = true;
            }
            else
            {
                direcaoDireita = false;
            }

            while (count < nivelMaximo)
            {
                if (direcaoDireita)
                {
                    if (n.P4 == 1)
                    {
                        origem = n.p4Position;
                    }

                    else if (n.P3 == 1)
                    {
                        origem = n.p3Position;
                        count++;
                    }

                    else if (n.P5 == 1)
                    {
                        origem = n.p5Position;
                        count++;
                    }
                }

                else
                {
                    if (n.P8 == 1)
                    {
                        origem = n.p8Position;
                    }

                    else if (n.P7 == 1)
                    {
                        origem = n.p7Position;
                        count++;
                    }

                    else if (n.P9 == 1)
                    {
                        origem = n.p9Position;
                        count++;
                    }
                }

                n = new Neighbors(img, origem);
            }

            return origem;
        }

        /// <param name="p1">Ponto 1 da primeira reta</param>
        /// <param name="p2">Ponto 2 da primeira reta</param>
        /// <param name="p3">Ponto 1 da segunda reta</param>
        /// <param name="p4">Ponto 2 da segunda reta</param>
        /// <returns></returns>
        private double AnguloEntreDuasRetas(Point p1, Point p2, Point p3, Point p4)
        {
            int x1 = p1.X;
            int y1 = p1.Y;
            int x2 = p2.X;
            int y2 = p2.Y;
            int x3 = p3.X;
            int y3 = p3.Y;
            int x4 = p4.X;
            int y4 = p4.Y;

            return Math.Abs(Math.Atan2(y2 - y1, x2 - x1) - Math.Atan2(y4 - y3, x4 - x3)) * ((double)180 / Math.PI);
        }

        // Retorna 'x' caso o ponto esteja no eixo X (topo ou base), 'y' caso esteja no eixo Y (margem esquerda ou direita)
        private char EixoDoPonto(Point ponto, int largura, int altura)
        {
            if (ponto.X == 0 || ponto.X == largura - 1) // eixo Y
            {
                return 'y';
            }
            else // eixo X
            {
                return 'x';
            }
        }

        private ImgArray PintarLinha(int width, int height, Point p1, Point p2)
        {
            Bitmap result = new Bitmap(width, height).InserirFundoBranco();
            Pen caneta = new Pen(Color.Black, 1);

            int x1 = p1.X;
            int y1 = p1.Y;
            int x2 = p2.X;
            int y2 = p2.Y;

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.DrawLine(caneta, x1, y1, x2, y2);
            }

            return new ImgArray(result);
        }

        private ChaveValor<List<Point>, bool> VerificarEReposicionarLinha(List<Point> linha, ImgArray imgCaptcha)
        {
            int[] erro = new int[9];
            List<Point>[] linhasDeslocadas = new List<Point>[9];
            bool linhaValida = true;

            int x = 0;
            int y = 0;

            linhasDeslocadas[0] = linha;

            for (int i = 0; i < 9; i++)
            {
                int[] deslocamento = Deslocar(i);

                if (i < 8)
                {
                    linhasDeslocadas[i + 1] = new List<Point>();
                }

                for (int p = 0; p < linhasDeslocadas[i].Count; p++)
                {
                    x = linhasDeslocadas[i][p].X;
                    y = linhasDeslocadas[i][p].Y;

                    if (x < 0 || x > imgCaptcha.Width - 1 || y < 0 || y > imgCaptcha.Height - 1)
                    {
                        continue;
                    }

                    if (!imgCaptcha.GetPixel(x, y).IsBlackPixel())
                    {
                        erro[i]++;
                    }

                    if (i < 8 && deslocamento != null)
                    {
                        linhasDeslocadas[i + 1].Add(new Point(linha[p].X + deslocamento[0], linha[p].Y + deslocamento[1]));
                    }
                }
            }

            int menorErro = erro.Min();
            
            if((linha.Count - menorErro) * 100 / linha.Count < 60)
            {
                linhaValida = false;
            }

            return new ChaveValor<List<Point>, bool> { Chave = linhasDeslocadas[Array.IndexOf(erro, menorErro)], Valor = linhaValida };
        }

        /// <summary>
        /// Recebe a posição da vizinhança e retorna o deslocamento dos eixos X e Y
        /// </summary>
        /// <param name="posicao"></param>
        /// <returns></returns>
        private int[] Deslocar(int posicao)
        {
            switch (posicao)
            {
                case 0: return new int[] { -1, -1 };
                case 1: return new int[] { 0, -1 };
                case 2: return new int[] { 1, -1 };
                case 3: return new int[] { -1, 0 };
                case 4: return new int[] { 1, 0 };
                case 5: return new int[] { -1, 1 };
                case 6: return new int[] { 0, 1 };
                case 7: return new int[] { 1, 1 };
                default: return null;
            }
        }

        private ImgArray LimparBordas(ImgArray img)
        {
            // Froteiras da parte onde fica o texto na imagem
            return img.GetSegment(new Rectangle(9, 13, 98, 19)).CortarECentralizar(img.Width, img.Height);
        }


    }
}