using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Core.Logic.Utils;

namespace TestesManuais
{
    public partial class ClassificacaoManual : Form
    {
        private readonly List<String> imagensParaMover = new List<string>();

        private readonly String[] ordinal =
            {
                "Primeira", "Segunda", "Terceira", "Quarta", "Quinta", "Sexta", "Sétima",
                "Oitava", "Nona", "Décima"
            };

        private readonly String pastaBaseCaptchas;
        private readonly String pastaBaseRede;
        private String naoListar = String.Empty;

        public ClassificacaoManual(String pastaBase)
        {
            InitializeComponent();
            pastaBaseRede = pastaBase;
            pastaBaseCaptchas = pastaBase.Replace(@"\Rede", "");
        }

        private void ClassificacaoManual_Load(object sender, EventArgs e)
        {
            var di = new DirectoryInfo(pastaBaseRede);
            foreach (var dir in di.GetDirectories())
            {
                IncluirPastaParaClassificacao(dir.Name);
            }
            CarregarTabPage(tabControl1.TabPages[0]);
        }

        private void IncluirPastaParaClassificacao(String dirName)
        {
            var tab = new TabPage(dirName);
            tab.Enter += tabPage_Enter;
            tabControl1.TabPages.Add(tab);
            cbMoverPara.Items.Add(dirName);
        }

        private void tabPage_Enter(object sender, EventArgs e)
        {
            var tab = (sender as TabPage);
            CarregarTabPage(tab);
            (sender as Control).Focus();
        }

