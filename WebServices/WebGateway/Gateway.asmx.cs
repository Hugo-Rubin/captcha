using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using Core.Common.Extensions;
using Core.Data;
using Core.Logic;
using Core.Logic.Captchas;
using Core.Logic.Filtros;
using Elmah;
using WebGateway.WS_AM;
using WebGateway.WS_CA;
using WebGateway.WS_CM;
using WebGateway.WS_CRJ;
using WebGateway.WS_MG;
using WebGateway.WS_NFE;
using WebGateway.WS_RF;
using WebGateway.WS_RF3;
using WebGateway.WS_RJ;
using WebGateway.WS_SI;
using WebGateway.WS_SP;

namespace WebGateway
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Gateway : WebService
    {
        private static Dictionary<string, Func<Bitmap, string, string>> servicosSuportados;
        private readonly ocrdbEntities db = new ocrdbEntities();

        public Gateway()
        {
            servicosSuportados = new Dictionary<string, Func<Bitmap, string, string>>
            {
                {"RF", ReceitaFederal},
                {"RF3", ReceitaFederal3},
                {"NFE", NotaFiscalEletronica},
                {"SI", Siscarga},
                {"SP", SintegraSP},
                {"RJ", SintegraRJ},
                {"MG", SintegraMG},
                {"AM", SintegraAM},
                {"CRJ", ConsigRJ},
                {"CRJa", ConsigRJ},
                {"CRJv", ConsigRJ},
                {"CAM", ConsigAeronautica},
                {"CA", ConsigAeronautica},
                {"CM", ConsigMarinha}
            };
        }

        // ReSharper disable InconsistentNaming
        // Apesar do nome dos parametros estarem em pascalcase qdo deveriam ser camel, é melhor nao mudar
        // pq vai ter que corrigir a referencia do servico no cliente
        [WebMethod]

        public string GetText(string Servico, byte[] Imagem, int w, int h, string Token)
        // ReSharper restore InconsistentNaming
        {
            var cliente = ListarClientePeloToken(Token);
            if (cliente == null)
            {
                ErrorSignal.FromCurrentContext().Raise(new Exception("Token not found " + Token));
                return "Licença inválida!";
            }

            if (!AcessoConcedido(cliente, Servico))
            {
                return "Acesso negado. O serviço solicitado não está contemplado em sua licença de uso.";
            }
            var palavra = "!!!!";

            try
            {
                var imag = Image.FromStream(new MemoryStream((Imagem)));
                // Derive BitMap object using Image instance, so that you can avoid the issue
                //"a graphics object cannot be created from an image that has an indexed pixel format"
                var bmp = new Bitmap(new Bitmap(imag));
                string ip;
                try
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                catch
                {
                    ip = "Desconhecido";
                }
                palavra = ReconhecerCaptcha(Servico, bmp, Token);

                var arquivoLog = cliente.Nome == "Tester" ? "testes.txt" : "requisicoes.txt";
                GravarLogExecucao(cliente.Nome, ip, Servico, palavra, arquivoLog);
            }
            catch (Exception exception)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
            }
            return palavra;
        }

        [WebMethod]
        public string GetTextFromZip(string servico, byte[] zip, int w, int h, string token)
        {
            var cliente = ListarClientePeloToken(token);
            if (cliente == null)
            {
                ErrorSignal.FromCurrentContext().Raise(new Exception("Token not found " + token));
                return "Licença inválida!";
            }
            var palavra = "!!!!";
            var ip = HttpContext.Current.Request.UserHostAddress;

            try
            {
                var grigoFilter = new GringoFilter();
                var bmp = grigoFilter.ApplyRF3FromStream(zip);

                //#if DEBUG
                //                //Salvar imagem para evitar de chamar servico da amazon para nossos testes
                //                var bmp2 = new Bitmap(bmp);
                //                const string path = @"C:\OCR\Testes\RF3\";
                //                var number = (new DirectoryInfo(path)).GetFiles("*.png").Count() + 1;
                //                bmp2.Save(String.Format(@"C:\OCR\Testes\RF3\{0:000}.png", number));
                //#endif
                palavra = ReceitaFederal3(bmp, token);

                var arquivoLog = cliente.Nome == "Tester" ? "testes.txt" : "requisicoes.txt";
                GravarLogExecucao(cliente.Nome, ip, servico, palavra, arquivoLog);
            }
            catch (Exception exception)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
            }
            return palavra;
        }

        private string ReconhecerCaptcha(string servico, Bitmap bmp, string token)
        {
            if (servicosSuportados.ContainsKey(servico) == false)
            {
                return "Desculpe. O serviço escolhido ainda não está disponível.";
            }
            return servicosSuportados[servico](bmp, token);
        }

        [WebMethod]
        public string WakeUp()
        {
            return "Hello";
        }

        private void GravarLogExecucao(string cliente, string ip, string servico, string resposta,
            string arquivo = "requisicoes.txt")
        {
            ServerLog.Append(String.Format("{0} em {1} - {2}:  {3}", cliente, ip, servico, resposta), arquivo);
        }

        private Clientes ListarClientePeloToken(String token)
        {
            try
            {
                var cliente = (from c in db.Clientes
                               where c.Token == token
                               select c).First();
                cliente.Nome = cliente.Nome.Trim();
                cliente.Token = cliente.Token.Trim();
                return cliente;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        private bool AcessoConcedido(Clientes cliente, String servico)
        {
            //TODO: Criar tabelas no BD
            return true;

            /*string[] estados =
                {
                    "SP", "RJ", "MG", "AM", "AC", "AL", "AP", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS"
                    , "PA", "PB", "PR", "PE", "PI", "RN", "RS", "RO", "RR", "SC", "SE", "TO"
                };
            if (estados.Contains(servico))
            {
                // Sintegra vendido em pacote com todos os estados
                servico = "SIN";
            }
            IQueryable<Servicos> servicos = ListarServicosPorCliente(cliente).Where(s => s.Sigla == servico);
            return servicos.FirstOrDefault() != null;
        
             */
        }

        private string ReceitaFederal(Bitmap imagem, string token)
        {
            var captcha = new CaptchaRF(imagem);
            var ws = new OCRRF();
            return ws.GetTextFromNano(captcha.ImgArray.ToNanoArray().GetInternalArray(), imagem.Width, imagem.Height, token);
        }

        private string ReceitaFederal3(Bitmap imagem, string token)
        {
            var captcha = new CaptchaRF3(imagem);
            var ws = new OCRRF3();
            var nanoImg = captcha.ImgArray.ToNanoArray().GetInternalArray();
            return ws.GetTextFromNano(nanoImg, imagem.Width, imagem.Height, token);
        }

        private string NotaFiscalEletronica(Bitmap imagem, string token)
        {
            var captcha = new CaptchaNFE(imagem);
            var ws = new OCRNFE();
            var nanoImg = captcha.ImgArray.ToNanoArray().GetInternalArray();
            return ws.GetTextFromNano(nanoImg, imagem.Width, imagem.Height, token);
        }

        private string Siscarga(Bitmap imagem, string token)
        {
            var captcha = new CaptchaSI(imagem);
            var ws = new OCRSI();
            var nanoImg = captcha.ImgArray.ToNanoArray().GetInternalArray();
            return ws.GetText(nanoImg, imagem.Width, imagem.Height, token);
        }

        private string SintegraSP(Bitmap imagem, string token)
        {
            var captcha = new CaptchaSP(imagem);
            var ws = new OCRSP();
            var nanoImg = captcha.ImgArray.ToNanoArray().GetInternalArray();
            return ws.GetTextFromNano(nanoImg, imagem.Width, imagem.Height, token);
        }

        private string SintegraRJ(Bitmap imagem, string token)
        {
            var ws = new OCRRJ();
            var rawImage = imagem.ToByteArray(ImageFormat.Png);
            return ws.GetText(rawImage, imagem.Width, imagem.Height, token);
        }

        private string SintegraMG(Bitmap imagem, string token)
        {
            var ws = new OCRMG();
            var rawImage = imagem.ToByteArray(ImageFormat.Png);
            return ws.GetText(rawImage, imagem.Width, imagem.Height, token);
        }

        private string SintegraAM(Bitmap imagem, string token)
        {
            var ws = new OCRAM();
            var rawImage = imagem.ToByteArray(ImageFormat.Png);
            return ws.GetText(rawImage, imagem.Width, imagem.Height, token);
        }

        private string ConsigRJ(Bitmap imagem, string token)
        {
            var ws = new OCRCRJ();
            var rawImage = imagem.ToByteArray(ImageFormat.Png);
            return ws.GetText(rawImage, imagem.Width, imagem.Height, token);
        }

        private string ConsigAeronautica(Bitmap imagem, string token)
        {
            var ws = new OCRCA();
            var rawImage = imagem.ToByteArray(ImageFormat.Png);
            return ws.GetText(rawImage, imagem.Width, imagem.Height, token);
        }

        private string ConsigMarinha(Bitmap imagem, string token)
        {
            var ws = new OCRCM();
            var rawImage = imagem.ToByteArray(ImageFormat.Png);
            return ws.GetText(rawImage, imagem.Width, imagem.Height, token);
        }
    }
}