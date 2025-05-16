using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Pages.Queries
{
    public class GetFilteredPagesQueryValidator:AbstractValidator<GetFilteredPagesQuery>
    {
        public GetFilteredPagesQueryValidator() {
            RuleFor(x => x.PageNumber)
                 .GreaterThan(0)
                 .WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.SortDirection)
                .Must(dir => dir!.ToLower() == "asc" || dir.ToLower() == "desc")
                .WithMessage("SortDirection must be 'asc' or 'desc'.");

            RuleFor(x => x.SortBy)
                .Must(field => field.IsValidSortField<UserDTO>())
                .WithMessage("SortBy must be a valid property of UserDto.");
        
        }
    }
}
