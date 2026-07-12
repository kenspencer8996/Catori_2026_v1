namespace CatoriApp.MachineLayoutDesigner.ViewModels.Robots
{
    public class MachineInstanceEditorViewModel : ViewmodelBase
    {
        private MachineDefinitionViewModel? _selectedDefinition;
        private MachineInstanceViewModel? _selectedInstance;
        private string _statusMessage = "";

        public ObservableCollection<MachineDefinitionViewModel> Definitions { get; } = new();
        public ObservableCollection<MachineInstanceViewModel> Instances { get; } = new();

        public MachineDefinitionViewModel? SelectedDefinition
        {
            get => _selectedDefinition;
            set => SetProperty(ref _selectedDefinition, value);
        }

        public MachineInstanceViewModel? SelectedInstance
        {
            get => _selectedInstance;
            set => SetProperty(ref _selectedInstance, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
    }
}
