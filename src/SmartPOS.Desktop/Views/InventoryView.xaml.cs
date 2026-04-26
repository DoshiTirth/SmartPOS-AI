using SmartPOS.Desktop.ViewModels;
using System.Windows.Controls;

namespace SmartPOS.Desktop.Views
{
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();
            DataContext = new InventoryViewModel();
        }
    }
}