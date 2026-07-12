using System.Diagnostics;
using System.Security.Policy;
using System.Windows.Threading;
namespace CatoriApp.Game.Controllers.Locations.Factory
{
    public class FactoryInterior_UController
    {
        FactoryInterior_UC _view;
        Cursor _currentCursor;
        LocationService layoutService = new LocationService();
        LocationLayoutItemService _layoutItemService = new LocationLayoutItemService();
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
        List<Polygon> _zones;
        List<Polygon> _interactiveZones = new List<Polygon>();
        Point _startPoint;
        Point _endPoint;
        bool _editMode = false;
        private bool _isDrawingLine = false;
        private Line? _previewLine;
        Point _currentPoint;
        LocationService _locationService;
        AnimationController moveXYOnControl;
        int _locationid = 0;
        LocationViewModel _locationViewModel;
        private bool _isOverInteractiveZone;
        private const int InteractiveZoneZIndex = 1603;
        private const int InteractiveZoneEditZIndex = 2101;
        private const double ConveyorHitAreaWidth = 60;
        ObservableCollection<LocationLayoutItemViewModel> _locationLayoutItemViewModels;
        public FactoryInterior_UController(FactoryInterior_UC view, int locationid)
        {
            _locationid = locationid;
            _view = view;
            _currentCursor = view.Cursor;
            moveXYOnControl = new AnimationController();
            _locationService = new LocationService();
            _locationViewModel = _locationService.GetByLocationId(locationid);
            _layoutItemService = new LocationLayoutItemService();
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
            var locitems = await _layoutItemService.GetByLocationIdAsync(_locationid);
            _locationLayoutItemViewModels = new ObservableCollection<LocationLayoutItemViewModel>();
            foreach (var item in locitems)
            {
                _locationLayoutItemViewModels.Add(item);
            }
            
            LoadZones(_locationLayoutItemViewModels);
            if (viewModel.Points.Count == 0)
            {
                _view.ShowrobotControlPanel();
            }

        }

