using System;
using Core.Common;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Predict.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebservicesTest.WS_Gateway;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Common;
using Core.Common.Extensions;

namespace WebservicesTest
{
    [TestClass]
    public abstract class BaseWebServiceTest<TCaptcha>
        where TCaptcha : Captcha
    {
        protected readonly Type CaptchaType;
        protected readonly string SamplesDir;
        protected readonly string ServiceName;
        
        String token = "yWAmlOxGfMEiIz0FY58B";

        protected BaseWebServiceTest()
        {
            CaptchaType = typeof(TCaptcha);
            var typeName = CaptchaType.ToString();
            typeName = typeName.Substring(typeName.LastIndexOf(".", StringComparison.Ordinal) + 1);

            ServiceName = typeName.Replace("Captcha", "");

            SamplesDir = string.Format(@"{0}\{1}", DirectoryManager.SamplesDirectory.FullName, typeName);
        }


        protected string PredictImage(string fileName)
        {
            var fullName = string.Format(@"{0}\{1}", SamplesDir, fileName);
            var imgCaptcha = (Bitmap)Image.FromFile(fullName);
            var imageArray = imgCaptcha.ToByteArray(ImageFormat.Png);
            
            var output = "!!!!";
            try
            {
                var ws = new Gateway();
                output = ws.GetText(ServiceName, imageArray, imgCaptcha.Width, imgCaptcha.Height, token);
            }
            catch (Exception exception)
            {
                Log.Append(String.Format("{0}", exception.Message));
            }
            return output;
        }
    }
}