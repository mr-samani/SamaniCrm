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
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public TokenGenerator(IConfiguration config, ApplicationDbContext context)
        {
            _configuration = config;
            _context = context;
        }


        public string GenerateAccessToken(Guid userId, string userName, string lang, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                new Claim("lang",lang)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expiration"])),
                signingCredentials: signingCredentials
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }





        public async Task<string> GenerateRefreshToken(Guid userId, string accessToken)
        {
            var newRefreshToken = new RefreshToken
            {
                Active = true,
                AccessToken = accessToken,
                Expiration = DateTime.UtcNow.AddDays(7),
                RefreshTokenValue = Guid.NewGuid().ToString("N"),
                Used = false,
                UserId = userId
            };
            _context.Add(newRefreshToken);
            await _context.SaveChangesAsync();
            return newRefreshToken.RefreshTokenValue;
        }

    }
}