        private void LoadZones(ObservableCollection<LocationLayoutItemViewModel> layouts)
        {

            if (_zones != null)
            {
                foreach (var zone in _zones)
                    _view.MainCanvas.Children.Remove(zone);
            }

            _zones = new List<Polygon>();
            _interactiveZones.Clear();
            int zMainIndex = Panel.GetZIndex(_view.FactoryInteriorImage);
            foreach (var thislayout in _locationLayoutItemViewModels)
            {
                System.Windows.Media.Brush fillBrush;
                fillBrush = MediaCommon.GetBrushForMajorItemType(thislayout.MajorItemType);

                Polygon polygon = new Polygon();
                polygon.Visibility = Visibility.Hidden;
                polygon.StrokeThickness = 2;
                polygon.Tag = "zone";
                polygon.IsHitTestVisible = true;
                polygon.Opacity = 1;
                int zCurrentIndex = zMainIndex+1;
                if (IsZoneLayout(thislayout))
                {
                        switch (thislayout.ItemType)
                        {
                            case LocationLayoutItemType.Conveyor:
                                polygon.Stroke = Brushes.Red;
                                polygon.Fill = fillBrush;
                                zCurrentIndex = InteractiveZoneZIndex;
                                MakeInteractiveZone(polygon);
                                _interactiveZones.Add(polygon);
                                break;
                            case LocationLayoutItemType.Table:
                                polygon.Stroke = Brushes.Red;
                                polygon.Fill = fillBrush;
                                zCurrentIndex = zMainIndex + 1;
                                break;
                            case LocationLayoutItemType.Workstation:
                                polygon.Stroke = Brushes.Red;
                                polygon.Fill = fillBrush;
                                zCurrentIndex = zMainIndex + 1;
                                break;
                            case LocationLayoutItemType.Storage:
                                polygon.Stroke = Brushes.Red;
                                polygon.Fill = fillBrush;
                                zCurrentIndex = zMainIndex + 1;
                                break;
                            //case LocationLayoutItemType.Decoration:
                            //    polygon.Stroke = Brushes.Red;
                            //    polygon.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                            //    zCurrentIndex = zMainIndex + 1;
                            //    if (IsConveyorZone(thislayout))
                            //    {
                            //        zCurrentIndex = InteractiveZoneZIndex;
                            //        MakeInteractiveZone(polygon);
                            //        _interactiveZones.Add(polygon);
                            //    }
                            //    break;
                            case LocationLayoutItemType.PickupZone:
                                polygon.Stroke = Brushes.Blue;
                                polygon.Fill = fillBrush;
                                zCurrentIndex = InteractiveZoneZIndex;
                                MakeInteractiveZone(polygon);
                                _interactiveZones.Add(polygon);
                                break;
                            case LocationLayoutItemType.DropoffZone:
                                polygon.Stroke = Brushes.AliceBlue;
                                polygon.Fill = fillBrush;
                                zCurrentIndex = InteractiveZoneZIndex;
                                MakeInteractiveZone(polygon);
                                _interactiveZones.Add(polygon);
                                break;
                            default:
                                break;
                        }
                }
                else if (string.Equals(thislayout.MajorItemType, "Robot", StringComparison.OrdinalIgnoreCase))
                {
                        switch (thislayout.ItemType)
                        {
                            case LocationLayoutItemType.Robot:
                                break;
                            default:
                                break;
                        }
                }
                foreach (LocationLayoutPointViewModel point in thislayout.Points)
                {
                    polygon.Points.Add(new Point(point.X, point.Y));
                }

                if (thislayout.ItemType == LocationLayoutItemType.Conveyor && polygon.Points.Count == 2)
                {
                    polygon.Points = CreateConveyorHitArea(polygon.Points[0], polygon.Points[1]);
                }
                Debug.WriteLine($"{thislayout.ItemName} {thislayout.ItemType} {thislayout.MajorItemType} Points={polygon.Points.Count}");
                if (polygon.Points.Count >= 3)
                {
                    //Canvas.SetLeft(polygon, thislayout.X);
                    //Canvas.SetTop(polygon, thislayout.Y );
                    Canvas.SetZIndex(polygon, zCurrentIndex);
                    if (_interactiveZones.Contains(polygon))
                    {
                        polygon.Visibility = Visibility.Visible;
                        polygon.Opacity = _editMode ? 1 : 0.01;
                    }

                    _view.MainCanvas.Children.Add(polygon);
                    _zones.Add(polygon);
                }
            }
        }

        private void MakeInteractiveZone(Polygon polygon)
        {
            polygon.Cursor = Cursors.Cross;
            polygon.ForceCursor = true;
            polygon.MouseEnter += Polygon_MouseEnter;
            polygon.MouseLeave += Polygon_MouseLeave;
        }

        private static bool IsZoneLayout(LocationLayoutItemViewModel layout)
        {
            if (string.Equals(layout.MajorItemType, "Zone", StringComparison.OrdinalIgnoreCase))
                return true;

            return layout.ItemType == LocationLayoutItemType.Conveyor
                || layout.ItemType == LocationLayoutItemType.PickupZone
                || layout.ItemType == LocationLayoutItemType.DropoffZone;
        }
        private static bool IsConveyorZone(LocationLayoutItemViewModel layout)
        {
            return string.Equals(layout.MajorItemType, "Zone", StringComparison.OrdinalIgnoreCase)
                && layout.ItemName.StartsWith("Conveyor", StringComparison.OrdinalIgnoreCase);
        }
        private static PointCollection CreateConveyorHitArea(Point startPoint, Point endPoint)
        {
            Vector direction = endPoint - startPoint;
            if (direction.Length == 0)
            {
                double halfSize = ConveyorHitAreaWidth / 2;
                return new PointCollection
                {
                    new Point(startPoint.X - halfSize, startPoint.Y - halfSize),
                    new Point(startPoint.X + halfSize, startPoint.Y - halfSize),
                    new Point(startPoint.X + halfSize, startPoint.Y + halfSize),
                    new Point(startPoint.X - halfSize, startPoint.Y + halfSize)
                };
            }

            direction.Normalize();
            Vector normal = new Vector(-direction.Y, direction.X) * (ConveyorHitAreaWidth / 2);

            return new PointCollection
            {
                startPoint + normal,
                endPoint + normal,
                endPoint - normal,
                startPoint - normal
            };
        }
        private void Polygon_MouseLeave(object sender, MouseEventArgs e)
        {
            ApplyInteractiveCursor(false);
        }

