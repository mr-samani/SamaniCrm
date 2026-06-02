using SamaniCrm.Core.Shared.Subscriptions;

namespace SamaniCrm.Application.SubscriptionManager.Interfaces;

public interface ISubscriptionService
{
    Task<bool> CreateOrEditPlan(PlanDto input, CancellationToken cancellation);
    Task<PlanDto> GetPlanForEdit(Guid planId, CancellationToken cancellation);
    Task<List<PlanDto>> GetAllPlans(bool onlyIsActive, CancellationToken cancellation);
    Task<bool> DeletePlan(Guid id, CancellationToken cancellation); 




    Task<List<PlanFeatureDto>> GetAllPlanFeatureForEdit(Guid planId, CancellationToken cancellation);
    Task<bool> CreateOrEditPlanFeature(List<PlanFeatureDto> input, CancellationToken cancellation);
}
