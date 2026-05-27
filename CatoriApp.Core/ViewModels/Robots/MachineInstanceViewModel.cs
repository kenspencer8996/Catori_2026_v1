using System.Collections.ObjectModel;

namespace CatoriApp.Core.ViewModels.Robots
{
    public class MachineInstanceViewModel : ViewmodelBase
    {
        private long _machineInstanceId;
        private long _machineDefinitionId;
        private string _instanceName = "";
        private string _displayName = "";
        private double _defaultScale = 1;
        private double _defaultWidth = 100;
        private double _defaultHeight = 100;

        public long MachineInstanceId { get => _machineInstanceId; set => SetProperty(ref _machineInstanceId, value); }
        public long MachineDefinitionId { get => _machineDefinitionId; set => SetProperty(ref _machineDefinitionId, value); }
        public string InstanceName { get => _instanceName; set => SetProperty(ref _instanceName, value ?? ""); }
        public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value ?? ""); }
        public double DefaultScale { get => _defaultScale; set => SetProperty(ref _defaultScale, value); }
        public double DefaultWidth { get => _defaultWidth; set => SetProperty(ref _defaultWidth, value); }
        public double DefaultHeight { get => _defaultHeight; set => SetProperty(ref _defaultHeight, value); }
        public ObservableCollection<MachineInstanceSegmentViewModel> Segments { get; } = new();
    }
}
