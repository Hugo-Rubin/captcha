using Core.Logic.Captchas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTestsEx;
using Core.Logic.Utils;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaProjudiAMTest : BaseCaptchaTest<CaptchaProjudiAM>
    {
        [TestMethod]
        public void CanRemoveBackgroud()
        {
            var captcha = CreateCaptcha("002.png");
            captcha.Save(@"C:\AM\Resultado.png");
            captcha.GetCaracteres().SalvarTodos(@"C:\AM");
            //response.Should().Not.Be.Null();
            //response.Should().Not.Contain("!");
        }


        public override void CanPredict()
        {
            Assert.Fail();
        }

        public override void PredictIsRight()
        {
            Assert.Fail();
        }
    }
}
