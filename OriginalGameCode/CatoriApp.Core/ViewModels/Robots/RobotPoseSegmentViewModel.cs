namespace CatoriApp.Core.ViewModels.Robots
{
    public class RobotPoseSegmentViewModel : ViewmodelBase
    {
          private int _segmentIndex;
        private double _angle;

        public int SegmentIndex { get => _segmentIndex; set => SetProperty(ref _segmentIndex, value); }
        public double Angle { get => _angle; set => SetProperty(ref _angle, value); }
    }
}
