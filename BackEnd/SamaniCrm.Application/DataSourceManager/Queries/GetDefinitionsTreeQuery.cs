using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DataSourceManager.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DataSourceManager.Queries;

public record GetDefinitionsTreeQuery() : IRequest<List<DataSourceDefinitionTreeDto>>;



public class GetDefinitionsTreeQueryHandler : IRequestHandler<GetDefinitionsTreeQuery, List<DataSourceDefinitionTreeDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetDefinitionsTreeQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DataSourceDefinitionTreeDto>> Handle(GetDefinitionsTreeQuery request, CancellationToken cancellationToken)
    {
        List<DataSourceDefinitionTreeDto> list = await _dbContext.DataSources
            .Include(p => p.Fields)
            .Select(s => new DataSourceDefinitionTreeDto()
            {
                Title = s.Title,
                DataSourceType = s.DataSourceType,
                Description = s.Description,
                CacheKey = s.CacheKey,
                Fields = s.Fields.Select(f => new DataSourceFieldDto()
                {
                    NameSpace = f.NameSpace,
                    Title = f.Title,
                    Type = f.Type,
                }).ToList()
            })
            .ToListAsync();
        return list;
    }
}


