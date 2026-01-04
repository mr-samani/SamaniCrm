using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SecuritySetting.Queries;

public class GetExternalProviderByIdQuery : IRequest<CreateOrUpdateExternalProviderDto>
{
    public Guid Id { get; set; }
}

public class GetExternalProviderByIdQueryHandler : IRequestHandler<GetExternalProviderByIdQuery, CreateOrUpdateExternalProviderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetExternalProviderByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CreateOrUpdateExternalProviderDto> Handle(GetExternalProviderByIdQuery request, CancellationToken cancellationToken)
    {
        var provider = await _context.ExternalProviders
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (provider == null)
        {
            throw new NotFoundException("Provider not found");
        }

        var dto = _mapper.Map<CreateOrUpdateExternalProviderDto>(provider);
        return (dto);
    }
}