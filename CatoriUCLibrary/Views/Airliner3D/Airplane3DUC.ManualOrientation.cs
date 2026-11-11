using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    // Compatibility readings now refer to the single authoritative attitude.
    public double ManualRollDegrees => Roll;
    public double ManualPitchDegrees => Pitch;

    public void BankRight(double degrees)
    {
        BankRight(degrees, TimeSpan.FromSeconds(.6));
    }

    public void BankLeft(double degrees)
    {
        BankLeft(degrees, TimeSpan.FromSeconds(.6));
    }

    public void NoseUp(double degrees)
    {
        NoseUp(degrees, TimeSpan.FromSeconds(.6));
    }

    public void NoseDown(double degrees)
    {
        NoseDown(degrees, TimeSpan.FromSeconds(.6));
    }

    public void BankRight(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        SetAttitudeChannel(RollRotation, degrees, duration);
    }

    public void BankLeft(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        SetAttitudeChannel(RollRotation, -degrees, duration);
    }

    public void NoseUp(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        SetAttitudeChannel(PitchRotation, -degrees, duration);
    }

    public void NoseDown(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        SetAttitudeChannel(PitchRotation, degrees, duration);
    }

    public void ReturnToAutomaticOrientation()
    {
        ReturnToAutomaticOrientation(TimeSpan.FromSeconds(.6));
    }

    public void ReturnToAutomaticOrientation(TimeSpan duration)
    {
        Dispatcher.VerifyAccess();
        if (duration < TimeSpan.Zero || duration.TotalSeconds > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        // Relinquish explicit ownership; do not add manual rotations to the pose.
        CancelKeyframeHeading();
        ApplyPlaybackAnimation(RollRotation, AxisAngleRotation3D.AngleProperty, null);
        ApplyPlaybackAnimation(PitchRotation, AxisAngleRotation3D.AngleProperty, null);
        _hasAuthoredAttitude = false;
        _hasKeyframeHeading = false;
        UpdatePathOrientation();
    }

    private void ValidateAdjustment(double degrees)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(degrees) || degrees < 0 || degrees > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(degrees), "Use a nonnegative angle between 0 and 60 degrees.");
        }
    }

    private void AnimateAdjustment(AxisAngleRotation3D rotation, double target, TimeSpan? duration = null)
    {
        TimeSpan animationDuration = duration ?? TimeSpan.FromSeconds(.6);
        if (!double.IsFinite(animationDuration.TotalSeconds) || animationDuration.TotalSeconds < 0 || animationDuration.TotalSeconds > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        double current = rotation.Angle;
        ApplyPlaybackAnimation(rotation, AxisAngleRotation3D.AngleProperty, null);
        rotation.Angle = current;
        if (animationDuration == TimeSpan.Zero)
        {
            rotation.Angle = target;
            return;
        }
        var animation = new DoubleAnimation(current, target, animationDuration)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
            FillBehavior = FillBehavior.HoldEnd
        };
        ApplyPlaybackAnimation(rotation, AxisAngleRotation3D.AngleProperty, animation, HandoffBehavior.SnapshotAndReplace);
    }

    private void ClearManualOrientation()
    {
        // Legacy preview placeholders carry no visual orientation.
        ManualRollRotation.Angle = 0;
        ManualPitchRotation.Angle = 0;
    }
}
