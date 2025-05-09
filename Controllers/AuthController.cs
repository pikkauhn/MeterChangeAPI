using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using MeterChangeApi.Models;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Data.Logger;

namespace MeterChangeApi.Controllers
{
    /// <summary>
    /// API controller for handling user authentication and registration.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration configuration, IUserService userService, ITokenService tokenService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<AuthController> _logger = logger;

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="username">The desired username for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The optional email address for the new user.</param>
        /// <returns>An IActionResult indicating the success or failure of the registration.</returns>
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

        /// <summary>
        /// Generates a JWT token for an authenticated user.
        /// </summary>
        /// <param name="request">The login request containing the username and password.</param>
        /// <returns>An IActionResult containing the JWT token upon successful authentication, or an Unauthorized error for invalid credentials.</returns>
        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Entering GenerateToken action.");

            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _userService.AuthenticateUserAsync(request.Username, request.Password);

            if (user != null)
            {
                var jwtKey = _configuration["Jwt:Key"];
                _logger.LogInformation("Jwt:Key from configuration in GenerateToken: '{jwtKey}'", jwtKey);

                var token = _tokenService.GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("Invalid username or password.");
            }
        }
    }

    /// <summary>
    /// Represents the login request model.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The username for login.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The password for login.
        /// </summary>
        public string? Password { get; set; }
    }
}