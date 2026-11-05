using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private RotateTransform? _pathRotation;
    private bool orientationActionsAttached;
    private readonly RotateTransform _pathOrientationCorrection = new();
    private static readonly DependencyProperty PathOrientationBlendProperty =
        DependencyProperty.Register(
            "PathOrientationBlend",
            typeof(double),
            typeof(Airplane3DUC),
            new PropertyMetadata(1d, PathOrientationBlendChanged)
        );

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
            ? transforms.Children.OfType<RotateTransform>().FirstOrDefault()
            : null;
        if (_pathRotation != null)
        {
            RenderTransform = _pathOrientationCorrection;
            _pathRotation.Changed += PathRotationChanged;
            if (!orientationActionsAttached)
            {
                ManualPitchRotation.Changed += PathRotationChanged;
                ManualRollRotation.Changed += PathRotationChanged;
                PitchRotation.Changed += PathRotationChanged;
                RollRotation.Changed += PathRotationChanged;
                orientationActionsAttached = true;
            }
            Unloaded -= DetachPathOrientation;
            Unloaded += DetachPathOrientation;
            UpdatePathOrientation();
        }
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);
        // A same-frame handoff can reparent a loaded actor without raising Loaded again.
        // Reconnect to the new host now, before its first animation frame.
        DetachPathOrientation(this, new RoutedEventArgs());
        if (Parent is ContentControl)
        {
            AttachPathOrientation();
        }
    }

    private void PathOrientationLoaded(object sender, RoutedEventArgs e)
    {
        AttachPathOrientation();
    }

    private static void PathOrientationBlendChanged(
        DependencyObject control,
        DependencyPropertyChangedEventArgs e
    )
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
        if (AlignWithPath)
        {
            // Correct the projected nose, including authored 3D pitch/bank, rather than
            // cancelling the path tangent when WingsLevel is requested.
            var nose =
                AircraftModel.Transform.Transform(new Point3D(1, 0, 0))
                - AircraftModel.Transform.Transform(new Point3D(0, 0, 0));
            var look = FixedCamera.LookDirection;
            look.Normalize();
            var right = Vector3D.CrossProduct(look, FixedCamera.UpDirection);
            right.Normalize();
            var up = Vector3D.CrossProduct(right, look);
            up.Normalize();
            double x = Vector3D.DotProduct(nose, right);
            double y = -Vector3D.DotProduct(nose, up);
            if (x * x + y * y > .000001)
            {
                _pathOrientationCorrection.Angle = -Math.Atan2(y, x) * 180 / Math.PI;
            }
            return;
        }
        // When alignment is disabled, cancel host tilt and keep explicit 3D pitch/bank.
        _pathOrientationCorrection.Angle = -_pathRotation.Angle;
    }

    private void AnimatePathOrientation(double target, TimeSpan duration)
    {
        if (
            !double.IsFinite(duration.TotalSeconds)
            || duration.TotalSeconds < 0
            || duration.TotalSeconds > 30
        )
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        double current = (double)GetValue(PathOrientationBlendProperty);
        BeginAnimation(PathOrientationBlendProperty, null);
        SetValue(PathOrientationBlendProperty, target);
        if (duration > TimeSpan.Zero)
        {
            BeginAnimation(
                PathOrientationBlendProperty,
                new DoubleAnimation(current, target, duration)
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                    FillBehavior = FillBehavior.Stop,
                }
            );
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
        if (orientationActionsAttached)
        {
            ManualPitchRotation.Changed -= PathRotationChanged;
            ManualRollRotation.Changed -= PathRotationChanged;
            PitchRotation.Changed -= PathRotationChanged;
            RollRotation.Changed -= PathRotationChanged;
            orientationActionsAttached = false;
        }
        Unloaded -= DetachPathOrientation;
    }
}
