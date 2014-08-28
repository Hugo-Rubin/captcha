using System;

namespace Core.Data
{
    public static class RequisicoesManager
    {
        private static readonly ocrdbEntities Db = new ocrdbEntities();

        public static void SaveRequestLog(int clienteId, int ocrId, string resposta, string imagePath)
        {
            var requisicao = new Requisicoes
            {
                idCliente = clienteId,
                Data = DateTime.Now,
                Captcha = imagePath,
                Resposta = resposta,
                idOCR = ocrId
            };
            Db.AddToRequisicoes(requisicao);
            Db.SaveChanges();
        }
    } 
}
