using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using Core.Logic.Types;

namespace TestesManuais
{
    public partial class ClassificacaoIndividual : Form
    {
        private readonly string arquivo;
        private readonly List<FileInfo> captchaFiles;
        private readonly List<string> filesOk;
        private readonly IPredict p1;
        private readonly IPredict p2;
        private readonly string pastaCaptchas;
        private readonly string pastaRede;
        private readonly string pastaSemFundo;
        private readonly string pastaSeparados;
        private readonly StringBuilder sb;
        private int index;
        private string pastaBase;
        private PictureBox[] pictures;
        private int total;

        public ClassificacaoIndividual(String pastaBase)
        {
            InitializeComponent();
            ActiveControl = resposta;
            pastaCaptchas = pastaBase;
            pastaSemFundo = pastaBase + @"SemFundo\";
            pastaSeparados = pastaBase + @"Separados\";
            pastaRede = pastaBase + @"Rede\";
            this.pastaBase = pastaBase;
            arquivo = pastaBase + "ref.txt";
            sb = new StringBuilder();
            captchaFiles = new List<FileInfo>();
            filesOk = new List<string>();
            index = 0;
            total = 0;
            p1 = PredictCaptchaNFE.Instance;
            p2 = PredictCaptchaCAM.Instance;
        }

        private void ClassificacaoIndividual_Load(object sender, EventArgs e)
        {
            VerificarDependencias();
            pictures = new[] { letra1, letra2, letra3, letra4, letra5, letra6, letra7, letra8, letra9, letra10 };
            var di = new DirectoryInfo(pastaCaptchas);
            captchaFiles.AddRange(di.GetFiles("*.png"));

            if (File.Exists(arquivo))
            {
                using (var sr = new StreamReader(arquivo))
                {
                    if (sr.BaseStream.Length > 0)
                    {
                        var linha = sr.ReadLine();

                        while (linha != null)
                        {
                            sb.AppendLine(linha.Substring(0, linha.IndexOf(" - ")));
                            linha = sr.ReadLine();
                        }
                    }
                }

                filesOk.AddRange(Regex.Split(sb.ToString(), "\r\n"));
                filesOk.RemoveAt(filesOk.Count - 1);
            }


            foreach (var file in filesOk)
            {
                captchaFiles.Remove(
                    captchaFiles.Find(
                        delegate(FileInfo fi) { return fi.Name == file; }));
            }

            total = captchaFiles.Count;
            labelTimg.Text = string.Format("{0:0000}", total);
            LabelNimg.Text = string.Format("{0:0000}", index + 1);

            if (total != 0)
            {
                ExibirImagens(index);

                btnVoltar.Enabled = false;
                if (total == 1)
                {
                    btnVoltar.Enabled = false;
                    btnAvançar.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Todas as imagens foram processadas.");
                //Close();
                //Dispose();
            }
        }

        private void ExibirImagens(int index)
        {
            if (index > 0)
            {
                imagemAnterior.Image = Image.FromFile(captchaFiles[index - 1].FullName);
            }
            imagemAtual.Image = Image.FromFile(pastaSemFundo + captchaFiles[index].Name);
            imagemOriginal.Image = Image.FromFile(pastaBase + captchaFiles[index].Name);
        }

        private void btnAvançar_Click(object sender, EventArgs e)
        {
            index++;

            try
            {
                if (index >= total)
                {
                    index--;
                    MessageBox.Show("Todas as imagens foram processadas.");
                    Close();
                    Dispose();
                }

                ExibirImagens(index);

                ClearPictures();




                btnVoltar.Enabled = true;

                if (index == total - 1)
                {
                    btnAvançar.Enabled = false;
                }


                respostaAnterior.Visible = string.IsNullOrEmpty(resposta.Text);
                respostaAnterior.Text = resposta.Text;
                resposta.Clear();
                LabelNimg.Text = string.Format("{0:0000}", index + 1);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Erro: " + exception.Message);
            }
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            index--;

            if (respostaAnterior.Text != "defLabel")
            {
                resposta.Text = respostaAnterior.Text;
                respostaAnterior.Visible = false;
            }

            imagemAseguir.Image = Image.FromFile(captchaFiles[index + 1].FullName);
            imagemAtual.Image = Image.FromFile(pastaSemFundo + captchaFiles[index].Name);
            ClearPictures();


            if (index == 0)
            {
                imagemAnterior.Image = null;
                btnVoltar.Enabled = false;
            }
            else
            {
                imagemAnterior.Image = Image.FromFile(captchaFiles[index - 1].FullName);
            }

            btnAvançar.Enabled = true;
            LabelNimg.Text = string.Format("{0:0000}", index + 1);
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            var txt = resposta.Text;
            if (txt != null && txt.Length > 0)
            {
                var path = pastaSeparados + captchaFiles[index].Name.Split('.')[0] + @"\";
                var di = new DirectoryInfo(path);
                var files = di.GetFiles("*.png");
                for (var c = 0; c < txt.Length; c++)
                {
                    if (c < files.Count())
                    {
                        File.Copy(files[c].FullName,
                                  pastaRede + NomePasta(txt[c]) + @"\" + captchaFiles[index].Name.Split('.')[0] + "-" +
                                  files[c].Name, true);
                    }
                }
            }

            using (var sw = new StreamWriter(arquivo, true))
            {
                sw.WriteLine(captchaFiles[index].Name + " - " + txt);
            }

            btnAvançar_Click(sender, e);

            resposta.Clear();
            respostaAnterior.Text = txt;
            respostaAnterior.Visible = true;
        }

        private void CriarPastasRede()
        {
            var di = new DirectoryInfo(pastaRede);

            // Cria as pastas A-Z, 0-9, _lixo
            for (var c = 'A'; c <= 'z'; c++)
            {
                if (char.IsSymbol(c) || c == ']' || c == '[' || c == '_')
                {
                    continue;
                }
                if (char.IsUpper(c))
                {
                    Directory.CreateDirectory(di.FullName + c + c);
                    continue;
                }
                Directory.CreateDirectory(di.FullName + c);
            }
            for (var i = 0; i <= 9; i++)
            {
                Directory.CreateDirectory(di.FullName + i);
            }
            Directory.CreateDirectory(di.FullName + "_lixo");
        }

        private string NomePasta(char c)
        {
            if (c.Equals('*'))
            {
                return "_lixo";
            }
            if (char.IsUpper(c))
            {
                return "" + c + c;
            }
            return "" + c;
        }

        private void ExibirImagensSeparadas()
        {
            sugestao1.Text = "";
            sugestao2.Text = "";
            var path = pastaSeparados + captchaFiles[index].Name.Split('.')[0] + @"\";
            var d = new DirectoryInfo(path);
            var files = d.GetFiles("?.png");

            for (var i = 0; i < files.Length; i++)
            {
                var img = (Bitmap)Image.FromFile(files[i].FullName);
                pictures[i].Image = img;
                var imga = new ImgArray(img);
                sugestao1.Text += p1.Recognize(imga);
                sugestao2.Text += p2.Recognize(imga);
            }
        }

        private void ClearPictures()
        {
            foreach (var pic in pictures)
            {
                pic.Image = null;
            }
        }

        private void VerificarDependencias()
        {
            if (!File.Exists(arquivo))
            {
                var fs = File.Create(arquivo);
                fs.Close();
                fs.Dispose();
            }

            if (!Directory.Exists(pastaRede))
            {
                Directory.CreateDirectory(pastaRede);
            }

            if (Directory.GetDirectories(pastaRede).Count() < 1)
            {
                CriarPastasRede();
            }

            if (!Directory.Exists(pastaSeparados))
            {
                MessageBox.Show("Antes de executar este módulo você precisa rodar a separação.");
                Close();
                Dispose();
                return;
            }
            if (!Directory.Exists(pastaSemFundo))
            {
                MessageBox.Show("Antes de executar este módulo você precisa rodar a remoção de fundo.");
                Close();
                Dispose();
            }
        }

        private void sugestao1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            resposta.Text = sugestao1.Text;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            resposta.Text = sugestao2.Text;
        }

