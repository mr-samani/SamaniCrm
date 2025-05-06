
using System.Threading;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Localize.Commands
{
    public class CreateOrEditLanguageCommand : LanguageDTO, IRequest<bool>;

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
                await AddDefaultLanguageLocalizationKeys(cancellationToken, request.Culture);
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



        private async Task AddDefaultLanguageLocalizationKeys(CancellationToken cancellationToken, string newCulture)
        {

            var allActiveLangs = await _dbContext.Languages
                                                   .Where(x => x.IsActive)
                                                   .ToListAsync();

            var defaultLanguage = allActiveLangs.Where(l => l.IsDefault == true).FirstOrDefault();

            List<Localization> defaultKeys = [];
            if (defaultLanguage != null)
            {
                defaultKeys = await _dbContext.Localizations
                    .Where(l => l.Culture == defaultLanguage.Culture)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                if (allActiveLangs.Any())
                {
                    var firstLanguage = allActiveLangs[0];
                    defaultKeys = await _dbContext.Localizations
                        .Where(l => l.Culture == firstLanguage.Culture)
                        .ToListAsync(cancellationToken);
                }
            }
            if (defaultKeys.Count != 0)
            {
                // اضافه کردن کلیدهای زبان پیش فرض به زبان جدید
                var newLocalizations = defaultKeys.Select(key => new Localization
                {
                    Key = key.Key,
                    Value = key.Value,
                    Culture = newCulture,
                    Category = key.Category
                }).ToList();

                await _dbContext.Localizations.AddRangeAsync(newLocalizations, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

        }
    }
}


