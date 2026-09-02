using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.VisualParts;

public sealed class VisualPartConfiguration
{
    public required string Name { get; init; }
    public string? ImagePath { get; init; }
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    public double PivotX { get; init; }
    public double PivotY { get; init; }
    public double InitialAngle { get; init; }
    public int ZOrder { get; init; }
    public string? ParentPartName { get; init; }
}

public sealed record VisualPartPose(
    IReadOnlyDictionary<string, double> PartAngles,
    IReadOnlyDictionary<string, Point>? PartPositions = null
);

public sealed record VisualPartAnimationStep(
    VisualPartPose Pose,
    TimeSpan Duration,
    TimeSpan HoldDuration
);

public partial class VisualPartAnimatorUC : UserControl
{
    private sealed record RegisteredPart(
        FrameworkElement Visual,
        FrameworkElement RotationTarget,
        VisualPartConfiguration Configuration,
        double InitialLeft,
        double InitialTop,
        double InitialWidth,
        double InitialHeight
    );

    private readonly Dictionary<string, RegisteredPart> _parts = new(
        StringComparer.OrdinalIgnoreCase
    );

    public VisualPartAnimatorUC()
    {
        InitializeComponent();
    }

    public IReadOnlyCollection<string> PartNames => _parts.Keys;

    public void ClearParts()
    {
        _parts.Clear();
    }

    public void ConfigurePart(
        FrameworkElement visual,
        VisualPartConfiguration configuration,
        FrameworkElement? rotationTarget = null
    )
    {
        ArgumentNullException.ThrowIfNull(visual);
        ArgumentNullException.ThrowIfNull(configuration);
        if (string.IsNullOrWhiteSpace(configuration.Name))
        {
            throw new ArgumentException("A visual part must have a name.", nameof(configuration));
        }
        FrameworkElement target = rotationTarget ?? visual;
        if (visual is Image image)
        {
            image.Source = LoadImage(configuration.ImagePath);
        }
        visual.Width = configuration.Width;
        visual.Height = configuration.Height;
        Canvas.SetLeft(target, configuration.X);
        Canvas.SetTop(target, configuration.Y);
        Panel.SetZIndex(target, configuration.ZOrder);
        // SetRotation already applies the saved pixel pivot to RotateTransform.
        // A non-zero RenderTransformOrigin would apply that pivot a second time and make the part orbit.
        target.RenderTransformOrigin = new Point(0, 0);
        SetRotation(target, configuration.InitialAngle, configuration.PivotX * configuration.Width, configuration.PivotY * configuration.Height);
        _parts[configuration.Name] = new RegisteredPart(visual, target, configuration, configuration.X, configuration.Y, configuration.Width, configuration.Height);
    }

    public bool ContainsPart(string partName)
    {
        return _parts.ContainsKey(partName);
    }

    public FrameworkElement? GetPartVisual(string partName)
    {
        return _parts.TryGetValue(partName, out RegisteredPart? part) ? part.Visual : null;
    }

    public double GetPartAngle(string partName)
    {
        RegisteredPart part = GetRequiredPart(partName);
        return part.RotationTarget.RenderTransform is RotateTransform rotate ? rotate.Angle : 0;
    }

    public double GetPartWorldAngle(string partName)
    {
        RegisteredPart part = GetRequiredPart(partName);
        double parentAngle = string.IsNullOrWhiteSpace(part.Configuration.ParentPartName) ? 0 : GetPartWorldAngle(part.Configuration.ParentPartName);
        return parentAngle + GetPartAngle(partName);
    }

    public void SetPartAngle(string partName, double angle)
    {
        RegisteredPart part = GetRequiredPart(partName);
        SetRotation(part.RotationTarget, angle, part.Configuration.PivotX * part.Visual.Width, part.Configuration.PivotY * part.Visual.Height);
    }

    public void SetPartPosition(string partName, double x, double y)
    {
        RegisteredPart part = GetRequiredPart(partName);
        Canvas.SetLeft(part.RotationTarget, x);
        Canvas.SetTop(part.RotationTarget, y);
    }

    public Point GetPartPosition(string partName)
    {
        RegisteredPart part = GetRequiredPart(partName);
        return new Point(
            NormalizeCanvasCoordinate(Canvas.GetLeft(part.RotationTarget)),
            NormalizeCanvasCoordinate(Canvas.GetTop(part.RotationTarget))
        );
    }

    public void SetPartSize(string partName, double width, double height)
    {
        RegisteredPart part = GetRequiredPart(partName);
        part.Visual.Width = width;
        part.Visual.Height = height;
    }

