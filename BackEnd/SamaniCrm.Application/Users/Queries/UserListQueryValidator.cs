using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SamaniCrm.Application.Common.DTOs;

namespace SamaniCrm.Application.Users.Queries
{
    public class UserListQueryValidator : AbstractValidator<UserListQuery>
    {
        public UserListQueryValidator()
        {
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
                .Must(field => field.IsValidSortField<UserDto>())
                .WithMessage("SortBy must be a valid property of UserDto.");
        }

        
    }
}
