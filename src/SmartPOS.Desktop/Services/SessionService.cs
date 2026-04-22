namespace SmartPOS.Desktop.Services
{
    public static class SessionService
    {
        public static int UserId { get; private set; }
        public static string FullName { get; private set; } = string.Empty;
        public static string Email { get; private set; } = string.Empty;
        public static string Role { get; private set; } = string.Empty;
        public static string Token { get; private set; } = string.Empty;
        public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public static void SetSession(int userId, string fullName, string email, string role, string token)
        {
            UserId = userId;
            FullName = fullName;
            Email = email;
            Role = role;
            Token = token;
        }

        public static void ClearSession()
        {
            UserId = 0;
            FullName = string.Empty;
            Email = string.Empty;
            Role = string.Empty;
            Token = string.Empty;
        }

        public static bool IsAdmin => Role == "Admin";
        public static bool IsManager => Role is "Admin" or "Manager";
        public static bool IsCashier => Role == "Cashier";
    }
}