using CatoriApp.Game.Controllers.LocationSubControllers;
using CatoriApp.Game.Objects.AnimationOnPath;
using CatoriApp.MachineLayoutDesigner.ViewModels.Robots;
using CatoriUCLibrary.Views.RobotArm;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Windows.Threading;
namespace CatoriApp.Game.Controllers.Locations.Factory
{
    public class FactoryInterior_UController
    {
        FactoryInterior_UC _view;
        Cursor _currentCursor;
        ILocationSubController _locationSubController;
   
         readonly DispatcherTimer _animation_timer;
            public string ConveyorA;
        public string ConveyorB;
        public string ConveyorTarget;
        List<Polygon> _zones;
        List<Polygon> _interactiveZones = new List<Polygon>();
        private Point _segmentStart;
        private Point _currentPoint;
        private bool _editMode;
        AnimationController moveXYOnControl;
        int _locationid = 0;
        private bool _isOverInteractiveZone;
        private const int InteractiveZoneZIndex = 1603;
        private const int InteractiveZoneEditZIndex = 2101;
        private const double ConveyorHitAreaWidth = 60;
        private Polygon? _activeDrawingZone;
        public FactoryInterior_UController(FactoryInterior_UC view, 
            int locationid)
        {
            _locationid = locationid;
            _view = view;
            _currentCursor = view.Cursor;
            switch (_locationid)
            {
                case 1:
                    _locationSubController = new LocationInterior1SubController(_view,_locationid);
                    break;
                case 2:
                    _locationSubController = new Locationinterior2SubController(_view, _locationid);
                    break;
                case 3:
                    _locationSubController = new Locationinterior3SubController(_view, _locationid);
                    break;
                case 4:
                    _locationSubController = new LocationInterior1SubController(_view, _locationid);
                    break;
                default:
                    _locationSubController = CreateGeneratedSubController(_locationid);
                    break;
            }
            //moveXYOnControl = new AnimationController();
            //  _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);
            //string root = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "WIthSawBlade");
            //LoadViewModelAsync();
            //if (viewModel.Points.Count == 0)
            //{
            //    _view.ShowrobotControlPanel();
            //}

            //< Robots:RobotControl x:Name = "RobotBuilder" Canvas.Left = "20" Panel.ZIndex = "1202"
            //              Canvas.Top = "50" />
            //part1SimpleUC = new ProductUC("factories//SawBlade.png");
            //part2SimpleUC = new ProductUC("factories//SawHandle.png");
            //part3CompleteUC = new ProductUC("factories//SawPurpleHandle.png");
            //part1SimpleUC.Opacity = 0;
            //part2SimpleUC.Opacity = 0;
            //part3CompleteUC.Opacity = 0;
        }

        private ILocationSubController CreateGeneratedSubController(int locationId)
        {
            string typeName =
                $"CatoriApp.Game.Controllers.LocationSubControllers.Locationinterior{locationId}SubController";
            Type controllerType = typeof(FactoryInterior_UController).Assembly.GetType(typeName)
                ?? throw new InvalidOperationException(
                    $"The generated location controller '{typeName}' was not found. Save the location in Studio, rebuild CatoriApp, and try again.");
            object? controller = Activator.CreateInstance(controllerType, _view, (long)locationId);
            return controller as ILocationSubController
                ?? throw new InvalidOperationException(
                    $"The generated location controller '{typeName}' does not implement ILocationSubController.");
        }
        private async Task LoadViewModelAsync()
        {
            //viewModel = await locationService.GetByLocationIdAsync(_locationid) ?? new LocationViewModel();
            //_view.RobotPanel.LocationId = viewModel.LocationId;
          
            //LoadZones(_locationLayoutItemViewModels);
        }
        public async Task LoadedAsync()
        {
            //machineLayoutDesignermodel = await _robotdesignservice.LoadByLocationIdAsync(viewModel.LocationId);
            //double left = machineLayoutDesignermodel.RobotX;
            //double top = machineLayoutDesignermodel.RobotY;
            //left = 620;
            //top = 330;

            ////CatoriUCLibrary.Views.RobotArm.RobotPose targetPose =
            ////   new CatoriUCLibrary.Views.RobotArm.RobotPose(j1, j2, j3, j4);
            //_robotPoses = new List<RobotPose>();
            //Canvas.SetLeft(_view.RobotArmNew, left);
            //Canvas.SetTop(_view.RobotArmNew, top);
            //foreach (var pose in machineLayoutDesignermodel.Poses)
            //{
            //    List<double> angles = new List<double>();
            //    foreach (var segment in pose.Segments)
            //    {
            //        angles.Add(segment.Angle);
            //    }
            //    RobotPose robotpose = new RobotPose(angles[0], angles[1], angles[2], angles[3]);
            //    _robotPoses.Add(robotpose);
            //}
            //SetupAnimations();

        }
     
       
        public void ProducePart()
        {
            try
            {
                _locationSubController.StartProduction();
                
            }
            catch (Exception ex)
            {

                throw;
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
            //_isDrawingEnabled = false;
            CancelCurrentSegment();
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
            //SwitchPathColor();
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
            //SwitchPathColor();
            return handler;
        }
       
      

        private async Task ShortDelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }
        internal void StartDrawing()
        {
            //if (!_editMode)
            //    return;

            //CancelCurrentSegment();
            //_isDrawingEnabled = true;
            //UpdateInteractiveZoneCursor(Mouse.GetPosition(_view.MainCanvas));
        }
        

