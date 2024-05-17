using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Models;

namespace ReplaceAsset.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<AssetRequest> AssetRequest { get; set; }
		public DbSet<AssetScrap> AssetScrap { get; set; }

		public DbSet<ComponentAssetReplacement> ComponentAssetReplacement { get; set; }
		public DbSet<NewAssetReplacement> NewAssetReplacement { get; set; }
        public DbSet<NewHire> NewHire { get; set; }

    }
}