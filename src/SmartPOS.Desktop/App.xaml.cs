using Serilog;
using SmartPOS.Desktop.Services;
using System.Windows;

namespace SmartPOS.Desktop
{
    public partial class App : Application
    {
        public static ApiService ApiService { get; set; } = new ApiService();

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .CreateLogger();

            Log.Information("SmartPOS AI Desktop starting...");
            base.OnStartup(e);
        }
    }
}