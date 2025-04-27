using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptchaGen.NetCore;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using static System.Net.Mime.MediaTypeNames;

namespace SamaniCrm.Application.Captcha.Queries
{
    public class GetCaptchaQueryHandler : IRequestHandler<GetCaptchaQuery, CaptchaDto>
    {
        private readonly ICaptchaStore _captchaStore;

        public GetCaptchaQueryHandler(ICaptchaStore captchaStore)
        {
            _captchaStore = captchaStore;
        }

        public async Task<CaptchaDto> Handle(GetCaptchaQuery request, CancellationToken cancellationToken)
        {

            var randomText = GenerateRandomText(5);
            //   using FileStream fs = File.OpenWrite("d:/1.jpg") ;
            using var ms = new MemoryStream();
            using (Stream picStream = ImageFactory.BuildImage(randomText, 60, 120, 20, 10, ImageFormat.Jpeg))
            {
                // picStream.CopyTo(fs);
                picStream.CopyTo(ms);
                var base64Image = Convert.ToBase64String(ms.ToArray());

                // ذخیره کپچا
                var key = Guid.NewGuid().ToString();
                _captchaStore.SaveCaptcha(key, randomText);
                var dto = new CaptchaDto
                {
                    Key = key,
                    Img = $"data:image/png;base64,{base64Image}",
                    Sensitive = false
                };


                return await Task.FromResult(dto);
            };
        }

        private string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
