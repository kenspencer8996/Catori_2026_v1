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
        else if (ReferenceEquals(RenderTransform, _pathOrientationCorrection))
        {
            // A keyframed destination has no host rotation to cancel. Discard
            // the previous path's correction when the actor is handed over.
            _pathOrientationCorrection.Angle = 0;
            UpdateRotationalAxis();
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
        UpdateRotationalAxis();
    }

    private void UpdatePathOrientation()
    {
        if (_rotationPreview is not null || _pathRotation is null || _hasAuthoredAttitude)
        {
            return;
        }
        if (AlignWithPath)
        {
            ApplyAutomaticAttitude(_pathRotation.Angle);
        }
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
        ApplyPlaybackAnimation(this, PathOrientationBlendProperty, null);
        SetValue(PathOrientationBlendProperty, target);
        if (duration > TimeSpan.Zero)
        {
            ApplyPlaybackAnimation(this, 
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
