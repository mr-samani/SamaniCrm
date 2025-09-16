using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Application.InitialApp.Queries;

public record InitialAppQuery() : IRequest<InitialAppDTO>;


public class InitialAppQueryHandler : IRequestHandler<InitialAppQuery, InitialAppDTO>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILanguageService _languageService;
    private readonly ISecuritySettingService _securitySettingService;



    public InitialAppQueryHandler(IApplicationDbContext dbContext, IConfiguration configuration, ILanguageService languageService, ISecuritySettingService securitySettingService)
    {
        this.dbContext = dbContext;
        _configuration = configuration;
        _languageService = languageService;
        _securitySettingService = securitySettingService;
    }

    public async Task<InitialAppDTO> Handle(InitialAppQuery request, CancellationToken cancellationToken)
    {
        List<LanguageDTO> languages = await _languageService.GetAllActiveLanguages();
        string defaultLanguage = languages.Find(x => x.IsDefault)?.Culture ?? "";
        var settings = await _securitySettingService.GetSettingsAsync(cancellationToken);
        bool.TryParse(_configuration["Captcha:Enabled"], out var requiredCaptchaAppSetting);

        var requiredCaptcha = settings.RequireCaptchaOnLogin || requiredCaptchaAppSetting;
        return new InitialAppDTO()
        {
            Languages = languages,
            DefaultLang = defaultLanguage,
            RequireCaptcha = requiredCaptcha
        };
    }
}






