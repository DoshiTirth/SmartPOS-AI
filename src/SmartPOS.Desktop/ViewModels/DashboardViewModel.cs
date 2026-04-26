using SmartPOS.Core.DTOs;
using SmartPOS.Desktop.Helpers;
using SmartPOS.Desktop.Services;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SmartPOS.Desktop.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private int _totalTransactionsToday;
        private decimal _totalRevenueToday;
        private decimal _averageTransactionValue;
        private int _totalTransactionsMonth;
        private decimal _totalRevenueMonth;
        private int _lowStockProductCount;
        private int _unreviewedAnomalies;

        public int TotalTransactionsToday
        {
            get => _totalTransactionsToday;
            set => SetProperty(ref _totalTransactionsToday, value);
        }

        public decimal TotalRevenueToday
        {
            get => _totalRevenueToday;
            set => SetProperty(ref _totalRevenueToday, value);
        }

        public decimal AverageTransactionValue
        {
            get => _averageTransactionValue;
            set => SetProperty(ref _averageTransactionValue, value);
        }

        public int TotalTransactionsMonth
        {
            get => _totalTransactionsMonth;
            set => SetProperty(ref _totalTransactionsMonth, value);
        }

        public decimal TotalRevenueMonth
        {
            get => _totalRevenueMonth;
            set => SetProperty(ref _totalRevenueMonth, value);
        }

        public int LowStockProductCount
        {
            get => _lowStockProductCount;
            set => SetProperty(ref _lowStockProductCount, value);
        }

        public int UnreviewedAnomalies
        {
            get => _unreviewedAnomalies;
            set => SetProperty(ref _unreviewedAnomalies, value);
        }

        public ObservableCollection<TopSellingProductDto> TopProducts { get; set; } = new();

        public RelayCommand RefreshCommand { get; }

        public DashboardViewModel()
        {
            _apiService = App.ApiService;
            RefreshCommand = new RelayCommand(async _ => await LoadDashboardAsync());
            _ = LoadDashboardAsync();
        }

        private async Task LoadDashboardAsync()
        {
            var summary = await _apiService.GetAsync<DashboardSummaryDto>("dashboard/summary");
            if (summary != null)
            {
                TotalTransactionsToday = summary.TotalTransactionsToday;
                TotalRevenueToday = summary.TotalRevenueToday;
                AverageTransactionValue = summary.AverageTransactionValue;
                TotalTransactionsMonth = summary.TotalTransactionsMonth;
                TotalRevenueMonth = summary.TotalRevenueMonth;
                LowStockProductCount = summary.LowStockProductCount;
                UnreviewedAnomalies = summary.UnreviewedAnomalies;
            }

            var topProducts = await _apiService.GetAsync<List<TopSellingProductDto>>("dashboard/products/topselling");
            if (topProducts != null)
            {
                TopProducts.Clear();
                foreach (var p in topProducts) TopProducts.Add(p);
            }
        }
    }
}