using System;
using Core.Common;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;

namespace Core.Logic.Predict.Abstract
{
    public abstract class Predict<T> : Singleton<T>, IPredict 
        where T : new()
    {
        public abstract String CacheDependencyFile { get; }
        public abstract String SiglaServico { get; }
        public abstract Char Recognize(ImgArray caracter);

        public virtual string Recognize(ImgArray[] caracteres)
        {
            var result = String.Empty;
            foreach (var caracter in caracteres)
            {
                result += Recognize(caracter);
            }
            return result;
        }

        public virtual string Recognize(Captcha captcha)
        {
            return Recognize(captcha.GetCaracteresImgArray());
        }
    }
}