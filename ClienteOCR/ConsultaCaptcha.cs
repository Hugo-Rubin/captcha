using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Windows.Forms;
using ClienteSintegra.Properties;
using Core.Common;

namespace ClienteSintegra
{
    public class ConsultaCaptcha
    {
        private readonly WebProxy wp = WebProxy.GetDefaultProxy();
        private readonly RemoteGateway.Gateway ws = new RemoteGateway.Gateway();

        private byte[] imageArray;
        public Bitmap ImgCaptcha { get; set; }

        public bool CarregarCaptcha(String filename = null)
        {
            var result = true;
            if (filename == null)
            {
                var dlg = new OpenFileDialog { Filter = Resources.FileFilter };
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filename = dlg.FileName;
                }
                else
                {
                    result = false;
                }
            }

            if (filename != null)
            {
                ImgCaptcha = (Bitmap)Image.FromFile(filename);
            }

            imageArray = ImgCaptcha.ToByteArray(ImageFormat.Png);

            return result;
        }

        public string ReconhecerCaptcha(string servico, string token)
        {
            wp.UseDefaultCredentials = true;
            ws.Proxy = wp;
            var palavra = "!!!!";
            try
            {
                palavra = ws.GetText(servico, imageArray, ImgCaptcha.Width, ImgCaptcha.Height, token);
            }
            catch (Exception exception)
            {
                Log.Append(String.Format("{0}", exception.Message));
            }
            return palavra;
        }
    }
}