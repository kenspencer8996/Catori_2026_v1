using System.Collections.ObjectModel;

namespace CatoriApp.MachineLayoutDesigner.ViewModels.Robots
{
    public class MachineLayoutDesignerViewModel : ViewmodelBase
    {
        private long _machineLayoutDesignerId;
        private long _locationLayoutItemId;
        private long _locationId;
        private double _selectionX;
        private double _selectionY;
        private double _selectionWidth;
        private double _selectionHeight;
        private double _canvasWidth = 800;
        private double _canvasHeight = 600;
        private double _robotX = 300;
        private double _robotY = 200;
        private double _robotWidth = 100;
        private double _robotHeight = 100;
        private RobotPoseViewModel? _selectedPose;
        private string _statusMessage = "";

        public long MachineLayoutDesignerId { get => _machineLayoutDesignerId; set => SetProperty(ref _machineLayoutDesignerId, value); }
        public long LocationLayoutItemId { get => _locationLayoutItemId; set => SetProperty(ref _locationLayoutItemId, value); }
        public long LocationId { get => _locationId; set => SetProperty(ref _locationId, value); }
        public double SelectionX { get => _selectionX; set => SetProperty(ref _selectionX, value); }
        public double SelectionY { get => _selectionY; set => SetProperty(ref _selectionY, value); }
        public double SelectionWidth { get => _selectionWidth; set => SetProperty(ref _selectionWidth, value); }
        public double SelectionHeight { get => _selectionHeight; set => SetProperty(ref _selectionHeight, value); }
        public double CanvasWidth { get => _canvasWidth; set => SetProperty(ref _canvasWidth, value); }
        public double CanvasHeight { get => _canvasHeight; set => SetProperty(ref _canvasHeight, value); }
        public double RobotX { get => _robotX; set => SetProperty(ref _robotX, value); }
        public double RobotY { get => _robotY; set => SetProperty(ref _robotY, value); }
        public double RobotWidth { get => _robotWidth; set => SetProperty(ref _robotWidth, value); }
        public double RobotHeight { get => _robotHeight; set => SetProperty(ref _robotHeight, value); }
        public RobotPoseViewModel? SelectedPose { get => _selectedPose; set => SetProperty(ref _selectedPose, value); }
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public ObservableCollection<RobotPoseViewModel> Poses { get; } = new();

        public void AddPose(double joint1, double joint2, double joint3, double jointEnd)
        {
            var pose = new RobotPoseViewModel { LocationLayoutItemId = LocationLayoutItemId, PoseIndex = Poses.Count, PoseName = "Pose " + (Poses.Count + 1), DurationMilliseconds = 600 };
            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 0, Angle = joint1 });
            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 1, Angle = joint2 });
            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 2, Angle = joint3 });
            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 3, Angle = jointEnd });
            Poses.Add(pose);
            SelectedPose = pose;
            StatusMessage = "Pose added.";
        }

        public void DeleteSelectedPose()
        {
            if (SelectedPose == null) return;
            var index = Poses.IndexOf(SelectedPose);
            Poses.Remove(SelectedPose);
            RenumberPoses();
            SelectedPose = Poses.Count == 0 ? null : Poses[Math.Clamp(index, 0, Poses.Count - 1)];
            StatusMessage = "Pose deleted.";
        }

        public void MoveSelectedPoseUp() => MoveSelectedPose(-1);
        public void MoveSelectedPoseDown() => MoveSelectedPose(1);

        private void MoveSelectedPose(int offset)
        {
            if (SelectedPose == null) return;
            var index = Poses.IndexOf(SelectedPose);
            var target = index + offset;
            if (target < 0 || target >= Poses.Count) return;
            Poses.Move(index, target);
            RenumberPoses();
        }

        private void RenumberPoses()
        {
            for (var i = 0; i < Poses.Count; i++) Poses[i].PoseIndex = i;
        }
    }
}
