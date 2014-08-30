using Core.Logic.Captchas;
using Core.Logic.Predict;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaNFETest : BaseCaptchaTest<CaptchaNFE>
    {
        [TestInitialize]
        public override void Initialize()
        {
            PredictInstance = PredictCaptchaNFE.Instance;
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

            Assert.IsTrue(response1.Equals("smx1"));
            Assert.IsTrue(response2.Equals("pd4F"));
        }
    }
}
