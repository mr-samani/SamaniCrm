using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Localize.Queries
{
    public record GetAllLanguageQuery() : IRequest<List<LanguageDto>>;

    public class GetAllLanguageQueryHandler : IRequestHandler<GetAllLanguageQuery, List<LanguageDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetAllLanguageQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LanguageDto>> Handle(GetAllLanguageQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Languages.Select(s => new LanguageDto
            {
                Name = s.Name,
                Culture = s.Culture,
                Flag = s.Flag,
                IsRtl = s.IsRtl,
                IsDefault = s.IsDefault,
                IsActive = s.IsActive,
            }).ToListAsync();
            return result;
        }
    }
}
