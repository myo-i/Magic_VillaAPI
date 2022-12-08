using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Dto
{
    public class VillaCreateDto
    {
        // createする際は自動的にIdが付与されるので必要ない
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public string Details { get; set; }
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}