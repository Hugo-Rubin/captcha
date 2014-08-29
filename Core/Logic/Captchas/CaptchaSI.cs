using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Core.Common;
using Core.Common.Extensions;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Obsolete;
using Core.Logic.Types;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do captcha do SISCARGA
    /// </summary>
    public class CaptchaSI : Captcha
    {
        public CaptchaSI(String fileName)
            : base(fileName)
        {
        }

        public CaptchaSI(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaSI(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }


        public override int NumeroMinimoDeLetras
        {
            get { return 4; }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            throw new NotImplementedException();
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            // Elimina o fundo usando o algoritmo de Otsu
            if (source.CountNumberOfColors() == 2)
            {
                var bmp = source.Otsu().InvertImageColors();
                ConectarPixels(bmp);
                return bmp;
            }
            else
            {
                var bmp = source.TransformToGrayscale().Otsu().InvertImageColors();
                ConectarPixels(bmp);
                return bmp;
            }
        }

        private unsafe void ConectarPixels(Bitmap img)
        {
            var bitdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            const int pixelSize = 4;

            var cvEqC = new ChaveValorEqualityComparer();
            var toPaint = new HashSet<ChaveValor<int, int>>(cvEqC);

            for (var y = 1; y < bitdata.Height - 1; y++)
            {
                var previousLine = (byte*)bitdata.Scan0 + ((y - 1) * bitdata.Stride);
                var currentLine = (byte*)bitdata.Scan0 + (y * bitdata.Stride);
                var nextLine = (byte*)bitdata.Scan0 + ((y + 1) * bitdata.Stride);

                for (var x = 1; x < bitdata.Width - 1; x++)
                {
                    byte north, south, east, west, northeast, northwest, southeast, southwest;

                    if (currentLine[x * pixelSize] == 0)
                    {
                        west = currentLine[(x - 1) * pixelSize];
                        east = currentLine[(x + 1) * pixelSize];
                        north = previousLine[x * pixelSize];
                        south = nextLine[x * pixelSize];
                        northwest = previousLine[(x - 1) * pixelSize];
                        northeast = previousLine[(x + 1) * pixelSize];
                        southwest = nextLine[(x - 1) * pixelSize];
                        southeast = nextLine[(x + 1) * pixelSize];

                        if (west != 0 && east != 0 && north != 0 && south != 0 && northeast != 0 && northwest != 0 &&
                            southeast != 0 && southwest != 0)
                        {
                            toPaint = VerificarVizinhanca(bitdata, x, y, toPaint, 0);
                        }
                    }
                    else
                    {
                        west = currentLine[(x - 1) * pixelSize];
                        east = currentLine[(x + 1) * pixelSize];
                        north = previousLine[x * pixelSize];
                        south = nextLine[x * pixelSize];
                        northwest = previousLine[(x - 1) * pixelSize];
                        northeast = previousLine[(x + 1) * pixelSize];
                        southwest = nextLine[(x - 1) * pixelSize];
                        southeast = nextLine[(x + 1) * pixelSize];

                        if (southeast != 0 && northwest != 0)
                        {
                            if (MultipleXor(new[] { west == 0, south == 0, southwest == 0 }) &&
                                MultipleXor(new[] { east == 0, north == 0, northeast == 0 }))
                            {
                                var validSouthNeighborhood = 0;
                                var validNorthNeighborhood = 0;

                                if (west == 0)
                                {
                                    var checkIfPixelIsAlone = PegarVizinhanca(bitdata, x - 1, y);
                                    foreach (var item in checkIfPixelIsAlone)
                                    {
                                        if (item.Value == 0)
                                        {
                                            validSouthNeighborhood++;
                                        }
                                    }
                                }
                                if (south == 0)
                                {
                                    var checkIfPixelIsAlone = PegarVizinhanca(bitdata, x, y + 1);
                                    foreach (var item in checkIfPixelIsAlone)
                                    {
                                        if (item.Value == 0)
                                        {
                                            validSouthNeighborhood++;
                                        }
                                    }
                                }
                                if (southwest == 0)
                                {
                                    var checkIfPixelIsAlone = PegarVizinhanca(bitdata, x - 1, y + 1);
                                    foreach (var item in checkIfPixelIsAlone)
                                    {
                                        if (item.Value == 0)
                                        {
                                            validSouthNeighborhood++;
                                        }
                                    }
                                }
                                if (east == 0)
                                {
                                    var checkIfPixelIsAlone = PegarVizinhanca(bitdata, x + 1, y);
                                    foreach (var item in checkIfPixelIsAlone)
                                    {
                                        if (item.Value == 0)
                                        {
                                            validNorthNeighborhood++;
                                        }
                                    }
                                }
                                if (north == 0)
                                {
                                    var checkIfPixelIsAlone = PegarVizinhanca(bitdata, x, y - 1);
                                    foreach (var item in checkIfPixelIsAlone)
                                    {
                                        if (item.Value == 0)
                                        {
                                            validNorthNeighborhood++;
                                        }
                                    }
                                }
                                if (northeast == 0)
                                {
                                    var checkIfPixelIsAlone = PegarVizinhanca(bitdata, x + 1, y - 1);
                                    foreach (var item in checkIfPixelIsAlone)
                                    {
                                        if (item.Value == 0)
                                        {
                                            validNorthNeighborhood++;
                                        }
                                    }
                                }

                                if (validSouthNeighborhood != 0 && validNorthNeighborhood != 0)
                                {
                                    toPaint.Add(new ChaveValor<int, int> { Chave = x, Valor = y });
                                }
                            }
                        }
                    }
                }
            }

            foreach (var item in toPaint)
            {
                var paintAddress = (byte*)bitdata.Scan0 + (item.Valor * bitdata.Stride);
                paintAddress[item.Chave * pixelSize] = 0;
                paintAddress[item.Chave * pixelSize + 1] = 0;
                paintAddress[item.Chave * pixelSize + 2] = 0;
            }

            img.UnlockBits(bitdata);
        }

        private bool MultipleXor(IEnumerable<bool> tests)
        {
            return tests.Count(t => t) == 1;
        }

        private unsafe Dictionary<string, byte> PegarVizinhanca(BitmapData bitdata, int x, int y)
        {
            const int pixelSize = 4;
            var vizinhanca = new Dictionary<string, byte>();

            if (y >= 0 && y < bitdata.Height && x >= 0 && x < bitdata.Width)
            {
                if (y > 0)
                {
                    var north = (byte*)bitdata.Scan0 + ((y - 1) * bitdata.Stride);
                    vizinhanca.Add("north", north[x * pixelSize]);
                    if (x > 0)
                    {
                        vizinhanca.Add("northwest", north[(x - 1) * pixelSize]);
                    }
                    if (x < bitdata.Width - 1)
                    {
                        vizinhanca.Add("northeast", north[(x + 1) * pixelSize]);
                    }
                }
                if (y < bitdata.Height - 1)
                {
                    var south = (byte*)bitdata.Scan0 + ((y + 1) * bitdata.Stride);
                    vizinhanca.Add("south", south[x * pixelSize]);
                    if (x > 0)
                    {
                        vizinhanca.Add("southwest", south[(x - 1) * pixelSize]);
                    }
                    if (x < bitdata.Width - 1)
                    {
                        vizinhanca.Add("southeast", south[(x + 1) * pixelSize]);
                    }
                }
                if (x > 0)
                {
                    var lineY = (byte*)bitdata.Scan0 + (y * bitdata.Stride);
                    vizinhanca.Add("west", lineY[(x - 1) * pixelSize]);
                }
                if (x < bitdata.Width - 1)
                {
                    var lineY = (byte*)bitdata.Scan0 + (y * bitdata.Stride);
                    vizinhanca.Add("east", lineY[(x + 1) * pixelSize]);
                }
            }

            return vizinhanca;
        }

        private HashSet<ChaveValor<int, int>> VerificarVizinhanca(BitmapData bitdata, int x, int y,
                                                                  HashSet<ChaveValor<int, int>> toPaint, byte color)
        {
            var vizinhancaNordeste = PegarVizinhanca(bitdata, x + 1, y - 1);
            var vizinhancaSudoeste = PegarVizinhanca(bitdata, x - 1, y + 1);

            vizinhancaNordeste.Remove("southwest"); // Removo o pixel "atual" - já sei que ele é preto
            vizinhancaSudoeste.Remove("northeast"); // Removo o pixel "atual" - já sei que ele é preto

            var brk = false;
            foreach (var itemN in vizinhancaNordeste)
            {
                if (brk) break;
                if (itemN.Value == color && (new List<string> { "north", "northeast", "east" }).Contains(itemN.Key))
                {
                    foreach (var itemS in vizinhancaSudoeste)
                    {
                        if (itemS.Value == color &&
                            (new List<string> { "south", "southwest", "west" }).Contains(itemS.Key))
                        {
                            toPaint.Add(new ChaveValor<int, int> { Chave = x + 1, Valor = y - 1 });
                            toPaint.Add(new ChaveValor<int, int> { Chave = x - 1, Valor = y + 1 });

                            brk = true;
                            break;
                        }
                    }
                }
            }

            return toPaint;
        }

        public string Reconhecer()
        {
            var templates = ConfigurationManager.AppSettings["TemplatesC2"];

            var connected = ImgArray.InvertImageColors().ToBitmap();
            //TODO: Adaptar o método ConectarPixels para trabalhar com img convencional de fundo branco 
            ConectarPixels(connected);

            var invertedImageColors = new ImgArray(connected.InvertImageColors());

            var cfs = new ColorFillingSegmentation(invertedImageColors);

            var caracs = cfs.GetCaracs();
            var processedClusters = new List<Bitmap>();
            var clustersSize = new SortedList<int, byte>(new ServerUtil.ComparerWithDuplicates());

            var numClusters = 0;
            byte counter = 0;

            // Process clusters
            foreach (var cropped in
                from cluster in caracs
                where cluster.Height >= 18
                        && cluster.Width >= 3
                let minX = cluster.GetMinXNotBlack()
                let minY = cluster.GetMinYNotBlack()
                let maxX = cluster.GetMaxXNotBlack()
                let maxY = cluster.GetMaxYNotBlack()
                where (maxX - minX) > 0 && (maxY - minY) > 0
                let r = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1)
                select cluster.CropRectangle(r).RemoveBlackBorders(true).InvertImageColors())
            {
                // sujeira
                if (cropped.Height > 27)
                {
                    numClusters = -1;
                    break;
                }

                // cluster válido
                if (cropped.Height >= 18)
                {
                    processedClusters.Add(cropped);
                    clustersSize.Add(cropped.Width, counter++);
                }
            }

            if (numClusters != -1)
            {
                numClusters = processedClusters.Count;
            }

            var resposta = new char[5];
            var count = 0;
            string res;

            try
            {
                switch (numClusters)
                {
                    case 1:
                        {
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            foreach (var item in clustersSize)
                            {
                                var tm = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = tm.Match(1, item.Key, count++);

                                int indexer = item.Value;
                                foreach (var letra in result)
                                {
                                    if (letra != '\0' && indexer < 5)
                                    {
                                        if (resposta[indexer] == '\0')
                                        {
                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                        else
                                        {
                                            if (idx[indexer] > item.Value)
                                            {
                                                for (var i = resposta.Length - 1; i > indexer; i--)
                                                {
                                                    idx[i] = idx[i - 1];
                                                    resposta[i] = resposta[i - 1];
                                                }
                                            }
                                            else
                                            {
                                                do
                                                {
                                                    indexer++;
                                                } while (resposta[indexer] != '\0');
                                            }

                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                    }
                                }
                            }
                            //item.Clone(r, System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(Arq);
                            break;
                        }

                    case 2:
                        {
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            foreach (var item in clustersSize)
                            {
                                var tm = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = tm.Match(2, item.Key, count++, item.Value == 0 ? processedClusters[item.Value + 1].Width : processedClusters[item.Value - 1].Width);

                                int indexer = item.Value;
                                foreach (var letra in result)
                                {
                                    if (letra != '\0' && indexer < 5)
                                    {
                                        if (resposta[indexer] == '\0')
                                        {
                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                        else
                                        {
                                            if (idx[indexer] > item.Value)
                                            {
                                                for (var i = resposta.Length - 1; i > indexer; i--)
                                                {
                                                    idx[i] = idx[i - 1];
                                                    resposta[i] = resposta[i - 1];
                                                }
                                            }
                                            else
                                            {
                                                do
                                                {
                                                    indexer++;
                                                } while (resposta[indexer] != '\0');
                                            }

                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                    }
                                }
                            }
                            //item.Clone(r, System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(Arq);
                            break;
                        }

                    case 3:
                        {
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            foreach (var item in clustersSize)
                            {
                                var tm = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = tm.Match(3, item.Key, count++);

                                int indexer = item.Value;
                                foreach (var letra in result)
                                {
                                    if (letra != '\0' && indexer < 5)
                                    {
                                        if (resposta[indexer] == '\0')
                                        {
                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                        else
                                        {
                                            if (idx[indexer] > item.Value)
                                            {
                                                for (var i = resposta.Length - 1; i > indexer; i--)
                                                {
                                                    idx[i] = idx[i - 1];
                                                    resposta[i] = resposta[i - 1];
                                                }
                                            }
                                            else
                                            {
                                                do
                                                {
                                                    indexer++;
                                                } while (resposta[indexer] != '\0');
                                            }
                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                    }
                                }
                            }
                            //item.Clone(r, System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(Arq);
                            break;
                        }

                    case 4:
                        {
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            foreach (var item in clustersSize)
                            {
                                var tm = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = tm.Match(4, item.Key, count++);

                                int indexer = item.Value;
                                foreach (var letra in result)
                                {
                                    if (letra != '\0' && indexer < 5)
                                    {
                                        if (resposta[indexer] == '\0')
                                        {
                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                        else
                                        {
                                            if (idx[indexer] > item.Value)
                                            {
                                                for (var i = resposta.Length - 1; i > indexer; i--)
                                                {
                                                    idx[i] = idx[i - 1];
                                                    resposta[i] = resposta[i - 1];
                                                }
                                            }
                                            else
                                            {
                                                do
                                                {
                                                    indexer++;
                                                } while (resposta[indexer] != '\0');
                                            }

                                            idx[indexer] = item.Value;
                                            resposta[indexer++] = letra;
                                        }
                                    }
                                }
                            }
                            //item.Clone(r, System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(Arq);
                            break;
                        }

                    case 5:
                        {
                            foreach (var item in clustersSize)
                            {
                                var tm = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = tm.Match(5, item.Key, count++);

                                foreach (var letra in result.Where(letra => letra != '\0'))
                                {
                                    resposta[item.Value] = letra;
                                }
                            }
                            //item.Clone(r, System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(Arq);
                            break;
                        }

                    default:
                        {
                            resposta = new[] { 'X', 'X', 'X', 'X', 'X' };
                            break;
                        }
                }

                res = string.Empty;
                foreach (var item in resposta)
                {
                    if (Char.IsLetterOrDigit(item))
                    {
                        res += item;
                    }
                }
            }
            catch (Exception)
            {
                res = "XXXXX";
            }

            return res;
        }
    }
}