using CatoriCity2025WPF.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for BankListView.xaml
    /// </summary>
    public partial class BankListView : Window
    {
        private readonly BankListController _controller;

        public BankListView()
        {
            InitializeComponent();
            _controller = new BankListController(this);
            Loaded += BankListView_Loaded;
        }

        private async void BankListView_Loaded(object? sender, RoutedEventArgs e)
        {
            TxtStatus.Text = "Loading banks...";
            await _controller.LoadAsync();
            DgBanks.ItemsSource = _controller.Banks;
            TxtStatus.Text = $"Loaded {_controller.Banks.Count} banks";
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TxtStatus.Text = "Refreshing...";
            await _controller.LoadAsync();
            DgBanks.ItemsSource = _controller.Banks;
            TxtStatus.Text = $"Loaded {_controller.Banks.Count} banks";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            _controller.New();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var vm = DgBanks.SelectedItem as ViewModels.BankViewModel;
            if (vm != null)
                _controller.Edit(vm);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var vm = DgBanks.SelectedItem as ViewModels.BankViewModel;
            if (vm != null)
                _controller.Delete(vm);
        }

        internal void SetStatus(string text) => TxtStatus.Text = text;

        private void DgBanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid grid && grid.SelectedItem is ViewModels.BankViewModel bank)
            {
                BankEditView bankEdit = new BankEditView(bank);
                bankEdit.Owner = this;
                bankEdit.ShowDialog();
            }
        }
    }
}
