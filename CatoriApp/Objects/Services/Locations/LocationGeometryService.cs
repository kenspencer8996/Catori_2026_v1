using System.Windows.Media;
namespace CatoriApp.Objects.Services.Locations
{
    public class LocationGeometryService
    {
        public PathGeometry BuildPathGeometry(IEnumerable<LocationLayoutPointViewModel> points)
        {
            var ordered = points.OrderBy(p => p.PointIndex).ToList();
            var geometry = new PathGeometry();

            if (ordered.Count == 0)
                return geometry;

            var figure = new PathFigure
            {
                StartPoint = new Point(ordered[0].X, ordered[0].Y),
                IsClosed = false
            };

            for (int i = 1; i < ordered.Count; i++)
            {
                var point = ordered[i];
                var endPoint = new Point(point.X, point.Y);

                switch (point.SegmentKind)
                {
                    case LocationLayoutSegmentKind.QuadraticBezier:
                        figure.Segments.Add(new QuadraticBezierSegment(
                            new Point(point.Control1X ?? point.X, point.Control1Y ?? point.Y),
                            endPoint,
                            true));
                        break;
                    case LocationLayoutSegmentKind.CubicBezier:
                        figure.Segments.Add(new BezierSegment(
                            new Point(point.Control1X ?? point.X, point.Control1Y ?? point.Y),
                            new Point(point.Control2X ?? point.X, point.Control2Y ?? point.Y),
                            endPoint,
                            true));
                        break;
                    default:
                        figure.Segments.Add(new LineSegment(endPoint, true));
                        break;
                }
            }

            geometry.Figures.Add(figure);
            return geometry;
        }

        public List<Point> SampleStraightPath(IEnumerable<LocationLayoutPointViewModel> points, double spacing)
        {
            var ordered = points.OrderBy(p => p.PointIndex).ToList();
            var sampled = new List<Point>();

            if (ordered.Count == 0 || spacing <= 0)
                return sampled;

            sampled.Add(new Point(ordered[0].X, ordered[0].Y));

            for (int i = 1; i < ordered.Count; i++)
            {
                var start = sampled[^1];
                var end = new Point(ordered[i].X, ordered[i].Y);
                double dx = end.X - start.X;
                double dy = end.Y - start.Y;
                double length = Math.Sqrt(dx * dx + dy * dy);
                int steps = Math.Max(1, (int)Math.Ceiling(length / spacing));

                for (int step = 1; step <= steps; step++)
                {
                    double t = step / (double)steps;
                    sampled.Add(new Point(start.X + dx * t, start.Y + dy * t));
                }
            }

            return sampled;
        }
    }
}


