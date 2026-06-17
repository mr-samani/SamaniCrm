using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs.PageBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.DynamicData.Queries;

public class GetDynamicDataQuery : PaginationRequest, IRequest<List<DynamicDataListDto>>
{
    public required string DataSource { get; set; }
    public string? Filter { get; set; }
}


public class GetDynamicDataQueryHandler : IRequestHandler<GetDynamicDataQuery, List<DynamicDataListDto>>
{
    private readonly IDynamicDataService _dynamicDataService;

    public GetDynamicDataQueryHandler(IDynamicDataService dynamicDataService)
    {
        _dynamicDataService = dynamicDataService;
    }

    public Task<List<DynamicDataListDto>> Handle(GetDynamicDataQuery request, CancellationToken cancellationToken)
    {
        List<DynamicDataListDto> result = _dynamicDataService.GetDynamicDataList(cancellationToken);
        return Task.FromResult(result);
    }
}