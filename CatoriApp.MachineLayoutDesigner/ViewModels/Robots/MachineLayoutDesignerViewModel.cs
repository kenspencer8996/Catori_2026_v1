using System.Collections.ObjectModel;
namespace CatoriApp.ViewModels.Robots
{
    public class MachineLayoutDesignerViewModel : ViewmodelBase
    {
        private long _robotDesignerId;
        private long _LocationId;
        private string _sequenceName = "";
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

        public long MachineLayoutDesignerId { get => _robotDesignerId; set => SetProperty(ref _robotDesignerId, value); }
        public long LocationId { get => _LocationId; set => SetProperty(ref _LocationId, value); }
        public string SequenceName
        {
            get => _sequenceName;
            set => SetProperty(ref _sequenceName, value ?? "");
        }
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

        public void AddPose(double joint1, double joint2, double joint3, double jointHand)
        {
            var pose = new RobotPoseViewModel
            {
                LocationId = LocationId,
                PoseIndex = Poses.Count,
                PoseName = "Pose " + (Poses.Count + 1),
                Joint1 = joint1,
                Joint2 = joint2,
                Joint3 = joint3,
                JointHand = jointHand,
                DurationMilliseconds = 600
            };

            Poses.Add(pose);
            SelectedPose = pose;
            StatusMessage = "Pose added.";
        }

        public void DeleteSelectedPose()
        {
            if (SelectedPose == null)
                return;

            var index = Poses.IndexOf(SelectedPose);
            Poses.Remove(SelectedPose);
            RenumberPoses();

            if (Poses.Count > 0)
                SelectedPose = Poses[Math.Clamp(index, 0, Poses.Count - 1)];
            else
                SelectedPose = null;

            StatusMessage = "Pose deleted.";
        }

        public void MoveSelectedPoseUp()
        {
            if (SelectedPose == null)
                return;

            var index = Poses.IndexOf(SelectedPose);
            if (index <= 0)
                return;

            Poses.Move(index, index - 1);
            RenumberPoses();
            StatusMessage = "Pose moved up.";
        }

        public void MoveSelectedPoseDown()
        {
            if (SelectedPose == null)
                return;

            var index = Poses.IndexOf(SelectedPose);
            if (index < 0 || index >= Poses.Count - 1)
                return;

            Poses.Move(index, index + 1);
            RenumberPoses();
            StatusMessage = "Pose moved down.";
        }

        private void RenumberPoses()
        {
            for (int i = 0; i < Poses.Count; i++)
            {
                Poses[i].PoseIndex = i;
                if (string.IsNullOrWhiteSpace(Poses[i].PoseName))
                    Poses[i].PoseName = "Pose " + (i + 1);
            }
        }
    }
}


