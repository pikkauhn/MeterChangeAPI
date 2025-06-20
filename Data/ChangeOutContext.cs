using MeterChangeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Data
{
    /// <summary>
    /// Represents the database context for the Meter Change API, providing access to the application's data.
    /// </summary>
    /// <param name="options">The options configured for this <see cref="DbContext"/> instance.</param>
    public class ChangeOutContext(DbContextOptions<ChangeOutContext> options) : DbContext(options)
    {
        /// <summary>
        /// Represents the database set for the <see cref="Users"/> entity.
        /// </summary>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// Represents the database set for the <see cref="Address"/> entity.
        /// </summary>
        public DbSet<Address> Addresses { get; set; }

        /// <summary>
        /// Represents the database set for the <see cref="WMeter"/> entity (Water Meter).
        /// </summary>
        public DbSet<WMeter> Meters { get; set; }

        /// <summary>
        /// Represents the database set for the <see cref="Models.WEndpoint"/> entity (Water Endpoint).
        /// </summary>
        public DbSet<Models.WEndpoint> Endpoints { get; set; }

        /// <summary>
        /// Represents the database set for the <see cref="ArcGISData"/> entity.
        /// </summary>
        public DbSet<ArcGISData> ArcGISData { get; set; }

        /// <summary>
        /// Called when the model is being created. This method is typically overridden to configure the database schema.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // You can add custom model configurations here (e.g., relationships, constraints).
        }
    }
}