using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Common.Interfaces;

public interface ITwoFactorService
{
    GenerateTwoFactorCodeDto GenerateSetupCode(string appName);
    bool VerifyCodeAsync(string secret, string code);
    Task<bool> Save2FaVerifyCodeAsync(string secret, string code);
    Task<bool> SetAttemptCount(Guid userId);
    Task ResetAttemptCount(Guid userId);
}
