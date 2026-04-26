using SmartPOS.Core.DTOs;
using SmartPOS.Desktop.Helpers;
using SmartPOS.Desktop.Services;
using System.Collections.ObjectModel;

namespace SmartPOS.Desktop.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private string _statusMessage = "Loading...";

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public int TotalProducts => Products.Count;
        public int LowStockCount => Products.Count(p => p.IsLowStock);
        public int OutOfStockCount => Products.Count(p => p.StockQuantity == 0);
        public decimal InventoryValue => Products.Sum(p => p.UnitPrice * p.StockQuantity);

        public ObservableCollection<ProductDto> Products { get; set; } = new();

        public RelayCommand RefreshCommand { get; }

        public InventoryViewModel()
        {
            _apiService = App.ApiService;
            RefreshCommand = new RelayCommand(async _ => await LoadProductsAsync());
            _ = LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            StatusMessage = "Loading products...";
            var products = await _apiService.GetAsync<List<ProductDto>>("products");
            if (products != null)
            {
                Products.Clear();
                foreach (var p in products) Products.Add(p);
                OnPropertyChanged(nameof(TotalProducts));
                OnPropertyChanged(nameof(LowStockCount));
                OnPropertyChanged(nameof(OutOfStockCount));
                OnPropertyChanged(nameof(InventoryValue));
                StatusMessage = $"Loaded {Products.Count} products";
            }
            else
            {
                StatusMessage = "Failed to load products";
            }
        }
    }
}