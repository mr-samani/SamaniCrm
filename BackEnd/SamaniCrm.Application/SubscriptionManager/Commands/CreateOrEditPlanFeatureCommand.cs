using MediatR;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.SubscriptionManager.Commands;

public class CreateOrEditPlanFeatureCommand : IRequest<bool>
{
    public required List<PlanFeatureDto> Items { get; set; }
}



public class CreateOrEditPlanFeatureCommandHandler : IRequestHandler<CreateOrEditPlanFeatureCommand, bool>
{
    private readonly ISubscriptionService _subscriptionService;

    public CreateOrEditPlanFeatureCommandHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<bool> Handle(CreateOrEditPlanFeatureCommand request, CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.CreateOrEditPlanFeature(request.Items, cancellationToken);
        return result;
    }
}