        internal void StopDrawing()
        {
            //_isDrawingEnabled = false;

            //// Complete should not create another point.
            //// Remove any unfinished preview segment.
            //if (_isDrawingSegment)
            //{
            //    CancelCurrentSegment();
            //}
            //else
            //{
            //    _view.MainCanvas.ReleaseMouseCapture();
            //}

            ApplyInteractiveCursor(false);

            //if (_drawingLayoutItem != null &&
            //    !string.IsNullOrWhiteSpace(_drawingLayoutItem.ItemDataJson))
            //{
            //    _layoutItemService.UpdateItemDataJson(
            //        _drawingLayoutItem.LocationLayoutItemId,
            //        _drawingLayoutItem.ItemDataJson);
            //}

            //_drawingLayoutItem = null;
            //_activeDrawingItem = null;
            //_lastPathPoint = null;
        }

        internal void HandleMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //GlobalAllApps.WriteDebugInfo($"Mouse left button down at {e.GetPosition(_view.MainCanvas)}. EditMode: {_editMode}, DrawingEnabled: {_isDrawingEnabled}, IsDrawingSegment: {_isDrawingSegment}");
            //if (!_editMode || !_isDrawingEnabled || _isDrawingSegment)
            //    return;
            
           
            //Point mousePoint = e.GetPosition(_view.MainCanvas);
            //Polygon? zone = GetInteractiveZoneAt(mousePoint);

            //if (zone?.Tag is not LocationLayoutItemViewModel layoutItem)
            //    return;

            //// Lock the whole drawing session to one layout item.
            //_drawingLayoutItem ??= layoutItem;

            //if (_drawingLayoutItem.LocationLayoutItemId !=
            //    layoutItem.LocationLayoutItemId)
            //{
            //    return;
            //}

            //// First segment starts at the click.
            //// Later segments start at the previous endpoint.

            //_activeDrawingZone = zone;
            //_activeDrawingItem = layoutItem;
            //_drawingLayoutItem ??= layoutItem;

            //_segmentStart = _lastPathPoint ?? mousePoint;
            //_currentPoint = mousePoint;
            //_isDrawingSegment = true;

            //StartPreviewSegment(_segmentStart);
            //_view.MainCanvas.CaptureMouse();
            //e.Handled = true;
        }

        internal void HandleMouseMove(MouseEventArgs e)
        {
            //Point mousePoint = e.GetPosition(_view.MainCanvas);

            //UpdateInteractiveZoneCursor(mousePoint);

            //if (!_isDrawingSegment || _previewPath == null)
            //    return;

            //// Do not let the preview follow the cursor after the button is released.
            //if (e.LeftButton != MouseButtonState.Pressed)
            //{
            //    CancelCurrentSegment();
            //    return;
            //}

            //_currentPoint = mousePoint;

            //_previewPath.Data =
            //    CreateSegmentGeometry(_segmentStart, _currentPoint);
        }

        internal void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //GlobalAllApps.WriteDebugInfo($"Mouse left button up at {e.GetPosition(_view.MainCanvas)}. EditMode: {_editMode}, DrawingEnabled: {_isDrawingEnabled}, IsDrawingSegment: {_isDrawingSegment}");
            //if (!_isDrawingSegment)
            //    return;

            //Point segmentEnd = e.GetPosition(_view.MainCanvas);

            //bool endedInSameZone =
            //    _activeDrawingZone != null &&
            //    IsPointInsidePolygon(segmentEnd, _activeDrawingZone);

            //GlobalAllApps.WriteDebugInfo(
            //    $"MouseUp: Start={_segmentStart}, End={segmentEnd}, " +
            //    $"SameZone={endedInSameZone}");

            //if (endedInSameZone &&
            //    !PointsAreEffectivelyEqual(_segmentStart, segmentEnd))
            //{
            //    CompleteSegment(segmentEnd);
            //}
            //else
            //{
            //    CancelCurrentSegment();
            //}

