namespace MeterChangeApi.Options
{
    /// <summary>
    /// Configuration options for generating and validating JSON Web Tokens (JWTs).
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// The secret key used to sign and verify JWTs.
        /// </summary>
        public string Key { get; set; } = String.Empty;

        /// <summary>
        /// The issuer of the JWT. This value is often your application's domain.
        /// </summary>
        public string Issuer { get; set; } = String.Empty;

        /// <summary>
        /// The intended audience of the JWT. This value indicates who is allowed to consume the token.
        /// </summary>
        public string Audience { get; set; } = String.Empty;

        /// <summary>
        /// The duration in hours after which the generated JWT will expire.
        /// </summary>
        public int ExpirationHours { get; set; }
    }
}