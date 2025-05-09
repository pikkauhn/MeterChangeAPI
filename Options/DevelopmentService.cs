using MeterChangeApi.Options;

/// <summary>
/// Provides access to development-specific configuration options.
/// </summary>
/// <param name="options">The <see cref="DevelopmentOptions"/> instance to be used by this service.</param>
public class DevelopmentService(DevelopmentOptions options)
{
    /// <summary>
    /// Gets the development configuration options.
    /// </summary>
    public DevelopmentOptions Options { get; } = options;
}