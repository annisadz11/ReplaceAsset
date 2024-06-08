using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Models;
using System.Configuration;

namespace ReplaceAsset.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<AssetRequest> AssetRequest { get; set; }
        public DbSet<AssetScrap> AssetScrap { get; set; }

        public DbSet<ComponentAssetReplacement> ComponentAssetReplacement { get; set; }
        public DbSet<NewAssetReplacement> NewAssetReplacement { get; set; }
        public DbSet<NewHire> NewHire { get; set; }
        public DbSet<UserAdmin> UserAdmins { get; set; }

        public DbSet<UserIntern> UserInterns { get; set; }

        public DbSet<UserManagerIT> UserManagerITs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(
            connectionString,
            x => x.MigrationsHistoryTable("IT-App_EFMigrationsHistory", "IT-App_Schema"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("IT-App_Schema");
            base.OnModelCreating(modelBuilder);
        }
    }
}