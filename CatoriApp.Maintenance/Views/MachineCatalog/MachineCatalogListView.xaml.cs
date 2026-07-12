using CatoriServices.Objects.database.Manufacturing;
using CatoriServices.Objects.Entities.Manufacturing;

namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
    public partial class MachineCatalogListView : Window
    {
        private readonly MachineCatalogService _service = new();
        private readonly MachineTypeRepository _serviceTypes = new();
        private readonly MachineCatalogEditorViewModel _viewModel = new();
        private List<MachineTypeEntity> _machineTypes = new();

        public MachineCatalogListView()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _machineTypes = _serviceTypes.GetAllAsync().GetAwaiter().GetResult();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCatalogAsync();
        }

        private async Task LoadCatalogAsync()
        {
            _viewModel.Definitions.Clear();

            foreach (var definition in await _service.GetAllDefinitionsAsync())
                _viewModel.Definitions.Add(definition);

            _viewModel.SelectedDefinition ??= _viewModel.Definitions.FirstOrDefault();
            _viewModel.StatusMessage = "Machine catalog loaded.";
        }

        private void NewMachineButton_Click(object sender, RoutedEventArgs e)
        {
            var definition = CreateDefaultMachineDefinition();
            _viewModel.Definitions.Add(definition);
            _viewModel.SelectedDefinition = definition;
            _viewModel.StatusMessage = "New machine created.";
            EditMachineDefinition(definition);
        }

        //private async void SaveMachineButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_viewModel.SelectedDefinition == null)
        //        return;

        //    await SaveDefinitionAsync(_viewModel.SelectedDefinition);
        //}

        private void EditInstancesButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new MachineInstanceEditorListView(_viewModel.SelectedDefinition)
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is MachineDefinitionViewModel definition)
                EditMachineDefinition(definition);
        }

        private void EditMachineDefinition(MachineDefinitionViewModel definition)
        {
            var view = new MachineCatalogEditorView(definition)
            {
                Owner = this
            };

            if (view.ShowDialog() == true)
                _viewModel.StatusMessage = "Machine saved.";
        }

        //private async Task SaveDefinitionAsync(MachineDefinitionViewModel definition)
        //{
        //    RenumberDefinitionSegments(definition);
        //    await _service.SaveDefinitionAsync(definition);
        //    _viewModel.StatusMessage = "Machine saved.";
        //}

        private MachineDefinitionViewModel CreateDefaultMachineDefinition()
        {
            var model = GetUndefinedName();
            return new MachineDefinitionViewModel
            {
                MachineType = model.MachineType,
                MachineName = model.MachineName,
                Description = model.MachineType,
                DefaultWidth = 400,
                DefaultHeight = 400
            };
        }

        private MachineDefinitionViewModel GetUndefinedName()
        {
            var unusedDefinition = new MachineDefinitionViewModel();

            foreach (var typeItem in _machineTypes)
            {
                var found = _viewModel.Definitions.FirstOrDefault(def => def.MachineType == typeItem.Name);
                if (found != null)
                    continue;

                unusedDefinition.MachineType = typeItem.Name;
                unusedDefinition.MachineName = typeItem.Name;
                break;
            }

            if (string.IsNullOrWhiteSpace(unusedDefinition.MachineName))
            {
                unusedDefinition.MachineType = "Robot Arm";
                unusedDefinition.MachineName = "Robot Arm";
            }

            return unusedDefinition;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}