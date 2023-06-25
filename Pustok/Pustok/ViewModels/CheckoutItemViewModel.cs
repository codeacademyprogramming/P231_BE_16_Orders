using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Pustok.ViewModels
{
    public class CheckoutItemViewModel
    {
        public string BookName { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
