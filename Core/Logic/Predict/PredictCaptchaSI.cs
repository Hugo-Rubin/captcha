using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    /// <summary>
    ///   Classe de predição do captcha do siscarga
    /// </summary>
    public class PredictCaptchaSi : PredictTemplateMatching<PredictCaptchaSi>
    {
        public override string SiglaServico
        {
            get { return "SI"; }
        }
    }
}