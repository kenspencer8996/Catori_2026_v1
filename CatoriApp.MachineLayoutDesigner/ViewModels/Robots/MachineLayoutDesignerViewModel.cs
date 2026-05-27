using System.Collections.ObjectModel;
namespace CatoriApp.MachineLayoutDesigner.ViewModels.Robots
{
    public class MachineLayoutDesignerViewModel : ViewmodelBase
    {
        private long _robotDesignerId;
        private long _LocationId;
        private double _selectionX;
        private double _selectionY;
        private double _selectionWidth;
        private double _selectionHeight;
        private double _canvasWidth = 800;
        private double _canvasHeight = 600;
        private RobotSequenceViewModel _selectedSequence = new();
        private string _statusMessage = "";

        public long MachineLayoutDesignerId { get => _robotDesignerId; set => SetProperty(ref _robotDesignerId, value); }
        public long LocationId
        {
            get => _LocationId;
            set
            {
                if (SetProperty(ref _LocationId, value))
                    SelectedSequence.LocationId = value;
            }
        }
        public string SequenceName
        {
            get => SelectedSequence.SequenceName;
            set { SelectedSequence.SequenceName = value ?? ""; OnPropertyChanged(nameof(SequenceName)); }
        }
        public double SelectionX { get => _selectionX; set => SetProperty(ref _selectionX, value); }
        public double SelectionY { get => _selectionY; set => SetProperty(ref _selectionY, value); }
        public double SelectionWidth { get => _selectionWidth; set => SetProperty(ref _selectionWidth, value); }
        public double SelectionHeight { get => _selectionHeight; set => SetProperty(ref _selectionHeight, value); }
        public double CanvasWidth { get => _canvasWidth; set => SetProperty(ref _canvasWidth, value); }
        public double CanvasHeight { get => _canvasHeight; set => SetProperty(ref _canvasHeight, value); }
        public RobotSequenceViewModel SelectedSequence
        {
            get => _selectedSequence;
            set
            {
                if (SetProperty(ref _selectedSequence, value))
                {
                    OnPropertyChanged(nameof(RobotSequenceId));
                    OnPropertyChanged(nameof(MachineInstanceId));
                    OnPropertyChanged(nameof(SequenceName));
                    OnPropertyChanged(nameof(RobotX));
                    OnPropertyChanged(nameof(RobotY));
                    OnPropertyChanged(nameof(RobotWidth));
                    OnPropertyChanged(nameof(RobotHeight));
                    OnPropertyChanged(nameof(SelectedPose));
                    OnPropertyChanged(nameof(Poses));
                }
            }
        }

        public long RobotSequenceId { get => SelectedSequence.RobotSequenceId; set { SelectedSequence.RobotSequenceId = value; OnPropertyChanged(nameof(RobotSequenceId)); } }
        public long MachineInstanceId { get => SelectedSequence.MachineInstanceId; set { SelectedSequence.MachineInstanceId = value; OnPropertyChanged(nameof(MachineInstanceId)); } }
        public double RobotX { get => SelectedSequence.RobotX; set { SelectedSequence.RobotX = value; OnPropertyChanged(nameof(RobotX)); } }
        public double RobotY { get => SelectedSequence.RobotY; set { SelectedSequence.RobotY = value; OnPropertyChanged(nameof(RobotY)); } }
        public double RobotWidth { get => SelectedSequence.RobotWidth; set { SelectedSequence.RobotWidth = value; OnPropertyChanged(nameof(RobotWidth)); } }
        public double RobotHeight { get => SelectedSequence.RobotHeight; set { SelectedSequence.RobotHeight = value; OnPropertyChanged(nameof(RobotHeight)); } }
        public RobotPoseViewModel? SelectedPose { get => SelectedSequence.SelectedPose; set { SelectedSequence.SelectedPose = value; OnPropertyChanged(nameof(SelectedPose)); } }
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
        public ObservableCollection<RobotSequenceViewModel> Sequences { get; } = new();
        public ObservableCollection<RobotPoseViewModel> Poses => SelectedSequence.Poses;

        public void AddPose(double joint1, double joint2, double joint3, double jointEnd)
        {
            var pose = new RobotPoseViewModel
            {
                LocationId = LocationId,
                RobotSequenceId = RobotSequenceId,
                PoseIndex = Poses.Count,
                PoseName = "Pose " + (Poses.Count + 1),
                Joint1 = joint1,
                Joint2 = joint2,
                Joint3 = joint3,
                JointEnd = jointEnd,
                DurationMilliseconds = 600
            };

            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 0, Angle = pose.Joint1 });
            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 1, Angle = pose.Joint2 });
            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 2, Angle = pose.Joint3 });
            pose.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = 3, Angle = pose.JointEnd });

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


