using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Interfaces;
using System.Linq.Expressions;

namespace SamaniCrm.Infrastructure.DbContexts;

public static class GlobalFilterBuilder
{
    public static void ApplyFilters(ModelBuilder builder, TenantDbContext dbContext)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var parameter =
                Expression.Parameter(entityType.ClrType, "e");

            Expression? finalExpression = null;

            var isDeletedProp =
                entityType.ClrType.GetProperty("IsDeleted");

            if (isDeletedProp?.PropertyType == typeof(bool))
            {
                finalExpression = Expression.Equal(
                    Expression.Property(parameter, isDeletedProp),
                    Expression.Constant(false));
            }

            if (typeof(IMayHaveTenant)
                .IsAssignableFrom(entityType.ClrType))
            {
                var tenantProperty =
                    Expression.Property(
                        Expression.Constant(dbContext),
                        nameof(TenantDbContext.CurrentTenantId));

                var tenantExpression =
                    Expression.Equal(
                        Expression.Property(
                            parameter,
                            nameof(IMayHaveTenant.TenantId)),
                        tenantProperty);

                finalExpression =
                    finalExpression == null
                    ? tenantExpression
                    : Expression.AndAlso(
                        finalExpression,
                        tenantExpression);
            }

            if (finalExpression != null)
            {
                entityType.SetQueryFilter(
                    Expression.Lambda(
                        finalExpression,
                        parameter));
            }
        }
    }




    public static void ApplyMasterDbFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var parameter =
                Expression.Parameter(entityType.ClrType, "e");

            Expression? finalExpression = null;

            var isDeletedProp =
                entityType.ClrType.GetProperty("IsDeleted");

            if (isDeletedProp?.PropertyType == typeof(bool))
            {
                finalExpression = Expression.Equal(
                    Expression.Property(parameter, isDeletedProp),
                    Expression.Constant(false));
            }

            if (finalExpression != null)
            {
                entityType.SetQueryFilter(
                    Expression.Lambda(
                        finalExpression,
                        parameter));
            }
        }
    }

}
