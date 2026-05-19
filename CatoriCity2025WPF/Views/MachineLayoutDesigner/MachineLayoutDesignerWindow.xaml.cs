using CatoriUCLibrary;
using CatoriUCLibrary.Views.RobotArm;
using System.IO;
using System.Windows.Media.Imaging;

namespace CatoriApp.Views.MachineLayoutDesigner
{
    public partial class MachineLayoutDesignerWindow : Window
    {
        private readonly MachineLayoutDesignerService _service = new MachineLayoutDesignerService();
        private readonly string? _backgroundImagePath;

        public MachineLayoutDesignerViewModel ViewModel { get; private set; }
        bool loading = true;
        public MachineLayoutDesignerWindow()
            : this(0, null, Rect.Empty)
        {
        }

        public MachineLayoutDesignerWindow(long locationId, string? backgroundImagePath, Rect sourceSelection)
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
            RobotArmNew.SetupRobot(RobotColorEnum.Blue);
            SetSequenceName();
            DataContext = ViewModel;
            MainImage.Source = UIUtility.GetImageControl(_backgroundImagePath, 1000, 1000, 0).Source;
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
            try
            {
                foreach (var pose in ViewModel.Poses.OrderBy(p => p.PoseIndex))
                {
                    ViewModel.SelectedPose = pose;
                    await RobotArmNew.MoveToPoseAsync(
                        new RobotPose(pose.Joint1, pose.Joint2, pose.Joint3, pose.JointHand),
                        pose.DurationMilliseconds);
                }

                ViewModel.StatusMessage = "Sequence played.";
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = "Play failed: " + ex.Message;
            }
        }

        private void ApplyPose(RobotPoseViewModel pose)
        {
            RobotArmNew.SetPose(
                (int)pose.Joint1,
                (int)pose.Joint2,
                (int)pose.Joint3,
                (int)pose.JointHand);
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
                var loaded = await _service.LoadSequenceAsync(ViewModel.SequenceName);
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

        private void SetSequenceName()
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
    }
}

