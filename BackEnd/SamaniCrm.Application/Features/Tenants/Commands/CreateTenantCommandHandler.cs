using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;

namespace SamaniCrm.Application.Features.Tenants;


public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, CreateTenantResponse>
{
    private readonly ITenantService createTenantService;


    public CreateTenantCommandHandler(
        ITenantService createTenantService
        )
    {
        this.createTenantService = createTenantService;
    }

    public async Task<CreateTenantResponse> Handle(CreateTenantCommand request, CancellationToken cancellation)
    {
        return await createTenantService.CreateTenantAsync(request, cancellation);
    }
}
