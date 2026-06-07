using Microsoft.Extensions.Configuration;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Infrastructure.TenantManager;

public sealed class CurrentTenant : ICurrentTenant
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly IConfiguration _configuration;

    public CurrentTenant(ITenantContextAccessor tenantContextAccessor, IConfiguration configuration)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _configuration = configuration;
    }

    public Guid? TenantId => _tenantContextAccessor.Current?.TenantId;
    public string? TenantSlug => _tenantContextAccessor.Current?.TenantSlug;
    public string? TenantName => _tenantContextAccessor.Current?.TenantName;
    public string? ConnectionString => _tenantContextAccessor.Current?.ConnectionString;
    public bool HasTenant => TenantId.HasValue;

    public IDisposable Change(
        Guid? tenantId,
        string? tenantSlug = null,
        string? tenantName = null,
        string? connectionString = null)
    {
        var parentContext = _tenantContextAccessor.Current;

        _tenantContextAccessor.Current = new TenantContext(
            tenantId,
            tenantSlug,
            tenantName,
            connectionString);

        return new DisposeAction(() =>
        {
            _tenantContextAccessor.Current = parentContext;
        });
    }

    public void Clear()
    {
        _tenantContextAccessor.Current = null;
    }

    private sealed class DisposeAction : IDisposable
    {
        private Action? _dispose;

        public DisposeAction(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose?.Invoke();
            _dispose = null;
        }
    }


    public string GetMasterConnectionString()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Master connection string not found.");

        return connectionString;
    }

    public string GetCurrentConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
            return ConnectionString!;

        return GetMasterConnectionString();
    }



}