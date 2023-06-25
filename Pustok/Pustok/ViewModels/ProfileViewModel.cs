using Pustok.Models;

namespace Pustok.ViewModels
{
    public class ProfileViewModel
    {
        public MemberUpdateViewModel Member { get; set; }
        public List<Order> Orders { get; set; }
    }
}
