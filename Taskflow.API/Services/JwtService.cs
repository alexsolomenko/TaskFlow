using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Taskflow.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string userName, List<string> roles);
    }

    internal class JwtService: IJwtService
    {
        private readonly string jwtKey;
        private readonly string issuer;
        private readonly string audience;
        private readonly int _expiryMinutes;

        public JwtService(IConfiguration config)
        {
            jwtKey = config["Jwt:Key"] ?? GenerateTestKey();
            issuer = config["Jwt:Issuer"] ?? "https://test-issuer.com";
            audience = config["Jwt:Audience"] ?? "https://test-audience.com";
            _expiryMinutes = config.GetValue<int>("Jwt:ExpiryMinutes", 60);
        }

        public string GenerateToken(string userId, string userName, List<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName)
                }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)))),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateTestKey()
        {
            var key = new byte[32];
            RandomNumberGenerator.Fill(key);
            return Convert.ToBase64String(key);
        }
    }
}
