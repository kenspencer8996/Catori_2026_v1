using CatoriUCLibrary;
using CatoriUCLibrary.Views.RobotArm;
namespace CatoriApp.MachineLayoutDesigner.Views.Robots.MachineLayoutDesigner
{
    public partial class MachineLayoutDesignerWindow : Window
    {
        private readonly MachineLayoutDesignerService _service = new MachineLayoutDesignerService();
        private readonly string? _backgroundImagePath;
        private bool _isMouseOverMaintenancePanel;

        public MachineLayoutDesignerViewModel ViewModel { get; private set; }
        bool loading = true;
        public MachineLayoutDesignerWindow(long locationId, 
            string? backgroundImagePath, Rect sourceSelection,
            MachineDesignerModeEnum mode)
        {
            InitializeComponent();

            _backgroundImagePath = backgroundImagePath;
            ViewModel = new MachineLayoutDesignerViewModel
            {
                LocationId = locationId,
                SelectionX = sourceSelection.X,
                SelectionY = sourceSelection.Y,
                SelectionWidth = sourceSelection.Width,
                SelectionHeight = sourceSelection.Height
            };
            switch (mode)
            {
                case MachineDesignerModeEnum.RobotArm:
                    RobotArmNew.SetupRobot(RobotColorEnum.Blue);
                    break;
                case MachineDesignerModeEnum.CraneFreeStanding:
                    break;
                case MachineDesignerModeEnum.CraneOverhead:

                    break;
                case MachineDesignerModeEnum.BullDozer:
                    break;
                default:
                    break;
            }
            SetImageFromSequenceName();
            DataContext = ViewModel;
            MainImage.Source = GetImageSource(_backgroundImagePath);
         }



        private static BitmapImage? GetImageSource(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
            return bitmap;
        }
        private void ArmSelectorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArmSelectorComboBox?.SelectedItem is not ComboBoxItem item || item.Content is not string colorName)
                return;

            if (Enum.TryParse<RobotColorEnum>(colorName, out var color))
                RobotArmNew.SetupRobot(color);
        }
        private void AddPose_Click(object sender, RoutedEventArgs e)
        {
            var pose = GetCurrentPose();
            ViewModel.AddPose(pose.Joint1, pose.Joint2, pose.Joint3, pose.JointHand);
        }

        private void DeletePose_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSelectedPose();
        }

        private void MovePoseUp_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MoveSelectedPoseUp();
        }

        private void MovePoseDown_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MoveSelectedPoseDown();
        }

        private async void PlaySequence_Click(object sender, RoutedEventArgs e)
        {
            ClearStatus();
            try
            {
                var poses = ViewModel.Poses.OrderBy(p => p.PoseIndex).ToList();
                if (poses.Count == 0)
                {
                    ViewModel.StatusMessage = "Play failed: no poses in sequence.";
                    return;
                }

                for (int i = 0; i < poses.Count; i++)
                {
                    var pose = poses[i];
                    ViewModel.SelectedPose = pose;
                    ViewModel.StatusMessage = $"Playing pose {i + 1} of {poses.Count}: {pose.PoseName}";
                    SetStatus(ViewModel.StatusMessage);
                    await System.Windows.Threading.Dispatcher.Yield();

                    await RobotArmNew.MoveToPoseAsync(
                        new RobotPose(pose.Joint1, pose.Joint2, pose.Joint3, pose.JointEnd),
                        Math.Max(100, pose.DurationMilliseconds));
                }

                ViewModel.StatusMessage = $"Sequence played ({poses.Count} poses).";
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = "Play failed: " + ex.Message;
            }
        }

        private void SetStatus(string message)
        {
            StatusListbox.Items.Add(message);
            ViewModel.StatusMessage = message;
        }
        private void ClearStatus()
        {
            StatusListbox.Items.Clear();
        }
        private void ApplyPose(RobotPoseViewModel pose)
        {
            RobotArmNew.SetPose(
                (int)pose.Joint1,
                (int)pose.Joint2,
                (int)pose.Joint3,
                (int)pose.JointEnd);
        }

        private RobotPose GetCurrentPose()
        {
            return RobotArmNew.GetCurrentPose();
        }

        private void RobotPositionLeftTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = RobotPositionLeftTextBox.Text;
            double left = 0;
            if (IsNumeric(text) && loading == false)
            {
                left = double.Parse(text);
                Canvas.SetLeft(RobotArmNew, left);
                ViewModel.RobotX = left;
            }
        }

        private void RobotPositionTopTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = RobotPositionTopTextBox.Text;
            double top = 0;
            if (IsNumeric(text) && loading == false)
            {
                top = double.Parse(text);
                Canvas.SetTop(RobotArmNew, top);
                ViewModel.RobotY = top;
            }
        }
        public bool IsNumeric(string value)
        {
            return double.TryParse(value, out _);
        }

        private async void Load()
        {
            await LoadBackgroundsPoses();
            double x = ViewModel.RobotX;
            double y = ViewModel.RobotY;
            Canvas.SetLeft(RobotArmNew, x);
            Canvas.SetTop(RobotArmNew, y);
            RobotPositionLeftTextBox.Text = x.ToString();
            RobotPositionTopTextBox.Text = y.ToString();
            loading = false;
        }
        private async void Save()
        {
            try
            {
                if (ViewModel.LocationId <= 0)
                {
                    ViewModel.StatusMessage = "Save failed: no location selected.";
                    return;
                }

                await _service.SaveSequenceAsync(ViewModel);
                ViewModel.StatusMessage = "Sequence saved.";
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = "Save failed: " + ex.Message;
            }
        }

        private async Task LoadBackgroundsPoses()
        {
            try
            {
                var loaded = await _service.LoadByLocationIdAsync(ViewModel.LocationId, ViewModel.SequenceName);
                if (loaded == null)
                {
                    ViewModel.StatusMessage = "Sequence not found.";
                    return;
                }

                loaded.LocationId = ViewModel.LocationId > 0 ? ViewModel.LocationId : loaded.LocationId;
                ViewModel = loaded;
                DataContext = ViewModel;

                if (ViewModel.SelectedPose != null)
                    ApplyPose(ViewModel.SelectedPose);

                ViewModel.StatusMessage = "Sequence loaded.";
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = "Load failed: " + ex.Message;
            }
        }

        private void SetImageFromSequenceName()
        {
            if (string.IsNullOrWhiteSpace(ViewModel.SequenceName) && !string.IsNullOrWhiteSpace(_backgroundImagePath))
                ViewModel.SequenceName = System.IO.Path.GetFileNameWithoutExtension(_backgroundImagePath);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void StatusListbox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string status = "";
            for(int i = 0; i <= StatusListbox.Items.Count -1; i++)
            {
                status += StatusListbox.Items[i].ToString() + "\n";
            }
            Clipboard.SetText(status);
        }

        private void MainImage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
         }

        private void MainImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void MaintenancePanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseOverMaintenancePanel = true;
        }

        private void MaintenancePanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void DesignerCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        

        private void MaintenancePanel_MachineCatalogRequested(object? sender, EventArgs e)
        {
            var window = new MachineCatalogEditorWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void MaintenancePanel_MachineInstancesRequested(object? sender, EventArgs e)
        {
            var window = new MachineInstanceEditorWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }
    }
}


