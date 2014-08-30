using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Logic.Captchas;
using Core.Logic.Predict;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaRJTest : BaseCaptchaTest<CaptchaRJ>
    {
        [TestInitialize]
        public override void Initialize()
        {
            PredictInstance = PredictCaptchaRJ.Instance;
        }

        [TestMethod]
        public void CanPredict()
        {
            var response = PredictImage("001.png");
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("!") == false);
        }

        [TestMethod]
        public void PredictIsRight()
        {
            var response1 = PredictImage("001.png");
            var response2 = PredictImage("002.png");

            Assert.IsTrue(response1.Equals("H64Y2"));
            Assert.IsTrue(response2.Equals("Q97DF"));
        }
    }
}
