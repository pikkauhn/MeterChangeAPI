using MeterChangeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeterChangeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("generate-token")]
        public IActionResult GenerateToken([FromBody] LoginRequest request)
        {
            if (request == null || request.Username != "testuser" || request.Password != "password")
            {
                return Unauthorized("Invalid credentials.");
            }

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return StatusCode(500, "JWT Key is missing or invalid in configuration.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, request.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120), 
                signingCredentials: credentials);
            
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}