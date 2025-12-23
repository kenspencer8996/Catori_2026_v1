using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace CatoriCity2025WPF.Controllers
{
    public class BankListController
    {
        private readonly BankService _service = new BankService();
        private readonly Views.BankListView _view;

        public ObservableCollection<BankViewModel> Banks { get; } = new ObservableCollection<BankViewModel>();

        public BankListController(Views.BankListView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public async Task LoadAsync()
        {
            try
            {
                var entities = await _service.GetAllAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Banks.Clear();
                    foreach (var e in entities)
                    {
                        Banks.Add(e);
                    }
                });
            }
            catch (Exception ex)
            {
                _view.SetStatus("Error loading banks: " + ex.Message);
            }
        }

        public void New()
        {
            var vm = new BankViewModel();
            Edit(vm);
        }

        public void Edit(BankViewModel vm)
        {
            if (vm == null) return;
            var editView = new Views.BankEditView(vm);
            editView.Owner = _view;
            var result = editView.ShowDialog();
            if (result == true)
            {
                // reload list
                _ = LoadAsync();
            }
        }

        public void Delete(BankViewModel vm)
        {
            if (vm == null) return;
            var res = MessageBox.Show($"Delete bank '{vm.BusinesskeyImageNameWOExtension}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes) return;

            try
            {
                // delete not implemented in repository template — attempt upsert with id=0 or implement delete in repository.
                // For now remove from UI and persist by calling UpsertAsync with zeroed entity if business logic requires.
                Banks.Remove(vm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
