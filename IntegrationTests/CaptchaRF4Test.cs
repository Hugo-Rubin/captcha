using Core.Logic.Captchas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaRF4Test:BaseCaptchaTest<CaptchaRF4>
    {
        private readonly IPredict predict = PredictCaptchaRF4.Instance;

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
            var response2 = PredictImage("003.png", predict);

            response1.ToLower().Should().Be.EqualTo("pg6cwv");
            response2.ToLower().Should().Be.EqualTo("ni5pdo");
        }
    }
}
