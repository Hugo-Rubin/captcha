using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Logic.Predict.Abstract;
using Core.Logic.Captchas;

namespace Core.Logic.Predict
{
    public class PredictCaptchaTRTSP : PredictNeuralNetwork<PredictCaptchaTRTSP>
    {
        protected override int HiddenUnits
        {
            get { return 149; }
        }

        protected override int TamanhoCaracter
        {
            get { return 3600; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                            {
                                '2', '3', '4', '5', '6', '7', '8', '9', 'a',
                                'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
                                'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
                                'u', 'v', 'w', 'x', 'y', 'z'
                            };
            }
        }

        public override string SiglaServico
        {
            get { return "TRTSP"; }

        }
    }

}
