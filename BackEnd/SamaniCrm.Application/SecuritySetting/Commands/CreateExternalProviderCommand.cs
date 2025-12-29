using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SecuritySetting.Commands;
public class CreateExternalProviderCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Scheme { get; set; }
    public ExternalProviderTypeEnum ProviderType { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AuthorizationEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string UserInfoEndpoint { get; set; }
    public string CallbackPath { get; set; }
    public string LogoutEndpoint { get; set; }
    public string MetadataJson { get; set; }
    public string Scopes { get; set; }
    public string ResponseType { get; set; }
    public string ResponseMode { get; set; }
    public bool UsePkce { get; set; }
}

public class CreateExternalProviderCommandHandler : IRequestHandler<CreateExternalProviderCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateExternalProviderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateExternalProviderCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.ExternalProviders
            .AnyAsync(x => x.Name == request.Name || x.Scheme == request.Scheme, cancellationToken);

        if (exists)
        {
            throw new UserFriendlyException("Provider with this name or scheme already exists");
        }

        var provider = new ExternalProvider
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DisplayName = request.DisplayName,
            Scheme = request.Scheme ?? request.Name,
            ProviderType = request.ProviderType,
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret, // TODO: Encrypt in production
            AuthorizationEndpoint = request.AuthorizationEndpoint,
            TokenEndpoint = request.TokenEndpoint,
            UserInfoEndpoint = request.UserInfoEndpoint,
            CallbackPath = request.CallbackPath ?? $"/signin-{request.Name.ToLower()}",
            LogoutEndpoint = request.LogoutEndpoint,
            MetadataJson = request.MetadataJson,
            Scopes = request.Scopes,
            ResponseType = request.ResponseType ?? "code",
            ResponseMode = request.ResponseMode ?? "query",
            UsePkce = request.UsePkce,
            IsActive = false,
        };

        _context.ExternalProviders.Add(provider);
        await _context.SaveChangesAsync(cancellationToken);

        return provider.Id;
    }
}