using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    public double ManualRollDegrees => ManualRollRotation.Angle;
    public double ManualPitchDegrees => -ManualPitchRotation.Angle;

    public void BankRight(double degrees)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualRollRotation, degrees);
    }

    public void BankLeft(double degrees)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualRollRotation, -degrees);
    }

    public void NoseUp(double degrees)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualPitchRotation, -degrees);
    }

    public void NoseDown(double degrees)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualPitchRotation, degrees);
    }

    public void ReturnToAutomaticOrientation()
    {
        Dispatcher.VerifyAccess();
        AnimateAdjustment(ManualRollRotation, 0);
        AnimateAdjustment(ManualPitchRotation, 0);
        AnimatePathOrientation(1, TimeSpan.FromSeconds(.6));
    }

    public void BankRight(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualRollRotation, degrees, duration);
    }

    public void BankLeft(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualRollRotation, -degrees, duration);
    }

    public void NoseUp(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualPitchRotation, -degrees, duration);
    }

    public void NoseDown(double degrees, TimeSpan duration)
    {
        ValidateAdjustment(degrees);
        AnimateAdjustment(ManualPitchRotation, degrees, duration);
    }

    public void ReturnToAutomaticOrientation(TimeSpan duration)
    {
        Dispatcher.VerifyAccess();
        AnimateAdjustment(ManualRollRotation, 0, duration);
        AnimateAdjustment(ManualPitchRotation, 0, duration);
        AnimatePathOrientation(1, duration);
    }

    private void ValidateAdjustment(double degrees)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(degrees) || degrees < 0 || degrees > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(degrees), "Use a nonnegative angle between 0 and 60 degrees.");
        }
    }

    private static void AnimateAdjustment(AxisAngleRotation3D rotation, double target, TimeSpan? duration = null)
    {
        TimeSpan animationDuration = duration ?? TimeSpan.FromSeconds(.6);
        if (!double.IsFinite(animationDuration.TotalSeconds) || animationDuration.TotalSeconds < 0 || animationDuration.TotalSeconds > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        if (animationDuration == TimeSpan.Zero)
        {
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
            rotation.Angle = target;
            return;
        }
        var animation = new DoubleAnimation(rotation.Angle, target, animationDuration)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
            FillBehavior = FillBehavior.HoldEnd
        };
        rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation, HandoffBehavior.SnapshotAndReplace);
    }

    private void ClearManualOrientation()
    {
        ManualRollRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
        ManualPitchRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
        ManualRollRotation.Angle = 0;
        ManualPitchRotation.Angle = 0;
    }
}
