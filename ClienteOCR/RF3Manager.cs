using System;
using System.IO;
using System.Net;
using System.Configuration;
using System.IO.Compression;
using Core.Common;

namespace ClienteSintegra
{
    public class RF3Manager
    {
        public byte[] BaixarImagens(string guid)
        {
            //var url1 = ConfigurationManager.AppSettings["rf3Url"];
            var urlImageFmt = ConfigurationManager.AppSettings["rf3CaptchaUrl"] + @"?type=rca&guid={0}";
            
            const string folder = @"imagens\";
            const string file = @"imagens.zip";
            int total;
            Int32.TryParse(ConfigurationManager.AppSettings["qtdeImagens"], out total);
            total = total < 1 ? 10 : total;

            var web = new WebClient();
            //var html = web.DownloadString(url1);
            //var guidPos = html.IndexOf("guid=", StringComparison.Ordinal);
            //if (guidPos < 0)
            //{
            //    throw new Exception("O codigo fonte do site da RF mudou. Utilize o metodo antigo da RF");
            //}

            //html = html.Substring(guidPos + 5);
            //var guid = html.Substring(0, html.IndexOf("'", StringComparison.Ordinal));

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);
            
            for (var i = 0; i < total; i++)
            {
                web.DownloadFile(string.Format(urlImageFmt, guid), String.Format(@"{0}{1:0000}.png", folder, i));
            }

            if (File.Exists(file))
            {
                File.Delete(file);
            }
            ZipFile.CreateFromDirectory(folder, file);

            return File.ReadAllBytes(file);
        }

        public string Reconhecer(string servico, byte[] zip, int w, int h, string token)
        {
            var palavra = "!!!!";
            try
            {
                palavra = Config.Ws.GetTextFromZip(servico, zip, w, h, token);
            }
            catch (Exception exception)
            {
                Log.Append(String.Format("{0}", exception.Message));
            }
            return palavra;
        }
    }
}
