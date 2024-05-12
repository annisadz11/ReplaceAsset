using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReplaceAsset.Models
{
	public class NewAssetReplacement
	{
		[Key]
        public int Id { get; set; }

        [ForeignKey("AssetRequest")]
        public int AssetRequestId { get; set; }

        public AssetRequest AssetRequest { get; set; }

        [Required]
        public string Name { get; set; }


        public string NewType { get; set; }

        public string NewSerialNumber { get; set; }

        public DateTime? DateReplace { get; set; }

	}
}
