using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Commands;


public record UpdateTenantCommand(
    Guid Id,
    string Name,
    string? Logo,
    string? PrimaryColor
) : IRequest<TenantDto>;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, TenantDto>
{
    private readonly ITenantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantCommandHandler(ITenantRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellation)
    {
        var tenant = await _repository.GetByIdAsync(request.Id,cancellation)
            ?? throw new NotFoundException("Tenant not found");

        throw new  NotImplementedException();
    }

}