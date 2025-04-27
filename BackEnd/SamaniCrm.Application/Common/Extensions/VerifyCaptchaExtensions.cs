using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application
{
    public static class VerifyCaptchaExtensions
    {
      
     

        private static ICaptchaStore _captchaStore;
        public static void Configure(ICaptchaStore captchaStore)
        {
            _captchaStore = captchaStore;
        }


        public static bool VerifyCaptcha(this InputCaptchaDTO? captcha)
        {
            if (captcha == null || string.IsNullOrEmpty(captcha.CaptchaKey) || string.IsNullOrEmpty(captcha.CaptchaText))
            {
                // throw new ArgumentException("Captcha key or text cannot be null or empty");
                return false;
            }

            try
            {
                if (_captchaStore == null)
                {
                    throw new InvalidOperationException("CaptchaStore is not configured. Call VerifyCaptchaExtensions.Configure() during application startup.");
                }

                var isCaptchaValid = _captchaStore.ValidateCaptcha(captcha.CaptchaKey, captcha.CaptchaText);
                _captchaStore.RemoveCaptcha(captcha.CaptchaKey);
                return isCaptchaValid;
            }
            catch (Exception ex)
            {
                // logger.Error($"Captcha validation failed: {ex.Message}");
                return false; 
            }
        }

    }
}
