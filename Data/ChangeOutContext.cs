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
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Models.Endpoint> Endpoints { get; set; }
        public DbSet<ArcGISData> ArcGISData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}