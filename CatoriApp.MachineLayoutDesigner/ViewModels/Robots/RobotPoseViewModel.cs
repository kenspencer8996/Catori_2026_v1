namespace CatoriApp.ViewModels.Robots
{
    public class RobotPoseViewModel : ViewmodelBase
    {
        private long _robotPoseId;
        private long _LocationId;
        private int _poseIndex;
        private string _poseName = "";
        private double _joint1;
        private double _joint2;
        private double _joint3;
        private double _jointHand;
        private int _durationMilliseconds = 600;

        public long RobotPoseId { get => _robotPoseId; set => SetProperty(ref _robotPoseId, value); }
        public long LocationId { get => _LocationId; set => SetProperty(ref _LocationId, value); }
        public int PoseIndex { get => _poseIndex; set => SetProperty(ref _poseIndex, value); }
        public string PoseName { get => _poseName; set => SetProperty(ref _poseName, value); }
        public double Joint1 { get => _joint1; set => SetProperty(ref _joint1, value); }
        public double Joint2 { get => _joint2; set => SetProperty(ref _joint2, value); }
        public double Joint3 { get => _joint3; set => SetProperty(ref _joint3, value); }
        public double JointHand { get => _jointHand; set => SetProperty(ref _jointHand, value); }
        public int DurationMilliseconds { get => _durationMilliseconds; set => SetProperty(ref _durationMilliseconds, value); }
    }
}


