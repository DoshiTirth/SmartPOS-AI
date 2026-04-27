using SmartPOS.Desktop.Helpers;
using SmartPOS.Desktop.Services;
using System.Windows;
using System.Windows.Threading;

namespace SmartPOS.Desktop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DispatcherTimer _clockTimer;
        private object _currentView = new();
        private string _currentViewTitle = "POS Terminal";

        public string CashierName => SessionService.FullName;
        public string CashierRole => SessionService.Role;
        public bool IsManager => SessionService.IsManager;

        private string _currentTime = string.Empty;
        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set => SetProperty(ref _currentViewTitle, value);
        }

        public RelayCommand NavigateCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand(Navigate);
            LogoutCommand = new RelayCommand(_ => Logout());

            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += (s, e) => CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");
            _clockTimer.Start();
            CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");

            Navigate("POS");
        }

        private void Navigate(object? parameter)
        {
            var destination = parameter?.ToString() ?? "POS";
            switch (destination)
            {
                case "POS":
                    CurrentView = new Views.POSView();
                    CurrentViewTitle = "POS Terminal";
                    break;
                case "Inventory":
                    CurrentView = new Views.InventoryView();
                    CurrentViewTitle = "Inventory Management";
                    break;
                case "Transactions":
                    CurrentView = new Views.TransactionsView();
                    CurrentViewTitle = "Transaction History";
                    break;
                case "Dashboard":
                    CurrentView = new Views.DashboardView();
                    CurrentViewTitle = "Dashboard & Analytics";
                    break;
            }
        }

        private void Logout()
        {
            _clockTimer.Stop();
            SessionService.ClearSession();
            var loginView = new Views.LoginView();
            loginView.Show();
            foreach (Window window in Application.Current.Windows)
                if (window is not Views.LoginView)
                    window.Close();
        }
    }
}