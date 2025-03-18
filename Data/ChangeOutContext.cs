using MeterChangeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Data
{
    public class ChangeOutContext : DbContext
    {
        public ChangeOutContext(DbContextOptions<ChangeOutContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Wmeter> meters { get; set; }
        public DbSet<Models.WEndpoint> endpoints { get; set; }
        public DbSet<ArcGISData> ArcGISData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}