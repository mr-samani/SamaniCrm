using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Commands;
public record DeleteTenantCommand(Guid Id) : IRequest<bool>;
public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, bool>
{
    private readonly ITenantRepository _repository;

    public DeleteTenantCommandHandler(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteTenantCommand request, CancellationToken cancellation)
    {
        var tenant = await _repository.GetByIdAsync(request.Id,cancellation)
            ?? throw new NotFoundException("Tenant not found");

        var result= await _repository.DeleteAsync(request.Id, cancellation);
      
        return result;
    }
}