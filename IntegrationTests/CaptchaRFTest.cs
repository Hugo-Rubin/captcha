using Core.Logic.Captchas;
using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaRFTest : BaseCaptchaTest<CaptchaRF>
    {
        private readonly IPredict predict = PredictCaptchaRF.Instance;

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

            response1.Should().Be.EqualTo("197v45");
            response2.Should().Be.EqualTo("9FA2R2");
        }
    }
}
