using System.ComponentModel.DataAnnotations;

namespace ReplaceAsset.Models
{
    public class NewHire
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Device { get; set; }
        public string SerialNumber { get; set; }
        public string ModelAsset { get; set; }
        public DateTime? DateOfJoin { get; set; }
        public bool StatusCompleted { get; set; }

        // Checklist items
        public bool HeadsetGiven { get; set; }
        public bool LaptopGiven { get; set; }
        public bool AdaptorGiven { get; set; }
        public bool PowerCableGiven { get; set; }
        public bool BagGiven { get; set; }
    }
}
