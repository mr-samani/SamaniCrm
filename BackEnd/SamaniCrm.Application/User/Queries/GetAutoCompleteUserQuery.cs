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

public record GetAutoCompleteUserQuery(string? Filter) : IRequest<List<AutoCompleteDto<Guid>>>;



public class GetAutoCompleteUserQueryHandler : IRequestHandler<GetAutoCompleteUserQuery, List<AutoCompleteDto<Guid>>>
{
    private readonly ILocalizer L;
    private readonly IIdentityService _identityService;

    public GetAutoCompleteUserQueryHandler(ILocalizer l,
IIdentityService identityService)
    {
        L = l;
        _identityService = identityService;
    }

    public async Task<List<AutoCompleteDto<Guid>>> Handle(GetAutoCompleteUserQuery request, CancellationToken cancellationToken)
    {

        return await _identityService.GetAutoCompleteUsers(request.Filter, cancellationToken);

    }
}


