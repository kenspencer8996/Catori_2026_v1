using CatoriUCLibrary.Views.RobotArm;

namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
    public partial class MachineArmDesignerWindow : Window
    {
        private readonly MachineCatalogService _service = new();
        private readonly MachineArmDesignerViewModel _viewModel = new();
        private bool _loading = true;

        public MachineArmDesignerWindow()
            : this(CreateDefaultDefinition(), null)
        {
        }

        public MachineArmDesignerWindow(MachineDefinitionViewModel definition, MachineInstanceViewModel? instance = null)
        {
            InitializeComponent();
            _viewModel.Definition = definition;
            _viewModel.Instance = instance ?? CreateInstance(definition);
            DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplySegmentsToPreview();
            ResetPoseFromDefinition();
            _loading = false;
        }

        private void AddSegment_Click(object sender, RoutedEventArgs e)
        {
            AddDefaultPart(_viewModel.Instance);
            ApplySegmentsToPreview();
        }

        private void ApplySegments_Click(object sender, RoutedEventArgs e)
        {
            ApplySegmentsToPreview();
        }

        private void JointSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_loading || ArmPreview == null)
                return;

            ArmPreview.SetPose(_viewModel.Joint1, _viewModel.Joint2, _viewModel.Joint3, _viewModel.JointEnd);
        }

        private void ResetPose_Click(object sender, RoutedEventArgs e)
        {
            ResetPoseFromDefinition();
        }

        private void CaptureInitialPose_Click(object sender, RoutedEventArgs e)
        {
            var segments = _viewModel.Instance.Segments.OrderBy(s => s.SegmentIndex).ToList();
            var values = new[] { _viewModel.Joint1, _viewModel.Joint2, _viewModel.Joint3, _viewModel.JointEnd };

            for (int i = 0; i < segments.Count && i < values.Length; i++)
                segments[i].InitialAngle = values[i];

            _viewModel.StatusMessage = "Current pose captured as initial segment angles.";
        }

        private async void SaveDefinition_Click(object sender, RoutedEventArgs e)
        {
            await _service.SaveDefinitionAsync(_viewModel.Definition);
            _viewModel.Instance.MachineDefinitionId = _viewModel.Definition.MachineDefinitionId;
            _viewModel.StatusMessage = "Machine saved.";
        }

        private async void SaveInstance_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Instance.MachineDefinitionId <= 0)
                _viewModel.Instance.MachineDefinitionId = _viewModel.Definition.MachineDefinitionId;

            RenumberInstanceSegments(_viewModel.Instance);
            await _service.SaveInstanceAsync(_viewModel.Instance);
            _viewModel.StatusMessage = "Machine instance saved.";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplySegmentsToPreview()
        {
            RenumberInstanceSegments(_viewModel.Instance);
            ArmPreview.ConfigureSegments(_viewModel.Instance.Segments
                .OrderBy(s => s.SegmentIndex)
                .Select(ToArmSegmentDefinition)
                .ToList());
            ResetPoseFromDefinition();
            _viewModel.StatusMessage = "Preview updated.";
        }

        private void ResetPoseFromDefinition()
        {
            var values = _viewModel.Instance.Segments
                .OrderBy(s => s.SegmentIndex)
                .Select(s => s.InitialAngle)
                .ToList();

            _loading = true;
            _viewModel.Joint1 = GetAngle(values, 0, -90);
            _viewModel.Joint2 = GetAngle(values, 1, 0);
            _viewModel.Joint3 = GetAngle(values, 2, 0);
            _viewModel.JointEnd = GetAngle(values, 3, 0);
            _loading = false;

            ArmPreview.SetPose(_viewModel.Joint1, _viewModel.Joint2, _viewModel.Joint3, _viewModel.JointEnd);
        }

        private static RobotArmSegmentDefinition ToArmSegmentDefinition(MachineInstanceSegmentViewModel segment)
        {
            return new RobotArmSegmentDefinition
            {
                Name = string.IsNullOrWhiteSpace(segment.SegmentName) ? "Segment" + segment.SegmentIndex : ToWpfName(segment.SegmentName, segment.SegmentIndex),
                ImagePath = string.IsNullOrWhiteSpace(segment.ImageName) ? "robotArmLongBlue.png" : segment.ImageName,
                JointLength = segment.Length,
                Width = Math.Max(1, segment.Length + segment.Overlap),
                Height = Math.Max(1, segment.Width),
                InitialAngle = segment.InitialAngle
            };
        }

        private static string ToWpfName(string name, int index)
        {
            var clean = new string(name.Where(char.IsLetterOrDigit).ToArray());
            return string.IsNullOrWhiteSpace(clean) ? "Segment" + index : clean;
        }

        private static double GetAngle(IList<double> values, int index, double fallback)
            => index < values.Count ? values[index] : fallback;

        private static MachineDefinitionViewModel CreateDefaultDefinition()
        {
            var definition = new MachineDefinitionViewModel
            {
                MachineType = "RobotArm",
                MachineName = "Robot Arm",
                Description = "Configurable articulated robot arm",
                DefaultWidth = 400,
                DefaultHeight = 400
            };

            return definition;
        }

        private static void AddDefaultPart(MachineInstanceViewModel instance, string? name = null, double angle = 0, string imageName = "robotArmLongBlue.png")
        {
            var index = instance.Segments.Count;
            instance.Segments.Add(new MachineInstanceSegmentViewModel
            {
                SegmentIndex = index,
                SegmentName = name ?? "Segment " + (index + 1),
                Length = 124,
                Width = 40,
                InitialAngle = angle,
                MinAngle = -180,
                MaxAngle = 180,
                Overlap = 16,
                ImageName = imageName
            });
        }

        private static MachineInstanceViewModel CreateInstance(MachineDefinitionViewModel definition)
        {
            var instance = new MachineInstanceViewModel
            {
                MachineDefinitionId = definition.MachineDefinitionId,
                InstanceName = definition.MachineName + " Instance",
                DisplayName = definition.MachineName,
                DefaultScale = 1,
                DefaultWidth = definition.DefaultWidth,
                DefaultHeight = definition.DefaultHeight
            };

            AddDefaultPart(instance, "Shoulder", -90);
            AddDefaultPart(instance, "Upper Arm", 0);
            AddDefaultPart(instance, "Forearm", 0);
            AddDefaultPart(instance, "End", 0, "RobotArmHandBlue.png");

            return instance;
        }

        private static void RenumberDefinitionSegments(MachineDefinitionViewModel definition)
        {
        }

        private static void RenumberInstanceSegments(MachineInstanceViewModel instance)
        {
            for (int i = 0; i < instance.Segments.Count; i++)
                instance.Segments[i].SegmentIndex = i;
        }
    }
}
