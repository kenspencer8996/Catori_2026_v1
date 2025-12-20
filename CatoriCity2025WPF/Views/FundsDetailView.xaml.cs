using CatoriCity2025WPF.Convertors;
using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Viewmodels;
using CatoriCity2025WPF.ViewModels;
using System.Windows;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for FundsDetailView.xaml
    /// </summary>
    public partial class FundsDetailView : Window
    {
        FundsDetailViewController _controller;
        public FundsDetailView(PersonViewModel person)
        {
            InitializeComponent();
            _controller = new FundsDetailViewController(this,person);
        }

        private void DepositAmountTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            double i = 0;
            string s = DepositAmountTextBox.Text;
            bool result = double.TryParse(s, out i); //i now = 108  
            if (result == false)
            {
                return;
            }
            _controller.DepositAmountChanged();
        }
      
        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.SendDepositToBank();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BankStackComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedItem = (BankViewModel)BankStackComboBox.SelectedItem;
            _controller.BankSelected(selectedItem);
        }
    }
}
