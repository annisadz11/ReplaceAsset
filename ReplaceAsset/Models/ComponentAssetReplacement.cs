using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReplaceAsset.Models
{
    public class ComponentAssetReplacement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AssetRequest")]
        public int AssetRequestId { get; set; }

        public AssetRequest AssetRequest { get; set; }

        [Required]
        public string Name { get; set; }

        public bool ValidationReplace { get; set; }

        public DateTime? ComponentReplaceDate { get; set; }

    }
}
