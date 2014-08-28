using System;
using Core.Logic.Types;

namespace Core.Logic.Predict.Abstract
{
    public abstract class PredictTemplateMatching<T> : Predict<T> where T : new()
    {
        public override string CacheDependencyFile
        {
            get { throw new NotImplementedException(); }
        }

        public override char Recognize(ImgArray caracter)
        {
            throw new NotImplementedException();
        }
    }
}