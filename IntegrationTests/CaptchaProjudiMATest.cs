using Core.Logic.Captchas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaProjudiMATest : BaseCaptchaTest<CaptchaProjudiMA>
    {
        [TestMethod]
        public override void CanPredict()
        {
            Assert.Fail();
        }

        [TestMethod]
        public override void PredictIsRight()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void CanRemoveBackground()
        {
            var captcha = CreateCaptcha("001.png");
        }
    }
}
