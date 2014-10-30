using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common;
using Core.Logic;
using Core.Logic.Predict;
using Core.Logic.Types;
using Core.Logic.Utils;
using Core.Logic.Filtros;
using System.Drawing;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Captchas;
using Core.Logic.Tratamento;
using System.IO.Compression;

namespace TestesManuais
{
    public partial class BatchTestesForm : Form
    {
        private readonly HashSet<string> padroesIdentificados = new HashSet<string>();
        private List<LogItem> LogList = new List<LogItem>();
        private List<LogItem> WorkingList = new List<LogItem>();
        private Batch batch;
        private Type captchaType;
        private int erros;
        private DateTime inicioExecucao;

        private DateTime inicioProcessamento;
        private Type predictType;

        public BatchTestesForm()
        {
            InitializeComponent();
            comboEstado.SelectedIndex = comboWebService.SelectedIndex = 0;
        }

        private void AoReconhecer(string imgPath, string resposta, string padrao)
        {
            var item = new LogItem(imgPath, resposta, true, padrao);
            if (padrao != "")
            {
                padroesIdentificados.Add(padrao);
            }
            if (workingLog.Items.Count > LogList.Count)
            {
                var idx = LogList.Count;
                if ((workingLog.Items[idx] as LogItem).FileName == item.FileName
                    && (workingLog.Items[idx] as LogItem).Text != item.Text)
                {
                    item.Correto = false;
                    erros++;
                    btnLog.Text = erros + " erros";
                    btnLog.Visible = true;
                }
            }
            LogList.Add(item);
            log.DataSource = null;
            log.DataSource = LogList;
            log.Update();
            if (log.Items.Count > 0)
            {
                log.SelectedIndex = log.Items.Count - 1;
            }

            if (chkExibirImagens.Checked)
            {
                pictureBox1.Image = BitmapUtils.LoadImageWithoutLockFile(txtPasta.Text + imgPath);
                lblResposta.Text = resposta;
            }
            Application.DoEvents();
        }

        private void AoProcessar(int total, int processados)
        {
            RefreshLabels(total, processados);
            inicioExecucao = DateTime.Now;
            progressBar1.Value = processados;
            Application.DoEvents();
        }

        private void InstanciarBatch(string id, string pasta)
        {
            erros = 0;
            var resultado = ServerUtil.GetTypeCaptchaAndPredictById(id);
            captchaType = resultado.Chave;
            predictType = resultado.Valor;

            batch = new Batch(captchaType, pasta, id);
            batch.Processando += AoProcessar;
            batch.Reconheceu += AoReconhecer;

            RefreshLabels(batch.Total, 0);
            var logTxtPath = txtPasta.Text + @"\reconhecimento.txt";
            var workingTxtPath = txtPasta.Text + @"\working.txt";
            WorkingList = CarregarLogItems(workingTxtPath).ToList();
            LogList = CarregarLogItems(logTxtPath).ToList();
            btnLog.Visible = false;

            foreach (var item in WorkingList)
            {
                var logIt = (from i in LogList
                             where i.FileName == item.FileName
                             select i).FirstOrDefault();
                if (logIt != null && logIt.Text != item.Text)
                {
                    logIt.Correto = false;
                    erros++;
                    btnLog.Visible = true;
                }
            }
            btnLog.Text = erros.ToString() + " erros";

            workingLog.DataSource = WorkingList;
            log.DataSource = LogList;
            if (workingLog.Items.Count > 0)
            {
                workingLog_SelectedIndexChanged(workingLog.Items[0], null);
            }
        }

        private LogItem[] CarregarLogItems(String arquivo)
        {
            var text = LerArquivoTexto(arquivo);
            var result = new LogItem[text.Length];
            for (var i = 0; i < text.Length; i++)
            {
                var colunas = text[i].Split(';');
                var fileName = colunas[0];
                var resposta = colunas[1];
                var padrao = colunas[2];
                result[i] = new LogItem(fileName, resposta, true, padrao);
            }
            comboPadroes.SelectedIndex = 0;
            return result;
        }

