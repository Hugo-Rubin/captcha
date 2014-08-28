using System;
using System.Configuration;
using System.IO;
using System.Web;
using Core.Data;
using Core.Logic.Captchas.Abstract;

namespace Core.Logic.Services
{
    public abstract class BasicService
    {
        public virtual string ImagesPath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogDir"] + "\\requisicoes\\";
            }
        }

        protected static string GetClientIpAddress()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }

        protected static void SaveRequestLogText(string token, string ip, string result)
        {
            ServerLog.Append(String.Format("Resposta para {0} em {1}:  {2}", token, ip, result), "Processamento.txt");
        }

        protected void SaveRequestLog(int idCliente, Captcha captcha, string resposta)
        {
            var cliente = ClientesManager.GetCliente(idCliente);
            var imagePath = SaveRequestImageFile(cliente.Token, captcha);
            RequisicoesManager.SaveRequestLog(cliente.id, captcha.OcrId, resposta, imagePath);
        }

        protected string SaveRequestImageFile(string token, Captcha captcha)
        {
            //Todo Handle exceptions

            var imagesFullPath = ImagesPath + token;

            if (Directory.Exists(imagesFullPath) == false)
            {
                Directory.CreateDirectory(imagesFullPath);
            }

            var filePath = String.Format(
                                @"{0}\{1}.png",
                                imagesFullPath,
                                DateTime.Now.ToHorarioBrasileiro().ToString("yyyyMMddhhmmssffff")
                           );

            captcha.ImgArray.Save(filePath);

            return filePath;
        }

        protected static bool HasValidLicense(string token)
        {
            return ClientesManager.GetCliente(token) != null;
        }
    } 
}
