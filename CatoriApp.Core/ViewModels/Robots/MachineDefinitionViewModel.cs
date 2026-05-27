namespace CatoriApp.Core.ViewModels.Robots
{
    public class MachineDefinitionViewModel : ViewmodelBase
    {
        private long _machineDefinitionId;
        private string _machineType = "";
        private string _machineName = "";
        private string _description = "";
        private double _defaultWidth = 100;
        private double _defaultHeight = 100;

        public long MachineDefinitionId { get => _machineDefinitionId; set => SetProperty(ref _machineDefinitionId, value); }
        public string MachineType { get => _machineType; set => SetProperty(ref _machineType, value ?? ""); }
        public string MachineName { get => _machineName; set => SetProperty(ref _machineName, value ?? ""); }
        public string Description { get => _description; set => SetProperty(ref _description, value ?? ""); }
        public double DefaultWidth { get => _defaultWidth; set => SetProperty(ref _defaultWidth, value); }
        public double DefaultHeight { get => _defaultHeight; set => SetProperty(ref _defaultHeight, value); }
    }
}
