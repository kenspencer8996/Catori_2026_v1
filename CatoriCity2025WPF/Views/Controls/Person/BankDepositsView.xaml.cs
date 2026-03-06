using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.ViewModels;
using System.Windows;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for BankDepositsView.xaml
    /// </summary>
    public partial class BankDepositsView : Window
    {
        private readonly BankDepositsController _controller;

        public BankDepositsView(BankViewModel bankViewModel)
        {
            InitializeComponent();
            _controller = new BankDepositsController(this, bankViewModel);
            this.Loaded += BankDepositsView_Loaded;
        }

        private void BankDepositsView_Loaded(object? sender, RoutedEventArgs e)
        {
            txtStatus.Text = "Loading...";
            _controller.Load();
            dgDeposits.ItemsSource = _controller.Deposits;
            txtStatus.Text = $"Loaded {_controller.Deposits.Count} deposits";
        }


        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "Refreshing...";
            await _controller.Load();
            dgDeposits.ItemsSource = _controller.Deposits;
            txtStatus.Text = $"Loaded {_controller.Deposits.Count} deposits";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal void SetStatus(string text)
        {
            txtStatus.Text = text;
        }
    }
}
