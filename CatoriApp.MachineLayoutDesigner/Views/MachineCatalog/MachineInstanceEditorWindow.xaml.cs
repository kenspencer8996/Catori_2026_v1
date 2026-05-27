namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
    public partial class MachineInstanceEditorWindow : Window
    {
        private readonly MachineCatalogService _service = new();
        private readonly MachineInstanceEditorViewModel _viewModel = new();
        private readonly long _initialDefinitionId;
        private bool _isNew = false;
        public MachineInstanceEditorWindow(MachineDefinitionViewModel? selectedDefinition = null)
        {
            InitializeComponent();
            _initialDefinitionId = selectedDefinition?.MachineDefinitionId ?? 0;
            DataContext = _viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCatalogAsync();
        }

        private async Task LoadCatalogAsync()
        {
            _viewModel.Definitions.Clear();
            _viewModel.Instances.Clear();

            foreach (var definition in await _service.GetAllDefinitionsAsync())
                _viewModel.Definitions.Add(definition);

            foreach (var instance in await _service.GetAllInstancesAsync())
            {
                ApplySegmentEditorDefaults(instance);
                _viewModel.Instances.Add(instance);
            }

            _viewModel.SelectedDefinition = _initialDefinitionId > 0
                ? _viewModel.Definitions.FirstOrDefault(d => d.MachineDefinitionId == _initialDefinitionId)
                : _viewModel.Definitions.FirstOrDefault();

            _viewModel.SelectedInstance = _viewModel.Instances.FirstOrDefault(i => i.MachineDefinitionId == _viewModel.SelectedDefinition?.MachineDefinitionId)
                ?? _viewModel.Instances.FirstOrDefault();

            _viewModel.StatusMessage = "Machine instances loaded.";

            if (_viewModel.Instances.Count == 0)
            {
                CreateNewInstance();

            }
        }

        private void NewInstance_Click(object sender, RoutedEventArgs e)
        {
            CreateNewInstance();
            
        }

        private void CreateNewInstance()
        {
            var definition = _viewModel.SelectedDefinition ?? _viewModel.Definitions.FirstOrDefault();
            if (definition == null)
            {
                _viewModel.StatusMessage = "Create a machine before adding an instance.";
                return;
            }

            var instance = CreateInstanceFromDefinition(definition);
            _viewModel.Instances.Add(instance);
            _viewModel.SelectedInstance = instance;
            _viewModel.StatusMessage = "New machine instance created.";
            _isNew = true;
            MachineComboBox.IsEnabled = true;
        }
        private async void SaveInstance_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedInstance == null)
                return;
            _isNew = false;
            ApplySelectedDefinition(_viewModel.SelectedInstance);
            RenumberInstanceSegments(_viewModel.SelectedInstance);
            ApplySegmentImageNames(_viewModel.SelectedInstance);
            await _service.SaveInstanceAsync(_viewModel.SelectedInstance);
            _viewModel.StatusMessage = "Machine instance saved.";
        }
  
        private void DesignInstanceArm_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedInstance == null)
                return;

            var definition = _viewModel.SelectedDefinition
                ?? _viewModel.Definitions.FirstOrDefault(d => d.MachineDefinitionId == _viewModel.SelectedInstance.MachineDefinitionId)
                ?? CreateFallbackDefinition();

            ApplySelectedDefinition(_viewModel.SelectedInstance);
            ApplySegmentImageNames(_viewModel.SelectedInstance);

            var window = new MachineArmDesignerWindow(definition, _viewModel.SelectedInstance)
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void ApplySelectedDefinition(MachineInstanceViewModel instance)
        {
            if (_viewModel.SelectedDefinition == null)
                return;

            instance.MachineDefinitionId = _viewModel.SelectedDefinition.MachineDefinitionId;
            instance.DefaultWidth = _viewModel.SelectedDefinition.DefaultWidth;
            instance.DefaultHeight = _viewModel.SelectedDefinition.DefaultHeight;
        }

        private static void AddDefaultPart(MachineInstanceViewModel instance, string? name = null, double angle = 0, string color = "Blue", string partType = "long")
        {
            var index = instance.Segments.Count;
            var segment = new MachineInstanceSegmentViewModel
            {
                SegmentIndex = index,
                SegmentName = name ?? "Segment " + (index + 1),
                Length = 124,
                Width = 40,
                InitialAngle = angle,
                MinAngle = -180,
                MaxAngle = 180,
                Overlap = 16,
                Color = color,
                PartType = partType
            };

            segment.ImageName = GetSegmentImageName(segment.Color, segment.PartType);
            instance.Segments.Add(segment);
        }

        private static MachineInstanceViewModel CreateInstanceFromDefinition(MachineDefinitionViewModel definition)
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
            AddDefaultPart(instance, "End", 0, "Blue", "hand");

            return instance;
        }

        private static MachineDefinitionViewModel CreateFallbackDefinition()
        {
            return new MachineDefinitionViewModel
            {
                MachineType = "RobotArm",
                MachineName = "Robot Arm",
                Description = "Configurable articulated robot arm",
                DefaultWidth = 400,
                DefaultHeight = 400
            };
        }

        private static void ApplySegmentEditorDefaults(MachineInstanceViewModel instance)
        {
            foreach (var segment in instance.Segments)
            {
                if (string.IsNullOrWhiteSpace(segment.Color))
                    segment.Color = InferColor(segment.ImageName);

                if (string.IsNullOrWhiteSpace(segment.PartType))
                    segment.PartType = InferPartType(segment.ImageName);

                if (string.IsNullOrWhiteSpace(segment.ImageName))
                    segment.ImageName = GetSegmentImageName(segment.Color, segment.PartType);
            }
        }

        private static void ApplySegmentImageNames(MachineInstanceViewModel instance)
        {
            foreach (var segment in instance.Segments)
            {
                if (string.IsNullOrWhiteSpace(segment.Color))
                    segment.Color = "Blue";

                if (string.IsNullOrWhiteSpace(segment.PartType))
                    segment.PartType = "long";

                segment.ImageName = GetSegmentImageName(segment.Color, segment.PartType);
            }
        }

        private static string GetSegmentImageName(string color, string partType)
        {
            var normalizedColor = NormalizeColor(color);
            var normalizedPartType = NormalizePartType(partType);

            return (normalizedPartType, normalizedColor) switch
            {
                ("hand", "Red") => "RobotArmHandRed.png",
                ("hand", "Yellow") => "RobotArmHandYellow.png",
                ("hand", "Gray") => "RobotArmHandGray.png",
                ("hand", _) => "RobotArmHandBlue.png",

                ("medium", "Red") => "RobotArmMediumRed.png",
                ("medium", "Yellow") => "RobotArmMediumYellow.png",
                ("medium", "Gray") => "RobotArmMediumGray.png",
                ("medium", _) => "robotArmMediumBlue.png",

                ("short", "Red") => "robotArmShortRed.png",
                ("short", "Yellow") => "RobotArmShortYellow.png",
                ("short", "Gray") => "robotArmShortGray.png",
                ("short", _) => "robotArmShortBlue.png",

                ("long", "Red") => "RobotArmLongRed.png",
                ("long", "Yellow") => "RobotArmLongYellow.png",
                ("long", "Gray") => "RobotArmLong.png",
                _ => "robotArmLongBlue.png"
            };
        }

        private static string InferColor(string imageName)
        {
            if (imageName.Contains("Red", StringComparison.OrdinalIgnoreCase))
                return "Red";
            if (imageName.Contains("Yellow", StringComparison.OrdinalIgnoreCase))
                return "Yellow";
            if (imageName.Contains("Gray", StringComparison.OrdinalIgnoreCase) || imageName.Contains("Grey", StringComparison.OrdinalIgnoreCase))
                return "Gray";

            return "Blue";
        }

        private static string InferPartType(string imageName)
        {
            if (imageName.Contains("Hand", StringComparison.OrdinalIgnoreCase))
                return "hand";
            if (imageName.Contains("Medium", StringComparison.OrdinalIgnoreCase))
                return "medium";
            if (imageName.Contains("Short", StringComparison.OrdinalIgnoreCase))
                return "short";

            return "long";
        }

        private static string NormalizeColor(string color)
        {
            return color switch
            {
                "Red" => "Red",
                "Yellow" => "Yellow",
                "Gray" => "Gray",
                _ => "Blue"
            };
        }

        private static string NormalizePartType(string partType)
        {
            return partType switch
            {
                "medium" => "medium",
                "short" => "short",
                "hand" => "hand",
                _ => "long"
            };
        }

        private static void RenumberInstanceSegments(MachineInstanceViewModel instance)
        {
            for (int i = 0; i < instance.Segments.Count; i++)
                instance.Segments[i].SegmentIndex = i;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedInstance = _viewModel.SelectedInstance;
            if (selectedInstance == null || selectedInstance.MachineDefinitionId == 0)
            {
                selectedInstance = _viewModel.Instances.FirstOrDefault(i => i.MachineDefinitionId == _initialDefinitionId);
            };
            var SelectedDefinition = _viewModel.Definitions.FirstOrDefault(x => x.MachineDefinitionId == selectedInstance.MachineDefinitionId);
            MachineComboBox.SelectedItem = SelectedDefinition;
            MachineComboBox.IsEnabled = false;
        }
        private void HideDesignButtons()
        {
            EditDesignButton.Visibility = Visibility.Collapsed;
            EditCraneButton.Visibility = Visibility.Collapsed;
            EditDroneRollingButton.Visibility = Visibility.Collapsed;
            EditPaintBoothButton.Visibility = Visibility.Collapsed;
        }
        private void ShowDesignButton(string name)
        {
            HideDesignButtons();
            switch (name)
            {
                case "Robot Arm":
                    EditDesignButton.Visibility = Visibility.Visible;
                    break;
                case "Crane":
                    EditCraneButton.Visibility = Visibility.Visible;
                    break;
                case "DroneRolling":
                    EditDroneRollingButton.Visibility = Visibility.Visible;
                    break;
                case "PaintBooth":
                    EditPaintBoothButton.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void EditPaintBoothButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit PaintBooth Not implemented yet");
        }

        private void EditDroneRollingButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit Drone Not implemented yet");
        }

        private void EditCraneButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit crane Not implemented yet");

        }

        private void MachineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = MachineComboBox.SelectedItem as MachineDefinitionViewModel;
            if (item != null)
            {
                ShowDesignButton(item.MachineName);
            }
            else
            {
                HideDesignButtons();
            }
        }
    }
}