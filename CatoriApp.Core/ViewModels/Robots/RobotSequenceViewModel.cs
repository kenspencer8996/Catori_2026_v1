using System.Collections.ObjectModel;

namespace CatoriApp.Core.ViewModels.Robots
{
    public class RobotSequenceViewModel : ViewmodelBase
    {
        private long _robotSequenceId;
        private long _locationId;
        private long _machineInstanceId;
        private string _sequenceName = "";
        private double _robotX = 300;
        private double _robotY = 200;
        private double _robotWidth = 100;
        private double _robotHeight = 100;
        private RobotPoseViewModel? _selectedPose;

        public long RobotSequenceId { get => _robotSequenceId; set => SetProperty(ref _robotSequenceId, value); }
        public long LocationId { get => _locationId; set => SetProperty(ref _locationId, value); }
        public long MachineInstanceId { get => _machineInstanceId; set => SetProperty(ref _machineInstanceId, value); }
        public string SequenceName { get => _sequenceName; set => SetProperty(ref _sequenceName, value ?? ""); }
        public double RobotX { get => _robotX; set => SetProperty(ref _robotX, value); }
        public double RobotY { get => _robotY; set => SetProperty(ref _robotY, value); }
        public double RobotWidth { get => _robotWidth; set => SetProperty(ref _robotWidth, value); }
        public double RobotHeight { get => _robotHeight; set => SetProperty(ref _robotHeight, value); }
        public RobotPoseViewModel? SelectedPose { get => _selectedPose; set => SetProperty(ref _selectedPose, value); }
        public ObservableCollection<RobotPoseViewModel> Poses { get; } = new();
    }
}
