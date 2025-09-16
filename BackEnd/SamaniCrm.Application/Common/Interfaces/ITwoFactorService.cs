using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Common.Interfaces;

public interface ITwoFactorService
{
    public GenerateTwoFactorCodeDto GenerateSetupCode(string appName);
    public Task<bool> VerifyCodeAsync(string secret, string code);
}
