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

namespace SamaniCrm.Application.InitialApp.Queries;

public record InitialAppQuery() : IRequest<InitialAppDTO>;


public class InitialAppQueryHandler : IRequestHandler<InitialAppQuery, InitialAppDTO>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ICurrentUserService currentUserService;
    private readonly IConfiguration _configuration;



    public InitialAppQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IConfiguration configuration)
    {
        this.dbContext = dbContext;
        this.currentUserService = currentUserService;
        _configuration = configuration;
    }

    public async Task<InitialAppDTO> Handle(InitialAppQuery request, CancellationToken cancellationToken)
    {
        List<LanguageDTO> languages = await dbContext.Languages
                        .Where(x => x.IsActive)
            .Select(s => new LanguageDTO()
            {
                Name = s.Name,
                Culture = s.Culture,
                Flag = s.Flag,
                IsRtl = s.IsRtl,
                IsDefault = s.IsDefault
            }).ToListAsync();
        string defaultLanguage = languages.Find(x => x.IsDefault)?.Culture ?? "";
        bool.TryParse(_configuration["Captcha:Enabled"], out var requiredCaptcha);

        return new InitialAppDTO()
        {
            Languages = languages,
            CurrentLanguage = currentUserService.lang,
            DefaultLang = defaultLanguage,
            RequireCaptcha = requiredCaptcha
        };
    }
}






