using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private RotateTransform? _pathRotation;
    private readonly RotateTransform _pathOrientationCorrection = new();
    private static readonly DependencyProperty PathOrientationBlendProperty = DependencyProperty.Register(
        "PathOrientationBlend", typeof(double), typeof(Airplane3DUC),
        new PropertyMetadata(1d, PathOrientationBlendChanged));

    /// <summary>Follow the shared path host while retaining explicit bank/pitch controls.</summary>
    public void AttachPathOrientation()
    {
        Loaded -= PathOrientationLoaded;
        Loaded += PathOrientationLoaded;
        if (_pathRotation != null)
        {
            _pathRotation.Changed -= PathRotationChanged;
        }
        _pathRotation = (Parent as ContentControl)?.RenderTransform is TransformGroup transforms
            ? transforms.Children.OfType<RotateTransform>().FirstOrDefault() : null;
        if (_pathRotation != null)
        {
            RenderTransform = _pathOrientationCorrection;
            _pathRotation.Changed += PathRotationChanged;
            Unloaded -= DetachPathOrientation;
            Unloaded += DetachPathOrientation;
            UpdatePathOrientation();
        }
    }

    private void PathOrientationLoaded(object sender, RoutedEventArgs e)
    {
        AttachPathOrientation();
    }

    private static void PathOrientationBlendChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
    {
        ((Airplane3DUC)control).UpdatePathOrientation();
    }

    private void PathRotationChanged(object? sender, EventArgs e)
    {
        UpdatePathOrientation();
    }

    private void UpdatePathOrientation()
    {
        if (_pathRotation == null)
        {
            return;
        }
        double angle = Math.IEEERemainder(_pathRotation.Angle, 360);
        bool left = Math.Cos(angle * Math.PI / 180) < 0;
        double uprightAngle = angle - (left ? Math.CopySign(180, angle) : 0);
        double blend = (double)GetValue(PathOrientationBlendProperty);
        SetHeading(left ? 180 : 0);
        // At zero blend the complete host tilt is cancelled. Pitch and roll remain 3D actions.
        _pathOrientationCorrection.Angle = -_pathRotation.Angle + uprightAngle * blend;
    }

    private void AnimatePathOrientation(double target, TimeSpan duration)
    {
        if (!double.IsFinite(duration.TotalSeconds) || duration.TotalSeconds < 0 || duration.TotalSeconds > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        double current = (double)GetValue(PathOrientationBlendProperty);
        BeginAnimation(PathOrientationBlendProperty, null);
        SetValue(PathOrientationBlendProperty, target);
        if (duration > TimeSpan.Zero)
        {
            BeginAnimation(PathOrientationBlendProperty, new DoubleAnimation(current, target, duration)
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                FillBehavior = FillBehavior.Stop
            });
        }
        UpdatePathOrientation();
    }

    private void DetachPathOrientation(object sender, RoutedEventArgs e)
    {
        if (_pathRotation != null)
        {
            _pathRotation.Changed -= PathRotationChanged;
            _pathRotation = null;
        }
        Unloaded -= DetachPathOrientation;
    }
}
