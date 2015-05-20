using System;
using System.Drawing.Imaging;
using System.IO;
using Core.Logic.Captchas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Logic.Utils;
using System.Drawing;
using PalleteQuantizer.Helpers;
using PalleteQuantizer.Quantizers.XiaolinWu;

namespace IntegrationTests
{
    [TestClass]
    public class CaptchaProjudiAMTest : BaseCaptchaTest<CaptchaProjudiAM>
    {
        [TestMethod]
        public void TestPallete()
        {
            var activeQuantizer = new WuColorQuantizer();

            const int parallelTaskCount = 1;
            const int colorCount = 8;

            var fullName = string.Format(@"{0}\002.png", SamplesDir);
            var sourceImage = Image.FromFile(fullName);

            var targetImage = ImageBuffer.QuantizeImage(sourceImage, activeQuantizer, null, colorCount, parallelTaskCount);
            targetImage.Save(@"c:\AM\colors2.png");
        }
        
        [TestMethod]
        public void CanRemoveBackgroud()
        {
            var captcha = CreateCaptcha("001.png");
            captcha.Save(@"C:\AM\001\Resultado.png");
            captcha.GetCaracteres().SalvarTodos(@"C:\AM\001");

            captcha = CreateCaptcha("002.png");
            captcha.Save(@"C:\AM\002\Resultado.png");
            captcha.GetCaracteres().SalvarTodos(@"C:\AM\002");

            captcha = CreateCaptcha("003.png");
            captcha.Save(@"C:\AM\003\Resultado.png");
            captcha.GetCaracteres().SalvarTodos(@"C:\AM\003");
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
