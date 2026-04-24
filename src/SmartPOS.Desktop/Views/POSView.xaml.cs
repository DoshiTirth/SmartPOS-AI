using SmartPOS.Desktop.ViewModels;
using System.Windows.Controls;

namespace SmartPOS.Desktop.Views
{
    public partial class POSView : UserControl
    {
        public POSView()
        {
            InitializeComponent();
            DataContext = new POSViewModel();
        }
    }
}