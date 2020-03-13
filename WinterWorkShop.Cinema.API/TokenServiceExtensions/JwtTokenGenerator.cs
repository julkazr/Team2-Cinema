using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WinterWorkShop.Cinema.API.TokenServiceExtensions
{
    public static class JwtTokenGenerator
    {
        // WARNING: This is just for demo purpose
        public static string Generate(string name, bool isAdmin, bool isSuperUser, string issuer, string key)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            if (isSuperUser)
            {
                claims.Add(new Claim(ClaimTypes.Role, "superuser"));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
