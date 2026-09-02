using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Objects.Services.Locations
{
    public class PathPlayer
    {
        private LocationLayoutItemEntity _layoutitem;
        private readonly List<Point> _pathPoints = new();
        public readonly List<System.Windows.Shapes.Path> PathsForMovement = new();

        private System.Windows.Shapes.Path? _previewPath;
        private System.Windows.Shapes.Path? _displayPath;
        private LocationLayoutItemEntity? _activeDrawingItem;
        private LocationLayoutItemEntity? _drawingLayoutItem;
        private Point? _lastPathPoint;
        private readonly Location_UC _view;
       
        public PathPlayer(Location_UC view)
        {
            _view = view;
        }
        /// <summary>
        /// Distance before and after a corner where rounding begins.
        /// Larger values produce broader curves.
        /// </summary>
        public double CornerDistance { get; set; } = 40;

        /// <summary>
        /// Minimum direction change, in degrees, required before a point is rounded.
        /// Smaller turns remain straight.
        /// </summary>
        public double MinimumTurnAngle { get; set; } = 10;
        internal void RemoveObjectsFromCanvas()
        {

            foreach (var path in PathsForMovement.ToList())
                _view.MainCanvas.Children.Remove(path);

            PathsForMovement.Clear();
            _displayPath = null;
            _pathPoints.Clear();

            _drawingLayoutItem = null;
            _activeDrawingItem = null;
            _lastPathPoint = null;
        }
        internal void LoadExistingPath(LocationLayoutItemEntity? thisItem)
        {
            if (string.IsNullOrWhiteSpace(thisItem?.ItemDataJson))
                return;
            _layoutitem = thisItem;
            try
            {
                Geometry geometry = Geometry.Parse(thisItem.ItemDataJson);
                LoadPointsFromGeometry(geometry);

                if (_pathPoints.Count >= 2)
                {
                    // Rebuild old M/L path data using the new curve logic.
                    RebuildAndDisplayPath();
                    _lastPathPoint = _pathPoints[^1];
                }
            }
            catch (FormatException ex)
            {
                GlobalAllApps.WriteDebugInfo(
                    $"Invalid WPF path for layout item " +
                    $"{_layoutitem.LocationLayoutItemId}: {ex.Message}");
            }
        }
        private void LoadPointsFromGeometry(Geometry geometry)
        {
            _pathPoints.Clear();

            PathGeometry flattened = geometry.GetFlattenedPathGeometry();

            foreach (PathFigure figure in flattened.Figures)
            {
                AddPointIfDifferent(figure.StartPoint);

                foreach (PathSegment segment in figure.Segments)
                {
                    switch (segment)
                    {
                        case LineSegment line:
                            AddPointIfDifferent(line.Point);
                            break;

                        case PolyLineSegment polyLine:
                            foreach (Point point in polyLine.Points)
                                AddPointIfDifferent(point);
                            break;
                    }
                }
            }
        }
        private void AddPointIfDifferent(Point point)
        {
            if (_pathPoints.Count == 0 ||
                !PointsAreEffectivelyEqual(_pathPoints[^1], point))
            {
                _pathPoints.Add(point);
            }
        }
        private void RebuildAndDisplayPath()
        {
            if (_pathPoints.Count < 2)
                return;

            string pathData = BuildRoundedPath(
                _pathPoints,
                CornerDistance,
                MinimumTurnAngle);

            _layoutitem.ItemDataJson = pathData;

            Geometry geometry = Geometry.Parse(pathData);

            if (_displayPath == null)
            {
                _displayPath = new System.Windows.Shapes.Path
                {
                    Stroke = Brushes.LimeGreen,
                    StrokeThickness = 4,
                    Fill = Brushes.Transparent,
                    Stretch = Stretch.None,
                    IsHitTestVisible = false,
                    Tag = _layoutitem
                };

                Panel.SetZIndex(_displayPath, 2200);
                PathsForMovement.Add(_displayPath);
                _view.MainCanvas.Children.Add(_displayPath);
            }

            _displayPath.Data = geometry;
        }

        private static string BuildRoundedPath(
          IReadOnlyList<Point> points,
          double cornerDistance,
          double minimumTurnAngle)
        {
            if (points.Count < 2)
                return string.Empty;

            if (points.Count == 2)
                return $"M {Format(points[0])} L {Format(points[1])}";

            StringBuilder path = new();
            path.Append($"M {Format(points[0])} ");

            for (int i = 1; i < points.Count - 1; i++)
            {
                Point previous = points[i - 1];
                Point corner = points[i];
                Point next = points[i + 1];

                double turnAngle = GetTurnAngle(previous, corner, next);

                if (turnAngle < minimumTurnAngle)
                {
                    path.Append($"L {Format(corner)} ");
                    continue;
                }

                double incomingDistance = Math.Min(
                    cornerDistance,
                    Distance(previous, corner) / 2.0);

                double outgoingDistance = Math.Min(
                    cornerDistance,
                    Distance(corner, next) / 2.0);

                Point curveStart = MoveToward(corner, previous, incomingDistance);
                Point curveEnd = MoveToward(corner, next, outgoingDistance);

                path.Append($"L {Format(curveStart)} ");
                path.Append($"Q {Format(corner)} {Format(curveEnd)} ");
            }

            path.Append($"L {Format(points[^1])}");
            return path.ToString();
        }

        private static double GetTurnAngle(
            Point previous,
            Point corner,
            Point next)
        {
            Vector incoming = corner - previous;
            Vector outgoing = next - corner;

            if (incoming.Length < 0.001 || outgoing.Length < 0.001)
                return 0;

            incoming.Normalize();
            outgoing.Normalize();

            double dot = Vector.Multiply(incoming, outgoing);
            dot = Math.Clamp(dot, -1.0, 1.0);

            return Math.Acos(dot) * 180.0 / Math.PI;
        }
        private static Point MoveToward(
              Point from,
              Point toward,
              double distance)
        {
            Vector direction = toward - from;

            if (direction.Length < 0.001)
                return from;

            direction.Normalize();
            return from + direction * distance;
        }

        private static double Distance(Point first, Point second)
        {
            return (second - first).Length;
        }

        private static Geometry CreateSegmentGeometry(Point startPoint, Point endPoint)
        {
            return new LineGeometry(startPoint, endPoint);
        }

        private static bool PointsAreEffectivelyEqual(Point first, Point second)
        {
            const double tolerance = 0.5;

            return Math.Abs(first.X - second.X) < tolerance &&
                   Math.Abs(first.Y - second.Y) < tolerance;
        }

        private static string Format(Point point)
        {
            return $"{point.X:0.##},{point.Y:0.##}";
        }
    }
}
