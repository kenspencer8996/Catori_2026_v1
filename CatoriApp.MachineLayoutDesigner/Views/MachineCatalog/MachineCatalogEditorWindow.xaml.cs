using CatoriServices.Objects.database.Manufacturing;
using CatoriServices.Objects.Entities.Manufacturing;

namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
    public partial class MachineCatalogEditorWindow : Window
    {
        private readonly MachineCatalogService _service = new();
        private readonly MachineTypeRepository _serviceTypes = new();
        private readonly MachineCatalogEditorViewModel _viewModel = new();
        List<MachineTypeEntity> _machineTypes = new();
        public MachineCatalogEditorWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;

            _machineTypes = _serviceTypes.GetAllAsync().GetAwaiter().GetResult();
            MachineTypeComboBox.ItemsSource = _machineTypes;
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

        private void NewDefinition_Click(object sender, RoutedEventArgs e)
        {
            var definition = CreateDefaultMachineDefinition();
            _viewModel.Definitions.Add(definition);
            _viewModel.SelectedDefinition = definition;
            _viewModel.StatusMessage = "New machine created.";
            int index = 0;
            for (int i = 0; i < MachineTypeComboBox.Items.Count; i++)
            {
                MachineTypeEntity thisone = (MachineTypeEntity)MachineTypeComboBox.Items[i];

                if (thisone.Name ==definition.MachineType)
                {
                    index = i;
                    break;
                }
            }
            MachineTypeComboBox.SelectedIndex = index;
        }

        private async void SaveDefinition_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedDefinition == null)
                return;

            RenumberDefinitionSegments(_viewModel.SelectedDefinition);
            await _service.SaveDefinitionAsync(_viewModel.SelectedDefinition);
            _viewModel.StatusMessage = "Machine saved.";
            
        }

        private void EditInstances_Click(object sender, RoutedEventArgs e)
        {
            var window = new MachineInstanceEditorWindow(_viewModel.SelectedDefinition)
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private MachineDefinitionViewModel CreateDefaultMachineDefinition()
        {
            var model = GetUndefinedName("Robot Arm");
            var definition = new MachineDefinitionViewModel
            {
                MachineType = model.MachineType,
                MachineName = model.MachineName,
                Description = model.MachineType,
                DefaultWidth = 400,
                DefaultHeight = 400
            };

            return definition;
        }
        private MachineDefinitionViewModel GetUndefinedName(string machineName)
        {
            MachineDefinitionViewModel unuseddef = new();
            foreach (var typeitem in _machineTypes)
            {
                var found = (from def in _viewModel.Definitions
                             where def.MachineType == typeitem.Name
                             select def)
                            .FirstOrDefault();
                if (found == null)
                {
                    unuseddef.MachineType = typeitem.Name;
                    unuseddef.MachineName = typeitem.Name;
                    break;
                }
            }
            return unuseddef;
        }
        private static void RenumberDefinitionSegments(MachineDefinitionViewModel definition)
        {
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void MachineNameTextBox_LostFocusAsync(object sender, RoutedEventArgs e)
        {
            //string name = MachineNameTextBox.Text;
            //IsMachineNameUniqueAsync(name);         
        }
    

        private async Task IsMachineNameUniqueAsync(string name)
        {
            var model = await _service.GetDefinitionByNameAsync(name);
            if (model != null && model.MachineDefinitionId != 0)
            {
                _viewModel.StatusMessage = "Machine definition already exists.";
            }
        }
        private void MachineTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MachineTypeEntity entity;
            if (_viewModel.SelectedDefinition != null && MachineTypeComboBox.SelectedItem is MachineTypeEntity selected)
            {
                entity = selected;
                _viewModel.SelectedDefinition.MachineType = entity.Name;
            }
        }
    }
}
