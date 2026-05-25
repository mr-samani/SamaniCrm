using MediatR;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Application.SubscriptionManager.Queries;

public record GetAllPlansQuery() : IRequest<List<PlanDto>>;


public class GetAllPlansQueryHandler : IRequestHandler<GetAllPlansQuery, List<PlanDto>>
{
    private readonly ISubscriptionService _subscriptionService;

    public GetAllPlansQueryHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }
    public async Task<List<PlanDto>> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.GetAllPlans(false, cancellationToken);
        return result;
    }
}