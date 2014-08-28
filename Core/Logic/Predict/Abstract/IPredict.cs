using System;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;

namespace Core.Logic.Predict.Abstract
{
    public interface IPredict
    {
        Char Recognize(ImgArray caracter);
        string Recognize(ImgArray[] caracteres);
        string Recognize(Captcha captcha);        
    }
}