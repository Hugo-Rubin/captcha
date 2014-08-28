using System;
using System.Web.Services;
using Core.Common;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;

namespace Core.Logic.Services
{
    public abstract class OcrService : BasicService
    {
        public abstract Predict.Abstract.Predict Network { get; }
        
        [WebMethod]
        public String GetText(int[] nanoImg, int w, int h, string token)
        {
            if (HasValidLicense(token) == false)
            {
                return "Licença inválida!";
            }

            var nano = new NanoArray(nanoImg.BitmapFromNanoArray(w, h));

            var captchaType = ServerUtil.GetTypeCaptchaAndPredictById(Network.SiglaServico).Chave;
            var captcha = (Captcha) Activator.CreateInstance(captchaType, new object[] {nano});

            var ip = GetClientIpAddress();

            var charArray = captcha.GetCaracteresImgArray();
            var result = Network.Recognize(charArray);
            SaveRequestLogText(token, ip, result);

            return result;
        } 
    }
}
