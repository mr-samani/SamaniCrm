using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries;
public record GetAllProductCategoryTranslationQuery() : IRequest<List<ExportAllLocalizationValueDto>>;


public class ExportAllLocalizationValueDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

}

public class GetAllProductCategoryTranslationQueryHandler : IRequestHandler<GetAllProductCategoryTranslationQuery, List<ExportAllLocalizationValueDto>>
{
    private readonly ILocalizer L;
    private readonly IApplicationDbContext _context;

    public GetAllProductCategoryTranslationQueryHandler(ILocalizer l, IApplicationDbContext context)
    {
        L = l;
        _context = context;
    }

    public async Task<List<ExportAllLocalizationValueDto>> Handle(GetAllProductCategoryTranslationQuery request, CancellationToken cancellationToken)
    {
        var currentLanguage = L.CurrentLanguage;
        var result = await _context.ProductCategoryTranslations
                .Where(w => w.Culture == currentLanguage)
                .Select(s => new ExportAllLocalizationValueDto()
                {
                    Id = s.CategoryId,
                    Title = s.Title,
                    Description = s.Description ?? "",
                }).ToListAsync(cancellationToken);
        return result;
    }
}