            //e.Handled = true;
        }

        private void StartPreviewSegment(Point startPoint)
        {
            Path _previewPath = new System.Windows.Shapes.Path
            {
                Data = CreateSegmentGeometry(startPoint, startPoint),
                Stroke = Brushes.Yellow,
                StrokeThickness = 4,
                Name = "PreviewPath",
                IsHitTestVisible = false,
                StrokeDashArray = new DoubleCollection { 4, 2 }
            };

            Panel.SetZIndex(_previewPath, InteractiveZoneEditZIndex + 100);
            _view.MainCanvas.Children.Add(_previewPath);
        }

        private void CompleteSegment(Point endPoint)
        {
            //if (_drawingLayoutItem == null || _previewPath == null)
            //{
            //    CancelCurrentSegment();
            //    return;
            //}

            //_drawingLayoutItem.ItemDataJson = AppendSegment(
            //    _drawingLayoutItem.ItemDataJson,
            //    _segmentStart,
            //    endPoint);

            //_lastPathPoint = endPoint;

            //// Create a separate permanent path.
            //var completedPath = new System.Windows.Shapes.Path
            //{
            //    Data = CreateSegmentGeometry(_segmentStart, endPoint),
            //    Stroke = Brushes.LimeGreen,
            //    StrokeThickness = 4,
            //    IsHitTestVisible = false,
            //    Tag = _drawingLayoutItem
            //};

            //Panel.SetZIndex(
            //    completedPath,
            //    InteractiveZoneEditZIndex + 100);
            //_designerPaths.Add(completedPath);
            //_view.MainCanvas.Children.Add(completedPath);

            //GlobalAllApps.WriteDebugInfo(
            //    $"Completed path added. " +
            //    $"Canvas children={_view.MainCanvas.Children.Count}, " +
            //    $"Start={_segmentStart}, End={endPoint}");

            //// Now remove the temporary dashed path.
            //ResetDrawingSegmentState(removePreview: true);
        }
        private string AppendSegment(
            string? existingItemDataJson,
            Point startPoint,
            Point endPoint)
        {
            string newSegment =
                $"M {startPoint.X:0.##},{startPoint.Y:0.##} " +
                $"L {endPoint.X:0.##},{endPoint.Y:0.##}";

            if (string.IsNullOrWhiteSpace(existingItemDataJson))
            {
                return newSegment;
            }

            return $"{existingItemDataJson.Trim()} {newSegment}";
        }
        //private string AppendSegment(string? existingItemDataJson,
        //    Point startPoint,Point endPoint)
        //{
        //    string result;

        //    if (string.IsNullOrWhiteSpace(existingItemDataJson))
        //    {
        //        result =
        //            $"M {startPoint.X:0.##},{startPoint.Y:0.##} " +
        //            $"L {endPoint.X:0.##},{endPoint.Y:0.##}";
        //    }
        //    else
        //    {
        //        result =
        //            $"{existingItemDataJson.Trim()} " +
        //            $"M {_lastPathPosition.X:0.##},{_lastPathPosition.Y:0.##} " +
        //            $"L {endPoint.X:0.##},{endPoint.Y:0.##}";
        //    }

        //    _lastPathPosition = endPoint;

        //    return result;
        //}

        private static Geometry CreateSegmentGeometry(Point startPoint, Point endPoint)
        {
            //GlobalAllApps.WriteDebugInfo($"Creating segment geometry from {startPoint} to {endPoint}");
            return new LineGeometry(startPoint, endPoint);
        }

        private Polygon? GetInteractiveZoneAt(Point point)
        {
            return _interactiveZones
                .Where(zone => zone.Visibility == Visibility.Visible)
                .OrderByDescending(Panel.GetZIndex)
                .FirstOrDefault(zone => IsPointInsidePolygon(point, zone));
        }

        private static bool PointsAreEffectivelyEqual(Point first, Point second)
        {
            const double tolerance = 0.5;
            return Math.Abs(first.X - second.X) < tolerance &&
                   Math.Abs(first.Y - second.Y) < tolerance;
        }

        private void CancelCurrentSegment()
        {
            ResetDrawingSegmentState(removePreview: true);
        }

        private void ResetDrawingSegmentState(bool removePreview)
        {
            //if (removePreview && _previewPath != null)
            //{
            //    _view.MainCanvas.Children.Remove(_previewPath);
            //}

            //_view.MainCanvas.ReleaseMouseCapture();

            //_previewPath = null;
            //_activeDrawingItem = null;
            //_activeDrawingZone = null;
            //_isDrawingSegment = false;
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

        private System.Windows.Shapes.Path AddPath(string pathData)
        {
            if (string.IsNullOrWhiteSpace(pathData))
                return null;

            Geometry geometry;

            try
            {
                geometry = Geometry.Parse(pathData);
            }
            catch (FormatException ex)
            {
                Debug.WriteLine($"Invalid path data: {pathData}");
                Debug.WriteLine(ex);
                return null;
            }

            var path = new System.Windows.Shapes.Path
            {
                Data = geometry,
                Stroke = Brushes.LimeGreen,
                StrokeThickness = 4,
                Fill = Brushes.Transparent,
                Stretch = Stretch.None,
                SnapsToDevicePixels = true
            };

            Canvas.SetLeft(path, 0);
            Canvas.SetTop(path, 0);
            Panel.SetZIndex(path, 2500);

            _view.MainCanvas.Children.Add(path);

            return path;
        }
    }
}