        private void CarregarTabPage(TabPage tab)
        {
            foreach (Control item in tab.Controls)
            {
                item.Dispose();
            }
            tab.Controls.Clear();
            var panel = new Panel();
            panel.Name = "container" + tab.Text;
            panel.Size = tab.Size;
            panel.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel.AutoScroll = true;
            tab.Controls.Add(panel);

            if (naoListar != String.Empty)
            {
                cbMoverPara.Items.Add(naoListar);
            }
            naoListar = tab.Text;
            cbMoverPara.Items.Remove(naoListar);

            var pastaAtual = pastaBaseRede + @"\" + tab.Text;
            var di = new DirectoryInfo(pastaAtual);

            var i = 0;
            foreach (var file in di.GetFiles("*.png"))
            {
                var nomeImgLetra = file.Name.Replace(".png", "");
                var strIdxLetra = nomeImgLetra[nomeImgLetra.Length - 1].ToString();
                //.Substring(nomeImgLetra.IndexOf("-") + 1);

                var linha = new Panel();
                linha.Width = panel.Width;
                // linha.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
                linha.Height = 100;
                linha.Top = i;
                linha.Name = "linha" + nomeImgLetra;

                var imgCaptcha = new PictureBox();
                var nomeCaptcha = nomeImgLetra.Substring(0, nomeImgLetra.IndexOf("-"));
                imgCaptcha.Name = nomeCaptcha;
                imgCaptcha.Image = BitmapUtils.LoadImageWithoutLockFile(pastaBaseCaptchas + @"\" + nomeCaptcha + ".png");
                toolTip1.SetToolTip(imgCaptcha, nomeCaptcha);
                imgCaptcha.Top = 3;
                imgCaptcha.Height = imgCaptcha.Image.Height + 6;
                imgCaptcha.Width = imgCaptcha.Image.Width + 6;
                linha.Controls.Add(imgCaptcha);

                /*PictureBox imgSemFundo = new PictureBox();
                String nomeSemFundo = nomeImgLetra.Substring(0, nomeImgLetra.IndexOf("-"));
                imgSemFundo.Name = nomeSemFundo;
                imgSemFundo.Image = Image.FromFile(pastaBaseCaptchas + @"\SemFundo\" + nomeSemFundo + ".png");
                // imgCaptcha.Top = i;
                imgSemFundo.Top = imgCaptcha.Image.Height;
                imgSemFundo.Height = imgSemFundo.Image.Height + 6;
                imgSemFundo.Width = imgSemFundo.Image.Width + 6;
                linha.Controls.Add(imgSemFundo);*/

                var imgLetra = new PictureBox();
                imgLetra.Name = nomeImgLetra;
                imgLetra.Image = BitmapUtils.LoadImageWithoutLockFile(file.FullName);
                //imgLetra.Top = i;
                imgLetra.Top = 3;
                imgLetra.Left = imgCaptcha.Left + imgCaptcha.Width + 3;
                imgLetra.Height = imgLetra.Image.Height + 10;
                imgLetra.Width = imgLetra.Image.Width + 10;
                imgLetra.SizeMode = PictureBoxSizeMode.CenterImage;
                imgLetra.Tag = false;

                linha.Controls.Add(imgLetra);

                var idxLetra = -1;
                var lblIndexLetra = new Label();
                if (Int32.TryParse(strIdxLetra, out idxLetra))
                {
                    lblIndexLetra.Text = String.Format("{0} letra", ordinal[idxLetra]);
                }
                else
                {
                    lblIndexLetra.Text =
                        "Não foi possível identificar o indice da letra. O padrão do nome do arquivo parece ter sido alterado";
                }
                lblIndexLetra.Left = imgLetra.Left + imgLetra.Width + 5;
                //lblIndexLetra.Top = i;
                lblIndexLetra.Top = 3;
                linha.Controls.Add(lblIndexLetra);


                var check = new CheckBox();
                check.Name = "check" + nomeImgLetra;
                check.Click += delegate { img_Click(imgLetra, null); };
                check.GotFocus +=
                    delegate(object sender, EventArgs e) { (sender as Control).Parent.BackColor = SystemColors.ActiveCaption; };
                check.LostFocus += delegate(object sender, EventArgs e)
                                       {
                                           if ((sender as CheckBox).Checked)
                                           {
                                               (sender as Control).Parent.BackColor = Color.FromArgb(255, 192, 192);
                                           }
                                           else
                                           {
                                               (sender as Control).Parent.BackColor = SystemColors.Control;
                                           }
                                       };

                imgLetra.Click += delegate(object sender, EventArgs e)
                                      {
                                          check.Focus();
                                          check.Checked = !check.Checked;
                                          img_Click(sender, e);
                                      };
                linha.Click += delegate(object sender, EventArgs e)
                                   {
                                       check.Focus();
                                       check.Checked = !check.Checked;
                                       img_Click(imgLetra, e);
                                   };

                check.Left = lblIndexLetra.Left;
                //check.Top = i;// +lblIndexLetra.Top + 10;
                check.Top = 3;
                linha.Controls.Add(check);
                linha.Height = Math.Max(imgLetra.Height, imgCaptcha.Height);
                i += linha.Height + 5;
                panel.Controls.Add(linha);
            }

            // panel.Focus();
        }

        private void img_Click(object sender, EventArgs e)
        {
            var img = (sender as PictureBox);
            img.Tag = !(bool)img.Tag;
            if ((bool)img.Tag)
            {
                img.BackColor = Color.Red;
                imagensParaMover.Add(img.Name);
            }
            else
            {
                img.BackColor = Color.Transparent;
                imagensParaMover.Remove(img.Name);
            }

            ValidaSePainelEstaHabilitado();
        }

        private void ValidaSePainelEstaHabilitado()
        {
            panelMover.Enabled = imagensParaMover.Count > 0;
        }

        private void btnMover_Click(object sender, EventArgs e)
        {
            foreach (var imgName in imagensParaMover)
            {
                var origem = String.Format(@"{0}\{1}\{2}.png",
                                              pastaBaseRede,
                                              tabControl1.SelectedTab.Text,
                                              imgName);
                var pastaDestino = String.Format(@"{0}\{1}\",
                                                    pastaBaseRede,
                                                    chkNaoExiste.Checked ? txtNovaPasta.Text : cbMoverPara.Text);
                var destino = String.Format(@"{0}\{1}.png",
                                               pastaDestino,
                                               imgName);

                if (!Directory.Exists(pastaDestino))
                {
                    Directory.CreateDirectory(pastaDestino);
                    IncluirPastaParaClassificacao(txtNovaPasta.Text);
                }
                File.Move(origem, destino);
            }
            CarregarTabPage(tabControl1.SelectedTab);
            panelMover.Enabled = false;
            imagensParaMover.Clear();
        }

        private void chkNaoExiste_CheckedChanged(object sender, EventArgs e)
        {
            txtNovaPasta.Enabled = chkNaoExiste.Checked;
            cbMoverPara.Enabled = !txtNovaPasta.Enabled;
            if (chkNaoExiste.Checked)
            {
                btnMover.Enabled = txtNovaPasta.Text.Length > 0;
            }
            txtNovaPasta.Focus();
        }

        private void cbMoverPara_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnMover.Enabled = cbMoverPara.Text.Length > 0;
        }

        private void txtNovaPasta_TextChanged(object sender, EventArgs e)
        {
            btnMover.Enabled = txtNovaPasta.Text.Length > 0;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (imagensParaMover.Count > 0 &&
                MessageBox.Show("Se mudar de aba irá perder a atual seleção. Tem certeza que deseja mudar de aba?",
                                "", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                imagensParaMover.Clear();
            }
        }

        private void tabControl1_Deselected(object sender, TabControlEventArgs e)
        {
            e.TabPage.ImageIndex = 0;
            ValidaSePainelEstaHabilitado();
        }
    }
}