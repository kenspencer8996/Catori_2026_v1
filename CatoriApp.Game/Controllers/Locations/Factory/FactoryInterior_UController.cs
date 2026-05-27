using CatoriApp.Game.Controllers;
using CatoriApp.Core.ExtensionMethods;
using CatoriApp.Core.Objects.Arguments;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace CatoriApp.Game.Controllers.Locations.Factory
{
    public class FactoryInterior_UController
    {
        FactoryInterior_UC _view;
        LocationService layoutService = new LocationService();
        List<string> _factoryImages;
        public List<RobotPoseViewModel> robotposes { get; private set; }
        private RobotPoseService _poseservice = new RobotPoseService();
        string _backgroundImagePath;

        LocationViewModel viewModel = new LocationViewModel();    
        readonly DispatcherTimer _animation_timer;
        public string ImagesFolderRight = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "RightDrilling");
        public string restImageRight = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "RightDrilling", "RobotArm2rightDrilling00r.png");
        string root = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "WIthSawBlade");
          public string ConveyorA;
        public string ConveyorB;
        public string ConveyorTarget;

        Point _startPoint;
        Point _endPoint;
        bool _editMode = false;
        private bool _isDrawingLine = false;
        private Line? _previewLine;
        Point _currentPoint;
        LocationService _layoutService;
        AnimationController moveXYOnControl;
        int _locationid = 0;
        public FactoryInterior_UController(FactoryInterior_UC view, int locationid)
        {
            _locationid = locationid;
            _view = view;
 
            moveXYOnControl = new AnimationController();
            _layoutService = new LocationService();
            _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);
            string root = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "WIthSawBlade");
            LoadViewModelAsync();
            if (viewModel.Points.Count == 0)
            {
                _view.ShowrobotControlPanel();
            }
            //< Robots:RobotControl x:Name = "RobotBuilder" Canvas.Left = "20" Panel.ZIndex = "1202"
            //              Canvas.Top = "50" />

        }
        private async Task LoadViewModelAsync()
        {
            viewModel = await layoutService.GetByLocationIdAsync(_locationid) ?? new LocationViewModel();
            _view.RobotPanel.LocationId = viewModel.LocationId;
            robotposes = viewModel.LocationId > 0
                ? await _poseservice.GetByLocationIdAsync(viewModel.LocationId)
                : new List<RobotPoseViewModel>();

            await SetupInteriorAsync(viewModel);
        }
        internal EventHandler RunRequested(RobotControlPanelSelectionsArg e)
        {
            ConveyorA = e.InputA;
            ConveyorB = e.InputB;
            ConveyorTarget = e.Output;
            EventHandler handler = null;
            MovePartsToStep1();
            StartProductionLine();

            return handler;
        }
        internal void SetupInteriorImages()
        {
            _factoryImages = new List<string>();
            _factoryImages.Add("FactoryConVeyorTable1.png");

            _factoryImages.Add("FactoryInterior1.png");
            _factoryImages.Add("Factoryinterior2.png");
            _factoryImages.Add("FactoryInterior4.png");
            _factoryImages.Add("FactoryTables1.png");
            _factoryImages.Add("FactoryTables2.png");
        }
        private async void StartProductionLine()
        {
            //await moveXYOnControl.MoveXYOnControlToAsync(partSimpleUC,transform,finalPoint, duration);
            //moveXYOnControl.StartAnimation(RobotRightUC, 10, 0, 100, 0, 500);
        }
        internal EventHandler EditModeEnd()
        {
            EventHandler handler = null;
            _editMode = false;
            CancelCurrentLines();
            return handler;
        }

        internal EventHandler EditMode()
        {
            EventHandler handler = null;
            _editMode = true;
            return handler;
        }
        private async void MovePartsToStep1()
        {
            //_view.part1SimpleUC.Opacity = 1;
            //_view.part2SimpleUC.Opacity = 1;

            //TimeSpan duration = TimeSpan.FromSeconds(2);
            //var founda = viewModel.Points.FirstOrDefault(p => p.PointType == ConveyorA);
            //var foundb = viewModel.Points.FirstOrDefault(p => p.PointType == ConveyorB);
            //double halfcontrolwidth = _view.part1SimpleUC.ActualWidth / 2;
            //double halfcontrolheight = _view.part1SimpleUC.ActualHeight / 2;
            //double ax = founda.XLoc - halfcontrolwidth;
            //double ay = founda.YLoc - halfcontrolheight;
            //double aendx = founda.XLocEnd;
            //double aendy = founda.YLocEnd;
            //double bx = foundb.XLoc - halfcontrolwidth;
            //double by = foundb.YLoc - halfcontrolheight;
            //double bendx = foundb.XLocEnd;
            //double bendy = foundb.YLocEnd;
            //Canvas.SetLeft(_view.part1SimpleUC, ax);
            //Canvas.SetTop(_view.part1SimpleUC, ay);
            //Storyboard sb1 = moveXYOnControl.MoveXYOnControlToAsync(_view.part1SimpleUC,
            //    aendx, aendy, duration);

            ////center conveyor
            //Canvas.SetLeft(_view.part2SimpleUC, bx);
            //Canvas.SetTop(_view.part2SimpleUC, by);
            //Storyboard sb2 = moveXYOnControl.MoveXYOnControlToAsync(_view.part2SimpleUC,
            //    bendx, bendy, duration);

            //sb2.Completed += (s, e) =>
            //{
            //};
            //sb1.Begin();
            //sb2.Begin();

        }
        public async Task SetupInteriorAsync(LocationViewModel vm)
        {
            SetupInteriorImages();
            _backgroundImagePath =System.IO.Path.Combine(GlobalAllApps.ImageFolder, "LocationInteriors", "Factory", vm.BackgroundImagePath);
            _view.FactoryInteriorImage.Source = UIUtility.GetImageControl(_backgroundImagePath, 1920, 1080, 0).Source;

            _view.RobotPanel.LocationBackgroundImagePath = _backgroundImagePath;
        }
        private TranslateTransform GetTransform()
        {

            TranslateTransform transform = new TranslateTransform();
            return transform;

        }

        private async Task ShortDelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }
        private void FinishLine(Point endPoint)
        {
            if (_previewLine == null)
                return;

            _previewLine.X2 = endPoint.X;
            _previewLine.Y2 = endPoint.Y;

            // Optional: make final line solid instead of dashed
            _previewLine.StrokeDashArray = null;
            _previewLine.Stroke = Brushes.LimeGreen;

            _isDrawingLine = false;
            _view.MainCanvas.ReleaseMouseCapture();

            // Save to database here
            SaveConveyorLine(_startPoint, endPoint);

            _previewLine = null;
        }
        private void SaveConveyorLine(Point startPoint, Point endPoint)
        {
            string PointType = "";

            PointType = GetLineName(startPoint);

            if (PointType != "")
            {
                LocationLayoutPointEntity newPoint = new LocationLayoutPointEntity
                {
                    LocationLayoutPointId = 0, // Assuming 0 means new entry
                    LocationId = viewModel.LocationId,
                    XLoc = startPoint.X,
                    YLoc = startPoint.Y,
                    PointType = PointType,
                    XLocEnd = endPoint.X,
                    YLocEnd = endPoint.Y
                };
                layoutService.UpdatePoints(viewModel.LocationId, newPoint);
            }
        }
        /// <summary>
        /// remove edit preview lines and reset edit state. Called when exiting edit mode or canceling current line drawing
        /// </summary>
        private void CancelCurrentLines()
        {
            List<Line> lines = UIHelper.FindChildrenByNamePrefix<Line>(_view.MainCanvas, "PreviewLine");
            if (lines != null && lines.Count > 0)
            {
                foreach (var line in lines)
                {
                    _view.MainCanvas.Children.Remove(line);
                }
                //MainCanvas.Children.Remove(_previewLine);
            }

            _isDrawingLine = false;
            _view.MainCanvas.ReleaseMouseCapture();
            LoadViewModelAsync();
        }

        internal void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (_editMode)
            {
                if (!_isDrawingLine)
                {
                    _startPoint = e.GetPosition(_view.MainCanvas);
                    System.Diagnostics.Debug.WriteLine("Start loc " + _startPoint.X + "  " + _startPoint.Y);
                    _isDrawingLine = true;
                    string lineName = GetLineName(_startPoint);
                    _previewLine = new Line
                    {
                        X1 = _startPoint.X,
                        Y1 = _startPoint.Y,
                        X2 = _currentPoint.X,
                        Y2 = _currentPoint.Y,
                        Stroke = Brushes.Yellow,
                        StrokeThickness = 4,
                        Name = "PreviewLine",
                        StrokeDashArray = new DoubleCollection { 4, 2 } // optional preview style
                    };

                    _view.MainCanvas.Children.Add(_previewLine);
                    _view.MainCanvas.CaptureMouse();
                }
                else
                {
                    // Second click = finalize line
                    FinishLine(_currentPoint);
                }
            }
        }

        private string GetLineName(Point startPoint)
        {
            int leftA = 971;
            int rightA = 1240;
            int leftB = 1158;
            int rightB = 1566;
            int rightC = 1838;
            string PointType = "";
            if (startPoint.X > leftA && startPoint.X < rightA)
                PointType = "A";
            else if (startPoint.X > rightA && startPoint.X < rightB)
                PointType = "B";
            else if (startPoint.X > rightC)
                PointType = "C";
            return PointType;
        }

        internal void HandleMouseMove(MouseEventArgs e)
        {
            _currentPoint = e.GetPosition(_view.MainCanvas);
            if (!_editMode || !_isDrawingLine || _previewLine == null)
                return;

            _previewLine.X2 = _currentPoint.X;
            _previewLine.Y2 = _currentPoint.Y;
        }

        internal void MouseUpAfterRobotMove(RobotArg arg)
        {
        }
    }
}





