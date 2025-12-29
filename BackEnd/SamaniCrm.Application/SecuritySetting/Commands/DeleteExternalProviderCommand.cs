using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SecuritySetting.Commands;

public class DeleteExternalProviderCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteExternalProviderCommandHandler : IRequestHandler<DeleteExternalProviderCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteExternalProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteExternalProviderCommand request, CancellationToken cancellationToken)
    {
        var provider = await _context.ExternalProviders
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (provider == null)
        {
            throw new NotFoundException("Provider not found");
        }

        _context.ExternalProviders.Remove(provider);
        await _context.SaveChangesAsync(cancellationToken);

        return (true);
    }
}