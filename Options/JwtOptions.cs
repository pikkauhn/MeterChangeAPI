namespace MeterChangeApi.Options
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = String.Empty;
        public string Issuer { get; set; } = String.Empty;
        public string Audience { get; set; } = String.Empty;
        public int ExpirationHours { get; set; }
    }
}
