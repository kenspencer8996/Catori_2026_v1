using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;

namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomInteriorNavigation
{
    private readonly HelixViewport3D viewport;
    private readonly DispatcherTimer timer;
    private readonly Stopwatch clock = new();
    private readonly HashSet<Key> pressed = new();
    private ProjectionCamera? previousCamera;
    private bool[] previousSettings = Array.Empty<bool>();
    private Point? lookPoint;
    private double yaw;
    private double pitch;
    public bool IsInside => previousCamera is not null;

    public RoomInteriorNavigation(HelixViewport3D viewport)
    {
        this.viewport = viewport;
        timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        timer.Tick += (_, _) =>
        {
            double elapsed = clock.Elapsed.TotalSeconds;
            clock.Restart();
            Move(Math.Min(elapsed, 0.1));
        };
    }

    public void Enter(RoomChildModel room)
    {
        if (IsInside)
        {
            return;
        }
        if (room.VisualType != RoomVisualCatalog.RoomType || room.Scale <= 0)
        {
            throw new InvalidOperationException("Select a Room3D object first.");
        }
        var settings = room.Room ?? new RoomConstructionSettings();
        viewport.CameraController?.StopSpin();
        previousCamera = viewport.Camera ?? throw new InvalidOperationException("The viewport needs a camera before entering a room.");
        previousSettings = new[] { viewport.IsRotationEnabled, viewport.IsPanEnabled, viewport.IsZoomEnabled, viewport.IsMoveEnabled, viewport.IsInertiaEnabled, viewport.IsChangeFieldOfViewEnabled, viewport.IsTouchZoomEnabled, viewport.ShowViewCube };
        viewport.SetCurrentValue(HelixViewport3D.IsRotationEnabledProperty, false);
        viewport.SetCurrentValue(HelixViewport3D.IsPanEnabledProperty, false);
        viewport.SetCurrentValue(HelixViewport3D.IsZoomEnabledProperty, false);
        viewport.SetCurrentValue(HelixViewport3D.IsMoveEnabledProperty, false);
        viewport.SetCurrentValue(HelixViewport3D.IsInertiaEnabledProperty, false);
        viewport.SetCurrentValue(HelixViewport3D.IsChangeFieldOfViewEnabledProperty, false);
        viewport.SetCurrentValue(HelixViewport3D.IsTouchZoomEnabledProperty, false);
        viewport.SetCurrentValue(HelixViewport3D.ShowViewCubeProperty, false);
        yaw = (room.Rotation + 90) * Math.PI / 180;
        pitch = 0;
        var camera = new PerspectiveCamera
        {
            Position = new Point3D(room.X, room.Y, room.Z + Math.Min(1.6, settings.Height * 0.55) * room.Scale),
            UpDirection = new Vector3D(0, 0, 1),
            FieldOfView = 65,
            NearPlaneDistance = Math.Max(0.001, Math.Min(0.05, room.Scale * 0.02)),
            FarPlaneDistance = Math.Max(500, Math.Max(settings.Width, settings.Length) * room.Scale * 10)
        };
        viewport.Camera = camera;
        UpdateLook();
        viewport.Focus();
        clock.Restart();
        timer.Start();
    }

    public void Exit()
    {
        if (!IsInside)
        {
            return;
        }
        timer.Stop();
        clock.Stop();
        ClearInput();
        viewport.CameraController?.StopSpin();
        viewport.Camera = previousCamera;
        viewport.SetCurrentValue(HelixViewport3D.IsRotationEnabledProperty, previousSettings[0]);
        viewport.SetCurrentValue(HelixViewport3D.IsPanEnabledProperty, previousSettings[1]);
        viewport.SetCurrentValue(HelixViewport3D.IsZoomEnabledProperty, previousSettings[2]);
        viewport.SetCurrentValue(HelixViewport3D.IsMoveEnabledProperty, previousSettings[3]);
        viewport.SetCurrentValue(HelixViewport3D.IsInertiaEnabledProperty, previousSettings[4]);
        viewport.SetCurrentValue(HelixViewport3D.IsChangeFieldOfViewEnabledProperty, previousSettings[5]);
        viewport.SetCurrentValue(HelixViewport3D.IsTouchZoomEnabledProperty, previousSettings[6]);
        viewport.SetCurrentValue(HelixViewport3D.ShowViewCubeProperty, previousSettings[7]);
        previousCamera = null;
    }

    public bool SetKey(Key key, bool down)
    {
        if (!IsInside || key is not (Key.W or Key.A or Key.S or Key.D))
        {
            return false;
        }
        if (down)
        {
            pressed.Add(key);
        }
        else
        {
            pressed.Remove(key);
        }
        return true;
    }

    public void Move(double seconds)
    {
        if (!IsInside || viewport.Camera is null || !double.IsFinite(seconds) || seconds <= 0)
        {
            return;
        }
        var forward = new Vector3D(Math.Cos(yaw), Math.Sin(yaw), 0);
        var right = new Vector3D(Math.Sin(yaw), -Math.Cos(yaw), 0);
        var direction = new Vector3D();
        direction += forward * ((pressed.Contains(Key.W) ? 1 : 0) - (pressed.Contains(Key.S) ? 1 : 0));
        direction += right * ((pressed.Contains(Key.D) ? 1 : 0) - (pressed.Contains(Key.A) ? 1 : 0));
        if (direction.LengthSquared > 0)
        {
            direction.Normalize();
            viewport.Camera.Position += direction * (2.5 * Math.Min(seconds, 0.1));
        }
    }

    public void BeginLook(Point point)
    {
        lookPoint = point;
        viewport.Cursor = Cursors.Cross;
    }

    public void Look(Point point)
    {
        if (!IsInside || !lookPoint.HasValue)
        {
            return;
        }
        yaw -= (point.X - lookPoint.Value.X) * 0.005;
        pitch = Math.Clamp(pitch - (point.Y - lookPoint.Value.Y) * 0.005, -1.5, 1.5);
        lookPoint = point;
        UpdateLook();
    }

    public void EndLook()
    {
        lookPoint = null;
        viewport.Cursor = null;
    }

    public void ClearInput()
    {
        pressed.Clear();
        EndLook();
        viewport.ReleaseMouseCapture();
    }

    private void UpdateLook()
    {
        if (viewport.Camera is not null)
        {
            viewport.Camera.LookDirection = new Vector3D(Math.Cos(yaw) * Math.Cos(pitch), Math.Sin(yaw) * Math.Cos(pitch), Math.Sin(pitch));
        }
    }
}
