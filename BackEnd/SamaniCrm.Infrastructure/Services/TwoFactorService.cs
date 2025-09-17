using OtpNet;
using QRCoder;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SamaniCrm.Infrastructure.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly ICurrentUserService currentUser;
    private readonly IApplicationDbContext applicationDbContext;
    private readonly IIdentityService identityService;
    public TwoFactorService(ICurrentUserService currentUser, IApplicationDbContext applicationDbContext, IIdentityService identityService)
    {
        this.currentUser = currentUser;
        this.applicationDbContext = applicationDbContext;
        this.identityService = identityService;
    }




    /// <summary>
    /// تولید Secret و QRCode برای اسکن در اپلیکیشن‌هایی مثل Google Authenticator
    /// </summary>
    public GenerateTwoFactorCodeDto GenerateSetupCode(string appName)
    {
        var userName = currentUser.UserName;

        // تولید secret (20 بایت)
        var key = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(key);

        // فرمت استاندارد otpauth:// برای سازگاری با Google/Microsoft Authenticator
        string otpauthUrl = $"otpauth://totp/{appName}:{userName}?secret={base32Secret}&issuer={appName}&digits=6";

        // تولید QRCode
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(otpauthUrl, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCoder.Base64QRCode(qrData);
        var qrCodeBase64 = qrCode.GetGraphic(20);
        var img = ("data:image/png;base64," + qrCodeBase64);


        return new GenerateTwoFactorCodeDto()
        {
            QrCode = img,
            Secret = base32Secret
        };
    }

    /// <summary>
    /// اعتبارسنجی کد وارد شده توسط کاربر
    /// </summary>
    public bool VerifyCodeAsync(string secret, string code)
    {
        var bytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(bytes);
        var checkResult = totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));
        // 30 ثانیه بازه زمانی
        return checkResult;
    }


    public async Task<bool> Save2FaVerifyCodeAsync(string secret, string code)
    {
        var bytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(bytes);
        var checkResult = totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));
        // 30 ثانیه بازه زمانی
        if (checkResult == false || currentUser.UserId == null)
        {
            return false;
        }
        var userId = Guid.Parse(currentUser.UserId);
        var data = applicationDbContext.UserSetting.Where(x => x.UserId == userId).FirstOrDefault();
        if (data == null)
        {
            data = new UserSetting()
            {
                UserId = userId,
                AttemptCount = 0,
                EnableTwoFactor = true,
                IsVerified = true,
                Secret = secret,
                TwoFactorType = TwoFactorTypeEnum.AuthenticatorApp,
            };
            await applicationDbContext.UserSetting.AddAsync(data);
        }
        else
        {
            data.AttemptCount = 0;
            data.EnableTwoFactor = true;
            data.TwoFactorType = TwoFactorTypeEnum.AuthenticatorApp;
            data.IsVerified = true;
            data.Secret = secret;

            applicationDbContext.UserSetting.Update(data);
        }
        await applicationDbContext.SaveChangesAsync();

        return true;
    }



    public async Task<bool> SetAttemptCount(Guid userId)
    {
        var data = applicationDbContext.UserSetting.Where(x => x.UserId == userId).FirstOrDefault();
        if (data == null)
        {
            data = new UserSetting()
            {
                UserId = userId,
                AttemptCount = 1,
                EnableTwoFactor = true,
                IsVerified = true,
                Secret = "",
                TwoFactorType = TwoFactorTypeEnum.AuthenticatorApp,
                LastAttemptAt = DateTime.UtcNow
            };
            await applicationDbContext.UserSetting.AddAsync(data);
        }
        else
        {
            data.AttemptCount = data.AttemptCount + 1;
            data.LastAttemptAt = DateTime.UtcNow;
            applicationDbContext.UserSetting.Update(data);
        }
        await applicationDbContext.SaveChangesAsync();
        return true;
    }

    public async Task ResetAttemptCount(Guid userId)
    {
        var data = applicationDbContext.UserSetting.Where(x => x.UserId == userId).FirstOrDefault();
        if (data != null)
        {
            data.AttemptCount = 0;
            data.LastAttemptAt = DateTime.UtcNow;
            applicationDbContext.UserSetting.Update(data);
            await applicationDbContext.SaveChangesAsync();
        }
    }
}
