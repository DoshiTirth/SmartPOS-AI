using SmartPOS.Core.DTOs;
using SmartPOS.Desktop.Helpers;
using SmartPOS.Desktop.Services;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Threading;

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

    public class MainViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly DispatcherTimer _clockTimer;

        private string _searchQuery = string.Empty;
        private string _statusMessage = "Ready";
        private string _currentTime = string.Empty;
        private bool _isCash = true;
        private bool _isCard;
        private bool _isOther;
        private int _transactionCount = 0;

        public string CashierName => SessionService.FullName;

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
                FilterProducts();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsCash
        {
            get => _isCash;
            set => SetProperty(ref _isCash, value);
        }

        public bool IsCard
        {
            get => _isCard;
            set => SetProperty(ref _isCard, value);
        }

        public bool IsOther
        {
            get => _isOther;
            set => SetProperty(ref _isOther, value);
        }

        public int TransactionCount
        {
            get => _transactionCount;
            set => SetProperty(ref _transactionCount, value);
        }

        public decimal Subtotal => CartItems.Sum(i => i.LineTotal);
        public decimal TaxAmount => Math.Round(Subtotal * 0.13m, 2);
        public decimal TotalAmount => Subtotal + TaxAmount;
        public int ProductCount => FilteredProducts.Count;
        public bool CanProcessPayment => CartItems.Any();

        public ObservableCollection<ProductDto> AllProducts { get; set; } = new();
        public ObservableCollection<ProductDto> FilteredProducts { get; set; } = new();
        public ObservableCollection<CartItem> CartItems { get; set; } = new();

        public RelayCommand AddToCartCommand { get; }
        public RelayCommand IncreaseQuantityCommand { get; }
        public RelayCommand DecreaseQuantityCommand { get; }
        public RelayCommand ClearCartCommand { get; }
        public RelayCommand ProcessPaymentCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public MainViewModel()
        {
            _apiService = App.ApiService;

            AddToCartCommand = new RelayCommand(AddToCart);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
            ClearCartCommand = new RelayCommand(_ => ClearCart());
            ProcessPaymentCommand = new RelayCommand(async _ => await ProcessPaymentAsync());
            LogoutCommand = new RelayCommand(_ => Logout());

            CartItems.CollectionChanged += (s, e) => RefreshTotals();

            // Clock timer
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += (s, e) => CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");
            _clockTimer.Start();
            CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");

            _ = LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            StatusMessage = "Loading products...";
            var products = await _apiService.GetAsync<List<ProductDto>>("products");
            if (products != null)
            {
                AllProducts.Clear();
                foreach (var p in products)
                    AllProducts.Add(p);

                FilterProducts();
                StatusMessage = $"Loaded {AllProducts.Count} products";
            }
            else
            {
                StatusMessage = "Failed to load products";
            }
        }

        private void FilterProducts()
        {
            FilteredProducts.Clear();
            var query = SearchQuery?.ToLower() ?? string.Empty;

            var filtered = string.IsNullOrWhiteSpace(query)
                ? AllProducts
                : AllProducts.Where(p =>
                    p.ProductName.ToLower().Contains(query) ||
                    p.SKU.ToLower().Contains(query) ||
                    (p.Barcode?.ToLower().Contains(query) ?? false));

            foreach (var p in filtered)
                FilteredProducts.Add(p);

            OnPropertyChanged(nameof(ProductCount));
        }

        private void AddToCart(object? parameter)
        {
            if (parameter is not ProductDto product) return;
            if (product.StockQuantity <= 0)
            {
                StatusMessage = $"{product.ProductName} is out of stock";
                return;
            }

            var existing = CartItems.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    Quantity = 1
                });
            }

            RefreshTotals();
            StatusMessage = $"Added {product.ProductName} to cart";
        }

        private void IncreaseQuantity(object? parameter)
        {
            if (parameter is not CartItem item) return;
            item.Quantity++;
            RefreshTotals();
        }

        private void DecreaseQuantity(object? parameter)
        {
            if (parameter is not CartItem item) return;
            if (item.Quantity <= 1)
                CartItems.Remove(item);
            else
                item.Quantity--;
            RefreshTotals();
        }


        private void ClearCart()
        {
            CartItems.Clear();
            RefreshTotals();
            StatusMessage = "Cart cleared";
        }

        private void RefreshTotals()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxAmount));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(CanProcessPayment));
        }

        private async Task ProcessPaymentAsync()
        {
            if (!CartItems.Any()) return;

            StatusMessage = "Processing payment...";

            var paymentMethod = IsCash ? "Cash" : IsCard ? "Card" : "Other";

            var dto = new CreateTransactionDto
            {
                CashierId = SessionService.UserId,
                PaymentMethod = paymentMethod,
                Items = CartItems.Select(i => new TransactionItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var result = await _apiService.PostAsync<TransactionResultDto>("transactions", dto);

            if (result != null)
            {
                TransactionCount++;
                StatusMessage = $"Transaction {result.TransactionCode} completed — ${result.TotalAmount:F2}";
                ClearCart();
                await LoadProductsAsync();
            }
            else
            {
                StatusMessage = "Payment failed. Please try again.";
            }
        }

        private void Logout()
        {
            _clockTimer.Stop();
            SessionService.ClearSession();
            var loginView = new Views.LoginView();
            loginView.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is not Views.LoginView)
                    window.Close();
            }
        }
    }
}