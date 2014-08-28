using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Core.Data;
using Core.Logic;
using Core.Logic.Captchas.Abstract;

namespace WebCommon.Logging
{
    public class WebLog : IWebLog
    {
        private readonly ocrdbEntities db = new ocrdbEntities();

        public void GravarLogExecucao(string tk, string ip, string resposta)
        {
            ServerLog.Append(String.Format("Resposta para {0} em {1}:  {2}", tk, ip, resposta), "Processamento.txt");
        }

        public void GravarRequisicao(int idCliente, Captcha captcha, string resposta)
        {
            try
            {
                var cliente = (from c in db.Clientes
                               where c.id == idCliente
                               select c).FirstOrDefault();
                if (cliente == null)
                {
                    return;
                }

                var requisicao = new Requisicoes
                {
                    idCliente = idCliente,
                    Data = DateTime.Now,
                    Captcha = GravarImagemRequisicao(cliente.Token, captcha),
                    Resposta = resposta,
                    idOCR = 5
                };
                db.AddToRequisicoes(requisicao);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ServerLog.Append("Erro ao gravar requisiçao no BD: " + e.Message);
            }
        }

        public string GravarImagemRequisicao(string token, Captcha captcha)
        {
            var dir = ConfigurationManager.AppSettings["LogDir"] + "\\requisicoes\\" + token;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var arquivo = String.Format(@"{0}\{1}.png",
                dir,
                DateTime.Now.ToHorarioBrasileiro().ToString("yyyyMMddhhmmssffff"));
            captcha.ImgArray.Save(arquivo);
            arquivo = arquivo.Replace(dir, @"/requisicoes/" + token).Replace(@"\", @"/");
            return arquivo;
        }

    }
}
