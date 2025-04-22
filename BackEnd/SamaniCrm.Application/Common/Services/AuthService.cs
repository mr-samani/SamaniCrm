using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Application.Common.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthService(IConfiguration config, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _configuration = config;
            _userManager = userManager;
            _context = context;
        }


        public string GenerateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName??""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }





        public async Task<string> GenerateRefreshToken(ApplicationUser user, string accessToken)
        {
            var newRefreshToken = new RefreshToken
            {
                Active = true,
                AccessToken = accessToken,
                Expiration = DateTime.UtcNow.AddDays(7),
                RefreshTokenValue = Guid.NewGuid().ToString("N"),
                Used = false,
                UserId = user.Id
            };
            _context.Add(newRefreshToken);
            await _context.SaveChangesAsync();
            return newRefreshToken.RefreshTokenValue;
        }

    }
}