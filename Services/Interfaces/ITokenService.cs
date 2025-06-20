using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for a service responsible for generating and validating JSON Web Tokens (JWTs).
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JSON Web Token (JWT) for the given user.
        /// </summary>
        /// <param name="user">The <see cref="Users"/> object for whom to generate the token.</param>
        /// <returns>The generated JWT as a string, or null if token generation fails.</returns>
        string? GenerateJwtToken(Users user);

        /// <summary>
        /// Validates a given JSON Web Token (JWT).
        /// </summary>
        /// <param name="authToken">The JWT string to validate.</param>
        /// <returns>True if the token is valid, false otherwise.</returns>
        bool ValidateToken(string authToken);
    }
}