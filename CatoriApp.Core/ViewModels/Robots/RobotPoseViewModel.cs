using System.Collections.ObjectModel;

namespace CatoriApp.Core.ViewModels.Robots
{
    public class RobotPoseViewModel : ViewmodelBase
    {
        private long _robotPoseId;
        private long _robotId;
        private int _poseIndex;
        private string _poseName = "";
        private int _durationMilliseconds = 600;

        public long RobotPoseId { get => _robotPoseId; set => SetProperty(ref _robotPoseId, value); }
        public long RobotId { get => _robotId; set => SetProperty(ref _robotId, value); }
        public int PoseIndex { get => _poseIndex; set => SetProperty(ref _poseIndex, value); }
        public string PoseName { get => _poseName; set => SetProperty(ref _poseName, value); }
        public int DurationMilliseconds { get => _durationMilliseconds; set => SetProperty(ref _durationMilliseconds, value); }
        public ObservableCollection<RobotPoseSegmentViewModel> Segments { get; } = new();
    }
}
