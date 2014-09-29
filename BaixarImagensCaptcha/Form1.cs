using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using System.Net;

namespace BaixarImagensCaptcha
{
    public partial class Form1 : Form
    {
        private string path;
        private int qtde;
        private int copies;
        private int counter = 1;
        bool allowResize = false;
        Point panelMouseDownLocation;
        Rectangle panelRec;

        public Form1()
        {
            InitializeComponent();
        }

        private void ReadUserInput()
        {
            path = txtFolder.Text;
            if (path.EndsWith(@"\") == false)
            {
                path += @"\";
            }
            qtde = int.Parse(txtQtde.Text);
            copies = int.Parse(txtCopiasRF3.Text);
        }

        private bool CheckIfDestinationFolderExists()
        {
            if (path == null || !Directory.Exists(path))
            {
                MessageBox.Show("O diretório de destino é inválido ou não existe.", "Erro", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadUserInput();
            InicializarContador();
        }

        private void txtQtde_TextChanged(object sender, EventArgs e)
        {
            this.qtde = Int32.Parse(txtQtde.Text);
        }

        private void InicializarContador()
        {
            if (CreateFolder(path))
            {
                counter = 1;
                return;
            }

            var di = new DirectoryInfo(path);
            var files = di.GetFiles("*.png");
            if (files.Any() == false)
            {
                counter = 1;
            }
            else
            {
                var numeros = new List<int>();
                foreach (var file in files)
                {
                    var valor = 0;
                    Int32.TryParse(file.Name.Replace(".png", ""), out valor);
                    numeros.Add(valor);
                }
                counter = numeros.Max() + 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckIfDestinationFolderExists())
            {
                return;
            }
            
            if (MessageBox.Show("Verifique no código se o retangulo do corte do printscreen está ajustado corretamente para o seu captcha", "Atenção", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return;
            }

            try
            {
                var dt = DateTime.Now;
                button1.Enabled = false;
                InitProgressBar(qtde);
                for (var i = 0; i < qtde; i++)
                {
                    Text = String.Format("Processando ({0} de {1}). Tempo decorrido: {2}",
                        i + 1, 
                        txtQtde.Text,
                        DateTime.Now-dt);
                    Application.DoEvents();
                    webBrowser1.Navigate(txtUrlSite.Text);
                    while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                    {
                        Application.DoEvents();
                    }

                    // MessageBox.Show("Confirme qdo o captcha estiver carregado");
                    Thread.Sleep(300);
                    Application.DoEvents();
                    SalvarCaptchaFromPrintScreen();
                    IncrementProgress();
                }
            }
            finally
            {
                button1.Enabled = true;
                progressBar1.Value = 0;
                MessageBox.Show("Concluído!");
            }
        }

        private void BaixarImagens(int Qtde, string url)
        {
            InicializarContador();
            InitProgressBar(qtde);
            for (var i = 0; i < Qtde; i++)
            {
                var web = new WebClient();
                web.DownloadFile(url, String.Format(@"{0}{1:0000}.png", path, counter++));
                IncrementProgress();
            }
            progressBar1.Value = 0;
            MessageBox.Show("Concluído!");
        }

        private void SalvarCaptchaFromPrintScreen()
        {
            var bmp = new Bitmap(panelRec.Width, panelRec.Height, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(bmp);
            var sp = this.PointToScreen(panelRec.Location);
            g.CopyFromScreen(sp.X, sp.Y, 0, 0, this.Bounds.Size, CopyPixelOperation.SourceCopy);
            bmp.Save(String.Format(@"{0}{1:0000}.png", path, counter++), ImageFormat.Png);
        }

        private void btnBaixarImagens_Click(object sender, EventArgs e)
        {
            if (!CheckIfDestinationFolderExists())
            {
                return;
            }
            BaixarImagens(Int32.Parse(txtQtde.Text), txtURLImagem.Text);
        }

        private void RefreshRectangle()
        {
            panelRec = new Rectangle(panel1.Location, panel1.Size);
            lblRecPosition.Text = String.Format("X={0}, Y={1}\nW={2}, H={3}", 
                panelRec.Location.X,
                panelRec.Location.Y,
                panelRec.Size.Width,
                panelRec.Size.Height);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (allowResize)
            {
                panel1.Height = pictureBox1.Top + e.Y;
                panel1.Width = pictureBox1.Left + e.X;
                RefreshRectangle();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            allowResize = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            allowResize = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panel1.Left += e.X - panelMouseDownLocation.X;
                panel1.Top += e.Y - panelMouseDownLocation.Y;
                RefreshRectangle();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) panelMouseDownLocation = e.Location;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text[0] == 'E')
            {
                if (MessageBox.Show("Carregar site?", "Confirma", MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    webBrowser1.Navigate(txtUrlSite.Text);
                }
                RefreshRectangle();
                panel1.Show();
                button2.Text = "Ocultar Retângulo";
            }
            else
            {
                panel1.Hide();
                button2.Text = "Exibir Retângulo";
            }
        }

        private void btnRF3_Click(object sender, EventArgs e)
        {
            const string url1 = @"http://www.receita.fazenda.gov.br/pessoajuridica/cnpj/cnpjreva/cnpjreva_solicitacao2.asp";
            const string urlImageFmt = @"http://www.receita.fazenda.gov.br/scripts/captcha/Telerik.Web.UI.WebResource.axd?type=rca&guid={0}";
            
            InitProgressBar(qtde*copies);

            var start = NextFolderNumber(path);
            
            for (var folderNumber = start; folderNumber < start + qtde; folderNumber++)
            {
                var web = new WebClient();
                var html = web.DownloadString(url1);
                var guidPos = html.IndexOf("guid=", StringComparison.Ordinal);
                if (guidPos < 0)
                {
                    throw new Exception("O codigo fonte do site da RF mudou. Altere o codigo");
                }

                html = html.Substring(guidPos + 5);
                var guid = html.Substring(0, html.IndexOf("'", StringComparison.Ordinal));

                for (var fileNumber = 1; fileNumber < copies + 1; fileNumber++)
                {
                    var folder = string.Format(@"{0}{1:0000}\", path, folderNumber);
                    CreateFolder(folder);
                    web.DownloadFile(string.Format(urlImageFmt, guid), String.Format(@"{0}{1:0000}.png", folder, fileNumber));
                    IncrementProgress();
                }
            }
            progressBar1.Value = 0;
            MessageBox.Show("Concluído!");
        }

        private bool CreateFolder(string folder)
        {
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
                return true;
            }
            return false;
        }

        private void IncrementProgress(int incrementBy = 1)
        {
            progressBar1.Value = progressBar1.Value + incrementBy;
            Application.DoEvents();
        }

        private int NextFolderNumber(string path)
        {
            if (Directory.Exists(path) == false)
            {
                throw new DirectoryNotFoundException(path);
            }

            var info = new DirectoryInfo(path);

            var dirs = (from d in info.GetDirectories()
                       select int.Parse(d.Name)).ToArray();

            if (dirs.Any() == false)
            {
                return 1;
            }

            return dirs.Max() + 1;
        }

        private void InitProgressBar(int maximum, int value = 0)
        {
            progressBar1.Value = value;
            progressBar1.Maximum = maximum;
        }
    }
}
