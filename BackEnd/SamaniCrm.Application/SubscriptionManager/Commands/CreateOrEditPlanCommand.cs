using MediatR;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.SubscriptionManager.Commands;

public class CreateOrEditPlanCommand : PlanDto, IRequest<bool>
{
}



public class CreateOrEditPlanCommandHandler : IRequestHandler<CreateOrEditPlanCommand, bool>
{
    private readonly ISubscriptionService _subscriptionService;

    public CreateOrEditPlanCommandHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<bool> Handle(CreateOrEditPlanCommand request, CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.CreateOrEditPlan(request, cancellationToken);
        return result;
    }
}
