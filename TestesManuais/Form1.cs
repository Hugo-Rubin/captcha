using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Core.Common;
using Core.Common.Extensions;
using Core.Logic;
using Core.Logic.Captchas;
using Core.Logic.ImageLevels;
using Core.Logic.Obsolete;
using Core.Logic.Predict;
using Core.Logic.RemocaoFundo;
using Core.Logic.Types;
using PalleteQuantizer.Quantizers.XiaolinWu;
using TestesManuais.WS;

namespace TestesManuais
{
    public partial class Form1 : Form
    {
        private readonly string dirTeste = CustomConfigurationManager.ReadAppSetting("ImagesDir");

        private readonly Batch loteSintegraSP = new Batch(typeof(CaptchaSP), @"C:\SintegraSP", "SP");

        private const String PostUrlLocal = "http://localhost:50231/GetText.aspx";
        private const String PostUrlServidor = "http://www.ml-research.com/testes2Redes/GetText.aspx";
        private object webService;

        public Form1()
        {
            InitializeComponent();
        }

        private String ReconhecerCaracteresViaWebService(CaptchaRF captcha, object webService)
        {
            var result = "....";

            var nano = captcha.ImgArray.ToNanoArray().GetInternalArray();

            if (chkServidor.Checked)
            {
                result = (webService as OCRNFE).GetText(nano, captcha.Width, captcha.Height, "yWAmlOxGfMEiIz0FY58B");
            }
            else
            {
                //todo: corrigir chamada de metodo
                result = (webService as WSLocal.OCRNFE).GetText(null, captcha.Width, captcha.Height, "yWAmlOxGfMEiIz0FY58B");
            }
            return result;
        }

        public static string ArrayToStringGeneric<T>(IList<T> array, string delimeter)
        {
            var outputString = "";

            for (var i = 0; i < array.Count; i++)
            {
                if (array[i] is IList<T>)
                {
                    //Recursively convert nested arrays to string
                    outputString += ArrayToStringGeneric((IList<T>)array[i], delimeter);
                }
                else
                {
                    outputString += array[i];
                }

                if (i != array.Count - 1)
                    outputString += delimeter;
            }

            return outputString;
        }

