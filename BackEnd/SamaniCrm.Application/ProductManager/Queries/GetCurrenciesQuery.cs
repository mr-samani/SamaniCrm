using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManager.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries
{
    public record GetCurrenciesQuery:IRequest<List<CurrencyDto>>;

    public class GetCurrentcyQueryHandler : IRequestHandler<GetCurrenciesQuery, List<CurrencyDto>>
    {
        private readonly IApplicationDbContext _contenxt;

        public GetCurrentcyQueryHandler(IApplicationDbContext contenxt)
        {
            _contenxt = contenxt;
        }

        public async Task<List<CurrencyDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var result= await _contenxt.Currency.Select(s=> new CurrencyDto()
            {
                Id = s.Id,
                CurrencyCode=s.CurrencyCode,
                Symbol=s.Symbol,
                Name = s.Name,
                ExchangeRateToBase=s.ExchangeRateToBase,
                IsActive=s.IsActive,
                IsDefault=s.IsDefault,
            }).ToListAsync(cancellationToken);
            return result;
        }
    }
}
