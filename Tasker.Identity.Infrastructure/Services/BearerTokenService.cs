using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tasker.Identity.Application.Models;
using Tasker.Identity.Application.Services;
using Tasker.Identity.Infrastructure.Configuration;

namespace Tasker.Identity.Infrastructure.Services
{
    internal class BearerTokenService : IBearerTokenService
    {
        private readonly JwtConfigurationOptions _jwtConfiguration;

        public BearerTokenService(IOptions<JwtConfigurationOptions> configurationOptions)
        {
            _jwtConfiguration = configurationOptions.Value;
        }

        public string GetBearerToken(IIdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[] 
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken
            (
                _jwtConfiguration.Issuer,
                null,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtConfiguration.ExpireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
