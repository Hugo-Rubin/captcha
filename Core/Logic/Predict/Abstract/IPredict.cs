using System;
using System.Collections.Generic;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;

namespace Core.Logic.Predict.Abstract
{
    public interface IPredict
    {
        Char Recognize(ImgArray caracter);
        string Recognize(IEnumerable<ImgArray> caracteres);
        string Recognize(Captcha captcha);        
    }
}