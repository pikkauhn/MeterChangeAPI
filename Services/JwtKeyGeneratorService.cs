using System.Text;
using System.Security.Cryptography;

namespace MeterChangeApi.Services
{
    /// <summary>
    /// A background service responsible for generating and rotating JWT signing keys.
    /// It generates a new key periodically and manages a previous key for smooth key rollover.
    /// The keys are stored in files within the application's content root.
    /// </summary>
    public class JwtKeyGeneratorService : BackgroundService
    {
        private readonly ILogger<JwtKeyGeneratorService> _logger;
        private readonly string _keysDirectory;
        private readonly string _currentKeyFile;
        private readonly string _previousKeyFile;
        private readonly TimeSpan _generationInterval = TimeSpan.FromDays(7); // Rotate keys every 7 days.
        private DateTime _lastGenerationTime = DateTime.MinValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtKeyGeneratorService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging messages within this service.</param>
        /// <param name="environment">The hosting environment to access the content root path.</param>
        public JwtKeyGeneratorService(ILogger<JwtKeyGeneratorService> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _keysDirectory = Path.Combine(environment.ContentRootPath, "jwt_keys");
            Directory.CreateDirectory(_keysDirectory); // Ensure the directory exists.
            _currentKeyFile = Path.Combine(_keysDirectory, "current_jwt_key.txt");
            _previousKeyFile = Path.Combine(_keysDirectory, "previous_jwt_key.txt");
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JwtKeyGeneratorService started.");

            // Generate initial key if it doesn't exist.
            if (!File.Exists(_currentKeyFile))
            {
                await GenerateAndStoreKey(_currentKeyFile);
                _lastGenerationTime = DateTime.UtcNow;
            }
            else
            {
                _lastGenerationTime = File.GetLastWriteTimeUtc(_currentKeyFile);
            }

            // Periodically check if it's time to rotate the keys.
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.UtcNow - _lastGenerationTime >= _generationInterval)
                {
                    _logger.LogInformation("Generating new JWT key.");
                    await RotateKeys();
                    _lastGenerationTime = DateTime.UtcNow;
                }

                // Wait for an hour before checking again.
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("JwtKeyGeneratorService stopped.");
        }

        /// <summary>
        /// Rotates the JWT signing keys by generating a new key, moving the current key to the previous key file,
        /// and saving the new key as the current key. Handles potential file system exceptions.
        /// </summary>
        private async Task RotateKeys()
        {
            string newKey = GenerateSecureKey();

            // Move the current key to the previous key file.
            if (File.Exists(_currentKeyFile))
            {
                if (File.Exists(_previousKeyFile))
                {
                    try
                    {
                        File.Delete(_previousKeyFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error deleting previous key file: {ErrorMessage}", ex.Message);
                        return;
                    }
                }
                try
                {
                    File.Move(_currentKeyFile, _previousKeyFile, true); // Overwrite if it exists.
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error moving current key to previous: {ErrorMessage}", ex.Message);
                    return;
                }
            }

            // Save the new key to the current key file.
            await File.WriteAllTextAsync(_currentKeyFile, newKey, Encoding.UTF8);
            _logger.LogInformation("New JWT key generated and stored.");
        }

        /// <summary>
        /// Generates a new secure JWT signing key and stores it in the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file where the key should be stored.</param>
        private async Task GenerateAndStoreKey(string filePath)
        {
            string newKey = GenerateSecureKey();
            await File.WriteAllTextAsync(filePath, newKey, Encoding.UTF8);
            _logger.LogInformation("Initial JWT key generated and stored at {filePath}.", filePath);
        }

        /// <summary>
        /// Generates a cryptographically secure key using <see cref="RandomNumberGenerator"/>.
        /// </summary>
        /// <returns>A base64 encoded string representing the secure key.</returns>
        private static string GenerateSecureKey()
        {
            byte[] keyBytes = new byte[64]; // 512 bits for strong security.
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            return Convert.ToBase64String(keyBytes);
        }
    }
}