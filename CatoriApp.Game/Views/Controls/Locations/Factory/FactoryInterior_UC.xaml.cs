using CatoriApp.Core.Convertors;
using CatoriApp.Core.Objects.DragDrop;
using CatoriApp.MachineLayoutDesigner.Objects.Enums;
using System.Windows.Input;
using System.Windows.Threading;
namespace CatoriApp.Game.Views.Controls.Locations.Factory
{
    /// <summary>
    /// Interaction logic for FactoryInterior_UC.xaml
    /// </summary>
    public partial class FactoryInterior_UC : UserControl,IDropTarget
    {
        FactoryInterior_UController _controller;
        public string ImagesFolderLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "Left");
        public string restImageLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "00RobotArmStart.png");
        DragManager _dragManager;
        readonly DispatcherTimer _animation_timer;

        public FactoryInterior_UC(int locationId)
        {
            InitializeComponent();
            _dragManager = GlobalCode.GetDragmanager(MainCanvas);
            _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);

            // Debug.WriteLine("RobotControl is now the source: " + e.Source);
            _controller = new FactoryInterior_UController(this, locationId);
            RobotPanel.RunRequested += RobotPanel_RunRequested;
            RobotPanel.DesignModeRequested += RobotPanel_EditModeRequested; ;
            RobotPanel.DesignModeEndRequested += RobotPanel_EditModeEndRequested; ;
            _dragManager.RegisterDropTarget(this);
            Canvas.SetLeft(lightPanel, 1476);
            Canvas.SetTop(lightPanel, 815);
            RobotArmNew.SetupRobot(CatoriUCLibrary.RobotColorEnum.Blue);

            RobotArmNew.Width = 400;
            RobotArmNew.Height = 400;
            Canvas.SetLeft(RobotArmNew, 976);
            Canvas.SetTop(RobotArmNew, 760);

            //Canvas.SetLeft(RobotConveyorALabel, 930);
            //Canvas.SetTop(RobotConveyorALabel, 840);
            //Canvas.SetLeft(RobotConveyorBLabel, 1404);
            //Canvas.SetTop(RobotConveyorBLabel, 1000);
            //Canvas.SetLeft(RobotConveyorCLabel, 1705);
            //Canvas.SetTop(RobotConveyorCLabel, 1000);

            //Canvas.SetLeft(RobotConveyorCLabel, 1705);
            //Canvas.SetTop(RobotConveyorCLabel, 1000);
            lightPanel.PanelTriggered += LightPanel_PanelTriggered;
            lightPanel.StartFlicker();


            List<string> conveyors = new List<string?>();
            conveyors.Add("A");
            conveyors.Add("B");
            conveyors.Add("C");
            List<string> outPutItems = new List<string>();
            outPutItems.Add("Paint Booth");
            outPutItems.Add("No Paint");
            RobotPanel.LoadData("Yellow Builder Robot", conveyors, conveyors, outPutItems);
            // _controller.RobotBuilder.MouseUpAfterRobotMove += RobotBuilder_MoveRobotComplete;
            _animation_timer.Interval = TimeSpan.FromMilliseconds(1000);
            _animation_timer.Tick += Animation_timer_Tick;
            _animation_timer.Start();

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

        private void LightPanel_PanelTriggered(object? sender, EventArgs e)
        {
            ShowrobotControlPanel();
        }

        private void Animation_timer_Tick(object? sender, EventArgs e)
        {
            _animation_timer.Stop();
            //run robot animation step here
            _animation_timer.Start();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

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

        private void ShowRobotControlsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowrobotControlPanel();
        }
        public void ShowrobotControlPanel()
        {
            RobotPanel.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _controller.HandleMouseDown(e);
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                Point thispoint = e.GetPosition(this);
                System.Diagnostics.Debug.WriteLine("loc " + thispoint.X + "  " + thispoint.Y);
            }

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
        public void DanceRobot()
        {
            Random rnd = new Random();
            double j1 = rnd.Next(30, 130) * -1;
            double j2 = rnd.Next(70, 120);
            double j3 = rnd.Next(2, 110);
            double j4 = rnd.Next(1, 80);
            int i = rnd.Next(1, 2000);
            if (is_odd(i))
            {
                j4 = j4 * -1;
                j2 = j2 * -1;
            }
            CatoriUCLibrary.Views.RobotArm.RobotPose targetPose =
                new CatoriUCLibrary.Views.RobotArm.RobotPose(j1, j2, j3, j4);

            RobotArmNew.MoveToPoseAsync(targetPose);

        }
        bool is_odd(int n)
        {
            return n % 2 != 0;
        }
        private void UC_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.D && Keyboard.Modifiers == ModifierKeys.Control)
            {
                DanceRobot();
            }
        }

    }
}



