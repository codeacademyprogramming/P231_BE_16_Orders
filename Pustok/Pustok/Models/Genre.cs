using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20,ErrorMessage = "20-den uzun ola bilmez!")]
        [MinLength(2)]
        public string Name { get; set; }
        public virtual List<Book> Books { get; set; }
    }
}
