using CatoriServices.Objects.database.Manufacturing;
using CatoriServices.Objects.Entities.Manufacturing;

namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
    public partial class MachineCatalogEditorView : Window
    {
        private readonly MachineCatalogService _service = new();
        private readonly MachineDefinitionViewModel _definition;

        public MachineCatalogEditorView(MachineDefinitionViewModel definition)
        {
            _definition = definition;
            InitializeComponent();
            DataContext = _definition;
            if (_definition != null && _definition.MachineType != "")
                MachineTypeComboBox.SelectedItem = _definition.MachineType;
            LoadMachineTypes();
        }

        private void LoadMachineTypes()
        {
            var repository = new MachineTypeRepository();
            List<MachineTypeEntity> machineTypes = repository.GetAllAsync().GetAwaiter().GetResult();
            MachineTypeComboBox.ItemsSource = machineTypes;
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            await _service.SaveDefinitionAsync(_definition);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}