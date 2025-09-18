using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SecuritySetting.Commands;


public record ChangeIsActiveExternalProviderCommand(Guid id, bool isActive) : IRequest<bool>;



public class ChangeIsActiveExternalProviderCommandHandler : IRequestHandler<ChangeIsActiveExternalProviderCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ChangeIsActiveExternalProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ChangeIsActiveExternalProviderCommand request, CancellationToken cancellationToken)
    {
        var provider = await _context.ExternalProviders
            .AsTracking()
            .Where(x => x.Id == request.id)
            .FirstOrDefaultAsync(cancellationToken);
        if (provider == null)
        {
            throw new NotFoundException("Provider not found");
        }
        else
        {
            provider.IsActive = request.isActive;
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}