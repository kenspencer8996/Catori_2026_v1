namespace CatoriApp.MachineLayoutDesigner.ViewModels.Robots
{
    public class MachineCatalogEditorViewModel : ViewmodelBase
    {
        private MachineDefinitionViewModel? _selectedDefinition;
        private string _statusMessage = "";

        public ObservableCollection<MachineDefinitionViewModel> Definitions { get; } = new();

        public MachineDefinitionViewModel? SelectedDefinition
        {
            get => _selectedDefinition;
            set => SetProperty(ref _selectedDefinition, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
    }
}
