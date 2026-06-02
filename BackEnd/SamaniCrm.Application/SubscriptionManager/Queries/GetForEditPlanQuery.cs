using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Subscriptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.SubscriptionManager.Queries;

public record GetForEditPlanQuery(Guid Id) : IRequest<PlanDto>;

public class GetForEditPlanQueryHandler : IRequestHandler<GetForEditPlanQuery, PlanDto>
{

    private readonly ISubscriptionService _subscriptionService;

    public GetForEditPlanQueryHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<PlanDto> Handle(GetForEditPlanQuery request, CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.GetPlanForEdit(request.Id, cancellationToken);
        return result;
    }
}