        private static string HttpPost(string url, string[] paramName, string[] paramVal)
        {
            var req = WebRequest.Create(new Uri(url))
                      as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            // Build a string with all the params, properly encoded.
            // We assume that the arrays paramName and paramVal are
            // of equal length:
            var paramz = new StringBuilder();
            for (var i = 0; i < paramName.Length; i++)
            {
                paramz.Append(paramName[i]);
                paramz.Append("=");
                paramz.Append(paramVal[i]);
                paramz.Append("&");
            }

            // Encode the parameters as form data:
            var postData =
                Encoding.UTF8.GetBytes(paramz.ToString());
            req.ContentLength = postData.Length;

            // Send the request:
            using (var post = req.GetRequestStream())
            {
                post.Write(postData, 0, postData.Length);
            }

            // Pick up the response:
            string result = null;
            using (var resp = req.GetResponse()
                              as HttpWebResponse)
            {
                var reader =
                    new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }

        private void btnWSVetorizado_Click(object sender, EventArgs e)
        {
            #region

            if (listBox2.Items.Count > 0)
            {
                if (MessageBox.Show("Limpar resultados?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    listBox2.Items.Clear();
                }
                else
                {
                    listBox2.Items.Add("===== NOVA EXECUÇÃO =====");
                }
            }

            var di = new DirectoryInfo(dirTeste);

            var imagens = di.GetFiles("*.png");

            var wp = WebProxy.GetDefaultProxy();
            wp.UseDefaultCredentials = true;

            if (chkServidor.Checked)
            {
                webService = new OCRNFE();
                (webService as OCRNFE).Proxy = wp;
            }
            else
            {
                webService = new WSLocal.OCRNFE();
                (webService as WSLocal.OCRNFE).Proxy = wp;
            }

            #endregion

            foreach (var i in imagens)
            {
                try
                {
                    var captcha = new CaptchaRF(i.FullName);
                    pictureBox1.Image = captcha.ImgArray.ToBitmap();

                    var dt1 = DateTime.Now;

                    var palavra = ReconhecerCaracteresViaWebService(captcha, webService);

                    var dt2 = DateTime.Now;
                    var tempo = dt2 - dt1;
                    listBox2.Items.Add(String.Format("{0} - {1} \t {2}\n", i.Name, palavra, tempo));
                    Log.Append(String.Format("{0} \t {1}\n", palavra, tempo), "Executadas.txt");
                    listBox2.SelectedIndex = listBox2.Items.Count - 1;

                    if (chkMsg.Checked)
                    {
                        MessageBox.Show(palavra);
                    }
                    Application.DoEvents();
                }
                catch (Exception E)
                {
                    MessageBox.Show(String.Format("{0} : {1}", i.Name, E.Message));
                }
            }
        }

        private void btnPostCaptchaTipo1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                if (MessageBox.Show("Limpar resultados?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    listBox1.Items.Clear();
                }
                else
                {
                    listBox1.Items.Add("===== NOVA EXECUÇÃO =====");
                }
            }

            var di = new DirectoryInfo(dirTeste);

            var imagens = di.GetFiles("*.png");

            var postURL = chkServidor.Checked ? PostUrlServidor : PostUrlLocal;
            listBox1.Items.Add("Consultando em: " + postURL);

            foreach (var i in imagens)
            {
                try
                {
                    var captcha = new CaptchaRF(i.FullName);
                    pictureBox1.Image = captcha.ImgArray.ToBitmap();

                    var dt1 = DateTime.Now;

                    var strImg = ArrayToStringGeneric(captcha.ImgArray.ToNanoArray().GetInternalArray(), "x");


                    var palavra = HttpPost(postURL,
                                              new[] { "i", "w", "h", "n" },
                                              new[] { strImg, captcha.Width.ToString(), captcha.Height.ToString(), "true" }
                        );
                    var dt2 = DateTime.Now;
                    var tempo = dt2 - dt1;
                    listBox1.Items.Add(String.Format("{0} \t {1}\n", palavra, tempo));
                    Log.Append(String.Format("{0} \t {1}\n", palavra, tempo), "Executadas.txt");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;

                    if (chkMsg.Checked)
                    {
                        MessageBox.Show(palavra);
                    }

                    Application.DoEvents();
                }
                catch (Exception E)
                {
                    Log.Append(String.Format("{0} : {1}", i.Name, E.Message));
                }
            }
        }

        private void btnTesteLocal_Click(object sender, EventArgs e)
        {
            var rede = PredictCaptchaRF.Instance;


            if (listBox1.Items.Count > 0)
            {
                if (MessageBox.Show("Limpar resultados?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    listBox1.Items.Clear();
                }
                else
                {
                    listBox1.Items.Add("===== NOVA EXECUÇÃO =====");
                }
            }

            var di = new DirectoryInfo(dirTeste);

            var imagens = di.GetFiles("*.png");

            foreach (var i in imagens)
            {
                try
                {
                    var captcha = new CaptchaRF(i.FullName);
                    pictureBox1.Image = captcha.ImgArray.ToBitmap();

                    var dt1 = DateTime.Now;


                    var caracteres = captcha.GetCaracteres();
                    var palavra = rede.Recognize(caracteres);

                    var dt2 = DateTime.Now;
                    var tempo = dt2 - dt1;
                    listBox1.Items.Add(String.Format("{0} \t {1}\n", palavra, tempo));
                    Log.Append(String.Format("{0} \t {1}\n", palavra, tempo), "Executadas.txt");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;

                    if (chkMsg.Checked)
                    {
                        MessageBox.Show(palavra);
                    }

                    Application.DoEvents();
                }
                catch (Exception E)
                {
                    Log.Append(String.Format("{0} : {1}", i.Name, E.Message));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            //    Bitmap grayImage = filter.Apply((Bitmap)Image.FromFile(@"C:\Imagens SISCARGA\Captcha_Ssicarga18.tif"));
            //    grayImage.Save(@"C:\Gray.png");
            //    return;

            var di = new DirectoryInfo(@"D:\Arquivos e Pastas\Captcha\Siscarga");
            var imagens = di.GetFiles("*.tif");

            foreach (var img in imagens)
            {
                var c = new CaptchaSI(img.FullName);

                //c.ImgArray.Save(@"C:\Users\Hugo\" + img.Name.Replace(".tif", ".png"));

                var invertedImageColors = c.ImgArray.InvertImageColors();

                var cfs = new ColorFillingSegmentation(invertedImageColors);
                var labels = cfs.GetLabels();

                var carac = cfs.GetCaracs();

                var i = 1;
                foreach (var item in carac)
                {
                    var Arq = @"C:\Users\Hugo\kabuto\" + img.Name.Replace(".tif", " - " + i++.ToString() + ".png");
                    var minX = item.GetMinXNotBlack();
                    var minY = item.GetMinYNotBlack();
                    var maxX = item.GetMaxXNotBlack();
                    var maxY = item.GetMaxYNotBlack();
                    if ((maxX - minX) > 0 && (maxY - minY) > 0)
                    {
                        var r = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
                        var cropped = item.CropRectangle(r).RemoveBlackBorders(true);
                        cropped = cropped.InvertImageColors();

                        if (cropped.Height >= 18 && cropped.Height <= 27)
                        {
                            cropped.Save(Arq);
                        }
                        //item.Clone(r, System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(Arq);
                    }
                }

                /*int i = 1;
                foreach (Bitmap item in chars)
                {
                    String Arq = @"C:\SemFundo\Chars\" + img.Name.Replace(".tif", "-" + i.ToString() + ".png");
                    int minX = item.GetMinX();
                    int minY = item.GetMinY();
                    int maxX = item.GetMaxX();
                    int maxY = item.GetMaxY();
                    Rectangle r = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                    //item.Clone(r, System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(Arq);
                    item.Save(Arq);
                }*/
            }
            MessageBox.Show("Terminei");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var AL = new AdjustLevels(@"C:\Users\Hugo\Pictures\foto-raio.jpg", new[] { 86, 129, 200 }, new[] { 0, 255 });
            MessageBox.Show("Terminei");
        }

        /// <summary>
        ///   Salva o PixelIntensity dos Templates em arquivo
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void button3_Click(object sender, EventArgs e)
        {
            //BLL.ServerUtil.SalvarVetorDePixelsCaptchaTipo2(); // Siscarga
            ServerUtil.SalvarVetorDePixels(); // Sintegra

            var dirs =
                new DirectoryInfo(@"C:\Users\Hugo\DropBox\OCR\Testes\RJ\REDE_CLASSIFICADA").GetDirectories();
            var sb = new StringBuilder();

            foreach (var dir in dirs)
            {
                if (dir.Name[0] == '_')
                {
                    continue;
                }

                using (var sr = new StreamReader(dir.FullName + @"\vTemplates_" + dir.Name + ".txt"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            sb.Remove(sb.Length - 1, 1);
            var sw = new StreamWriter(@"C:\Users\Hugo\DropBox\OCR\Testes\RJ\REDE_CLASSIFICADA\Templates.txt");
            sw.Write(sb.ToString());
            sw.Close();
            sw.Dispose();
            GC.Collect();


            MessageBox.Show("Terminei");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var di = new DirectoryInfo(@"C:\Users\Hugo\kabuto").GetDirectories();

            foreach (var d in di)
            {
                if (d.Name[0] != '_')
                {
                    var dir = new DirectoryInfo(d.FullName + @"\Repetidas").GetFiles("*.png");
                    var templates = d.FullName + @"\vTemplates_" + d.Name + ".txt";
                    var sb = new StringBuilder();

                    foreach (var file in dir)
                    {
                        var image = ImgArray.LoadFromFile(file.FullName);
                        // @"C:\Users\Hugo\Templates\0\Repetidas\Captcha_Ssicarga329 - 3.png");
                        var TM = new TemplateMatching(image, templates);
                        // @"C:\Users\Hugo\Templates\0\vTemplates_0.txt");
                        var result = TM.Match(2, image.Width, 0);
                        //IEnumerable<char> result = TM.Match(2, image.Width, 0);
                    }

                    var TM1 = new TemplateMatching(new ImgArray(1, 1), templates);
                    sb = TM1.GetErrors();

                    var sw = new StreamWriter(@"C:\Users\Hugo\kabuto" + @"\" + "count_errors_" + d.Name + ".txt");
                    sw.Write(sb.ToString());
                    sw.Close();
                    sw.Dispose();

                    TM1.ClearErrors();
                }
            }


            /*ImgArray image = ImgArray.LoadFromFile(@"D:\Arquivos e Pastas\Captcha\Siscarga\Chars\Teste TM\img.png");
            string templates = @"C:\Users\Hugo\SemFundo\Templates.txt";
            TemplateMatching TM = new TemplateMatching(image, templates);
            IEnumerable<char> result = TM.Match();

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }*/

            MessageBox.Show("Terminei");
        }

        /// <summary>
        ///   Testa todos os módulos
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void button5_Click(object sender, EventArgs e)
        {
            //FileInfo[] files = new DirectoryInfo(@"D:\Arquivos e Pastas\Captcha\Siscarga\1000").GetFiles("*.tif");

            var ns = new ServerUtil.NumericComparer();

            var files = Directory.GetFiles(@"D:\Arquivos e Pastas\Captcha\Siscarga\1000", "*.tif");
            Array.Sort(files, ns);

            var sb = new StringBuilder();
            var templates = @"C:\Users\Hugo\kabuto\Templates.txt";

            foreach (var s in files)
            {
                var f = new FileInfo(s);
                var StartTime = DateTime.Now;
                var stopWatch = new Stopwatch();
                stopWatch.Start();


                // Bitmap img = (Bitmap)Bitmap.FromFile(@"D:\Arquivos e Pastas\Captcha\Siscarga\1000\Captcha_Ssicarga71.tif");
                var img = (Bitmap)Image.FromFile(f.FullName);
                //Bitmap img = (Bitmap)Bitmap.FromFile(@"D:\Arquivos e Pastas\Captcha\Siscarga\100\1 (87).tif");
                //Grayscale filter = new Grayscale(0.015, 0.25, 0.83); // 0, 0.098, 0.8 (inicial) - 0, 0.25, 0.8 (+claro) - 0, 0.25, 0.83 (+claro) - 0.015, 0.25, 0.83 (+claro)
                var grayImage = img.TransformToGrayscale(); // filter.Apply(img);

                var c = new CaptchaSI(grayImage);

                //c.ImgArray.Save(@"C:\Users\Hugo\" + img.Name.Replace(".tif", ".png"));
                var invertedImageColors = c.ImgArray.InvertImageColors();

                var cfs = new ColorFillingSegmentation(invertedImageColors);
                var labels = cfs.GetLabels();
                var caracs = cfs.GetCaracs();
                var processedClusters = new List<Bitmap>();
                //SortedList<int, byte> clusterSizePosition = new SortedList<int, byte>();
                var clustersSize = new SortedList<int, byte>(new ServerUtil.ComparerWithDuplicates());

                var numClusters = 0;
                byte counter = 0;

                // Process clusters
                foreach (var cluster in caracs)
                {
                    // cluster válido
                    if (cluster.Height >= 18 && cluster.Width >= 3)
                    {
                        var minX = cluster.GetMinXNotBlack();
                        var minY = cluster.GetMinYNotBlack();
                        var maxX = cluster.GetMaxXNotBlack();
                        var maxY = cluster.GetMaxYNotBlack();

                        if ((maxX - minX) > 0 && (maxY - minY) > 0)
                        {
                            var r = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
                            var cropped = cluster.CropRectangle(r).RemoveBlackBorders(true).InvertImageColors();

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
                    }
                }

                if (numClusters != -1)
                {
                    numClusters = processedClusters.Count;
                }

                // Tamanhos (em largura) do menor pro maior
                //clustersSize.Sort();

                var resposta = new char[5];
                var count = 0;

                /*foreach (var item in processedClusters)
                {
                    item.Save(@"C:\Users\Hugo\teste" + count++.ToString() + ".png");
                }
                count = 0*/

                switch (numClusters)
                {
                    case 1:
                        {
                            int indexer;
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            foreach (var item in clustersSize)
                            {
                                var TM = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = TM.Match(1, item.Key, count++);

                                indexer = item.Value;
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
                            int indexer;
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            IEnumerable<char> result;
                            foreach (var item in clustersSize)
                            {
                                var TM = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                if (item.Value == 0)
                                {
                                    result = TM.Match(2, item.Key, count++, processedClusters[item.Value + 1].Width);
                                }
                                else
                                {
                                    result = TM.Match(2, item.Key, count++, processedClusters[item.Value - 1].Width);
                                }

                                indexer = item.Value;
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
                            int indexer;
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            foreach (var item in clustersSize)
                            {
                                var TM = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = TM.Match(3, item.Key, count++);

                                indexer = item.Value;
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
                            int indexer;
                            var idx = new int[5];
                            Util.Init(idx, -1);
                            foreach (var item in clustersSize)
                            {
                                var TM = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = TM.Match(4, item.Key, count++);

                                indexer = item.Value;
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
                                var TM = new TemplateMatching(new ImgArray(processedClusters[item.Value]), templates);
                                IEnumerable<char> result = TM.Match(5, item.Key, count++);

                                foreach (var letra in result)
                                {
                                    if (letra != '\0')
                                    {
                                        resposta[item.Value] = letra;
                                    }
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

                stopWatch.Stop();

                var res = string.Empty;
                foreach (var item in resposta)
                {
                    if (Char.IsLetterOrDigit(item))
                    {
                        res += item;
                    }
                }

                //sb.AppendLine(f.Name + " -> " + res + " --|-- Tempo Gasto: " + ts.Duration().ToString());

                sb.AppendLine(res);

                //MessageBox.Show(new string(resposta), ts.Duration().ToString());
            }

            var sw = new StreamWriter(@"C:\Users\Hugo\Teste 2010-07-03 1000.txt");
            sw.Write(sb.ToString());
            sw.Close();
            sw.Dispose();

            MessageBox.Show("Concluído.");
        }

        /// <summary>
        ///   Cria as pastas para os Templates
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void button6_Click(object sender, EventArgs e)
        {
            var di = new DirectoryInfo(@"C:\Users\Hugo\DropBox\OCR\SintegraSP\Todas\Predicted");
            //@"C:\Users\Hugo\kabuto");

            // Cria as pastas A-Z, 0-9, _lixo
            for (var c = 'A'; c <= 'z'; c++)
            {
                if (char.IsSymbol(c) || c == 'l' || c == 'o' || c == 'C' || c == 'K' || c == 'L' || c == 'O' || c == 'S' ||
                    c == 'V' || c == 'W' || c == 'X' || c == 'Z')
                {
                    continue;
                }
                if (char.IsUpper(c))
                {
                    Directory.CreateDirectory(di.FullName + @"\" + c + c);
                    continue;
                }
                Directory.CreateDirectory(di.FullName + @"\" + c);
                //Directory.CreateDirectory(di.FullName + @"\" + c + @"\Repetidas");
            }
            for (var i = 1; i <= 9; i++)
            {
                Directory.CreateDirectory(di.FullName + @"\" + i);
                //Directory.CreateDirectory(di.FullName + @"\" + i + @"\Repetidas");
            }
            Directory.CreateDirectory(di.FullName + @"\" + "_lixo");

            MessageBox.Show("Pastas criadas em:\n" + di.FullName);
        }

        /// <summary>
        ///   Deleta arquivos replicados em determinadas pastas de uma pasta alvo
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void button7_Click(object sender, EventArgs e)
        {
            var pastasOcupadas = new[]
                                     {
                                         new DirectoryInfo(@"C:\Users\Hugo\SemFundo\W"),
                                         new DirectoryInfo(@"C:\Users\Hugo\SemFundo\X"),
                                         new DirectoryInfo(@"C:\Users\Hugo\SemFundo\Y"),
                                         new DirectoryInfo(@"C:\Users\Hugo\SemFundo\Z")
                                     };
            var root = new DirectoryInfo(@"C:\Users\Hugo\SemFundo");
            var rootFiles = root.GetFiles();

            foreach (var pasta in pastasOcupadas)
            {
                var fi = pasta.GetFiles();
                foreach (var file in fi)
                {
                    try
                    {
                        File.Delete(root + @"\" + file.Name);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            MessageBox.Show("Arquivos excluídos com sucesso!");
        }

        /// <summary>
        ///   Copia arquivos existentes e já organizados em pastas em outro diretório de uma pasta raiz onde eles não estão organizados para suas subpastas
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void button8_Click(object sender, EventArgs e)
        {
            /*DirectoryInfo[] d = new DirectoryInfo(@"C:\Users\Hugo\kabuto").GetDirectories();
            foreach (var dir in d)
            {
                if (dir.Name[0] != '_')
                {
                    FileInfo[] f = dir.GetFiles("*.png");
                    foreach (var file in f)
                    {
                        try
                        {
                            File.Move(file.FullName, dir.FullName + @"\Repetidas\" + file.Name);
                        }
                        catch (Exception) { }
                    }
                }
            }*/

            var raizOrigem = new DirectoryInfo(@"D:\DEV\Resources\CaptchaTipo2\Chars");
            var raizDestino = new DirectoryInfo(@"C:\Users\Hugo\SemFundo");

            var subOrigem = raizOrigem.GetDirectories();

            foreach (var pastaOrigem in subOrigem)
            {
                var arqsOrigem = pastaOrigem.GetFiles();
                foreach (var arqs in arqsOrigem)
                {
                    try
                    {
                        Func<char, bool> procuraTraco = x => x == '-';
                        var idx = Array.IndexOf(arqs.Name.ToCharArray(), arqs.Name.First(procuraTraco));
                        var nome = arqs.Name.Substring(0, idx) + " " + arqs.Name[idx] + " " +
                                      arqs.Name.Substring(idx + 1, (arqs.Name.Length) - (idx + 1));
                        File.Copy(raizDestino.FullName + @"\" + nome,
                                  raizDestino.FullName + @"\" + pastaOrigem + @"\" + nome);
                        File.Delete(raizDestino.FullName + @"\" + nome);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            MessageBox.Show("Arquivos movidos com sucesso!");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var files = new DirectoryInfo(@"D:\Arquivos e Pastas\Captcha\Siscarga").GetFiles("*.tif");

            foreach (var file in files)
            {
                var img = (Bitmap)Image.FromFile(file.FullName);
                var c = new CaptchaSI(img);

                //Grayscale filter = new Grayscale(0.13333, 0.13333, 0.06666); // 0, 0.098, 0.8 (inicial) - 0, 0.25, 0.8 (+claro) - 0, 0.25, 0.83 (+claro) - 0.015, 0.25, 0.83 (+claro) - 0.015, 0.25, 0.73 (adjusted) - (0.4, 0.4, 0.2)
                //Bitmap bness = AdjustBrightness(grayimage, 105);
                //Bitmap ctrast = AdjustContrast(bness, 5);

                //ImgArray bwImage = new ImgArray(img.MakeGrayscale().Otsu()); //filter.Apply(img);
                c.ImgArray.Save(@"C:\Users\Hugo\BG Removal\newModelImproved2\" + "BW " +
                                file.Name.Substring(0, file.Name.Length - 4) + ".png");
            }

            MessageBox.Show("Imagens salvas em:\n" + @"C:\Users\Hugo\BG Removal\newModelImproved2\");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //string[] dicionario = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "_lixo" }; // siscarga
            var dicionario = new[]
                                 {
                                     "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "AA", "b", "BB", "c", "d", "DD",
                                     "diversos",
                                     "e", "EE", "f", "FF", "g", "GG", "h", "HH", "i", "II", "j", "JJ", "k", "L", "m",
                                     "MM",
                                     "n", "NN", "p", "PP", "q", "QQ", "r", "RR", "s", "t", "TT", "u", "UU", "v", "w",
                                     "x",
                                     "y", "YY", "z"
                                 }; // sintegra/sp

            var diDestino = new DirectoryInfo(@"C:\Users\Hugo\DropBox\OCR\SintegraSP\Todas");
            var diOrigem = new DirectoryInfo(@"D:\Arquivos e Pastas\Captcha\Sintegra\SP").GetDirectories();
            var copiarPara = diDestino.FullName;

            var separados = new FileInfo[diOrigem.Length][];

            var counter = 0;
            foreach (var item in diOrigem)
            {
                separados[counter] = new FileInfo[item.GetFiles("*.png").Length];
                separados[counter] = item.GetFiles("*.png");
                counter++;
            }

            for (var i = 0; i < dicionario.Length; i++)
            {
                if (!Directory.Exists(copiarPara + @"\" + dicionario[i]))
                {
                    Directory.CreateDirectory(copiarPara + @"\" + dicionario[i]);
                }
            }

            for (var i = 0; i < separados.GetLength(0); i++)
            {
                for (var j = 0; j < separados[i].Length; j++)
                {
                    try
                    {
                        var arquivo = separados[i][j];
                        //string origem = copiarPara + @"\" + dicionario[i] + @"\Repetidas\" + arquivo.Name;
                        var origem = copiarPara + @"\" + arquivo.Name;
                        var destino = copiarPara + @"\" + dicionario[i] + @"\" + arquivo.Name;
                        File.Move(origem, destino);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            MessageBox.Show("Fim.");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var b = (Bitmap)Image.FromFile(@"C:\Users\Hugo\Templates\W\Repetidas\Captcha_Ssicarga902 - 1.png");
            b.RemoveWhiteBorders(); // TODO: Validar esse cara. Antes chamava o RWB com o parâmetro crop.

            var di = new DirectoryInfo(@"C:\Users\Hugo\Templates").GetDirectories();

            foreach (var d in di)
            {
                if (d.Name != "_lixo")
                {
                    var dir = new DirectoryInfo(d.FullName + @"\Repetidas").GetFiles("*.png");
                    var sb = new StringBuilder();

                    foreach (var file in dir)
                    {
                        sb.AppendLine(file.Name);
                    }
                    var sw1 =
                        new StreamWriter(@"C:\Users\Hugo\Templates" + @"\" + "count_errors_" + d.Name + "_IMAGENS.txt");
                    sw1.Write(sb.ToString());
                    sw1.Close();
                    sw1.Dispose();
                }
            }
        }

        private void btnSepararSintegraSP_Click(object sender, EventArgs e)
        {
            var dt = DateTime.Now;
            Text = String.Format("Inicio do processo: {0}", dt);
            Application.DoEvents();
            var resp = loteSintegraSP.SepararCaracteres();
            var erros = resp.Chave;
            var tempoMedio = resp.Valor;
            MessageBox.Show(String.Format(
                "Concluído com pelo menos {0} possíveis erros. Tempo médio da separação: {1}", erros, tempoMedio));
        }

        private void btnVerificarSintegraSP_Click(object sender, EventArgs e)
        {
            //TimeSpan tempoMedio = LoteSintegraSP.ValidarRemocaoDeFundo();
            loteSintegraSP.ChecarErrosSeparacao();
            MessageBox.Show("Processo concluído."); // Tempo médio: " + tempoMedio.ToString());
        }

        private void btnSepararSintegraRJ_Click(object sender, EventArgs e)
        {
            var tempoMedio = loteSintegraSP.ValidarRemocaoDeFundo();
            MessageBox.Show("Processo concluído. Tempo médio: " + tempoMedio.ToString());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var c = new CaptchaSP(@"C:\sintegraSP.png");
            var caracteres = c.GetCaracteres();
            int i = 0;
            foreach (var item in caracteres.Where(obj => obj != null))
            {
                item.Save(String.Format(@"D:\{0}.jpg", i++));
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string lineOCR, lineCertas;
            var matchingResults = 0;
            var matchingCharacters = 0;
            var wrongCharacters = 0;
            var imagesCount = 0;
            var bgRemovalErrors = 0;

            using (StreamReader respostasOCR = new StreamReader(@"C:\Users\Hugo\Teste 2010-07-03 1000.txt"),
                                respostasCertas = new StreamReader(@"C:\Users\Hugo\RESPOSTAS_SisC1000.txt"))
            {
                while ((lineOCR = respostasOCR.ReadLine()) != null && (lineCertas = respostasCertas.ReadLine()) != null)
                {
                    imagesCount++;

                    int sizeOCR = lineOCR.Length, sizeCertas = lineCertas.Length;
                    var smallestSize = sizeOCR > sizeCertas ? sizeCertas : sizeOCR;

                    if (lineOCR == "XXXXX")
                    {
                        bgRemovalErrors++;
                        continue;
                    }

                    for (var i = 0; i < smallestSize; i++)
                    {
                        if (i == 4)
                        {
                            if (sizeCertas != sizeOCR)
                            {
                                wrongCharacters++;
                                break;
                            }
                        }
                        if (lineOCR[i] == lineCertas[i])
                        {
                            matchingCharacters++;
                        }
                        else
                        {
                            wrongCharacters++;
                        }
                    }

                    if (lineOCR.Equals(lineCertas, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingResults++;
                    }
                }
            }

            MessageBox.Show("Efficiency: " + String.Format("{0:F2}", ((double)matchingResults / imagesCount) * 100)
                            + " %\n" + matchingResults + " out of " + imagesCount + " images were identified rightly."
                            + "\n" + "Background removal errors: " + bgRemovalErrors
                            + "\n" + "Total number of characters: " + (wrongCharacters + matchingCharacters)
                            + "\n" + "# Characters identified correctly: " + matchingCharacters
                            + "\n" + "# Characters identified wrongly: " + wrongCharacters);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            var di = new DirectoryInfo(@"D:\Arquivos e Pastas\Captcha\Sintegra\SP");
            var di2 = di.GetDirectories();
            var s = new StringBuilder();
            Color c;
            char limp;
            int luminance;

            var vImg = di.FullName + @"\PixInt_SP.txt";

            foreach (var folder in di2)
            {
                if (folder.Name == "diversos")
                {
                    continue;
                }

                var imagens = folder.GetFiles("*.png");

                foreach (var i in imagens)
                {
                    var m = new double[1, 3600]; //[1, 3600] para imagens 60 x 60, [1, 18000] para imagens 200 x 90
                    var img = Image.FromFile(i.FullName);
                    var bmp = (Bitmap)img;
                    var k = 0;
                    for (var y = 0; y < bmp.Height; y++)
                    {
                        for (var x = 0; x < bmp.Width; x++)
                        {
                            c = bmp.GetPixel(x, y);
                            luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                            m[0, k] = luminance / 255;
                            s.Append(string.Format("{0};", m[0, k]));
                            k++;
                        }
                    }

                    limp = CharacterSymbol(folder.Name);
                    s.Append(string.Format("{0};\n", limp));
                    GC.Collect();
                }
            }

            var sw = new StreamWriter(vImg);
            sw.Write(s.ToString());
            sw.Close();
            sw.Dispose();
            MessageBox.Show("Concluído");
        }

        private char CharacterSymbol(string s)
        {
            return s[0];
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string Yline;
            char firstCharac;
            var location = @"D:\Backup\Todas\Predicted\";
            var sb = new StringBuilder();

            var dicio = new[]
                            {
                                '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd',
                                'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r',
                                's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'D', 'E',
                                'F', 'G', 'H', 'I', 'J', 'M', 'N', 'P', 'Q', 'R', 'T', 'U', 'Y'
                            };

            using (var Yfile = new StreamReader(location + "y.txt"))
            {
                while ((Yline = Yfile.ReadLine()) != null)
                {
                    firstCharac = Yline[0];
                    if (firstCharac == 'C' || firstCharac == 'K' || firstCharac == 'S' || firstCharac == 'V' ||
                        firstCharac == 'W' || firstCharac == 'X' || firstCharac == 'Z')
                    {
                        firstCharac = Char.ToLower(firstCharac);
                    }

                    sb.AppendLine((Array.IndexOf(dicio, firstCharac) + 1).ToString());
                }
            }

            var translatedFile = new StreamWriter(location + "y_translated.txt");
            translatedFile.Write(sb.ToString());
            translatedFile.Close();
            translatedFile.Dispose();

            MessageBox.Show("Concluído");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            var rootFolder = new DirectoryInfo(@"C:\OCR\Testes\RF\Separados").GetDirectories();
            var destination = new DirectoryInfo(@"C:\OCR\Testes\RF\Classificar2");
            var count = 1;
            var countCaptcha = 1;

            if (!Directory.Exists(destination.FullName))
            {
                Directory.CreateDirectory(destination.FullName);
            }

            try
            {
                foreach (var folder in rootFolder)
                {
                    var files = folder.GetFiles();
                    foreach (var file in files)
                    {
                        if (file.Name == "Captcha.png")
                        {
                            File.Copy(file.FullName,
                                      destination.FullName + @"\" + count.ToString() + " (" + countCaptcha++ + ").png");
                            continue;
                        }
                        File.Copy(file.FullName, destination.FullName + @"\" + count++.ToString() + ".png");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show("Arquivos extraídos com sucesso.");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ServerUtil.RenomearImagens('p');
        }


        private void btnBatchReconhecer_Click(object sender, EventArgs e)
        {
            var result = loteSintegraSP.Reconhecer(typeof(PredictCaptchaSP));
            MessageBox.Show("Tempo médio do CaptchaTipo3: " + result.Chave.ToString());
            Log.GravarLinhasEmArquivo(@"C:\CaptchaTipo3.txt", result.Valor, true);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            var img = (Bitmap)Image.FromFile(@"C:\Users\Hugo\DropBox\OCR\Testes\RJ\00078.png");

            var wu = new WuColorQuantizer();
            var pq = new MyPalleteQuantizer(img, wu, 16);
            var result = pq.ApplyFilter();
            result.Save(@"C:\Users\Hugo\NOVA_2.jpg");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            var img = new Bitmap(@"D:\Arquivos e Pastas\Captcha\Sintegra\AM2.jpg");
            img.TransformToGrayscale().Otsu().InvertImageColors().Save(@"C:\Users\Hugo\am1.jpg");
            img.TransformToGrayscale().InvertImageColors().Otsu().Save(@"C:\Users\Hugo\am3.jpg");
            img.TransformToGrayscale().Otsu().Save(@"C:\Users\Hugo\am2.jpg");
        }

        private void OrdenarCarac_Click(object sender, EventArgs e)
        {
            ServerUtil.OrderByPixels();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            var sImagens = @"C:\OCR\Testes\RF2\Imagens";

            if (!Directory.Exists(sImagens))
            {
                MessageBox.Show("Coloque as imagens a serem tratadas em:\n" + sImagens);
                return;
            }

            var dImagens = new DirectoryInfo(@"C:\OCR\Testes\RF2\Imagens");
            var dTratadas = @"C:\OCR\Testes\RF2\Imagens\Tratadas 3x3";

            if (!Directory.Exists(dTratadas))
            {
                Directory.CreateDirectory(dTratadas);
            }

            var imagens = dImagens.GetFiles("*.png");

            foreach (var im in imagens)
            {
                var bmp = (Bitmap)Image.FromFile(im.FullName);
                var km = new KMeans(2, bmp);
                bmp = km.Apply();

                for (var y = 0; y < bmp.Height; y++)
                {
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        var c = bmp.GetPixel(x, y);
                        if (!ServerUtil.IsBlackPixel(c))
                        {
                            bmp.SetPixel(x, y, Color.White);
                        }
                    }
                }

                var img = new ImgArray(bmp.InvertImageColors());
                var er = new Erosion(img);
                var er2 = new Erosion(er.Result);

                er2.Result.InvertImageColors().Save(dTratadas + "\\" + im.Name);
            }
        }

        /// <summary>
        ///   Copia arquivos de uma pasta para outra, caso exista uma pasta no destino com o mesmo nome do arquivo na pasta de origem
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void copyFilesInFolder_Click(object sender, EventArgs e)
        {
            var fi = new DirectoryInfo(@"C:\OCR\Testes\RF").GetFiles();
            var di = new DirectoryInfo(@"C:\OCR\Testes\RF\Separados").GetDirectories();

            foreach (var dir in di)
            {
                var file = Array.Find(fi,
                                           element =>
                                           (element.Name.Substring(0, element.Name.Length - 4)).Equals(dir.Name));
                if (file != null && Char.IsDigit(dir.Name[0]))
                {
                    file.CopyTo(dir.FullName + @"\Captcha.png");
                }
            }

            MessageBox.Show("Fim");
        }
    }
}