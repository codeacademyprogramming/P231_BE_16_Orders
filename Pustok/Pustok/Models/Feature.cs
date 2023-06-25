using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class Feature
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string Desc { get; set; }
        [Required]
        [MaxLength(50)]
        public string Icon { get; set; }

    }
}
