using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries;

public record GetAutoCompleteProductTypeQuery(string? Filter) : IRequest<List<AutoCompleteDto<Guid>>>;



public class GetAutoCompleteProductTypeQueryHandler : IRequestHandler<GetAutoCompleteProductTypeQuery, List<AutoCompleteDto<Guid>>>
{
    private readonly ILocalizer L;
    private readonly IApplicationDbContext _context;

    public GetAutoCompleteProductTypeQueryHandler(ILocalizer l, IApplicationDbContext context)
    {
        L = l;
        _context = context;
    }

    public async Task<List<AutoCompleteDto<Guid>>> Handle(GetAutoCompleteProductTypeQuery request, CancellationToken cancellationToken)
    {
        var currentLanguage = L.CurrentLanguage;

        var query = _context.ProductTypes.AsQueryable();


        if (!string.IsNullOrEmpty(request.Filter))
        {
            var filter = request.Filter;
            query = query.Where(c =>
                c.Translations.Any(t => t.Culture == currentLanguage && (
                    t.Name.Contains(filter)
                ))
            );
        }

        var items = await query.Select(s => new AutoCompleteDto<Guid>
        {
            Id = s.Id,
            Title = s.Translations.Where(w => w.Culture == currentLanguage).Select(s => s.Name).FirstOrDefault() ??
            s.Translations.Where(w => w.Name != null).Select(s => s.Name).FirstOrDefault() ?? "",
        })
            .Skip(0)
            .Take(50)
            .ToListAsync(cancellationToken);
        return items;

    }
}


