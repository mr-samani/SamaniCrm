using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Subscriptions;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.Subscription;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Infrastructure.Identity.Migrations;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.SubscriptionManager;


public class SubscriptionService : ISubscriptionService
{
    private readonly IApplicationDbContext _dbContext;

    public SubscriptionService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateOrEditPlan(PlanDto input, CancellationToken cancellation)
    {
        Plan? plan;
        if (input.Id.HasValue)
        {
            plan = await _dbContext.Plans.FindAsync(input.Id, cancellation);
            if (plan == null)
            {
                throw new NotFoundException("Plan not found");
            }
            plan.Name = input.Name;
            plan.Code = input.Code;
            plan.Description = input.Description;
            plan.BillingType = input.BillingType;
            plan.IsActive = input.IsActive;
            plan.IsPublic = input.IsPublic;
        }
        else
        {
            plan = new Plan()
            {
                Name = input.Name,
                Code = input.Code,
                Description = input.Description,
                BillingType = input.BillingType,
                IsActive = input.IsActive,
                IsPublic = input.IsPublic,
            };
            await _dbContext.Plans.AddAsync(plan, cancellation);
        }

        var result = await _dbContext.SaveChangesAsync(cancellation);

        return result > 0;
    }



    public async Task<List<PlanDto>> GetAllPlans(CancellationToken cancellation)
    {
        var result = await _dbContext.Plans
            .OrderBy(x => x.CreatedAt)
                        .Select(s => new PlanDto()
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Code = s.Code,
                            Description = s.Description,
                            BillingType = s.BillingType,
                            IsActive = s.IsActive,
                            IsPublic = s.IsPublic,
                            CreatedAt = s.CreatedAt

                        }).ToListAsync(cancellation);
        return result;
    }


    public async Task<bool> DeletePlan(Guid id, CancellationToken cancellation)
    {
        Plan? plan = await _dbContext.Plans.FindAsync(id, cancellation);
        if (plan == null)
        {
            throw new NotFoundException("Plan not found");
        }
        _dbContext.Plans.Remove(plan);
        var result = await _dbContext.SaveChangesAsync(cancellation);
        return result > 0;
    }



    public async Task<List<PlanFeatureDto>> GetPlanFeatures(Guid planId, CancellationToken cancellation)
    {
        var result = await _dbContext.PlanFeatures
            .Where(x => x.PlanId == planId)
            .OrderBy(x => x.SortOrder)
            .Select(s => new PlanFeatureDto()
            {
                Id = s.Id,
                PlanId = s.PlanId,
                DisplayName = s.DisplayName,
                FeatureKey = s.FeatureKey,
                PlanFeatureType = s.PlanFeatureType,
                Unit = s.Unit,
                SortOrder = s.SortOrder,
                Value = s.Value
            })
            .ToListAsync(cancellation);
        return result;
    }

    public async Task<bool> CreateOrUpdatePlanFeature(PlanFeatureDto input, CancellationToken cancellation)
    {
        PlanFeature? planFeature = null;
        if (input.Id.HasValue)
        {
            planFeature = await _dbContext.PlanFeatures.FindAsync(input.Id, cancellation);
            if (planFeature == null)
            {
                throw new NotFoundException("PlanFeature not found!");
            }
            planFeature.Value = input.Value;
            planFeature.DisplayName = input.DisplayName;
            planFeature.FeatureKey = input.FeatureKey;
            planFeature.PlanFeatureType = input.PlanFeatureType;
            planFeature.Unit = input.Unit;
            planFeature.SortOrder = input.SortOrder;
        }
        else
        {
            planFeature = new PlanFeature()
            {
                DisplayName = input.DisplayName,
                FeatureKey = input.FeatureKey,
                PlanFeatureType = input.PlanFeatureType,
                Unit = input.Unit,
                SortOrder = input.SortOrder,
                Value = input.Value,

            };
            await _dbContext.PlanFeatures.AddAsync(planFeature, cancellation);
        }
        var result = await _dbContext.SaveChangesAsync(cancellation);
        return result > 0;
    }
}