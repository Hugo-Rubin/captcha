using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Logic.Predict.Abstract;
using Core.Logic.Captchas;

namespace Core.Logic.Predict
{
    public class PredictCaptchaTJPE : PredictNeuralNetwork<PredictCaptchaTJPE>
    {
        protected override int HiddenUnits
        {
            get { return 234; }
        }

        protected override int TamanhoCaracter
        {
            get { return 4225; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                            {
                                '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'b' ,'B', 'c',
                                'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', 'h', 'H', 'i', 'j', 'J',
                                'k', 'l', 'L', 'm', 'M', 'n', 'N', 'o', 'O', 'p', 'q', 'Q', 'r',
                                'R', 's', 't', 'T', 'u', 'U', 'v', 'w', 'W', 'x', 'y', 'Y', 'z'
                            };
            }
        }

        public override string SiglaServico
        {
            get { return "TJPE"; }

        }
    }
}
