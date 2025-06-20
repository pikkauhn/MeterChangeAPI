namespace MeterChangeApi.Options
{
    /// <summary>
    /// Configuration options specific to the development environment.
    /// </summary>
    public class DevelopmentOptions
    {
        /// <summary>
        /// Indicates whether Swagger UI and its related endpoints should be enabled in the development environment.
        /// Defaults to true.
        /// </summary>
        public bool EnableSwagger { get; set; } = true;

        /// <summary>
        /// The connection string to be used for the database in the development environment.
        /// Provides a default value for convenience.
        /// </summary>
        public string DevelopmentConnectionString { get; set; } = "Server=localhost;Database=DevDb;...";

        /// <summary>
        /// Configuration options for Swagger UI.
        /// </summary>
        public SwaggerOptions Swagger { get; set; } = new SwaggerOptions();
    }

    /// <summary>
    /// Configuration options for Swagger UI.
    /// </summary>
    public class SwaggerOptions
    {
        /// <summary>
        /// Indicates whether authorization support (e.g., the "Authorize" button) should be enabled in Swagger UI.
        /// Defaults to true.
        /// </summary>
        public bool EnableAuthorization { get; set; } = true;

        /// <summary>
        /// The title to be displayed in the Swagger UI.
        /// Defaults to "MeterChangeApi".
        /// </summary>
        public string Title { get; set; } = "MeterChangeApi";

        /// <summary>
        /// The version of the API to be displayed in the Swagger UI.
        /// Defaults to "v1".
        /// </summary>
        public string Version { get; set; } = "v1";
    }
}