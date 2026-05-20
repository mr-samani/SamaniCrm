using MediatR;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.Features.Tenants.Queries;


public record GetTenantsAutoCompleteQuery(string? filter):IRequest<List<AutoCompleteDto<Guid>>>
{
}


public class GetTenantsAutoCompleteQueryHandler : IRequestHandler<GetTenantsAutoCompleteQuery, List<AutoCompleteDto<Guid>>>
{
    private readonly ITenantService tenantService;

    public GetTenantsAutoCompleteQueryHandler(ITenantService tenantService)
    {
        this.tenantService = tenantService;
    }

    public Task<List<AutoCompleteDto<Guid>>> Handle(GetTenantsAutoCompleteQuery request, CancellationToken cancellationToken)
    {
        var result = tenantService.GetTenantsAutoComplete(request.filter, cancellationToken);
        return result;
    }
}