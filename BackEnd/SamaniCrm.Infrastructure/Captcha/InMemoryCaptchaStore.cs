using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Infrastructure.Captcha
{
    internal class CaptchaEntry
    {
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
    }

    public class InMemoryCaptchaStore : ICaptchaStore
    {
        private readonly Dictionary<string, CaptchaEntry> _captchas = new();
        private readonly TimeSpan _expiration = TimeSpan.FromMinutes(2);

        public void SaveCaptcha(string key, string value)
        {
            _captchas[key] = new CaptchaEntry
            {
                Value = value,
                ExpireAt = DateTime.UtcNow.Add(_expiration)
            };
        }

        public bool ValidateCaptcha(string key, string input)
        {
            if (_captchas.TryGetValue(key, out var entry))
            {
                if (DateTime.UtcNow > entry.ExpireAt)
                {
                    _captchas.Remove(key);
                    return false;
                }

                return string.Equals(entry.Value, input, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public void RemoveCaptcha(string key)
        {
            _captchas.Remove(key);
        }




        public void RemoveExpiredCaptchas()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _captchas
                .Where(kvp => kvp.Value.ExpireAt <= now)
                .Select(kvp => kvp.Key)
                .ToList(); // ToList چون در حال تغییر کالکشن هستیم

            foreach (var key in expiredKeys)
            {
                _captchas.Remove(key);
            }
        }


    }
}
