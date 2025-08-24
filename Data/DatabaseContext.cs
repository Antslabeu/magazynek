
using Microsoft.EntityFrameworkCore;
using Magazynek.Entities;


namespace Magazynek.Data
{
    public class DatabaseContext : DbContext
    {
        public required DbSet<ShippingEntry> ShippingEntries{ get; set; }
        public required DbSet<Product> Products { get; set; }
        public required DbSet<Project> Projects { get; set; }
        public required DbSet<ProjectRealization> ProjectRealizations { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShippingEntry>()
                .HasIndex(p => p.id)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.id)
                .IsUnique();
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.id)
                .IsUnique();

            modelBuilder.Entity<ProjectRealization>()
                .HasIndex(p => p.id)
                .IsUnique();
                
            base.OnModelCreating(modelBuilder);
        }
    }
}