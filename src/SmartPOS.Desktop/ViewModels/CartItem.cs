using SmartPOS.Desktop.Helpers;

namespace SmartPOS.Desktop.ViewModels
{
    public class CartItem : BaseViewModel
    {
        private int _quantity;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                SetProperty(ref _quantity, value);
                OnPropertyChanged(nameof(LineTotal));
            }
        }

        public decimal LineTotal => UnitPrice * Quantity;
    }
}