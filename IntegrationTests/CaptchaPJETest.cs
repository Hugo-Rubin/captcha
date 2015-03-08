using Core.Logic.Captchas;
using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaPJETest : BaseCaptchaTest<CaptchaPJE>
    {
        private readonly IPredict predict = PredictCaptchaPJE.Instance;

        [TestMethod]
        public override void CanPredict()
        {
            var response = PredictImage("001.png", predict);
            response.Should().Not.Be.Null();
            response.Should().Not.Contain("!");
        }

        [TestMethod]
        public override void PredictIsRight()
        {
            var response1 = PredictImage("001.png", predict);
            var response2 = PredictImage("002.png", predict);

            response1.ToLower().Should().Be.EqualTo("596958");
            response2.ToLower().Should().Be.EqualTo("496268");
        }
    }
}