        private String[] LerArquivoTexto(String arquivo)
        {
            var result = new List<string>();
            if (File.Exists(arquivo))
            {
                using (var sr = new StreamReader(arquivo))
                {
                    var linha = sr.ReadLine();
                    while (linha != null)
                    {
                        result.Add(linha);
                        linha = sr.ReadLine();
                    }
                }
            }
            return result.ToArray();
        }

        private void RefreshLabels(int total, int processados)
        {
            lblUltimaExecucao.Text = "Ultima execucao: " + (DateTime.Now - inicioExecucao).ToString();
            lblEncontrados.Text = "Arquivos encontrados: " + total.ToString();
            lblProcessados.Text = "Arquivos processados: " + processados.ToString();
            lblRestantes.Text = "Arquivos restantes: " + (total - processados).ToString();
            progressBar1.Value = processados > progressBar1.Maximum ? progressBar1.Maximum : processados;
            progressBar1.Maximum = total;
            if (processados == 0)
            {
                lblTempo.Text = "Tempo decorrido: 00:00:00.0000";
                lblResposta.Text = String.Empty;
                pictureBox1.Image = null;
            }
            else
            {
                lblTempo.Text = "Tempo decorrido: " + (DateTime.Now - inicioProcessamento).ToString();
            }
        }

