using System;
using Core.Common;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Predict.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public abstract class BaseCaptchaTest<TCaptcha>
        where TCaptcha : Captcha
    {
        protected readonly Type CaptchaType;
        protected readonly string SamplesDir;

        protected BaseCaptchaTest()
        {
            CaptchaType = typeof(TCaptcha);
            var typeName = CaptchaType.ToString();
            typeName = typeName.Substring(typeName.LastIndexOf(".", StringComparison.Ordinal) + 1);
            SamplesDir = string.Format(@"{0}\{1}", DirectoryManager.SamplesDirectory.FullName, typeName);
        }

        [TestMethod]
        public abstract void CanPredict();

        [TestMethod]
        public abstract void PredictIsRight();

        protected Captcha CreateCaptchaInstance(String filename, Type captchaType)
        {
            return (Captcha)Activator.CreateInstance(captchaType, new object[] { filename });
        }

        protected string PredictCaptcha(Captcha captcha, IPredict predict)
        {
            var characters = captcha.GetCaracteres();
            return predict.Recognize(characters);
        }

        protected string PredictImage(string fileName, IPredict predictInstance)
        {
            var fullName = string.Format(@"{0}\{1}", SamplesDir, fileName);
            var captcha = CreateCaptchaInstance(fullName, CaptchaType);
            return predictInstance.Recognize(captcha);
        }
    }
}