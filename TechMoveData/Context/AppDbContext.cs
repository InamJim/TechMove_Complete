using Microsoft.EntityFrameworkCore;
using TechMoveData.Entities;
using TechMoveData.Enums;

namespace TechMoveData.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Contract> Contracts { get; set; } = null!;
        public DbSet<ServiceRequest> ServiceRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Contracts)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contract>()
                .HasMany(c => c.ServiceRequests)
                .WithOne(sr => sr.Contract)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contract>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.CostUSD)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.CostZAR)
                .HasPrecision(18, 2);
        }
    }
}