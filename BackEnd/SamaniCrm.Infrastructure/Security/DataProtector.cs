using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Security;

public interface ITenantDataProtector
{
    ITenantDataProtector CreateProtector(string purpose);
    string Protect(string data);
    string Unprotect(string protectedData);
}

public class TenantDataProtector : ITenantDataProtector
{
    private readonly IDataProtectionProvider _provider;
    private readonly string _applicationName = "MultiTenantApp";

    public TenantDataProtector(IDataProtectionProvider provider)
    {
        _provider = provider;
    }

    public ITenantDataProtector CreateProtector(string purpose)
    {
        return (ITenantDataProtector)_provider.CreateProtector($"{_applicationName}.{purpose}");
    }

    public string Protect(string data)
    {
        var protector = _provider.CreateProtector(_applicationName);
        return protector.Protect(data);
    }

    public string Unprotect(string protectedData)
    {
        var protector = _provider.CreateProtector(_applicationName);
        return protector.Unprotect(protectedData);
    }
}