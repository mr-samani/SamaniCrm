using MediatR;
using SamaniCrm.Application.SubscriptionManager.Interfaces;

namespace SamaniCrm.Application.SubscriptionManager.Commands;

public record DeletePlanCommand(Guid planId) : IRequest<bool>;



public class DeletePlanCommandCommandHandler : IRequestHandler<DeletePlanCommand, bool>
{
    private readonly ISubscriptionService _subscriptionService;

    public DeletePlanCommandCommandHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<bool> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var result = await _subscriptionService.DeletePlan(request.planId, cancellationToken);
        return result;
    }
}

