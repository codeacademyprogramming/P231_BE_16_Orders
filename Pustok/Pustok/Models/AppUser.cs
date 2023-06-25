using Microsoft.AspNetCore.Identity;

namespace Pustok.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<BasketItem> BasketItems { get; set; }
        public bool IsAdmin { get; set; }
    }
}
