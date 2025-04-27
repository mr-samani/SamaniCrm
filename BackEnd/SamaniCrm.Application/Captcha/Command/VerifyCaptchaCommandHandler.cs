using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Captcha.Command
{
    public class VerifyCaptchaCommandHandler : IRequestHandler<VerifyCaptchaCommand, bool>
    {
        private readonly ICaptchaStore _captchaStore;

        public VerifyCaptchaCommandHandler(ICaptchaStore captchaStore)
        {
            _captchaStore = captchaStore;
        }

        public async Task<bool> Handle(VerifyCaptchaCommand request, CancellationToken cancellationToken)
        {
            var isValid = _captchaStore.ValidateCaptcha(request.Key, request.CaptchaText);

            if (isValid)
            {
                _captchaStore.RemoveCaptcha(request.Key); // delete captcha after use
            }

            return await Task.FromResult(isValid);
        }
    }

}
