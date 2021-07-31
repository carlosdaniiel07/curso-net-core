using CursoNetCore.Domain.Entities;
using CursoNetCore.Domain.Interfaces.Services;
using CursoNetCore.Domain.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CursoNetCore.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfiguration _tokenConfiguration;

        public TokenService(IConfiguration configuration)
        {
            _tokenConfiguration = new TokenConfiguration
            {
                Secret = configuration.GetSection("Jwt:Secret").Value,
                Audience = configuration.GetSection("Jwt:Audience").Value,
                Issuer = configuration.GetSection("Jwt:Issuer").Value,
                Hours = int.Parse(configuration.GetSection("Jwt:Hours").Value),
            };
        }

        public string Generate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.ASCII.GetBytes(_tokenConfiguration.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(_tokenConfiguration.Hours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwtToken);
        }
    }
}
