using Core.Logic.Captchas.Abstract;

namespace WebCommon.Logging
{
    public interface IWebLog
    {
        void GravarLogExecucao(string tk, string ip, string resposta);

        void GravarRequisicao(int idCliente, Captcha captcha, string resposta);

        string GravarImagemRequisicao(string token, Captcha captcha);
    }
}