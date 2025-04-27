using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface ICaptchaStore
    {
        void SaveCaptcha(string key, string value);
        bool ValidateCaptcha(string key, string input);
        void RemoveCaptcha(string key);

        void RemoveExpiredCaptchas();
    }

 

}
