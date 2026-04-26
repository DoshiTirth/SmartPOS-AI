using SmartPOS.Desktop.ViewModels;
using System.Windows.Controls;

namespace SmartPOS.Desktop.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }
    }
}