        private void comboEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPasta.Text = @"C:\OCR\Testes\";
            try
            {
                var idx = comboEstado.Text.IndexOf(" - ");
                if (idx > 0)
                {
                    var id = comboEstado.Text.Substring(0, idx);
                    txtPasta.Text += id + "\\";
                    InstanciarBatch(id, txtPasta.Text);
                    comboPadroes.Enabled = (id == "NFE");
                    comboPadroes.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("ERRO: Verifique o texto do combobox. O ID do captcha deve estar logo após ' - '");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemocaoDeFundo_Click(object sender, EventArgs e)
        {
            inicioProcessamento = DateTime.Now;
            MessageBox.Show("Tempo médio da remoção de fundo: " + batch.ValidarRemocaoDeFundo().ToString());
        }

        private void btnSeparacao_Click(object sender, EventArgs e)
        {
            inicioProcessamento = DateTime.Now;
            var resultado = batch.SepararCaracteres(cbExcluirPasta.Checked);
            MessageBox.Show("Possíveis erros encontrados: " + resultado.Chave.ToString() +
                            "\nTempo médio da separação: " + resultado.Valor.ToString());
        }

        private void btnClassificacao_Click(object sender, EventArgs e)
        {
            //Rede
            var di = new DirectoryInfo(@"C:\OCR\Testes\RF3\Separados\");
            var dirs = di.GetDirectories();

            var i = 1;
            var letra = 97;

            foreach (var dir in dirs)
            {
                foreach (var file in dir.GetFiles())
                {
                    var baseDir = @"C:\OCR\Testes\RF3\Rede\" + (char)letra;
                    if (Directory.Exists(baseDir) == false)
                    {
                        Directory.CreateDirectory(baseDir);
                    }
                    var destination = string.Format(@"{0}\{1}-{2}", baseDir, dir.Name, file.Name);
                    File.Copy(file.FullName, destination);
                    i++;
                    if (i % 40 == 0)
                    {
                        letra++;
                    }
                }

            }


            ////Separados
            //var di = new DirectoryInfo(@"C:\Users\Pablo\Downloads\Felipe\");
            //var files = di.GetFiles();


            //foreach (var file in files)
            //{
            //    var baseDir = @"C:\OCR\Testes\RF3\Separados\" + file.Name.Split('_')[0] + @"\";
            //    if (Directory.Exists(baseDir) == false)
            //    {
            //        Directory.CreateDirectory(baseDir);
            //    }
            //    var destination = string.Format(@"{0}\{1}", baseDir, file.Name.Split(' ')[1]);
            //    File.Copy(file.FullName, destination);
            //}




            inicioProcessamento = DateTime.Now;
            var predict = predictType == null ? typeof(PredictCaptchaSP) : predictType;
            if (
                MessageBox.Show(
                    "Este procedimento utiliza o " + predict +
                    " pois assume que o predict especifico ainda não foi implementado. Deseja continuar?", "Aviso",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                batch.ClassificarCaracteres(predict);
                //batch.ClassificarCaracteresBaseadoNaPastaRede(predict);
            }
        }

        private void btnReconhecimento_Click(object sender, EventArgs e)
        {
            if (!chkUsarWebService.Checked && predictType == null)
            {
                MessageBox.Show("A predição deste captcha ainda não foi implementada");
                return;
            }

            inicioProcessamento = DateTime.Now;
            var pausa = cbPausa.Checked ? Int32.Parse(tbPausa.Text) : 0;
            ChaveValor<TimeSpan, List<String>> resultado;

            workingLog.DataSource = WorkingList;
            LogList.Clear();
            log.DataSource = LogList;
            padroesIdentificados.Clear();
            padroesIdentificados.Add("Todos");
            btnLog.Visible = false;
            erros = 0;

            inicioExecucao = DateTime.Now;

            if (chkUsarWebService.Checked)
            {
                var servico = comboEstado.Text;
                servico = servico.Substring(0, servico.IndexOf(" - "));

                resultado = batch.TestarWebService(servico, comboWebService.Text == "Remoto", pausa);
            }
            else
            {
                resultado = batch.Reconhecer(predictType, pausa);
            }
            comboPadroes.DataSource = padroesIdentificados.ToArray();
            MessageBox.Show(String.Format("Menor tempo: {0}\nTempo médio: {1}\nMaior tempo: {2}",
                                          batch.MenorTempo.ToString(),
                                          resultado.Chave.ToString(),
                                          batch.MaiorTempo.ToString()));
            Log.GravarLinhasEmArquivo(txtPasta.Text + "\\reconhecimento.txt", resultado.Valor, true);
            if (!File.Exists(txtPasta.Text + "\\working.txt"))
            {
                var workingTxtPath = txtPasta.Text + "\\working.txt";
                Log.GravarLinhasEmArquivo(workingTxtPath, resultado.Valor, true);
                workingLog.DataSource = CarregarLogItems(workingTxtPath);
            }
        }

        private void btnChecarErros_Click(object sender, EventArgs e)
        {
            inicioProcessamento = DateTime.Now;
            batch.ChecarErrosSeparacao();
            MessageBox.Show("Processo concluído, verifique a pasta de erros.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pastaBase = txtPasta.Text + @"\Rede";
            if (!Directory.Exists(pastaBase))
            {
                MessageBox.Show("Pasta de rede não encontrada. Execute a classificação automática antes de prosseguir.");
                return;
            }

            using (var formManual = new ClassificacaoManual(pastaBase))
            {
                formManual.Text += " - " + comboEstado.Text;
                formManual.ShowDialog();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboWebService.Enabled = chkUsarWebService.Checked;
        }

        private void chkUsarWebService_CheckedChanged(object sender, EventArgs e)
        {
            comboWebService.Enabled = chkUsarWebService.Checked;
        }

        private void log_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (log.Items.Count > 0 && workingLog.Items.Count >= log.Items.Count)
            {
                var idx = log.SelectedIndex;
                workingLog.SelectedIndex = idx;
            }
            if (!batch.Running)
            {
                workingLog_SelectedIndexChanged(sender, e);
            }
        }

        private void workingLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Control)
            {
                if (sender != null && !batch.Running && (sender as Control).Focused)
                {
                    var item = ((sender as ListBox).SelectedItem as LogItem);
                    if (item != null)
                    {
                        pictureBox1.Image = BitmapUtils.LoadImageWithoutLockFile(txtPasta.Text + item.FileName);
                        lblResposta.Text = item.Text;
                    }
                }
            }
            else
            {
                if (sender != null && !batch.Running)
                {
                    var item = (sender as LogItem);
                    if (item != null)
                    {
                        pictureBox1.Image = BitmapUtils.LoadImageWithoutLockFile(txtPasta.Text + item.FileName);
                        lblResposta.Text = item.Text;
                    }
                }
            }
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            var erros = from l in LogList
                        where l.Correto == false
                        select l;

            var fileNames = from erro in erros
                            select erro.FileName;

            var working = from w in WorkingList
                          where fileNames.Contains(w.FileName)
                          select w;

            if (log.Items.Count == erros.Count())
            {
                log.DataSource = LogList;
                workingLog.DataSource = WorkingList;
                btnLog.Text = erros.Count().ToString() + " erros";
                comboPadroes.Enabled = true;
                comboPadroes.SelectedIndex = 0;
            }
            else
            {
                if (erros.Count() > 0)
                {
                    log.DataSource = erros.ToArray();
                    workingLog.DataSource = working.ToArray();
                    btnLog.Text = "Ver todos";
                    log.SelectedIndex = 0;
                    comboPadroes.Enabled = false;
                }
            }
            workingLog_SelectedIndexChanged(log.SelectedItem, null);
        }

        private void comboPadroes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPadroes.Text == "Todos")
            {
                log.DataSource = LogList;
                workingLog.DataSource = WorkingList;
            }
            else
            {
                var filtro = from l in LogList
                             where l.Padrao == comboPadroes.Text
                             select l;
                log.DataSource = filtro.ToArray();

                var fileNames = from fl in filtro
                                select fl.FileName;

                var workingFiltro = from w in WorkingList
                                    where fileNames.Contains(w.FileName)
                                    select w;
                workingLog.DataSource = workingFiltro.ToArray();
            }
            if (log.Items.Count > 0)
            {
                log.SelectedIndex = 0;
                log_SelectedIndexChanged(log.Items[0], null);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var servico = comboEstado.Text;
            servico = servico.Substring(0, servico.IndexOf(" - "));
            var numImagensRemovidas = batch.RemoverImagensIdenticas();
            MessageBox.Show(
                String.Format("{0} imagens repetidas foram removidas de {1}.", numImagensRemovidas, servico),
                numImagensRemovidas != 0 ? "Remoção Concluída Com Sucesso!" : "Nenhuma Imagem Pôde Ser Removida.");
        }

        /// Copia as imagens da pasta Separados para a pasta Rede, de acordo com um modelo já organizado (pasta Rede em diOrigem)
        private void button3_Click(object sender, EventArgs e)
        {
            var diOrigem = new DirectoryInfo(@"C:\OCR\Testes\Rede\Rede").GetDirectories();
            var dirSeparados = @"C:\OCR\Testes\CAM\Separados\";
            var dirDestino = @"C:\OCR\Testes\CAM\Rede\";

            foreach (var d in diOrigem)
            {
                var fiOrigem = d.GetFiles("*.png");
                if (!Directory.Exists(dirDestino + d.Name))
                {
                    Directory.CreateDirectory(dirDestino + d.Name);
                }

                foreach (var f in fiOrigem)
                {
                    var pasta = f.Name.Split('-')[0];
                    var arquivo = f.Name.Split('-')[1];

                    File.Copy(dirSeparados + pasta + @"\" + arquivo, dirDestino + d.Name + @"\" + f.Name);
                }
            }

            MessageBox.Show("Concluído!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Cria pastas do alfabeto
            ServerUtil.CriarPastas(@"C:\Users\Hugo\Desktop\Characters\RF");

            // Extrai arquivos de uma pasta
            //ExtrairArquivos(@"C:\OCR\Testes\CAM Pão\Rede_Pao\_Lixo", "*.*", true);


            // Imprime o caminho mais longo no grafo
            /*ImgArray img = ImgArray.LoadFromFile(Constants.DesktopHugo + "t2.png");

			/*var v = new FSharpLibrary.Vertice[img.Width, img.Height];
			var v2 = new BLL.Graph.Vertice[img.Width, img.Height];
			FSharpLibrary.Graph g = new FSharpLibrary.Graph(v2);
			//pegar o primeiro pixel preto
			var verticeInicial = g.MontarGrafo(img.GetMinXPoint(), img);*/

            /*var cm = new CaptchaCM(img.ToBitmap());

            var gs = new GraphSearch(img);
            gs.FindGraphs();
            //sg.PrintGraph();
            gs.PrintLongestPaths();*/



            // Cria todas as pastas do alfabeto do Captcha dentro da pasta lixo (para que o lixo seja filtrado e saibamos o que acontece com cada identificação errada, por letra).
            /*DirectoryInfo[] diOrigem = new DirectoryInfo(@"C:\OCR\Testes\CAM\Rede\").GetDirectories("?");
            var diDestino = new DirectoryInfo(@"C:\OCR\Testes\CAM\Rede\_Lixo\");

            foreach (DirectoryInfo info in diOrigem)
            {
                string name = diDestino + info.Name;
                if (!Directory.Exists(name))
                {
                    Directory.CreateDirectory(name);
                }
            }
            MessageBox.Show("Pastas criadas com sucesso.");*/


            /*for (int i = 1; i <= 5; i++)
            {

                Bitmap source = (Bitmap) Image.FromFile(Constants.DesktopHugo + "1 (" + i + ").jpg");
                ImgArray img = new ImgArray(source).RemoverMargem();

                for (int y = 1; y < img.Height - 1; y++)
                {
                    for (int x = 1; x < img.Width - 1; x++)
                    {
                        if (!ServerUtil.IsBlackPixel(img.GetPixel(x, y)))
                        {
                            img.SetPixel(x, y, Color.White);
                        }
                    }
                }


                ICM_Basic icm = new ICM_Basic();
                img = icm.Apply(img, 10, 20, 1);
                ForwardDerivative fd = new ForwardDerivative();

                img = fd.Apply(img, true, false);
                img = fd.Apply(img, true, false);
                fd.Apply(img).Save(Constants.DesktopHugo + "FD Vertical 3x Horizontal 1x (" + i + ").jpg");
                //fd.Apply(img).Save(Constants.DesktopHugo + "FD Completo (" + i + ").jpg");
            }*/

            // Move imagens para outra pasta e as renomeia
            /*string[] files = Directory.GetFiles(@"C:\OCR\Testes\CRJ", "*.png");
            int index = 1;
            foreach (var fi in files)
            {
                File.Move(fi, @"C:\OCR\Testes\CRJ\Renomeados\" + index++ + ".png");
            }*/

            // Remove traços pretos causados pela captura irregular das imagens do ConsigRJ
            /*DirectoryInfo[] directories = new DirectoryInfo(@"C:\OCR\Testes\CRJ\Rede").GetDirectories();
            int count = 0;

            foreach (var dir in directories)
            {
                if (dir.Name[0] == '_')
                {
                    continue;
                }

                FileInfo[] files = dir.GetFiles();

                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i].FullName;

                    ImgArray img;
                    using (FileStream fileBase = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) // Leio o arquivo em um stream temporário para não causar conflitos
                    {
                        img = new ImgArray((Bitmap)Image.FromStream(fileBase));
                        fileBase.Flush();
                        fileBase.Close();
                    }

                    int yinicial = 60;

                    for (int h = 59; h > 0; h--)
                    {
                        if (img.GetPixel(29, h).R == 0)
                        {
                            yinicial = h;
                            break;
                        }
                    }

                    if (yinicial < 60)
                    {
                        bool removeLinha = false;
                        int countP = 0;
                        int wInicial = 0;
                        for (int w = 0; w < 60; w++)
                        {

                            if (img.GetPixel(w, yinicial).R == 0)
                            {
                                if (countP == 0)
                                {
                                    wInicial = w;
                                }
                                countP++;
                            }
                            else
                            {

                                if (countP > 30)
                                {
                                    removeLinha = true;
                                    break;
                                }
                                else
                                {
                                    countP = 0;
                                }


                            }


                        }

                        if (removeLinha)
                        {

                            for (int y = yinicial; y > yinicial - 3; y--)
                            {
                                for (int x = wInicial; x < wInicial + countP; x++)
                                {
                                    img.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                                }
                            }

                            img.CortarECentralizar(60, 60).Save(file);
                        }
                    }
                }*/

            // Separa Caracteres Por Padrao (Para ConsigRJ) - Selecionar CRJ como serviço no batch antes de clicar no botão
            //batch.SepararPorPadraoConsigRJ("Imagens 3");

            // Remoção de fundo modelo verde:
            /*int k = 0;

                for (int i = 1; i < 5; i++)
                {
                    Bitmap source = (Bitmap) Image.FromFile(Constants.DesktopHugo + i + ".png");
                    source = source.MakeGrayscale();

                    ImgArray img = new ImgArray(source.Width, source.Height);

                    for (int y = 0; y < source.Height; y++)
                    {
                        for (int x = 0; x < source.Width; x++)
                        {
                            if (source.GetPixel(x, y).R < 220)
                            {
                                img.SetPixel(x, y, Color.Black);
                            }
                        }
                    }

                    ForwardDerivative fd = new ForwardDerivative();
                    ManhattanDilation di = new ManhattanDilation();

                    ImgArray im = di.Apply(fd.Apply(img), 1).RemoverRuidos(8).InserirBordaX(1).InserirBordaY(1);
				
                    SeamCarving2 sc = new SeamCarving2(4, (int) Math.Round((double) im.Width / 4));
                    ImgArray[] imC = sc.GetClusters(im, true);*/

            /*Seam_Carving seam = new Seam_Carving();
                    Bitmap c = seam.DrawCarves(im.ToBitmap(), im.Width / 4);
                    Bitmap[] clusters = seam.CheckClusters(im.ToBitmap(), im.Width/4, 0);
                    c.Save(Constants.DesktopHugo + "sc_Carves.png");
                    clusters.SalvarTodos(Constants.DesktopHugo, k++ + "- SC_");*/

            /*for (int j = 0; j < 5; j++)
                {
                    imC[j].Save(Constants.DesktopHugo + "SCbutton - " + k++ + "_" + j + ".png");
                }*/

            //imC[4].Save(Constants.DesktopHugo + "SC " + i + ".png");*/
            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var pastaBase = txtPasta.Text;
            if (!Directory.Exists(pastaBase))
            {
                MessageBox.Show(
                    "Pasta do captcha não encontrada. Crie a pasta e adicione as imagens do captcha antes de continuar.");
                return;
            }

            using (var formIndividual = new ClassificacaoIndividual(pastaBase))
            {
                formIndividual.Text += " - " + comboEstado.Text;
                formIndividual.ShowDialog();
            }
        }

        private void ExtrairArquivos(string pastaBase, string searchPattern = "*.*", bool incluirSubPastas = false, bool mover = true)
        {
            var di = new DirectoryInfo(pastaBase);
            var files = di.GetFiles(searchPattern, incluirSubPastas ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var fi in files)
            {
                if (mover)
                {
                    try
                    {
                        fi.MoveTo(pastaBase + @"\" + fi.Name);
                    }
                    catch (Exception e)
                    {
                        var r = new Random(0);
                        fi.MoveTo(pastaBase + @"\" + fi.Name + r.Next(100));
                    }
                }
                else
                {
                    try
                    {
                        fi.CopyTo(pastaBase + @"\" + fi.Name);
                    }
                    catch (Exception e)
                    {
                        var r = new Random(0);
                        fi.CopyTo(pastaBase + @"\" + fi.Name + r.Next(100));
                    }
                }
            }
        }

        private void intTestsBtn_Click(object sender, EventArgs e)
        {
            var dirs = new DirectoryInfo(Constants.DesktopHugo + @"\RF3\").GetDirectories();
            var destination = Constants.DesktopHugo + @"\results\";

            foreach (var d in dirs)
            {
                string startPath = d.FullName;
                string zipPath = startPath + @"\compressed.zip";

                try
                {
                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Optimal, false);
                }
                catch (Exception) { }



                var gringoFilter = new GringoFilter();
                Bitmap bmp = new Bitmap(gringoFilter.ApplyRF3(zipPath, GringoFilterType.RF3));

                bmp.Save(destination + d.Name + "_POST.png");

                ImgArray img = new ImgArray(bmp);

                /*Captcha c = new CaptchaRF(bmp);
                c.ImgArray.Save(destination + d.Name + "_POST.png");*/
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ImgArray src = new ImgArray((Bitmap)Image.FromFile(@"E:\Users\Hugo\Desktop\1.png"));
            ImgArray dtn = new ImgArray(src.Width * 2, src.Height * 2);

            var end = new Endireitamento(true, false);
            dtn = end.ApplyTo(src);

            dtn.Save(@"E:\Users\Hugo\Desktop\2.png");
        }

    }
}