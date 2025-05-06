using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Localize.Commands
{
    public record CreateOrEditLocalizeKeyCommand(string culture, List<LocalizationKeyDTO> items) : IRequest<bool>;


    public class CreateOrEditLocalizeKeyCommandHandler : IRequestHandler<CreateOrEditLocalizeKeyCommand, bool>
    {

        private readonly IApplicationDbContext _dbContext;

        public CreateOrEditLocalizeKeyCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CreateOrEditLocalizeKeyCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود زبان
            var language = await _dbContext.Languages.FindAsync(new object[] { request.culture }, cancellationToken);
            if (language == null)
                throw new NotFoundException(nameof(Language), request.culture);

            // گرفتن کلیدهای موجود برای این زبان
            var existingLocalizations = await _dbContext.Localizations
                .Where(l => l.Culture == request.culture)
                .ToDictionaryAsync(l => l.Key, cancellationToken);
            List<Localization> addKeys = [];
            foreach (var item in request.items)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                    continue;

                // update value if exist
                if (existingLocalizations.TryGetValue(item.Key, out var localization))
                {
                    localization.Value = item.Value;
                }
                else
                {
                    addKeys.Add(new Localization
                    {
                        Culture = request.culture,
                        Key = item.Key,
                        Value = item.Value,
                        Category = item.Category,
                    });
                }
            }
            if (addKeys.Any())
            {
                await _dbContext.Localizations.AddRangeAsync(addKeys, cancellationToken);
            }
            var saved = await _dbContext.SaveChangesAsync(cancellationToken);
            return saved > 0;
        }
    }
}
