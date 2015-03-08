using Core.Logic.Captchas;
using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace WebservicesTest
{
    [TestClass]
    public class GatewayPJETest : BaseWebServiceTest<CaptchaPJE>
    {        
        [TestMethod]
        public void GetTextPJEIsRight()
        {
            var response1 = PredictImage("001.png");
            var response2 = PredictImage("002.png");

            response1.ToLower().Should().Be.EqualTo("596958");
            response2.ToLower().Should().Be.EqualTo("496268");
        }
    }
}
