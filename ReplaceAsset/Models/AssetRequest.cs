using System.ComponentModel.DataAnnotations;

namespace ReplaceAsset.Models
{
	public class AssetRequest
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string Departement { get; set; }
		public string Type { get; set; }
		public string SerialNumber { get; set; }
		public string Baseline { get; set; }
		public string UsageLocation { get; set; }
		public DateTime? RequestDate { get; set; }
		public string Reason { get; set; }
		public bool? Status { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public string? Justify { get; set; }
		public string? TypeReplace { get; set; }

		// Navigasi properti ke NewAssetReplacement
		public NewAssetReplacement? NewAssetReplacement { get; set; }

		// Navigasi properti ke ComponentAssetReplacement
		public ComponentAssetReplacement? ComponentAssetReplacement { get; set; }

		public string? Username { get; set; }
	}
}