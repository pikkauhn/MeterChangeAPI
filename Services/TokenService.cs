using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Models;
using MeterChangeApi.Options;

namespace MeterChangeApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;

        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateJwtToken(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()) // You can add other user-specific claims here
            };

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}