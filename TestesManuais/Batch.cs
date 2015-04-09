using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Core.Common.Extensions;
using Core.Logic;
using Core.Logic.Captchas.Abstract;
using Core.Logic.ImageQuantizer.Quantizers.XiaolinWu;
using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace TestesManuais
{
    /// <summary>
    ///   <    /// Esta classe auxilia no desenvolvimento de novos anti Captchas pois faz o processamento em Lote que é
    ///     normalmente executado nos testes iniciais e no treinamento de rede/ criação dos templatesB.
    /// </summary>
    public class Batch
    {
        #region Delegates

        public delegate void ProcessandoDelegate(int total, int processados);

        public delegate void ReconheceuDelegate(string imgPath, string resposta, string padrao);

        #endregion

        private const string FORMATOS = "*.png";
        private readonly String baseDir;

        private readonly Type captchaType;
        private Captcha captcha;
        private TimeSpan maiorTempo;
        private TimeSpan menorTempo;
        private int processados;
        private bool running;
        private String servico;

        private int total;

        /// <summary>
        ///   Construtor padrão
        /// </summary>
        /// param name="captchaType">Exemplo: typeof(CaptchaRf)
        /// </param>
        /// <param name="directory"> Diretório com as imagens dos captchas </param>
        public Batch(Type captchaType, String directory, String servico)
        {
            this.servico = servico;
            this.captchaType = captchaType;
            baseDir = (directory + @"\").Replace(@"\\", @"\");
            running = false;
            menorTempo = new TimeSpan(10, 0, 0);
            maiorTempo = new TimeSpan(0);
            if (Directory.Exists(baseDir))
            {
                var di = new DirectoryInfo(baseDir);
                total = di.GetFiles(FORMATOS).Count();
            }
        }

        public int Total
        {
            get { return total; }
        }

        public TimeSpan MenorTempo
        {
            get { return menorTempo; }
        }

        public TimeSpan MaiorTempo
        {
            get { return maiorTempo; }
        }

        public bool Running
        {
            get { return running; }
        }

        public event ReconheceuDelegate Reconheceu;

        public event ProcessandoDelegate Processando;

        private Captcha CreateCaptchaInstance(String filename)
        {
            return (Captcha) Activator.CreateInstance(captchaType, new[] {filename});
        }

        /// <summary>
        ///   Cria uma pasta para cada imagem com um PNG para cada caracter
        /// </summary>
        /// <returns> return.Chave = Erros encontrados; return.Valor = Tempo decorrido </returns>
        public ChaveValor<int, TimeSpan> SepararCaracteres(bool ExcluirPasta = false)
        {
            var Erros = 0;
            running = true;

            var di = new DirectoryInfo(baseDir);
            var files = di.GetFiles(FORMATOS);
            var times = new List<TimeSpan>();
            menorTempo = new TimeSpan(9, 0, 0);
            maiorTempo = new TimeSpan(0);

            total = files.Count();
            processados = 0;
            if (Processando != null)
            {
                Processando(total, processados);
            }

            var separadosPath = baseDir + @"Separados\";
            if (ExcluirPasta && Directory.Exists(separadosPath))
            {
                Directory.Delete(separadosPath, true);
            }

            foreach (var file in files)
            {
                separadosPath = baseDir + @"Separados\" + file.Name.Replace(".png", "") + @"\";

                if (!Directory.Exists(separadosPath))
                {
                    Directory.CreateDirectory(separadosPath);

                    captcha = CreateCaptchaInstance(file.FullName);
                    if (null == captcha.ImgArray)
                    {
                        continue;
                    }
                    captcha.ImgArray.Save(separadosPath + "Captcha.png");
                    var dt2 = DateTime.Now;
                    var caracteres = captcha.GetCaracteres();
                    times.Add(contabilizarTempo(DateTime.Now - dt2));

                    caracteres.SalvarTodos(separadosPath);
                    

                    if (caracteres.Count() < captcha.NumeroMinimoDeLetras)
                    {
                        Erros++;
                    }
                    if (Processando != null)
                    {
                        Processando(total, ++processados);
                    }
                }
            }

            var avg = TimeSpan.FromMilliseconds(0);
            if (times.Count >= 1)
            {
                for (var i = 0; i < times.Count; i++)
                {
                    avg += times[i];
                }
                avg = TimeSpan.FromMilliseconds(avg.TotalMilliseconds/times.Count);
            }

            running = false;
            return new ChaveValor<int, TimeSpan>
                       {
                           Chave = Erros,
                           Valor = avg
                       };
        }

        private TimeSpan contabilizarTempo(TimeSpan tempo)
        {
            if (tempo < menorTempo)
            {
                menorTempo = tempo;
            }
            if (tempo > maiorTempo)
            {
                maiorTempo = tempo;
            }
            return tempo;
        }


        /// <summary>
        ///   Varre a pasta com caracteres separados à procura de possiveis erros na separação
        /// </summary>
        public void ChecarErrosSeparacao()
        {
            running = true;
            var errosPath = baseDir + @"Separados\Erros\";
            if (!Directory.Exists(errosPath))
            {
                Directory.CreateDirectory(errosPath);
            }
            var di = new DirectoryInfo(baseDir);
            var files = di.GetFiles(FORMATOS);

            total = files.Count();
            processados = 0;
            if (Processando != null)
            {
                Processando(total, processados);
            }
            foreach (var file in files)
            {
                var separadosPath = baseDir + @"Separados\" + file.Name.Replace(".png", "") + @"\";
                var di2 = new DirectoryInfo(separadosPath);
                var imgs = di2.GetFiles(FORMATOS);

                if (captcha == null)
                {
                    captcha = CreateCaptchaInstance(file.FullName);
                }

                if (imgs.Count() == captcha.NumeroMinimoDeLetras)
                {
                    var erro = false;
                    foreach (var item in imgs)
                    {
                        if (!erro
                            &&
                            ((Bitmap) BitmapUtils.LoadImageWithoutLockFile(item.FullName)).CountPixelsWithColor(
                                Color.Black) < captcha.NumeroMinimoDePixelsEmCluster)
                        {
                            File.Copy(file.FullName, errosPath + file.Name, true);
                            erro = true;
                        }
                    }
                }
                else
                {
                    File.Copy(file.FullName, errosPath + file.Name, true);
                }
                if (Processando != null)
                {
                    Processando(total, ++processados);
                }
            }
            running = false;
        }

        /// <summary>
        ///   Classifica os caracteres em pastas de acordo com a resposta do predict
        /// </summary>
        /// <param name="predictType"> </param>
        public TimeSpan ClassificarCaracteres(Type predictType)
        {
            running = true;
            var di = new DirectoryInfo(baseDir);
            var files = di.GetFiles(FORMATOS);
            var redePath = baseDir + @"Rede\";
            var erros = new List<String>();

            if (Directory.Exists(baseDir + @"Separados\Erros"))
            {
                var diErros = new DirectoryInfo(baseDir + @"Separados\Erros");
                var arqErros = diErros.GetFiles(FORMATOS);

                foreach (var item in arqErros)
                {
                    erros.Add(item.Name);
                }
            }
            var times = new List<TimeSpan>();
            menorTempo = new TimeSpan(9, 0, 0);
            maiorTempo = new TimeSpan(0);

            total = files.Count() - erros.Count();
            processados = 0;

            if (Processando != null)
            {
                Processando(total, processados);
            }

            var p = GetPredictInstance(predictType);

            foreach (var file in files)
            {
                if (erros.IndexOf(file.Name) < 0)
                {
                    var NomeArquivoCaptcha = file.Name.Replace(".png", "");
                    var separadosPath = baseDir + @"Separados\" + NomeArquivoCaptcha + @"\";

                    captcha = CreateCaptchaInstance(file.FullName);
                    if (null == captcha.ImgArray)
                    {
                        continue;
                    }

                    var caracteres = captcha.GetCaracteres().ToArray();

                    var dt = DateTime.Now;
                    var resposta = p.Recognize(caracteres);
                    times.Add(contabilizarTempo(DateTime.Now - dt));

                    for (var i = 0; i < resposta.Length; i++)
                    {
                        var nomeDaPasta = resposta[i].ToString();
                        if (Char.IsUpper(resposta[i]))
                        {
                            nomeDaPasta += resposta[i];
                        }
                        var dir = redePath + nomeDaPasta;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        caracteres[i].Save(String.Format("{0}\\{1}-{2}.png", dir, NomeArquivoCaptcha, i));
                    }

                    if (Processando != null)
                    {
                        Processando(total, ++processados);
                    }
                }
            }

            var avg = TimeSpan.FromMilliseconds(0);
            if (times.Count >= 1)
            {
                for (var i = 0; i < times.Count; i++)
                {
                    avg += times[i];
                }
                avg = TimeSpan.FromMilliseconds(avg.TotalMilliseconds/times.Count);
            }

            running = false;
            return avg;
        }

        /// <summary>
        ///   Classifica os caracteres em pastas de acordo com a resposta do predict
        /// </summary>
        /// <param name="predictType"> </param>
        public TimeSpan ClassificarCaracteresBaseadoNaPastaRede(Type predictType)
        {
            running = true;
            var di = new DirectoryInfo(baseDir + @"Separados\");
            var files = di.GetFiles(FORMATOS);


            var redePath = baseDir + @"Rede\";
            var erros = new List<String>();

            if (Directory.Exists(baseDir + @"Separados\Erros"))
            {
                var diErros = new DirectoryInfo(baseDir + @"Separados\Erros");
                var arqErros = diErros.GetFiles(FORMATOS);

                foreach (var item in arqErros)
                {
                    erros.Add(item.Name);
                }
            } /**/
            var times = new List<TimeSpan>();
            menorTempo = new TimeSpan(9, 0, 0);
            maiorTempo = new TimeSpan(0);

            total = files.Count(); // -erros.Count();
            processados = 0;

            if (Processando != null)
            {
                Processando(total, processados);
            }

            var cont = 0;
            foreach (var file in files)
            {
                //if (erros.IndexOf(file.Name) < 0)
                //{
                //  String separadosPath = baseDir + @"Separados\" + file.Name.Replace(".png", "") + @"\";

                //this.captcha = CreateCaptchaInstance(file.FullName);
                //Bitmap[] caracteres = captcha.GetCaracteres();

                var p = (IPredict) Activator.CreateInstance(predictType);
                var dt = DateTime.Now;

                var bmp = (Bitmap) BitmapUtils.LoadImageWithoutLockFile(file.FullName);
                var imgArray = new ImgArray(bmp);
                var resposta = p.Recognize(imgArray);
                times.Add(contabilizarTempo(DateTime.Now - dt));

                var nomeDaPasta = resposta.ToString();
                if (Char.IsUpper(resposta))
                {
                    nomeDaPasta += resposta;
                }
                var dir = baseDir + @"Rede\" + nomeDaPasta;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                imgArray.Save(String.Format("{0}\\{1}.png", dir, cont++));

                if (Processando != null)
                {
                    Processando(total, ++processados);
                }
                //}
            }

            var avg = TimeSpan.FromMilliseconds(0);
            if (times.Count >= 1)
            {
                for (var i = 0; i < times.Count; i++)
                {
                    avg += times[i];
                }
                avg = TimeSpan.FromMilliseconds(avg.TotalMilliseconds/times.Count);
            }
            running = false;
            return avg;
        }

        /// <summary>
        ///   Salva a imagem original ao lado da imagem com o fundo removido
        /// </summary>
        public TimeSpan ValidarRemocaoDeFundo()
        {
            running = true;
            var di = new DirectoryInfo(baseDir);
            var files = di.GetFiles(FORMATOS);
            var semFundoPath = baseDir + @"SemFundo\";
            if (!Directory.Exists(semFundoPath))
            {
                Directory.CreateDirectory(semFundoPath);
            }

            var times = new List<TimeSpan>();
            menorTempo = new TimeSpan(9, 0, 0);
            maiorTempo = new TimeSpan(0);

            processados = 0;
            total = files.Count();

            if (Processando != null)
            {
                Processando(total, processados);
            }

            foreach (var file in files)
            {
                var dt = DateTime.Now;
                var c = CreateCaptchaInstance(file.FullName);
                times.Add(contabilizarTempo(DateTime.Now - dt));
                var b = new Bitmap(c.Width*2, c.Height);
                var g = Graphics.FromImage(b);

                var bmp = (Bitmap) BitmapUtils.LoadImageWithoutLockFile(file.FullName);
                //var wu = new WuColorQuantizer();
                //var pq = new PalleteQuantizer(bmp, wu, 16);
                //bmp = (Bitmap) pq.ApplyFilter();


                g.DrawImage(bmp, 0, 0);

                g.DrawImage(c.ImgArray.ToBitmap(), c.Width + 1, 0);
                b.Save(String.Format(@"{0}{1}", semFundoPath, file.Name), ImageFormat.Png);

                if (Processando != null)
                {
                    Processando(total, ++processados);
                }
            }

            var avg = TimeSpan.FromMilliseconds(0);
            if (times.Count >= 1)
            {
                for (var i = 0; i < times.Count; i++)
                {
                    avg += times[i];
                }
                avg = TimeSpan.FromMilliseconds(avg.TotalMilliseconds/times.Count);
            }
            running = false;
            return avg;
        }

        public ChaveValor<TimeSpan, List<String>> TestarWebService(String servico, bool testarServidorRemoto = false,
                                                                   int sleep = 0)
        {
            running = true;
            var di = new DirectoryInfo(baseDir);
            var files = di.GetFiles(FORMATOS);

            var times = new List<TimeSpan>();
            menorTempo = new TimeSpan(9, 0, 0);
            maiorTempo = new TimeSpan(0);

            var respostas = new List<String>();

            total = files.Count();
            processados = 0;

            if (Processando != null)
            {
                Processando(total, processados);
            }

            foreach (var file in files)
            {
                var dt = DateTime.Now;
                var consulta = new ConsultaCaptcha(testarServidorRemoto);
                consulta.CarregarCaptcha(file.FullName);
                //TODO: Carregar arquivo de token
                var resposta = consulta.ReconhecerCaptcha(servico, "yWAmlOxGfMEiIz0FY58B");

                respostas.Add(String.Format("{0};{1};{2}", file.Name, resposta, ""));
                times.Add(contabilizarTempo(DateTime.Now - dt));
                if (Reconheceu != null)
                {
                    Reconheceu(file.Name, resposta, "Nao disponivel");
                }

                if (Processando != null)
                {
                    Processando(total, ++processados);
                }

                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }
            }

            var avg = TimeSpan.FromMilliseconds(0);
            if (times.Count >= 1)
            {
                for (var i = 0; i < times.Count; i++)
                {
                    avg += times[i];
                }
                avg = TimeSpan.FromMilliseconds(avg.TotalMilliseconds/times.Count);
            }
            running = false;
            return new ChaveValor<TimeSpan, List<string>>
                       {
                           Chave = avg,
                           Valor = respostas
                       };
        }

        private IPredict GetPredictInstance(Type predictType)
        {
            // Infelizmente nao encontrei uma maneira melhor de fazer isso porque nao eh possivel implementar 
            // Singleton na classe abstrata e nao eh possivel criar um metodo statico em Interface 

            /*Predict obj;
            BLL.ServerUtil.TypeSwitch.Do(
                BLL.ServerUtil.TypeSwitch.Case<PredictCaptchaNFE>(() => obj = PredictCaptchaNFE.Instance),
                BLL.ServerUtil.TypeSwitch.Case<PredictCaptchaMG>(() => obj = PredictCaptchaMG.Instance),
                BLL.ServerUtil.TypeSwitch.Case<PredictCaptchaAM>(() => obj = PredictCaptchaAM.Instance),
                BLL.ServerUtil.TypeSwitch.Case<PredictCaptchaRf>(() => obj = PredictCaptchaRf.Instance),
                BLL.ServerUtil.TypeSwitch.Case<PredictCaptchaRJ>(() => obj = PredictCaptchaRJ.Instance),
                BLL.ServerUtil.TypeSwitch.Case<PredictCaptchaSp>(() => obj = PredictCaptchaSp.Instance),
                BLL.ServerUtil.TypeSwitch.Case<PredictCaptchaSi>(() => obj = PredictCaptchaSi.Instance),
                BLL.ServerUtil.TypeSwitch.Default(() => obj = (PredictNeuralNetwork)Activator.CreateInstance(predictType))
            );*/

            if (predictType == typeof (PredictCaptchaNFE))
            {
                return PredictCaptchaNFE.Instance;
            }
            if (predictType == typeof (PredictCaptchaMG))
            {
                return PredictCaptchaMG.Instance;
            }
            if (predictType == typeof (PredictCaptchaAM))
            {
                return PredictCaptchaAM.Instance;
            }
            if (predictType == typeof (PredictCaptchaRF))
            {
                return PredictCaptchaRF.Instance;
            }
            if (predictType == typeof(PredictCaptchaRF3))
            {
                return PredictCaptchaRF3.Instance;
            }
            if (predictType == typeof (PredictCaptchaRJ))
            {
                return PredictCaptchaRJ.Instance;
            }
            if (predictType == typeof (PredictCaptchaSP))
            {
                return PredictCaptchaSP.Instance;
            }
            if (predictType == typeof (PredictCaptchaSi))
            {
                return PredictCaptchaSi.Instance;
            }
            if (predictType == typeof (PredictCaptchaCRJ))
            {
                return PredictCaptchaCRJ.Instance;
            }
            if (predictType == typeof (PredictCaptchaCRJAzul))
            {
                return PredictCaptchaCRJAzul.Instance;
            }
            if (predictType == typeof (PredictCaptchaCAM))
            {
                return PredictCaptchaCAM.Instance;
            }
            if (predictType == typeof(PredictCaptchaESAJ))
            {
                return PredictCaptchaESAJ.Instance;
            }
            if (predictType == typeof(PredictCaptchaPJE))
            {
                return PredictCaptchaPJE.Instance;
            }
            if (predictType == typeof(PredictCaptchaRF4))
            {
                return PredictCaptchaRF4.Instance;
            }
            throw new ArgumentOutOfRangeException("predictType");
        }

        public ChaveValor<TimeSpan, List<String>> Reconhecer(Type predictType, int sleep = 0)
        {
            running = true;
            var di = new DirectoryInfo(baseDir);
            var files = di.GetFiles(FORMATOS);

            var times = new List<TimeSpan>();
            menorTempo = new TimeSpan(9, 0, 0);
            maiorTempo = new TimeSpan(0);

            var respostas = new List<String>();

            total = files.Count();
            processados = 0;

            if (Processando != null)
            {
                Processando(total, processados);
            }

            var predict = GetPredictInstance(predictType);

            foreach (var file in files)
            {
                var dt = DateTime.Now;
                captcha = CreateCaptchaInstance(file.FullName);
                var caracteres = captcha.GetCaracteres();
                var resposta = predict.Recognize(caracteres);
                times.Add(contabilizarTempo(DateTime.Now - dt));
                respostas.Add(String.Format("{0};{1};{2}", file.Name, resposta, captcha.PadraoIdentificado));

                if (Reconheceu != null)
                {
                    Reconheceu(file.Name, resposta, captcha.PadraoIdentificado);
                }

                if (Processando != null)
                {
                    Processando(total, ++processados);
                }

                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }
            }

            var avg = TimeSpan.FromMilliseconds(0);
            if (times.Count >= 1)
            {
                for (var i = 0; i < times.Count; i++)
                {
                    avg += times[i];
                }
                avg = TimeSpan.FromMilliseconds(avg.TotalMilliseconds/times.Count);
            }
            running = false;
            return new ChaveValor<TimeSpan, List<string>>
                       {
                           Chave = avg,
                           Valor = respostas
                       };
        }

        public int RemoverImagensIdenticas()
        {
            var path = baseDir + "Rede\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return 0; // Se a pasta não existia, então não existe nada a ser removido
            }

            var directories = new DirectoryInfo(path).GetDirectories();
            var count = 0;

            foreach (var dir in directories)
            {
                var files = dir.GetFiles();
                for (var i = 0; i < files.Length; i++)
                {
                    for (var j = i + 1; j < files.Length; j++)
                    {
                        ImgArray imgBase;
                        using (
                            var fileBase = new FileStream(files[i].FullName, FileMode.Open, FileAccess.Read,
                                                          FileShare.Read))
                            // Leio o arquivo em um stream temporário para não causar conflitos
                        {
                            imgBase = new ImgArray((Bitmap) Image.FromStream(fileBase));
                            fileBase.Flush();
                            fileBase.Close();
                        }

                        ImgArray imgComparing;
                        using (
                            var fileComparing = new FileStream(files[j].FullName, FileMode.Open, FileAccess.Read,
                                                               FileShare.Read))
                            // Leio o arquivo em um stream temporário para não causar conflitos ou problemas caso o arquivo precise ser excluído
                        {
                            imgComparing = new ImgArray((Bitmap) Image.FromStream(fileComparing));
                            fileComparing.Flush();
                            fileComparing.Close();
                        }

                        if (imgBase.IsEqual(imgComparing))
                        {
                            var fileToRemove = files[j].FullName;
                            files = files.Where((val, idx) => idx != j).ToArray(); // removo a imagem repetida do vetor
                            File.Delete(fileToRemove); // deleto a imagem da pasta
                            j--; // todas as imagens restantes andaram uma casa à esquerda no vetor "files"
                            count++; // incremento o contador de imagens removidas
                        }
                    }
                }
            }

            return count;
        }

        /// <summary>
        ///   Separa as imagens em pastas de acordo com o padrão do Captcha
        /// </summary>
        /// <param name="pasta"> O nome da pasta (no diretório base) onde estão as imagens. Exemplo: "Rede" Selecione o serviço no combobox antes de usar! </param>
        public void SepararPorPadraoConsigRJ(string pasta)
        {
            var path = baseDir + pasta + "\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return; // Se a pasta não existia, então não existe nada a ser removido
            }

            var pathCinza = path + @"Cinza\";
            if (!Directory.Exists(pathCinza))
            {
                Directory.CreateDirectory(pathCinza);
            }

            var pathVerde = path + @"Verde\";
            if (!Directory.Exists(pathVerde))
            {
                Directory.CreateDirectory(pathVerde);
            }

            var pathAzul = path + @"Azul\";
            if (!Directory.Exists(pathAzul))
            {
                Directory.CreateDirectory(pathAzul);
            }

            var pathErro = path + @"Erro\";
            if (!Directory.Exists(pathErro))
            {
                Directory.CreateDirectory(pathErro);
            }

            var dir = new DirectoryInfo(path);

            var files = dir.GetFiles();
            for (var i = 0; i < files.Length; i++)
            {
                Bitmap imgBase;
                using (var fileBase = new FileStream(files[i].FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    // Leio o arquivo em um stream temporário para não causar conflitos
                {
                    imgBase = (Bitmap) Image.FromStream(fileBase);
                    fileBase.Flush();
                    fileBase.Close();
                }

                var id = new IdentificaPadraoConsigRJ();
                var padrao = id.GetPadrao(imgBase);

                switch (padrao)
                {
                    case TipoPadraoConsigRJ.Cinza:
                        files[i].MoveTo(pathCinza + files[i].Name);
                        break;
                    case TipoPadraoConsigRJ.Verde:
                        files[i].MoveTo(pathVerde + files[i].Name);
                        break;
                    case TipoPadraoConsigRJ.Azul:
                        files[i].MoveTo(pathAzul + files[i].Name);
                        break;
                    case TipoPadraoConsigRJ.NaoEncontrado:
                        files[i].MoveTo(pathErro + files[i].Name);
                        break;
                }
            }
        }
    }
}