using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Threading;
using CatoriApp.Game.Objects.Services.Locations;
using CatoriApp.Game.ViewModels.Locations;
using CatoriUCLibrary.Views.RobotArm;
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
        private readonly LocationLayoutItemService _layoutItemService = new();
        private readonly List<RoboticArmUC> _loadedRobotArms = new();
        private readonly int _locationId;

        public FactoryInterior_UC(int locationId)
        {
            _locationId = locationId;
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
            RobotArmNew.Visibility = Visibility.Collapsed;

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
        private Window? _hostWindow;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            ////GlobalGame.MainWindow.AddHandler(Keyboard.PreviewKeyDownEvent,
            ////    new KeyEventHandler(UC_KeyDown), handledEventsToo: true);
            _hostWindow = Window.GetWindow(this);

            if (_hostWindow != null)
            {
                _hostWindow.PreviewKeyDown += UC_KeyDown;
            }
            await _controller.LoadedAsync();
            await LoadLayoutRobotsAsync();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"Canvas size: {MainCanvas.ActualWidth}, {MainCanvas.ActualHeight}");
                Debug.WriteLine($"Image size: {FactoryInteriorImage.ActualWidth}, {FactoryInteriorImage.ActualHeight}");

                System.Diagnostics.Debug.WriteLine(
                    $"FactoryInterior_UC arranged size: {ActualWidth}x{ActualHeight}; canvas: {MainCanvas.ActualWidth}x{MainCanvas.ActualHeight}");
            }), DispatcherPriority.Loaded);
        }

        private async Task LoadLayoutRobotsAsync()
        {
            foreach (var robot in _loadedRobotArms.Skip(1).ToList())
                MainCanvas.Children.Remove(robot);
            _loadedRobotArms.Clear();

            var robotItems = (await _layoutItemService.GetByLocationIdAsync(_locationId, includePoints: false))
                .Where(item => string.Equals(item.MajorItemType, "Robot", StringComparison.OrdinalIgnoreCase))
                .ToList();

            for (var index = 0; index < robotItems.Count; index++)
            {
                var item = robotItems[index];
                var robot = index == 0 ? RobotArmNew : new RoboticArmUC();
                ConfigureLayoutRobot(robot, item);
                if (index > 0)
                    MainCanvas.Children.Add(robot);
                _loadedRobotArms.Add(robot);
            }
        }

        private static void ConfigureLayoutRobot(RoboticArmUC robot, LocationLayoutItemViewModel layoutItem)
        {
            robot.Tag = layoutItem;
            robot.IsDragEnabled = false;
            robot.Width = layoutItem.Width > 0 ? layoutItem.Width : 400;
            robot.Height = layoutItem.Height > 0 ? layoutItem.Height : 400;
            robot.SetBinding(RoboticArmUC.ItemDataJsonProperty, new Binding(nameof(LocationLayoutItemViewModel.ItemDataJson))
            {
                Source = layoutItem,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            robot.SetupRobot(CatoriUCLibrary.RobotColorEnum.Blue);
            Canvas.SetLeft(robot, layoutItem.X);
            Canvas.SetTop(robot, layoutItem.Y);
            Canvas.SetZIndex(robot, layoutItem.ZIndex == 0 ? 1221 : layoutItem.ZIndex);
            robot.Visibility = Visibility.Visible;
        }
        private void UC_Unloaded(object sender, RoutedEventArgs e)
        {
            GlobalGame.MainWindow?.RemoveHandler(Keyboard.PreviewKeyDownEvent,
                   new KeyEventHandler(UC_KeyDown));
        }

        private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _controller.HandleMouseMove(e);
        }
        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _controller.HandleMouseLeftButtonDown(e);
        }
        private void UC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _controller.HandleMouseLeftButtonUp(e);
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
       

        private void MenuItem_Start_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Cross;
            _controller.StartDrawing();
        }

        private void MenuItem_Complete_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            _controller.StopDrawing();
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

            (_loadedRobotArms.FirstOrDefault() ?? RobotArmNew).MoveToPoseAsync(targetPose);

        }
        bool is_odd(int n)
        {
            return n % 2 != 0;
        }
        private void UC_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.None:
                    break;
                case Key.A:
                    break;
                case Key.B:
                    break;
                case Key.C:
                    break;
                case Key.D:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        DanceRobot();
                    }
                    break;
                case Key.E:
                    break;
                case Key.F:
                    break;
                case Key.G:
                    break;
                case Key.H:
                    break;
                case Key.I:
                    break;
                case Key.J:
                    break;
                case Key.K:
                    break;
                case Key.L:
                    break;
                case Key.M:
                    break;
                case Key.N:
                    break;
                case Key.O:
                    break;
                case Key.P:
                    break;
                case Key.Q:
                    break;
                case Key.R:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        _controller.ProducePart();
                    }
                    break;
                case Key.S:
                    break;
                case Key.T:
                    break;
                case Key.U:
                    break;
                case Key.V:
                    break;
                case Key.W:
                    break;
                case Key.X:
                    break;
                case Key.Y:
                    break;
                case Key.Z:
                    break;
                default:
                    break;
            }
          }

        private void Zone1_MouseEnter(object sender, MouseEventArgs e)
        {

        }

     }
}







