using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MeterChangeApi.Data
{
    public class ChangeOutContextFactory : IDesignTimeDbContextFactory<ChangeOutContext>
    {
        public ChangeOutContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<ChangeOutContext>()
                .Build();

            // var connectionString = "Server=127.0.0.1;Port=3306;Database=searcy_water_utilities;User=Pikkauhn;Password=On!onKn1ght!;";
            var connectionString = configuration.GetConnectionString("mysqlconnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The MySQL connection string 'mysqlconnection' is missing or empty.");
            }

            var builder = new DbContextOptionsBuilder<ChangeOutContext>();
            builder.UseMySQL(connectionString);

            return new ChangeOutContext(builder.Options);
        }
    }
}