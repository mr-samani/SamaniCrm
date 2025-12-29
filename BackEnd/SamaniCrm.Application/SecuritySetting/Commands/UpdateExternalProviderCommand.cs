using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SecuritySetting.Commands;

public class UpdateExternalProviderCommand : CreateOrUpdateExternalProviderDto, IRequest<bool>
{

}

public class UpdateExternalProviderCommandHandler : IRequestHandler<UpdateExternalProviderCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateExternalProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateExternalProviderCommand request, CancellationToken cancellationToken)
    {
        var provider = await _context.ExternalProviders
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (provider == null)
        {
            throw new NotFoundException("Provider not found");
        }

        var duplicateExists = await _context.ExternalProviders
            .AnyAsync(x => x.Id != request.Id && (x.Name == request.Name || x.Scheme == request.Scheme), cancellationToken);

        if (duplicateExists)
        {
            throw new UserFriendlyException("Provider with this name or scheme already exists");
        }

        provider.Name = request.Name;
        provider.DisplayName = request.DisplayName;
        provider.Scheme = request.Scheme;
        provider.ProviderType = request.ProviderType;
        provider.ClientId = request.ClientId;
        if (!string.IsNullOrEmpty(request.ClientSecret))
        {
            provider.ClientSecret = request.ClientSecret; // TODO: Encrypt in production
        }
        provider.AuthorizationEndpoint = request.AuthorizationEndpoint;
        provider.TokenEndpoint = request.TokenEndpoint;
        provider.UserInfoEndpoint = request.UserInfoEndpoint;
        provider.CallbackPath = request.CallbackPath;
        provider.LogoutEndpoint = request.LogoutEndpoint;
        provider.MetadataJson = request.MetadataJson;
        provider.Scopes = request.Scopes;
        provider.ResponseType = request.ResponseType;
        provider.ResponseMode = request.ResponseMode;
        provider.UsePkce = request.UsePkce;
        provider.LastModifiedTime = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (true);
    }
}