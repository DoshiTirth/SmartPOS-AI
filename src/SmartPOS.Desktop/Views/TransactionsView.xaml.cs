using SmartPOS.Desktop.ViewModels;
using System.Windows.Controls;

namespace SmartPOS.Desktop.Views
{
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
            DataContext = new TransactionsViewModel();
        }
    }
}