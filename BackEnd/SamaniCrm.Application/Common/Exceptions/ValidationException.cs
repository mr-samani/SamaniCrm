using Microsoft.AspNetCore.Identity;
using FluentValidation.Results;

namespace SamaniCrm.Application.Common.Exceptions
{
    public class CustomValidationException : BaseAppException
    {
        public CustomValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public CustomValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public CustomValidationException(IEnumerable<IdentityError> errors) : this()
        {
            Errors = errors
                .GroupBy(e => e.Code, e => e.Description)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
