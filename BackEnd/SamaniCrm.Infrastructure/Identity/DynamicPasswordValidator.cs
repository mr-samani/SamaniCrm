using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Infrastructure.Identity
{
    public class DynamicPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        private readonly ISecuritySettingService _settingsService;

        public DynamicPasswordValidator(ISecuritySettingService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            CancellationToken cancellationToken = default!;
            var securitySettings = await _settingsService.GetSettingsAsync(cancellationToken);
            var settings = securitySettings.PasswordComplexity;
            var errors = new List<IdentityError>();

            if (password.Length < settings.RequiredLength)
                errors.Add(new IdentityError { Description = $"Password must be at least {settings.RequiredLength} characters." });

            if (settings.RequireDigit && !password.Any(char.IsDigit))
                errors.Add(new IdentityError { Description = "Password must contain a digit." });

            if (settings.RequireUppercase && !password.Any(char.IsUpper))
                errors.Add(new IdentityError { Description = "Password must contain an uppercase letter." });

            if (settings.RequireLowercase && !password.Any(char.IsLower))
                errors.Add(new IdentityError { Description = "Password must contain an lowercase letter." });

            if (settings.RequireNonAlphanumeric && new Regex("[^a-zA-Z0-9]+").Match(password).Success == false)
                errors.Add(new IdentityError { Description = "Password must contain an special characters." });

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }

}
