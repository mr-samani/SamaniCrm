using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.DTOs.PageBuilder;
using SamaniCrm.Application.DynamicData.Queries;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Extensions;
using SamaniCrm.Infrastructure.Localizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace SamaniCrm.Infrastructure.PageBuilderManager;

public class DynamicDataService : IDynamicDataService
{
    private readonly ILocalizer L;
    private readonly IApplicationDbContext _db;

    public DynamicDataService(ILocalizer l, IApplicationDbContext db)
    {
        L = l;
        _db = db;
    }

    public List<DynamicDataListDto> GetDynamicDataList(CancellationToken cancellation)
    {
        var list = new List<DynamicDataListDto>();

        foreach (DataSourceEnum item in Enum.GetValues(typeof(DataSourceEnum)))
        {
            var datasource = item.ToString();
            // TODO: check saled feature
            // دریافت Attribute مربوط به هر فیلد
            // برای بررسی نسخه کاربر که ایا میتواند این دیتاسورس را داشته باشد یا خیر
            //var field = typeof(DataSourceEnum).GetField(item.ToString());
            //var attribute = field?.GetCustomAttribute<VersionRequirementAttribute>();


            var fieldsStructures = $"DynamicData{datasource}Fields";
            // پیدا کردن Type کلاس
            var type = FindTypeByName(fieldsStructures);

            if (type == null)
            {
                throw new UserFriendlyException($"Class '{fieldsStructures}' not found.");
            }


            // فقط property های public instance
            var fields = type
           .GetProperties(BindingFlags.Public | BindingFlags.Instance)
           .Select(prop => new DynamicDataStructure
           {
               Name = prop.Name,
               DisplayName = GetDisplayName(prop),
               Value = GetDefaultValue(prop.PropertyType)
           })
           .ToList();



            list.Add(new DynamicDataListDto()
            {
                Name = item.ToString(),
                DisplayName = L[item.ToString()],
                Type = DynamicValueType.Array,
                Fields = fields
            });
        }

        return list;
    }


    private Type? FindTypeByName(string typeName)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
    }

    private string GetDisplayName(PropertyInfo prop)
    {
        var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
        if (displayAttr != null)
            return displayAttr.Name ?? prop.Name;

        return prop.Name;
    }
    private object? GetDefaultValue(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType == typeof(string))
            return string.Empty;

        if (underlyingType == typeof(Guid))
            return Guid.Empty;

        if (underlyingType.IsValueType)
            return Activator.CreateInstance(underlyingType);

        return null;
    }


    public async Task<PaginatedResult<T>> GetDynamicDataAsync<T>(GetDynamicDataQuery request, CancellationToken cancellation) where T : class
    {
        // ۱. پیدا کردن DbSet بر اساس نام اینام
        var dataSource = request.DataSource;
        var dbSetName = dataSource.ToString();
        var propertyInfo = _db.GetType().GetProperty(dbSetName);

        if (propertyInfo == null)
        {
            throw new ArgumentException($"Data source '{dataSource}' not found in database context.");
        }

        // ۲. تبدیل به IQueryable
        var dbSet = propertyInfo.GetValue(_db) as IQueryable<T>;
        if (dbSet == null)
        {
            throw new InvalidOperationException($"Data source '{dataSource}' is not of the expected type.");
        }

        // ۳. متد داینامیک که قبلاً نوشتیم
        return await FilterDynamicDataAsync(dbSet, request, cancellation);
    }



    public async Task<PaginatedResult<T>> FilterDynamicDataAsync<T>(IQueryable<T> query, GetDynamicDataQuery request, CancellationToken cancellation)
    {
        // ۱. فیلتر کردن داینامیک
        if (!string.IsNullOrWhiteSpace(request.Filter))
        {
            query = query.Where(request.Filter);
        }

        // ۲. مرتب‌سازی داینامیک
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            //var sortString = $"{request.SortBy} {request.SortDirection}";
            //query = query.OrderBy(sortString);
            query = query.OrderByDynamic(request.SortBy, request.SortDirection!);
        }

        // ۳. محاسبه تعداد کل برای صفحه‌بندی
        int totalCount = await query.CountAsync();

        // ۴. اعمال صفحه‌بندی
        var data = await query.Skip(request.PageNumber * request.PageSize)
                              .Take(request.PageSize)
                              .ToListAsync();
        return new PaginatedResult<T>
        {
            Items = data,
            TotalCount = await query.CountAsync(cancellation),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };


    }




}


