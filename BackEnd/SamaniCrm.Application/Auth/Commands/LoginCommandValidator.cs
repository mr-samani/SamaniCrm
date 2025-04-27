using FluentValidation;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.UserName)
              .MinimumLength(3)
              .WithMessage("UserName must be min lenght 3.");

            RuleFor(x => x.Password)
                .MinimumLength(3)
                .WithMessage("Password must be min lenght 3.");

           
            RuleFor(x => x.captcha)
                .Must(field => field.VerifyCaptcha())
                .WithMessage("Invalid captcha!");

           
        }
    }

}