    public void ApplyPose(VisualPartPose pose)
    {
        ArgumentNullException.ThrowIfNull(pose);
        foreach ((string partName, double angle) in pose.PartAngles)
        {
            SetPartAngle(partName, angle);
        }
        if (pose.PartPositions == null)
        {
            return;
        }
        foreach ((string partName, Point position) in pose.PartPositions)
        {
            SetPartPosition(partName, position.X, position.Y);
        }
    }

    public async Task AnimateToPoseAsync(VisualPartPose pose, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pose);
        Dictionary<string, double> starts = pose.PartAngles.Keys.ToDictionary(partName => partName, GetPartAngle, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, Point> positionStarts = pose.PartPositions?.Keys.ToDictionary(
            partName => partName,
            GetPartPosition,
            StringComparer.OrdinalIgnoreCase
        ) ?? new Dictionary<string, Point>(StringComparer.OrdinalIgnoreCase);
        if (duration <= TimeSpan.Zero)
        {
            ApplyPose(pose);
            return;
        }
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < duration)
        {
            cancellationToken.ThrowIfCancellationRequested();
            double amount = EaseInOut(Math.Clamp(stopwatch.Elapsed.TotalMilliseconds / duration.TotalMilliseconds, 0, 1));
            foreach ((string partName, double targetAngle) in pose.PartAngles)
            {
                SetPartAngle(partName, Interpolate(starts[partName], targetAngle, amount));
            }
            if (pose.PartPositions != null)
            {
                foreach ((string partName, Point targetPosition) in pose.PartPositions)
                {
                    Point startPosition = positionStarts[partName];
                    SetPartPosition(
                        partName,
                        Interpolate(startPosition.X, targetPosition.X, amount),
                        Interpolate(startPosition.Y, targetPosition.Y, amount)
                    );
                }
            }
            await Task.Delay(16, cancellationToken);
        }
        ApplyPose(pose);
    }

    public async Task RunAnimationSequenceAsync(IEnumerable<VisualPartAnimationStep> steps, bool loop = false, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<VisualPartAnimationStep> sequence = steps.ToList();
        if (sequence.Count == 0)
        {
            return;
        }
        do
        {
            foreach (VisualPartAnimationStep step in sequence)
            {
                await AnimateToPoseAsync(step.Pose, step.Duration, cancellationToken);
                if (step.HoldDuration > TimeSpan.Zero)
                {
                    await Task.Delay(step.HoldDuration, cancellationToken);
                }
            }
        } while (loop && !cancellationToken.IsCancellationRequested);
    }

    public void ResetParts()
    {
        foreach ((string name, RegisteredPart part) in _parts)
        {
            part.Visual.Width = part.InitialWidth;
            part.Visual.Height = part.InitialHeight;
            Canvas.SetLeft(part.RotationTarget, part.InitialLeft);
            Canvas.SetTop(part.RotationTarget, part.InitialTop);
            SetPartAngle(name, part.Configuration.InitialAngle);
        }
    }

    private RegisteredPart GetRequiredPart(string partName)
    {
        if (_parts.TryGetValue(partName, out RegisteredPart? part))
        {
            return part;
        }
        throw new KeyNotFoundException($"Visual part '{partName}' is not registered.");
    }

    private static void SetRotation(FrameworkElement element, double angle, double centerX, double centerY)
    {
        if (element.RenderTransform is RotateTransform rotate)
        {
            rotate.Angle = angle;
            rotate.CenterX = centerX;
            rotate.CenterY = centerY;
            return;
        }
        element.RenderTransform = new RotateTransform(angle, centerX, centerY);
    }

    private static double EaseInOut(double amount)
    {
        return amount < .5 ? 2 * amount * amount : 1 - Math.Pow(-2 * amount + 2, 2) / 2;
    }

    private static double Interpolate(double start, double end, double amount)
    {
        return start + (end - start) * amount;
    }

    private static double NormalizeCanvasCoordinate(double value)
    {
        return double.IsNaN(value) ? 0 : value;
    }

    private static BitmapImage? LoadImage(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }
        try
        {
            Uri uri;
            if (Path.IsPathRooted(path) || path.StartsWith("pack:", StringComparison.OrdinalIgnoreCase))
            {
                uri = new Uri(path, UriKind.Absolute);
            }
            else if (path.Contains('/') || path.Contains('\\'))
            {
                uri = new Uri(path, UriKind.RelativeOrAbsolute);
            }
            else
            {
                uri = new Uri($"pack://application:,,,/CatoriUCLibrary;component/Images/{path}");
            }
            BitmapImage image = new();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = uri;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception ex) when (ex is IOException or UriFormatException or NotSupportedException)
        {
            Debug.WriteLine($"Could not load visual part image '{path}': {ex.Message}");
            return null;
        }
    }
}
