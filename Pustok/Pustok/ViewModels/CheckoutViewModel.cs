namespace Pustok.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CheckoutItemViewModel> Items { get; set; } = new List<CheckoutItemViewModel>();
        public decimal TotalAmount { get; set; }
        public OrderCreateViewModel Order { get; set; }
        
    }
}
