using FluentValidation;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants;


public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    private readonly ITenantUniquenessChecker _tenantUniqueness;

    public CreateTenantCommandValidator(ITenantUniquenessChecker tenantUniqueness)
    {
        _tenantUniqueness = tenantUniqueness;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام بهره‌بردار الزامی است")
            .MaximumLength(200)
            .MinimumLength(2);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug الزامی است")
            .MaximumLength(100)
            .MinimumLength(2)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug باید فقط شامل حروف کوچک انگلیسی، اعداد و خط تیره باشد")
            .MustAsync(BeUniqueSlugAsync)
            .WithMessage("این Slug قبلاً استفاده شده است");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("ایمیل الزامی است")
            .EmailAddress().WithMessage("فرمت ایمیل صحیح نیست")
            .MaximumLength(256)
            .MustAsync(BeUniqueEmailAsync)
            .WithMessage("این ایمیل قبلاً ثبت شده است");

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("ایمیل مدیر الزامی است")
            .EmailAddress().WithMessage("فرمت ایمیل مدیر صحیح نیست")
            .MustAsync(BeUniqueAdminEmailAsync)
            .WithMessage("این ایمیل قبلاً ثبت شده است");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("رمز عبور الزامی است")
            .MinimumLength(8).WithMessage("رمز عبور باید حداقل ۸ کاراکتر باشد")
            .Must(PasswordMustHaveSpecialChar)
            .WithMessage("رمز عبور باید شامل حداقل یک کاراکتر خاص باشد");

        //RuleFor(x => x.AdminNationalCode)
        //    .Must(BeValidNationalCode)
        //    .When(x => !string.IsNullOrEmpty(x.AdminNationalCode))
        //    .WithMessage("کد ملی صحیح نیست");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("شهر الزامی است");

        RuleFor(x => x.MaxUsers)
            .GreaterThan(0).WithMessage("تعداد کاربران باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(10000).WithMessage("تعداد کاربران نمی‌تواند بیشتر از ۱۰۰۰۰ باشد");

        RuleFor(x => x.TrialDays)
            .InclusiveBetween(0, 90)
            .WithMessage("مدت آزمایشی باید بین ۰ تا ۹۰ روز باشد");
    }

    private async Task<bool> BeUniqueSlugAsync(string slug, CancellationToken cancellation)
    {
        return !await _tenantUniqueness.ExistsBySlugAsync(slug, cancellation);
    }

    private async Task<bool> BeUniqueEmailAsync(string email, CancellationToken cancellation)
    {
        return !await _tenantUniqueness.ExistsByEmailAsync(email, cancellation);
    }

    private async Task<bool> BeUniqueAdminEmailAsync(string email, CancellationToken cancellation)
    {
        return !await _tenantUniqueness.ExistsByUserEmailAsync(email, cancellation);
    }

    private bool PasswordMustHaveSpecialChar(string password)
    {
        if (string.IsNullOrEmpty(password)) return false;
        return password.Any(c => !char.IsLetterOrDigit(c));
    }

    private bool BeValidNationalCode(string? nationalCode)
    {
        if (string.IsNullOrEmpty(nationalCode)) return true;
        if (nationalCode.Length != 10) return false;
        if (!nationalCode.All(char.IsDigit)) return false;

        // Iranian National Code validation algorithm
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += int.Parse(nationalCode[i].ToString()) * (10 - i);
        }
        int remainder = sum % 11;
        int checkDigit = int.Parse(nationalCode[9].ToString());

        return (remainder < 2 && checkDigit == remainder) ||
               (remainder >= 2 && checkDigit == 11 - remainder);
    }
}