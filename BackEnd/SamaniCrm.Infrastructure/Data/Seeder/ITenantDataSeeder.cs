using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Data.Seeder;

public interface ITenantDataSeeder
{
    int Order { get; }
    Task SeedAsync(Tenant tenant, CancellationToken cancellation);
}