        private void btnRef_Click(object sender, EventArgs e)
        {
            // TODO: Verificar se o número de letras digitado é maior ou igual ao número mínimo de caracteres do CAPTCHA sendo processado
            var countErros = 0;
            using (TextReader tr = new StreamReader(arquivo))
            {
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    var temp = line.Split('-');
                    temp[0] = temp[0].Trim();
                    temp[1] = temp[1].Trim();

                    // TODO: Substituir esse 5 por Captcha.NumeroMinimoDeLetras
                    if (temp[1].Length < 5)
                    {
                        countErros++;
                        continue;
                    }

                    var nomeImagem = temp[0].Split('.')[0];
                    var di = new DirectoryInfo(pastaSeparados + nomeImagem);
                    try
                    {
                        var fi = di.GetFiles("?.png");
                        var i = 0;

                        foreach (var f in fi)
                        {
                            try
                            {
                                var destino = pastaRede + temp[1][i] + @"\" + nomeImagem + "-" + i + ".png";
                                f.CopyTo(destino, true);
                                i++;
                            } catch(IndexOutOfRangeException) { }
                        }
                    }
                    catch (DirectoryNotFoundException)
                    {
                    }
                }
            }

            MessageBox.Show("Concluído!\n" + countErros + " imagens com número incorreto de caracteres digitados.");
        }
    }
}