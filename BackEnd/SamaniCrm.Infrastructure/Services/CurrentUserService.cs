using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private string? _overrideLang;
        public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub")
                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("unique_name");

        // public string lang => _httpContextAccessor.HttpContext?.User?.FindFirstValue("lang") ?? "fa-IR";

        public string lang
        {
            get
            {
                if (_overrideLang == null)
                {
                    _overrideLang =
                        _httpContextAccessor.HttpContext?.Request.Cookies["lang"]
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("lang")
                        ?? "fa-IR"; 
                }
                return _overrideLang!;
            }
            set
            {
                _overrideLang = value;
            }
        }
    }
}
