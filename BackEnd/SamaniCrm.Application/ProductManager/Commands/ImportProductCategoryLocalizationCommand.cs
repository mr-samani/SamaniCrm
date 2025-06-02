using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManager.Queries;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Commands;
public record ImportProductCategoryLocalizationCommand(List<ExportAllLocalizationValueDto> data) : IRequest<bool>;

public class ImportProductCategoryLocalizationCommandHandler : IRequestHandler<ImportProductCategoryLocalizationCommand, bool>
{
    private readonly ILocalizer L;
    private readonly IApplicationDbContext _context;

    public ImportProductCategoryLocalizationCommandHandler(IApplicationDbContext context, ILocalizer l)
    {
        _context = context;
        L = l;
    }

    public async Task<bool> Handle(ImportProductCategoryLocalizationCommand request, CancellationToken cancellationToken)
    {
        var currentLanguage = L.CurrentLanguage;
        List<ProductCategoryTranslation> addList = [];
        List<ProductCategoryTranslation> updateList = [];
        for (int i = 0; i < request.data.Count; i++)
        {
            var found = await _context.ProductCategoryTranslations
                .Where(x => x.CategoryId == request.data[i].Id && x.Culture == currentLanguage)
                .FirstOrDefaultAsync(cancellationToken);
            if (found != null)
            {
                found.Title = request.data[i].Title;
                found.Description = request.data[i].Description;
                updateList.Add(found);
            }
            else
            {
                addList.Add(new ProductCategoryTranslation()
                {
                    Culture = currentLanguage,
                    CategoryId = request.data[i].Id,
                    Title = request.data[i].Title,
                    Description = request.data[i].Description
                });
            }
        }
        if (addList.Count > 0)
        {
            _context.ProductCategoryTranslations.AddRange(addList);
        }
        if (updateList.Count > 0)
        {
            _context.ProductCategoryTranslations.UpdateRange(updateList);
        }
        var r = _context.SaveChanges();
        return r > 0;

    }
}