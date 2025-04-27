using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Infrastructure.BackgroundServices
{
    public class CaptchaCleanupBackgroundService : BackgroundService
    {
        private readonly ICaptchaStore _captchaStore;
        private readonly ILogger<CaptchaCleanupBackgroundService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1); // مثلا هر ۱ دقیقه چک کن

        public CaptchaCleanupBackgroundService(
            ICaptchaStore captchaStore,
            ILogger<CaptchaCleanupBackgroundService> logger)
        {
            _captchaStore = captchaStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Captcha Cleanup Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _captchaStore.RemoveExpiredCaptchas();
                    _logger.LogInformation("Expired captchas removed at: {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while removing expired captchas.");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Captcha Cleanup Service stopping.");
        }
    }
}
