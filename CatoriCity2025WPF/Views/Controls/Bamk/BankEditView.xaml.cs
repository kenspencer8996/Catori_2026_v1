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
    /// Interaction logic for BankEditView.xaml
    /// </summary>
    public partial class BankEditView : Window
    {
        public BankEditView()
        {
            InitializeComponent();
        }
        private readonly BankEditController _controller;

        public BankEditView(ViewModels.BankViewModel model)
        {
            InitializeComponent();
            _controller = new BankEditController(this, model);
            DataContext = _controller.EditModel;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            BtnSave.IsEnabled = false;
            try
            {
                await _controller.SaveAsync();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnSave.IsEnabled = true;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
