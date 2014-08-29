using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Core.Common;
using Core.Common.Extensions;
using Core.Logic.Utils;
using TestesManuais.GatewayRemoto;

namespace TestesManuais
{
    public class ConsultaCaptcha
    {
        private readonly bool usarServidorRemoto;
        private readonly WebProxy wp = WebProxy.GetDefaultProxy();

        private byte[] ImageArray;

        public ConsultaCaptcha(bool usarServidorRemoto = true)
        {
            this.usarServidorRemoto = usarServidorRemoto;
        }

        public Bitmap ImgCaptcha { get; set; }

        public bool CarregarCaptcha(String filename = null)
        {
            var result = true;
            if (filename == null)
            {
                var dlg = new OpenFileDialog();
                dlg.Filter = "PNG|*.png|TIF|*.tif|Bitmap|*.bmp|Jpeg|*.jpg|Todos|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filename = dlg.FileName;
                }
                else
                {
                    result = false;
                }
            }
            ImgCaptcha = (Bitmap)BitmapUtils.LoadImageWithoutLockFile(filename);

            ImageArray = ImgCaptcha.ToByteArray(ImageFormat.Png);

            return result;
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                var _FileStream = new FileStream(_FileName, FileMode.Create, FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception);
            }

            // error occured, return false
            return false;
        }


        public string ReconhecerCaptcha(string Servico, string Token)
        {
            wp.UseDefaultCredentials = true;
            var palavra = "!!!!";
            try
            {
                if (usarServidorRemoto)
                {
                    var ws = new Gateway();
                    ws.Proxy = wp;
                    palavra = ws.GetText(Servico, ImageArray, ImgCaptcha.Width, ImgCaptcha.Height, Token);
                }
                else
                {
                    var ws = new GatewayLocal.Gateway();
                    ws.Proxy = wp;
                    palavra = ws.GetText(Servico, ImageArray, ImgCaptcha.Width, ImgCaptcha.Height, Token);
                }
            }
            catch (Exception E)
            {
                Log.Append(String.Format("{0}", E.Message));
            }
            return palavra;
        }
    }
}