using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries
{
    public record GetActiveCurrenciesQuery : IRequest<List<AutoCompleteDto<string>>>;


    public class GetActiveCurrencyQueryHandler : IRequestHandler<GetActiveCurrenciesQuery, List<AutoCompleteDto<string>>>
    {
        private readonly IApplicationDbContext _context;

        public GetActiveCurrencyQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AutoCompleteDto<string>>> Handle(GetActiveCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Currency.Where(w => w.IsActive == true)
                .Select(s => new AutoCompleteDto<string>
                {
                    Id = s.CurrencyCode,
                    Title = s.Name,
                })
                .ToListAsync(cancellationToken);
            return result;
        }
    }
}
