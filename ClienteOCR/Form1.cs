using System;
using System.Windows.Forms;
using ClienteSintegra.Properties;
using Core.Common;

namespace ClienteSintegra
{
    public partial class Form1 : Form
    {
        private readonly Token token = new Token();

        public Form1()
        {
            InitializeComponent();
            cbEstado.SelectedIndex = 0;
        }

        private void btnReconhecer_Click(object sender, EventArgs e)
        {
            try
            {
                var consulta = new ConsultaCaptcha();
                consulta.CarregarCaptcha();
                pbImagem.Image = consulta.ImgCaptcha;
                txtResposta.Text = @"processando...";
                lblTempo.Text = @"Processando...";
                Application.DoEvents();

                var servico = cbEstado.Text;
                var idx = servico.IndexOf(" - ", StringComparison.Ordinal);
                servico = idx > -1 ? servico.Substring(0, servico.IndexOf(" - ", StringComparison.Ordinal)) : servico;
                var dt = DateTime.Now;
                var resposta = consulta.ReconhecerCaptcha(servico, token.Key);
                lblTempo.Text = String.Format("Tempo decorrido: {0} seg.", (DateTime.Now - dt).ToString(@"s\.fff"));

                if (resposta.Length > 10)
                {
                    txtResposta.Clear();
                    lblTempo.Text = "";
                    MessageBox.Show(resposta);
                }
                else
                {
                    txtResposta.Text = resposta;
                }
            }
            catch (Exception exception)
            {
                Log.Append(String.Format("{0}", exception.Message));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!token.IsValid())
            {
                MessageBox.Show(this, Resources.LicencaInvalida, Resources.Alerta, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }
    }
}