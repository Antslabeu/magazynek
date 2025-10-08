
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
        public required DbSet<ProjectItem> ProjectItems { get; set; }
        public required DbSet<SystemSetting> SystemSettings { get; set; }

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
            modelBuilder.Entity<ProjectItem>()
                .HasIndex(p => p.id)
                .IsUnique();

            modelBuilder.Entity<ProjectRealization>()
                .HasIndex(p => p.id)
                .IsUnique();

            modelBuilder.Entity<SystemSetting>()
                .HasIndex(s => s.Name)
                .IsUnique();
                
            base.OnModelCreating(modelBuilder);
        }
    }
}