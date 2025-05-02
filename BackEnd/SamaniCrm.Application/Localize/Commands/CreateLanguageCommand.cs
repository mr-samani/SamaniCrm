
using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Localize.Commands
{
    public class CreateOrEditLanguageCommand : LanguageDto, IRequest<bool>;

    public class CreateOrEditLanguageCommandHandler : IRequestHandler<CreateOrEditLanguageCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateOrEditLanguageCommandHandler(IApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }



        public async Task<bool> Handle(CreateOrEditLanguageCommand request, CancellationToken cancellationToken)
        {
            var existingLanguage = await _dbContext.Languages.FindAsync(new object[] { request.Culture }, cancellationToken);

            if (existingLanguage == null)
            {
                var newLang = new Language
                {
                    Culture = request.Culture,
                    Name = request.Name,
                    IsActive = request.IsActive,
                    IsDefault = request.IsDefault,
                    Flag = request.Flag,
                    IsRtl = request.IsRtl,
                };

                await _dbContext.Languages.AddAsync(newLang, cancellationToken);
            }
            else
            {
                existingLanguage.Name = request.Name;
                existingLanguage.IsActive = request.IsActive;
                existingLanguage.IsDefault = request.IsDefault;
                existingLanguage.Flag = request.Flag;
                existingLanguage.IsRtl = request.IsRtl;

            }

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}


