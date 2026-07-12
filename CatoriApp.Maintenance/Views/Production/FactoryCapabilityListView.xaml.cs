using CatoriApp.Maintenance.Objects.Services.Production;
using CatoriApp.Maintenance.ViewModels.Production;

namespace CatoriApp.Maintenance.Views.Production
{
    public partial class FactoryCapabilityListView : Window
    {
        private readonly ProductionMaintenanceService _service = new();
        private readonly ProductionMaintenanceListViewModel _viewModel = new();

        public FactoryCapabilityListView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            _viewModel.FactoryCapabilities.Clear();
            foreach (var item in await _service.GetAllFactoryCapabilitiesAsync())
                _viewModel.FactoryCapabilities.Add(item);
            _viewModel.SelectedFactoryCapability ??= _viewModel.FactoryCapabilities.FirstOrDefault();
            _viewModel.StatusMessage = "Factory capabilities loaded.";
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var model = new FactoryCapabilityViewModel { CapabilityType = "Assembly", IsEnabled = true };
            _viewModel.FactoryCapabilities.Add(model);
            _viewModel.SelectedFactoryCapability = model;
            Edit(model);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is FactoryCapabilityViewModel model)
                Edit(model);
        }

        private void Edit(FactoryCapabilityViewModel model)
        {
            var view = new FactoryCapabilityEditorView(model) { Owner = this };
            if (view.ShowDialog() == true)
                _viewModel.StatusMessage = "Factory capability saved.";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}