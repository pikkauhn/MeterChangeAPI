using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using MeterChangeApi.Models;
using MeterChangeApi.Options;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services
{
    /// <summary>
    /// Service responsible for generating and validating JSON Web Tokens (JWTs) for user authentication.
    /// It manages the current and a previous signing key for token rollover.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly string _currentKeyFile;
        private readonly string _previousKeyFile;
        private readonly ILogger<TokenService> _logger;
        private SymmetricSecurityKey? _currentSigningKey;
        private SymmetricSecurityKey? _previousSigningKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="jwtOptions">The application's JWT configuration options.</param>
        /// <param name="environment">The hosting environment to access the content root path.</param>
        /// <param name="logger">The logger instance for logging messages within this service.</param>
        public TokenService(IOptions<JwtOptions> jwtOptions, IHostEnvironment environment, ILogger<TokenService> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            string keysDirectory = Path.Combine(environment.ContentRootPath, "jwt_keys");
            _currentKeyFile = Path.Combine(keysDirectory, "current_jwt_key.txt");
            _previousKeyFile = Path.Combine(keysDirectory, "previous_jwt_key.txt");
            LoadSigningKeys();
        }

        /// <summary>
        /// Loads the current and previous JWT signing keys from files.
        /// Handles cases where the previous key file might not exist.
        /// Logs errors if the current key file is not found or if an exception occurs during loading.
        /// </summary>
        private void LoadSigningKeys()
        {
            try
            {
                if (File.Exists(_currentKeyFile))
                {
                    string currentKeyBase64 = File.ReadAllText(_currentKeyFile);
                    _currentSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(currentKeyBase64));
                }
                else
                {
                    _logger.LogError("Current JWT key file not found!");
                }

                if (File.Exists(_previousKeyFile))
                {
                    string previousKeyBase64 = File.ReadAllText(_previousKeyFile);
                    _previousSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(previousKeyBase64));
                }
                else
                {
                    _logger.LogInformation("Previous JWT key file not found (this is normal initially or after a clean start).");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading JWT signing keys: {ErrorMessage}", ex.Message);
            }
        }

        /// <summary>
        /// Generates a new JWT token for the given user.
        /// Includes the user's username and ID as claims in the token.
        /// Uses the current signing key to sign the token.
        /// </summary>
        /// <param name="user">The <see cref="Users"/> object for whom to generate the token.</param>
        /// <returns>The generated JWT token as a string, or null if the current signing key is not loaded.</returns>
        public string? GenerateJwtToken(Users user)
        {
            if (_currentSigningKey == null)
            {
                _logger.LogError("Current signing key is not loaded. Cannot generate token.");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", user.UserId.ToString()) // Custom claim for user ID.
                ]),
                Expires = DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours),
                SigningCredentials = new SigningCredentials(_currentSigningKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates a given JWT token. Checks the signature, issuer, audience, and expiration.
        /// It attempts validation against both the current and the previous signing keys to support token rollover.
        /// </summary>
        /// <param name="authToken">The JWT token string to validate.</param>
        /// <returns>True if the token is valid, false otherwise.</returns>
        public bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = ValidSigningKeys, // Use both current and previous keys for validation.
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Recommended to avoid issues with server/client time differences.
                };

                tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken validatedToken);
                return validatedToken != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token validation failed: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the list of valid signing keys to use for token validation.
        /// This includes both the current and the previous signing keys, if they are loaded.
        /// </summary>
        private IEnumerable<SecurityKey> ValidSigningKeys
        {
            get
            {
                var keys = new List<SecurityKey>();
                if (_currentSigningKey != null)
                {
                    keys.Add(_currentSigningKey);
                }
                if (_previousSigningKey != null)
                {
                    keys.Add(_previousSigningKey);
                }
                return keys;
            }
        }
    }
}