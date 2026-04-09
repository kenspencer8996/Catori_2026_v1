using CatoriCity2025WPF.Convertors;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Views.Controls.Factory;
using System.Diagnostics;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for FactoryInterior_1UC.xaml
    /// </summary>
    public partial class FactoryInterior_1UC : UserControl,IDropTarget
    {
        FactoryInterior_1UCController _controller;
        List <string> robotImagePaths = new List<string>();
        public string SearchPattern { get; set; } = "05*.png";
        public string ImagesFolderLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Factories", "RobotArms", "Left");
        public string restImageLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Factories", "RobotArms", "00RobotArmStart.png");

        public string SearchPatternRight { get; set; } = "*.png";

        public PartSimpleUserControl part1SimpleUC;
        public PartSimpleUserControl part2SimpleUC;
        public PartSimpleUserControl part3SimpleUC;
        public PartSimpleUserControl part4SimpleUC;
        public PartSimpleUserControl part5SimpleUC;
        DragManager _dragManager;
        public FactoryInterior_1UC()
        {
            InitializeComponent();
            _dragManager = GlobalCode.GetDragmanager(MainCanvas);

            // Debug.WriteLine("RobotControl is now the source: " + e.Source);
            _controller = new FactoryInterior_1UCController(this);
            RobotPanel.RunRequested += RobotPanel_RunRequested;
            RobotPanel.DesignModeRequested += RobotPanel_EditModeRequested; ;
            RobotPanel.DesignModeEndRequested += RobotPanel_EditModeEndRequested; ;

            _dragManager.RegisterDropTarget(this);
            Canvas.SetLeft(lightPanel, 1476);
            Canvas.SetTop(lightPanel, 815);

            Canvas.SetLeft(RobotConveyorALabel, 930);
            Canvas.SetTop(RobotConveyorALabel, 840);
            Canvas.SetLeft(RobotConveyorBLabel, 1404);
            Canvas.SetTop(RobotConveyorBLabel, 1000);
            Canvas.SetLeft(RobotConveyorCLabel, 1705);
            Canvas.SetTop(RobotConveyorCLabel, 1000);

            Canvas.SetLeft(RobotConveyorCLabel, 1705);
            Canvas.SetTop(RobotConveyorCLabel, 1000);
            lightPanel.PanelTriggered += LightPanel_PanelTriggered;
            lightPanel.StartFlicker();

            List<string> conveyors = new List<string?>();
            conveyors.Add("A");
            conveyors.Add("B");
            conveyors.Add("C");
            List<string> outPutItems = new List<string>();
            outPutItems.Add("Paint Booth");
            outPutItems.Add("No Paint");
            RobotPanel.LoadData("Yellow Builder Robot", conveyors,conveyors,outPutItems);
            _controller.RobotBuilder.MouseUpAfterRobotMove += RobotBuilder_MoveRobotComplete;
        }

        private void RobotBuilder_MoveRobotComplete(object? sender, RobotArg e)
        {
            _controller.MouseUpAfterRobotMove(e);
        }

        private void LightPanel_PanelTriggered(object? sender, EventArgs e)
        {
            ShowrobotControlPanel();
        }

        private void RobotPanel_EditModeEndRequested(object? sender, EventArgs e)
        {
            _controller.EditModeEnd();
        }

        private void RobotPanel_EditModeRequested(object? sender, EventArgs e)
        {
            _controller.EditMode();
        }

        private void RobotPanel_RunRequested(object? sender, RobotControlPanelSelectionsArg e)
        {
            _controller.RunRequested(e);
        }

        private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
      
        private void UC_Loaded(object sender, RoutedEventArgs e)
        {
            part1SimpleUC = new
                PartSimpleUserControl(120, 80, 0, 0, 0, "SawHandle.png");
            part1SimpleUC.Opacity = 0;
            MainCanvas.Children.Add(part1SimpleUC);
            part2SimpleUC = new
               PartSimpleUserControl(120, 80, -100,-50, 0, "SawBlade.png");
            part2SimpleUC.Opacity = 0;
            MainCanvas.Children.Add(part2SimpleUC);
            
        }

        public void StartWorking(string workerImagePath)
        {
            //RobotLeftUC.StartWorking();
            //RobotRightUC.StartWorking();
            //WorkerImage.Source = UIUtility.GetImageControl(workerImagePath, 10, 5, 0).Source; ;
        }
        public void StopWorking()
        {
            //_animation_timer.Stop();
            //RobotLeftUC.StopWorking();
            //RobotRightUC.StopWorking();
        }

        private void UC_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _controller.HandleMouseDown(e);
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Point thispoint = e.GetPosition(this);
                System.Diagnostics.Debug.WriteLine("loc " + thispoint.X + "  " + thispoint.Y);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ShowRobotControlsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowrobotControlPanel();
        }
        public void ShowrobotControlPanel()
        {
            RobotPanel.Visibility = Visibility.Visible;
        }
        private void UC_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _controller.HandleMouseMove(e);
        }

        public void OnDropped()
        {
        }

        public bool CanDrop(IDraggable element)
        {
            return true;
        }

        public void OnDrop(IDraggable element)
        {
            
        }

        public void HighlightOn()
        {
            
        }

        public void HighlightOff()
        {
            
        }

        public Point GetSnapPoint(IDraggable dragged)
        {
            return new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        }

        public UIElement Visual
        {
            get
            {
                return this;
            }
        }

        public Point OriginalPosition
        {
            get
            {
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);
                return new Point(left, top);
            }
        }
    }
}
