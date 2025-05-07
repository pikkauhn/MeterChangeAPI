using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Repositories.Interfaces;

namespace MeterChangeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration configuration, IUserService userService, ITokenService tokenService) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password, string? email)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password are required.");
            }

            var registrationResult = await _userService.RegisterUserAsync(username, password, email);

            if (registrationResult)
            {
                return Ok("User registered successfully.");
            }
            else
            {
                return BadRequest("Username already exists.");
            }
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _userService.AuthenticateUserAsync(request.Username, request.Password);

            if (user != null)
            {
                var token = _tokenService.GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("Invalid username or password.");
            }
        }
    }
}