using System.Windows;
using System.Windows.Media.Animation;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private TaskCompletionSource? gearCompletion;
    private CancellationTokenRegistration gearCancellation;
    private int gearVersion;
    private static readonly DependencyProperty LandingGearPositionProperty = DependencyProperty.Register(
        nameof(LandingGearPosition), typeof(double), typeof(Airplane3DUC),
        new PropertyMetadata(1d, LandingGearPositionChanged));

    /// <summary>Zero is fully retracted; one is fully lowered.</summary>
    public double LandingGearPosition => (double)GetValue(LandingGearPositionProperty);
    public bool IsLandingGearMoving => gearCompletion != null;
    public bool IsLandingGearLowered => !IsLandingGearMoving && LandingGearPosition >= .999999;
    public event Action? LandingGearChanged;

    public Task SetLandingGearAsync(bool lowered, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        Dispatcher.VerifyAccess();
        double seconds = (duration ?? TimeSpan.FromMilliseconds(900)).TotalSeconds;
        if (!double.IsFinite(seconds) || seconds < 0 || seconds > 30) throw new ArgumentOutOfRangeException(nameof(duration));
        cancellationToken.ThrowIfCancellationRequested();
        if (!lowered && tripCancellation != null) throw new InvalidOperationException("Landing gear must stay lowered during boarding and taxi.");
        CancelLandingGearMotion();
        double target = lowered ? 1 : 0;
        double from = LandingGearPosition;
        if (seconds == 0 || Math.Abs(target - from) < .000001)
        {
            SetValue(LandingGearPositionProperty, target);
            LandingGearChanged?.Invoke();
            return Task.CompletedTask;
        }
        int version = ++gearVersion;
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        gearCompletion = completion;
        gearCancellation = cancellationToken.Register(() => Dispatcher.BeginInvoke(() =>
        {
            if (version == gearVersion) CancelLandingGearMotion();
        }));
        var animation = new DoubleAnimation(from, target, TimeSpan.FromSeconds(seconds))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
            FillBehavior = FillBehavior.HoldEnd
        };
        animation.Completed += (_, _) =>
        {
            if (version != gearVersion) return;
            SetValue(LandingGearPositionProperty, target);
            BeginAnimation(LandingGearPositionProperty, null);
            gearCancellation.Dispose();
            gearCompletion = null;
            LandingGearChanged?.Invoke();
            completion.TrySetResult();
        };
        BeginAnimation(LandingGearPositionProperty, animation, HandoffBehavior.SnapshotAndReplace);
        LandingGearChanged?.Invoke();
        return completion.Task;
    }

    private static void LandingGearPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var plane = (Airplane3DUC)sender;
        double folded = 90 * (1 - plane.LandingGearPosition);
        plane.LeftMainGearRotation.Angle = folded;
        plane.RightMainGearRotation.Angle = -folded;
        plane.NoseGearRotation.Angle = folded;
        plane.LandingGearChanged?.Invoke();
    }

    private void CancelLandingGearMotion()
    {
        gearVersion++;
        double current = LandingGearPosition;
        SetValue(LandingGearPositionProperty, current);
        BeginAnimation(LandingGearPositionProperty, null);
        gearCancellation.Dispose();
        var previous = gearCompletion;
        gearCompletion = null;
        previous?.TrySetCanceled();
        LandingGearChanged?.Invoke();
    }

    private void ResetLandingGear()
    {
        CancelLandingGearMotion();
        SetValue(LandingGearPositionProperty, 1d);
        LandingGearChanged?.Invoke();
    }
}
