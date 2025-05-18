using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace SamaniCrm.Application.Validators
{
    public class SkuValidator : AbstractValidator<string>
    {
        public SkuValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Matches(@"^[a-zA-Z0-9_-]{3,100}$")
                .WithMessage("SKU must be 3-100 characters and contain only letters, numbers, dashes or underscores.");
        }
    }
}
