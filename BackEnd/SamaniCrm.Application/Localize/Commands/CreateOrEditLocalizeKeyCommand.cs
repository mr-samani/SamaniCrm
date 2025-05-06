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
    public record CreateOrEditLocalizeKeyCommand(
       string Key,
       List<LocalizationKeyDTO> Items
        ) : IRequest<bool>;

    public class CreateOrEditLocalizeKeyCommandHandler : IRequestHandler<CreateOrEditLocalizeKeyCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateOrEditLocalizeKeyCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CreateOrEditLocalizeKeyCommand request, CancellationToken cancellationToken)
        {


            List<Localization> addKeys = [];
            foreach (var item in request.Items)
            {
                var found = await _dbContext.Localizations
                .Where(w => w.Key == request.Key && w.Culture == item.Culture).FirstOrDefaultAsync();
                // update value if exist
                if (found != null)
                {
                    found.Value = item.Value;
                }
                else
                {
                    addKeys.Add(new Localization
                    {
                        Culture = item.Culture,
                        Key = request.Key,
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