        private void Polygon_MouseEnter(object sender, MouseEventArgs e)
        {
            ApplyInteractiveCursor(true);
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
            foreach (var zone in _zones)
            {
                if (_interactiveZones.Contains(zone))
                {
                    zone.Visibility = Visibility.Visible;
                    zone.Opacity = 0.01;
                    Canvas.SetZIndex(zone, InteractiveZoneZIndex);
                }
                else
                {
                    zone.Visibility = Visibility.Hidden;
                }
            }       
            ApplyInteractiveCursor(false);
            return handler;
        }

        internal EventHandler EditMode()
        {
            EventHandler handler = null;
            _editMode = true;
            foreach (var zone in _zones)
            {
                zone.Visibility = Visibility.Visible;
                zone.Opacity = 1;
                if (_interactiveZones.Contains(zone))
                {
                    Canvas.SetZIndex(zone, InteractiveZoneEditZIndex);
                }
            }
            
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
            _ = LoadViewModelAsync();
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
            UpdateInteractiveZoneCursor(_currentPoint);

            if (!_editMode || !_isDrawingLine || _previewLine == null)
                return;

            _previewLine.X2 = _currentPoint.X;
            _previewLine.Y2 = _currentPoint.Y;

         }

        private void UpdateInteractiveZoneCursor(Point point)
        {
            //System.Diagnostics.Debug.WriteLine($"Mouse move editmode {_editMode} at {point.X}, {point.Y}");
            if (_interactiveZones.Count == 0)
            {
                ApplyInteractiveCursor(false);
                return;
            }

            bool isOverInteractiveZone = _interactiveZones.Any(zone =>
                zone.Visibility == Visibility.Visible && IsPointInsidePolygon(point, zone));
            ApplyInteractiveCursor(isOverInteractiveZone);
            //System.Diagnostics.Debug.WriteLine($"Mouse move editmode {_editMode} at {point.X}, {point.Y}. Over interactive zone: {isOverInteractiveZone}");
        }

        private void ApplyInteractiveCursor(bool isOverInteractiveZone)
        {
            Cursor cursor = isOverInteractiveZone ? Cursors.Cross : _currentCursor;
            bool forceCursor = _editMode || isOverInteractiveZone;

            _isOverInteractiveZone = isOverInteractiveZone;
            _view.ForceCursor = forceCursor;
            _view.MainCanvas.ForceCursor = forceCursor;
            _view.Cursor = cursor;
            _view.MainCanvas.Cursor = cursor;
        }

        private static bool IsPointInsidePolygon(Point canvasPoint, Polygon polygon)
        {
            if (polygon.Points.Count < 3)
                return false;

            double left = Canvas.GetLeft(polygon);
            double top = Canvas.GetTop(polygon);
            if (double.IsNaN(left))
                left = 0;
            if (double.IsNaN(top))
                top = 0;

            Point testPoint = new Point(canvasPoint.X - left, canvasPoint.Y - top);
            bool isInside = false;
            int previousIndex = polygon.Points.Count - 1;

            for (int currentIndex = 0; currentIndex < polygon.Points.Count; currentIndex++)
            {
                Point current = polygon.Points[currentIndex];
                Point previous = polygon.Points[previousIndex];

                bool crossesY = current.Y > testPoint.Y != previous.Y > testPoint.Y;
                if (crossesY)
                {
                    double intersectionX = (previous.X - current.X) * (testPoint.Y - current.Y) / (previous.Y - current.Y) + current.X;
                    if (testPoint.X < intersectionX)
                        isInside = !isInside;
                }

                previousIndex = currentIndex;
            }

            return isInside;
        }

        internal void MouseUpAfterRobotMove(RobotArg arg)
        {
        }
    }
}














