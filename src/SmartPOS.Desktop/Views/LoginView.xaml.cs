using SmartPOS.Desktop.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SmartPOS.Desktop.Views
{
    public partial class LoginView : Window
    {
        private LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            _viewModel.LoginSuccessful += OnLoginSuccessful;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnLoginSuccessful()
        {
            var mainWindow = new MainView();
            mainWindow.Show();
            Close();
        }
    }
}