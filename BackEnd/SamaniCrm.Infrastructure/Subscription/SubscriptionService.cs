using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Subscriptions;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.Subscription;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Infrastructure.Identity.Migrations;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.SubscriptionManager;


public class SubscriptionService : ISubscriptionService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILocalizer L;

    public SubscriptionService(IApplicationDbContext dbContext, ILocalizer l)
    {
        _dbContext = dbContext;
        L = l;
    }

    public async Task<bool> CreateOrEditPlan(PlanDto input, CancellationToken cancellation)
    {
        Plan? plan;
        if (input.Id.HasValue)
        {
            plan = await _dbContext.Plans
                 .Include(p => p.Translations)
                 .OrderBy(x => x.CreatedAt)
                 .FirstOrDefaultAsync(p => p.Id == input.Id, cancellation);
            if (plan == null)
                throw new NotFoundException("Plan not found.");
            plan.Code = input.Code;
            plan.BillingType = input.BillingType;
            plan.IsActive = input.IsActive;
            plan.IsPublic = input.IsPublic;
        }
        else
        {
            plan = new Plan()
            {
                Code = input.Code,
                BillingType = input.BillingType,
                IsActive = input.IsActive,
                IsPublic = input.IsPublic,
            };
            _dbContext.Plans.Add(plan);

        }


        if (input.Translations != null)
        {
            var toRemove = plan.Translations.Where(t => !(input.Translations.Any(rt => rt.Culture == t.Culture))).ToList();
            foreach (var t in toRemove)
                plan.Translations.Remove(t);

            foreach (var item in input.Translations ?? [])
            {
                var existingTranslation = plan.Translations
                    .OrderBy(x => x.CreatedAt)
                    .FirstOrDefault(t => t.Culture == item.Culture);

                if (existingTranslation != null)
                {
                    // Update existing translation
                    existingTranslation.Title = item.Title;
                    existingTranslation.Description = item.Description;
                }
                else
                {
                    // Add new translation
                    plan.Translations.Add(new PlanTranslation
                    {
                        Culture = item.Culture,
                        Title = item.Title,
                        Description = item.Description,
                    });
                }
            }
        }



        var result = await _dbContext.SaveChangesAsync(cancellation);

        return result > 0;
    }

    public async Task<PlanDto> GetPlanForEdit(Guid planId, CancellationToken cancellation)
    {
        var currentLangugage = L.CurrentLanguage;

        var plan = await _dbContext.Plans
            .FirstOrDefaultAsync(m => m.Id == planId, cancellation);

        if (plan == null)
            throw new NotFoundException("Plan not found.");

        List<PlanTranslationDto> translations = await _dbContext.Languages
            .GroupJoin(_dbContext.PlanTranslations.Where(w => w.PlanId == planId),
              lang => lang.Culture,
              translation => translation.Culture,
              (lang, trans) => new { lang, trans }
            )
            .SelectMany(
            x => x.trans.DefaultIfEmpty(),
            (x, trans) => new PlanTranslationDto()
            {
                Culture = x.lang.Culture,
                PlanId = plan.Id,
                Title = trans != null ? trans.Title : "",
                Description = trans != null ? trans.Description : "",

            }
            ).ToListAsync(cancellation);


        return new PlanDto
        {
            Id = plan.Id,
            BillingType = plan.BillingType,
            Code = plan.Code,
            IsActive = plan.IsActive,
            IsPublic = plan.IsPublic,
            CreatedAt = plan.CreatedAt,
            Title = "",
            Description = "",
            Translations = translations
        };
    }


    public async Task<List<PlanDto>> GetAllPlans(bool onlyIsActive, CancellationToken cancellation)
    {
        var currentCulture = L.CurrentLanguage;


        var query = _dbContext.Plans
            .AsNoTracking()
            .Where(p => !onlyIsActive || p.IsActive)
            .Select(p => new PlanDto
            {
                Id = p.Id,
                Code = p.Code,
                BillingType = p.BillingType,
                IsActive = p.IsActive,
                IsPublic = p.IsPublic,
                CreatedAt = p.CreatedAt,
                Title = p.Translations
                    .Where(t => !string.IsNullOrWhiteSpace(t.Title))
                    .OrderByDescending(t => t.Culture == currentCulture)
                    .Select(t => t.Title)
                    .FirstOrDefault() ?? "",
                Description = p.Translations
                    .Where(t => !string.IsNullOrWhiteSpace(t.Title))
                    .OrderByDescending(t => t.Culture == currentCulture)
                    .Select(t => t.Description)
                    .FirstOrDefault() ?? ""
            })
            .OrderBy(x => x.CreatedAt);

        return await query.ToListAsync(cancellation);

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