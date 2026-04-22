using SmartPOS.Desktop.Services;
using System.Windows;

namespace SmartPOS.Desktop
{
    public partial class App : Application
    {
        public static ApiService ApiService { get; set; } = new ApiService();
    }
}