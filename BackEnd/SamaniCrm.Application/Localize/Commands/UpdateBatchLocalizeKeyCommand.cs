using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Localize.Commands
{
    public record UpdateBatchLocalizeKeyCommand(List<LocalizationKeyDTO> data, string culture) : IRequest<bool>;

    

    public class UpdateBatchLocalizeKeyCommandHandler : IRequestHandler<UpdateBatchLocalizeKeyCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateBatchLocalizeKeyCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateBatchLocalizeKeyCommand request, CancellationToken cancellationToken)
        {
            List<Localization> addKeys = new();
            List<Localization> updateKeys = new();
            foreach (var item in request.data)
            {
                var found = await _dbContext.Localizations.Where(w => w.Culture == request.culture && w.Key == item.Key).FirstAsync();
                if (found == null)
                {
                    addKeys.Add(new Localization()
                    {
                        Culture = request.culture,
                        Key = item.Key,
                        Value = item.Value,
                        Category = item.Category,
                    });
                }
                else if (found != null && found.Value != item.Value)
                {
                    found.Value = item.Value;
                    updateKeys.Add(found);
                }
            }
            if (addKeys.Count > 0)
            {
                _dbContext.Localizations.AddRange(addKeys);
            }
            if (updateKeys.Count > 0)
            {
                _dbContext.Localizations.UpdateRange(updateKeys);
            }
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }

}
