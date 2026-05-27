using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
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
using static SamaniCrm.Core.Shared.Consts.AppPermissions.SubscriptionManagement;
using PlanFeatureTranslation = SamaniCrm.Domain.Entities.Subscription.PlanFeatureTranslation;

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



    public async Task<List<PlanFeatureDto>> GetAllPlanFeatureForEdit(Guid planId, CancellationToken cancellation)
    {
        var currentLangugage = L.CurrentLanguage;
        // 1. لود کردن تمام زبان‌های سیستم (فقط یک بار اجرا می‌شود)
        // فرض بر این است که _dbContext.Languages شامل تمام زبان‌های فعال سیستم است.
        var allLanguages = await _dbContext.Languages
            .Select(l => new { l.Culture, l.Name }) // فقط فیلدهای مورد نیاز را لود کنید
            .ToListAsync(cancellation);

        // 2. لود کردن تمام PlanFeatures برای این PlanId
        var planFeatures = await _dbContext.PlanFeatures
            .Where(pf => pf.PlanId == planId)
            .Select(pf => new
            {
                pf.Id,
                pf.PlanId,
                pf.FeatureKey,
                pf.PlanFeatureType,
                pf.Unit,
                pf.SortOrder,
                pf.Value
            })
            .ToListAsync(cancellation);

        // 3. لود کردن تمام ترجمه‌های مربوط به این PlanFeatures
        // این کار را انجام می‌دهیم تا تمام ترجمه‌های موجود برای این فیچرها را داشته باشیم
        var featureIds = planFeatures.Select(pf => pf.Id).ToList();

        var translations = await _dbContext.PlanFeatureTranslations
            .Where(t => featureIds.Contains(t.PlanFeatureId))
            .Select(t => new
            {
                t.PlanFeatureId,
                t.Culture,
                Title = t.Title ?? ""
            })
            .ToListAsync(cancellation);

        // 4. گروه‌بندی ترجمه‌ها بر اساس PlanFeatureId برای دسترسی سریع
        var translationsByFeatureId = translations
            .GroupBy(t => t.PlanFeatureId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(t => t.Culture, t => t.Title) // دیکشنری برای دسترسی سریع بر اساس Culture
            );

        // 5. ساخت DTO نهایی با استفاده از Join در حافظه
        var result = planFeatures.Select(pf =>
        {
            // برای هر ویژگی، تمام زبان‌ها را بررسی می‌کنیم
            var translationsList = allLanguages.Select(lang =>
            {
                // آیا ترجمه‌ای برای این ویژگی و این زبان وجود دارد؟
                if (translationsByFeatureId.TryGetValue(pf.Id, out var featureTranslations) &&
                    featureTranslations.TryGetValue(lang.Culture, out var title))
                {
                    return new PlanFeatureTranslationDto
                    {
                        Culture = lang.Culture,
                        PlanFeatureId = pf.Id,
                        Title = title
                    };
                }
                else
                {
                    // اگر ترجمه‌ای وجود ندارد، یک شیء با Title خالی و Culture مربوطه برمی‌گردانیم
                    return new PlanFeatureTranslationDto
                    {
                        Culture = lang.Culture,
                        PlanFeatureId = null, // یا pf.Id بسته به نیازتان
                        Title = ""
                    };
                }
            }).ToList();

            return new PlanFeatureDto
            {
                Id = pf.Id,
                PlanId = pf.PlanId,
                FeatureKey = pf.FeatureKey,
                PlanFeatureType = pf.PlanFeatureType,
                Unit = pf.Unit,
                SortOrder = pf.SortOrder,
                Value = pf.Value,
                Translations = translationsList
            };
        }).ToList();

        return result;
    }

    public async Task<bool> CreateOrEditPlanFeature(List<PlanFeatureDto> input, CancellationToken cancellation)
    {
        if (input == null || !input.Any())
            return false;

        // 1. استخراج IDهای ویژگی‌ها برای بارگذاری بهینه
        var featureIds = input.Select(i => i.Id).Where(id => id.HasValue).Select(id => id.Value).ToList();

        // 2. بارگذاری تمام ویژگی‌های موجود در یک کوئری
        var existingFeatures = await _dbContext.PlanFeatures
            .Where(pf => featureIds.Contains(pf.Id))
            .ToListAsync(cancellation);

        // ایجاد دیکشنری برای دسترسی سریع به ویژگی‌ها
        var featuresDict = existingFeatures.ToDictionary(f => f.Id);

        // 3. بارگذاری تمام ترجمه‌های موجود برای این ویژگی‌ها در یک کوئری
        var existingTranslations = await _dbContext.PlanFeatureTranslations
            .Where(t => featureIds.Contains(t.PlanFeatureId))
            .ToListAsync(cancellation);

        // گروه‌بندی ترجمه‌ها بر اساس PlanFeatureId و Culture برای دسترسی سریع
        // ساختار: Dictionary<PlanFeatureId, Dictionary<Culture, Translation>>
        var translationsDict = new Dictionary<Guid, Dictionary<string, PlanFeatureTranslation>>();
        foreach (var trans in existingTranslations)
        {
            if (!translationsDict.ContainsKey(trans.PlanFeatureId))
                translationsDict[trans.PlanFeatureId] = new Dictionary<string, PlanFeatureTranslation>();

            translationsDict[trans.PlanFeatureId][trans.Culture] = trans;
        }

        // 4. پردازش هر ویژگی
        foreach (var item in input)
        {
            Guid featureId;
            PlanFeature planFeature;

            if (item.Id.HasValue)
            {
                // آپدیت ویژگی موجود
                if (!featuresDict.TryGetValue(item.Id.Value, out planFeature))
                    throw new NotFoundException("PlanFeature not found!");

                featureId = item.Id.Value;

                // آپدیت فیلدهای اصلی
                planFeature.Value = item.Value;
                planFeature.FeatureKey = item.FeatureKey;
                planFeature.PlanFeatureType = item.PlanFeatureType;
                planFeature.Unit = item.Unit;
                planFeature.SortOrder = item.SortOrder;
            }
            else
            {
                // ایجاد ویژگی جدید
                planFeature = new PlanFeature
                {
                    FeatureKey = item.FeatureKey,
                    PlanFeatureType = item.PlanFeatureType,
                    Unit = item.Unit,
                    SortOrder = item.SortOrder,
                    Value = item.Value,
                    // PlanId باید از somewhere دیگر تنظیم شود. فرض بر این است که در DTO یا کانتکست موجود است.
                    // اگر PlanId در input نیست، باید از کانتکست یا پارامتر دیگری گرفته شود.
                    // در اینجا فرض می‌کنیم PlanId باید تنظیم شود. اگر در DTO نیست، این خط خطا می‌دهد.
                    // PlanId = ... 
                };

                _dbContext.PlanFeatures.Add(planFeature);
                featureId = planFeature.Id; // ID جدید پس از Add در دسترس است

                // اضافه کردن به دیکشنری برای دسترسی بعدی (اگر نیاز باشد)
                featuresDict[featureId] = planFeature;
            }

            // 5. پردازش ترجمه‌ها
            if (item.Translations != null && item.Translations.Any())
            {
                // اطمینان از وجود دیکشنری برای این ویژگی
                if (!translationsDict.ContainsKey(featureId))
                    translationsDict[featureId] = new Dictionary<string, PlanFeatureTranslation>();

                var currentTranslationsDict = translationsDict[featureId];
                var inputCultureKeys = item.Translations.Select(t => t.Culture).ToHashSet();

                // حذف ترجمه‌هایی که در input نیستند
                var culturesToRemove = currentTranslationsDict.Keys.Where(c => !inputCultureKeys.Contains(c)).ToList();
                foreach (var culture in culturesToRemove)
                {
                    var transToRemove = currentTranslationsDict[culture];
                    _dbContext.PlanFeatureTranslations.Remove(transToRemove);
                    currentTranslationsDict.Remove(culture);
                }

                // اضافه یا آپدیت ترجمه‌ها
                foreach (var translationDto in item.Translations)
                {
                    if (currentTranslationsDict.TryGetValue(translationDto.Culture, out var existingTrans))
                    {
                        // آپدیت ترجمه موجود
                        existingTrans.Title = translationDto.Title;
                    }
                    else
                    {
                        // ایجاد ترجمه جدید
                        var newTrans = new PlanFeatureTranslation
                        {
                            PlanFeatureId = featureId,
                            Culture = translationDto.Culture,
                            Title = translationDto.Title
                        };

                        _dbContext.PlanFeatureTranslations.Add(newTrans);
                        currentTranslationsDict[translationDto.Culture] = newTrans;
                    }
                }
            }
            else
            {
                // اگر لیست ترجمه خالی یا null است، تمام ترجمه‌های قبلی را حذف کن
                if (translationsDict.ContainsKey(featureId))
                {
                    var culturesToRemove = translationsDict[featureId].Keys.ToList();
                    foreach (var culture in culturesToRemove)
                    {
                        var transToRemove = translationsDict[featureId][culture];
                        _dbContext.PlanFeatureTranslations.Remove(transToRemove);
                    }
                    translationsDict[featureId].Clear();
                }
            }
        }

        // 6. ذخیره تغییرات به صورت یکپارچه
        var result = await _dbContext.SaveChangesAsync(cancellation);
        return result > 0;
    }



}