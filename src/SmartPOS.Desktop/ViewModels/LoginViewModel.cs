using SmartPOS.Desktop.Helpers;
using SmartPOS.Desktop.Services;
using System.Text.Json.Serialization;

namespace SmartPOS.Desktop.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _hasError = false;

        public event Action? LoginSuccessful;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !_isLoading;

        public RelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            _apiService = new ApiService();
            LoginCommand = new RelayCommand(async _ => await ExecuteLoginAsync());
        }

        private async Task ExecuteLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your email and password.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _apiService.PostAsync<LoginResponse>("auth/login", new
                {
                    email = Email,
                    password = Password
                });

                if (result == null)
                {
                    ErrorMessage = "Invalid email or password.";
                    return;
                }

                _apiService.SetToken(result.Token);
                SessionService.SetSession(
                    result.UserId,
                    result.FullName,
                    result.Email,
                    result.Role,
                    result.Token
                );

                App.ApiService = _apiService;
                LoginSuccessful?.Invoke();
            }
            catch
            {
                ErrorMessage = "Unable to connect to server. Please try again.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    public class LoginResponse
    {
        [JsonPropertyName("token")] public string Token { get; set; } = string.Empty;
        [JsonPropertyName("fullName")] public string FullName { get; set; } = string.Empty;
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;
        [JsonPropertyName("userId")] public int UserId { get; set; }
    }
}