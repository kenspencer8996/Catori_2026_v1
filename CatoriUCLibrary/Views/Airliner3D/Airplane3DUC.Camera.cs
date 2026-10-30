using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private const double ThreeQuarterElevation = 53.13010235415598;
    private TaskCompletionSource? cameraTransition;
    private int cameraTransitionVersion;
    private static readonly DependencyProperty CameraElevationProperty = DependencyProperty.Register(
        nameof(CameraElevationDegrees), typeof(double), typeof(Airplane3DUC),
        new PropertyMetadata(ThreeQuarterElevation, CameraElevationChanged));

    public double CameraElevationDegrees => (double)GetValue(CameraElevationProperty);
    public AirplaneCameraPreset CurrentCameraPreset { get; private set; } = AirplaneCameraPreset.ThreeQuarter;
    public bool IsCameraTransitioning => cameraTransition != null;

    /// <summary>Orbit the view around the airplane without changing any flight simulation state.</summary>
    public Task TransitionCameraAsync(AirplaneCameraPreset preset, double durationSeconds = 1.1)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(durationSeconds) || durationSeconds < 0 || durationSeconds > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(durationSeconds));
        }
        double target = preset switch
        {
            AirplaneCameraPreset.Side => 0,
            AirplaneCameraPreset.Overhead => 90,
            AirplaneCameraPreset.ThreeQuarter => ThreeQuarterElevation,
            _ => throw new ArgumentOutOfRangeException(nameof(preset))
        };
        double from = CameraElevationDegrees;
        CancelCameraTransition();
        if (durationSeconds == 0 || Math.Abs(target - from) < .000001)
        {
            SetValue(CameraElevationProperty, target);
            CurrentCameraPreset = preset;
            return Task.CompletedTask;
        }
        int version = ++cameraTransitionVersion;
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        cameraTransition = completion;
        var animation = new DoubleAnimation(from, target, TimeSpan.FromSeconds(durationSeconds))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
            FillBehavior = FillBehavior.HoldEnd
        };
        animation.Completed += (_, _) =>
        {
            if (version != cameraTransitionVersion)
            {
                return;
            }
            // Commit the final base value before removing the animation clock.
            SetValue(CameraElevationProperty, target);
            BeginAnimation(CameraElevationProperty, null);
            CurrentCameraPreset = preset;
            cameraTransition = null;
            completion.TrySetResult();
        };
        BeginAnimation(CameraElevationProperty, animation, HandoffBehavior.SnapshotAndReplace);
        return completion.Task;
    }

    private static void CameraElevationChanged(DependencyObject control, DependencyPropertyChangedEventArgs e)
    {
        ((Airplane3DUC)control).UpdateCameraView();
    }

    private void CancelCameraTransition()
    {
        cameraTransitionVersion++;
        double current = CameraElevationDegrees;
        SetValue(CameraElevationProperty, current);
        BeginAnimation(CameraElevationProperty, null);
        cameraTransition?.TrySetCanceled();
        cameraTransition = null;
    }

    private void UpdateCameraView()
    {
        if (CurrentPose == null || FixedCamera == null)
        {
            return;
        }
        double radians = CameraElevationDegrees * Math.PI / 180;
        var look = new Vector3D(0, Math.Cos(radians), -Math.Sin(radians));
        var up = new Vector3D(0, Math.Sin(radians), Math.Cos(radians));
        var right = new Vector3D(1, 0, 0);
        var world = new Point3D(AirplanePosition.OffsetX, AirplanePosition.OffsetY, AirplanePosition.OffsetZ);
        double screenX = CurrentPose.Position.X - SceneProjection.Width / 2;
        double screenY = SceneProjection.Height / 2 - CurrentPose.Position.Y;
        // Translate the camera's target to retain the existing 2D screen anchor at any elevation.
        // Unlike inverse ground-plane projection, this remains valid at a true zero-degree side view.
        Point3D target = world - right * screenX - up * screenY;
        FixedCamera.Position = target - look * 1500;
        FixedCamera.LookDirection = look * 1500;
        FixedCamera.UpDirection = up;
    }
}
