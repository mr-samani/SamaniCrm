using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Services
{
    public class SecuritySettingService : ISecuritySettingService
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public SecuritySettingService(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<SecuritySettingDTO> GetSettingsAsync()
        {
            // TODO : must be cache setting
            var entity = await _applicationDbContext.SecuritySettings.FirstAsync();
            return new SecuritySettingDTO
            {
                PasswordComplexity = new PasswordComplexityDTO
                {
                    RequiredLength = entity.RequiredLength,
                    RequireDigit = entity.RequireDigit,
                    RequireLowercase = entity.RequireLowercase,
                    RequireUppercase = entity.RequireUppercase,
                    RequireNonAlphanumeric = entity.RequireNonAlphanumeric
                }             
            };
        }
    }
}
