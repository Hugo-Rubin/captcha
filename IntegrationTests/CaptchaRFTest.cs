using Core.Logic.Captchas;
using Core.Logic.Predict;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaRFTest : BaseCaptchaTest<CaptchaRF>
    {
        [TestInitialize]
        public override void Initialize()
        {
            PredictInstance = PredictCaptchaRF.Instance;
        }

        [TestMethod]
        public override void CanPredict()
        {
            var response = PredictImage("001.png");
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("!") == false);
        }

        [TestMethod]
        public override void PredictIsRight()
        {
            var response1 = PredictImage("001.png");
            var response2 = PredictImage("002.png");

            Assert.IsTrue(response1.Equals("197v45"));
            Assert.IsTrue(response2.Equals("9FA2R2"));
        }
    }
}
