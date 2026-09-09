using CatoriApp.Maintenance.Objects.Services.Production;
using CatoriApp.Maintenance.ViewModels.Production;

namespace CatoriApp.Maintenance.Views.Production
{
    public partial class LocationUnlockRuleListView : Window
    {
        private readonly ProductionMaintenanceService _service = new();
        private readonly ProductionMaintenanceListViewModel _viewModel = new();

        public LocationUnlockRuleListView()
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
            _viewModel.LocationUnlockRules.Clear();
            foreach (var item in await _service.GetAllLocationUnlockRulesAsync())
                _viewModel.LocationUnlockRules.Add(item);
            _viewModel.SelectedLocationUnlockRule ??= _viewModel.LocationUnlockRules.FirstOrDefault();
            _viewModel.StatusMessage = "Location unlock rules loaded.";
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var model = new LocationUnlockRuleViewModel();
            _viewModel.LocationUnlockRules.Add(model);
            _viewModel.SelectedLocationUnlockRule = model;
            Edit(model);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is LocationUnlockRuleViewModel model)
                Edit(model);
        }

        private void Edit(LocationUnlockRuleViewModel model)
        {
            var view = new LocationUnlockRuleEditorView(model) { Owner = this };
            if (view.ShowDialog() == true)
                _viewModel.StatusMessage = "Location unlock rule saved.";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}