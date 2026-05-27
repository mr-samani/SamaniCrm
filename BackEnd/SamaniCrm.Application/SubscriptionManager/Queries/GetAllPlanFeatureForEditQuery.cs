using MediatR;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.SubscriptionManager.Queries;

public class GetAllPlanFeatureForEditQuery : IRequest<List<PlanFeatureDto>>
{
    public Guid PlanId { get; set; }
}


public class GetAllPlanFeatureForEditQueryHandler : IRequestHandler<GetAllPlanFeatureForEditQuery, List<PlanFeatureDto>>
{
    private readonly ISubscriptionService _subscriptionService;

    public GetAllPlanFeatureForEditQueryHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<List<PlanFeatureDto>> Handle(GetAllPlanFeatureForEditQuery request, CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.GetAllPlanFeatureForEdit(request.PlanId, cancellationToken);
        return result;
    }
}