using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 占有率
        /// </summary>
        public int Occupancy { get; set; }

        /// <summary>
        /// 平方フィート
        /// </summary>
        public int Sqft { get; set; }

    }
}
