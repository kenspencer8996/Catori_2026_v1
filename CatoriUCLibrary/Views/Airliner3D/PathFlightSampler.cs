using System.Windows;
using System.Windows.Media;

namespace CatoriUCLibrary.Views.Airliner3D;

/// <summary>Samples the original WPF geometry; the flattened copy only measures length.</summary>
public sealed class PathFlightSampler
{
    private readonly PathGeometry geometry;
    public double Length { get; }

    public PathFlightSampler(PathGeometry path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (path.Figures.Count != 1)
        {
            throw new ArgumentException("Use one continuous WPF path figure.", nameof(path));
        }
        geometry = path.CloneCurrentValue();
        geometry.Freeze();
        var flattened = geometry.GetFlattenedPathGeometry(.02, ToleranceType.Absolute);
        double length = 0;
        foreach (var figure in flattened.Figures)
        {
            Point previous = figure.StartPoint;
            foreach (var segment in figure.Segments)
            {
                IEnumerable<Point> points = segment switch
                {
                    PolyLineSegment line => line.Points,
                    LineSegment line => new[] { line.Point },
                    _ => throw new ArgumentException("Could not flatten this WPF path.", nameof(path))
                };
                foreach (var point in points)
                {
                    length += (point - previous).Length;
                    previous = point;
                }
            }
            if (figure.IsClosed)
            {
                length += (figure.StartPoint - previous).Length;
            }
        }
        if (!double.IsFinite(length) || length < 1)
        {
            throw new ArgumentException("The path must have at least one unit of finite length.", nameof(path));
        }
        Length = length;
    }

    private double HeadingAt(double fraction)
    {
        geometry.GetPointAtFractionLength(Math.Clamp(fraction, 0, 1), out _, out var tangent);
        return Math.Atan2(tangent.Y, tangent.X) * 180 / Math.PI;
    }

    public FlightPose Sample(double progress, double speed, double maximumBank)
    {
        if (!double.IsFinite(progress) || !double.IsFinite(speed) || speed <= 0
            || !double.IsFinite(maximumBank) || maximumBank < 0 || maximumBank > 75)
        {
            throw new ArgumentOutOfRangeException(nameof(progress));
        }
        progress = Math.Clamp(progress, 0, 1);
        geometry.GetPointAtFractionLength(progress, out var position, out _);
        // Spatial averaging softens tangent discontinuities without accumulating frame-dependent lag.
        double heading = HeadingAt(progress);
        double sum = heading;
        foreach (double offset in new[] { -6.0, -3.0, 3.0, 6.0 })
        {
            sum += AngleMath.Unwrap(heading, HeadingAt(progress + offset / Length));
        }
        heading = sum / 5;
        double first = Math.Max(0, progress - 24 / Length);
        double last = Math.Min(1, progress + 24 / Length);
        double bend = AngleMath.Delta(HeadingAt(first), HeadingAt(last)) * Math.PI / 180;
        double curvature = bend / Math.Max(1, (last - first) * Length);
        double rawBank = Math.Atan(speed * speed * curvature / 600) * 180 / Math.PI;
        double bank = maximumBank == 0 ? 0 : maximumBank * Math.Tanh(rawBank / maximumBank);
        return new FlightPose(progress, position, heading, bank);
    }
}
