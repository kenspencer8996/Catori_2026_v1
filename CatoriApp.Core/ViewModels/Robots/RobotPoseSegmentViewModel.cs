namespace CatoriApp.Core.ViewModels.Robots
{
    public class RobotPoseSegmentViewModel : ViewmodelBase
    {
        private long _robotPoseSegmentId;
        private long _robotPoseId;
        private int _segmentIndex;
        private double _angle;

        public long RobotPoseSegmentId { get => _robotPoseSegmentId; set => SetProperty(ref _robotPoseSegmentId, value); }
        public long RobotPoseId { get => _robotPoseId; set => SetProperty(ref _robotPoseId, value); }
        public int SegmentIndex { get => _segmentIndex; set => SetProperty(ref _segmentIndex, value); }
        public double Angle { get => _angle; set => SetProperty(ref _angle, value); }
    }
}
