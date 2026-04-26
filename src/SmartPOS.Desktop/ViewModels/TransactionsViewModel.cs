using SmartPOS.Desktop.Helpers;
using SmartPOS.Desktop.Services;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SmartPOS.Desktop.ViewModels
{
    public class TransactionListItem
    {
        [JsonPropertyName("transactionId")] public int TransactionId { get; set; }
        [JsonPropertyName("transactionCode")] public string TransactionCode { get; set; } = string.Empty;
        [JsonPropertyName("totalAmount")] public decimal TotalAmount { get; set; }
        [JsonPropertyName("paymentMethod")] public string PaymentMethod { get; set; } = string.Empty;
        [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("cashierName")] public string CashierName { get; set; } = string.Empty;
        [JsonPropertyName("customerName")] public string? CustomerName { get; set; }
    }

    public class TransactionsViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        public int TotalTransactions => Transactions.Count;
        public decimal TotalRevenue => Transactions.Where(t => t.Status == "Completed").Sum(t => t.TotalAmount);
        public decimal AverageTransaction => TotalTransactions > 0 ? TotalRevenue / TotalTransactions : 0;

        public ObservableCollection<TransactionListItem> Transactions { get; set; } = new();

        public RelayCommand RefreshCommand { get; }

        public TransactionsViewModel()
        {
            _apiService = App.ApiService;
            RefreshCommand = new RelayCommand(async _ => await LoadTransactionsAsync());
            _ = LoadTransactionsAsync();
        }

        private async Task LoadTransactionsAsync()
        {
            var transactions = await _apiService.GetAsync<List<TransactionListItem>>("transactions/recent?count=50");
            if (transactions != null)
            {
                Transactions.Clear();
                foreach (var t in transactions) Transactions.Add(t);
                OnPropertyChanged(nameof(TotalTransactions));
                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(AverageTransaction));
            }
        }
    }
}