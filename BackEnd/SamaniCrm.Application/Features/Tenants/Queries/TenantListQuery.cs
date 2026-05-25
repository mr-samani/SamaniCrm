using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Queries;

public class TenantListQuery : PaginationRequest, IRequest<PaginatedResult<TenantListDto>>
{
    public string? Search { get; set; }
    public string? Status { get; set; }
}

public class TenantListQueryHandler : IRequestHandler<TenantListQuery, PaginatedResult<TenantListDto>>
{
    private readonly ITenantRepository _repository;

    public TenantListQueryHandler(ITenantRepository repository) => _repository = repository;

    public async Task<PaginatedResult<TenantListDto>> Handle(TenantListQuery request, CancellationToken cancellation)
    {
        var result = await _repository.GetAllAsync(request, cancellation);
        return result;
    }
}

