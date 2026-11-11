using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private RotationPreviewState? _rotationPreview;
    public bool IsRotationPreviewActive => _rotationPreview is not null;
    public bool HasRotationPreviewChanges => _rotationPreview?.Changed == true
        && ((PhysicalOrientationMatrix().Transform(new Vector3D(1, 0, 0))
            - _rotationPreview.Orientation.Transform(new Vector3D(1, 0, 0))).Length > .0000001
            || (PhysicalOrientationMatrix().Transform(new Vector3D(0, 1, 0))
            - _rotationPreview.Orientation.Transform(new Vector3D(0, 1, 0))).Length > .0000001);
    public event EventHandler? RotationPreviewChanged;
    public double PreviewRoll => Roll;
    public double PreviewPitch => Pitch;
    public double PreviewHeading => Yaw;
    public double CurrentPitch => Pitch;
    public double CurrentRoll => Roll;
    public double CurrentHeading => Yaw;

    private static double NormalizeHeading(double angle)
    {
        return (angle % 360 + 360) % 360;
    }

    private double OuterRotation()
    {
        return (_pathRotation?.Angle ?? 0) + Math.Atan2(RenderTransform.Value.M12, RenderTransform.Value.M11) * 180 / Math.PI;
    }

    private Matrix3D OrientationMatrix()
    {
        return PhysicalOrientationMatrix();
    }

    /// <summary>World-space axis whose positive rotation increases the selected physical channel.</summary>
    public Vector3D GetRotationAxis(RotationAxis axis)
    {
        if (axis == RotationAxis.Heading)
        {
            return new Vector3D(0, 0, -1);
        }
        if (axis == RotationAxis.Pitch)
        {
            var yaw = Matrix3D.Identity;
            yaw.Rotate(new Quaternion(new Vector3D(0, 0, -1), Yaw));
            return yaw.Transform(new Vector3D(0, -1, 0));
        }
        if (axis != RotationAxis.Roll)
        {
            throw new ArgumentOutOfRangeException(nameof(axis));
        }
        return PhysicalOrientationMatrix().Transform(new Vector3D(1, 0, 0));
    }

    public void BeginRotationPreview()
    {
        Dispatcher.VerifyAccess();
        if (!_animationsPaused || IsRotationPreviewActive)
        {
            throw new InvalidOperationException("Pause the airplane before opening the Keyframe Panel.");
        }
        var rotations = new[] { RollRotation, PitchRotation, YawRotation }.Select(rotation =>
        {
            _playbackClocks.TryGetValue((rotation, AxisAngleRotation3D.AngleProperty), out var clock);
            return new RotationClockState
            {
                Rotation = rotation,
                BaseAngle = (double)rotation.GetAnimationBaseValue(AxisAngleRotation3D.AngleProperty),
                CurrentAngle = rotation.Angle,
                ClockTime = clock?.CurrentTime ?? TimeSpan.Zero,
                Clock = clock
            };
        }).ToArray();
        _rotationPreview = new RotationPreviewState
        {
            Rotations = rotations,
            Orientation = PhysicalOrientationMatrix(),
            ShowAxes = ShowRotationalAxis,
            KeyframeHeading = _hasKeyframeHeading,
            AuthoredAttitude = _hasAuthoredAttitude
        };
        foreach (var state in rotations)
        {
            _playbackClocks.Remove((state.Rotation, AxisAngleRotation3D.AngleProperty));
            state.Rotation.ApplyAnimationClock(AxisAngleRotation3D.AngleProperty, null);
            state.Rotation.Angle = state.CurrentAngle;
        }
        ShowRotationalAxis = true;
        RotationalAxisOverlay.IsHitTestVisible = true;
        UpdateRotationalAxis();
    }

    /// <summary>Apply a drag delta to one authoritative physical channel; do not decompose a full pose.</summary>
    public void PreviewBodyRotation(RotationAxis axis, double degrees)
    {
        if (_rotationPreview is null || !double.IsFinite(degrees))
        {
            throw new InvalidOperationException("Open a paused rotation preview first.");
        }
        if (Math.Abs(degrees) < .0000001)
        {
            return;
        }
        switch (axis)
        {
            case RotationAxis.Roll:
                SetRoll(Roll + degrees);
                break;
            case RotationAxis.Pitch:
                SetPitch(Pitch + degrees);
                break;
            case RotationAxis.Heading:
                SetHeading(Yaw + degrees);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis));
        }
        _rotationPreview.Changed = true;
        RotationPreviewChanged?.Invoke(this, EventArgs.Empty);
    }

    public void EndRotationPreview(bool commit)
    {
        var preview = _rotationPreview;
        if (preview is null)
        {
            return;
        }
        ReleaseMouseCapture();
        _dragAxis = null;
        RotationalAxisOverlay.IsHitTestVisible = false;
        if (!commit || !HasRotationPreviewChanges)
        {
            foreach (var state in preview.Rotations)
            {
                state.Rotation.Angle = state.BaseAngle;
                state.Rotation.ApplyAnimationClock(AxisAngleRotation3D.AngleProperty, state.Clock);
                if (state.Clock is not null)
                {
                    state.Clock.Controller!.Pause();
                    state.Clock.Controller.SeekAlignedToLastTick(state.ClockTime, TimeSeekOrigin.BeginTime);
                    _playbackClocks[(state.Rotation, AxisAngleRotation3D.AngleProperty)] = state.Clock;
                }
            }
            _hasKeyframeHeading = preview.KeyframeHeading;
            _hasAuthoredAttitude = preview.AuthoredAttitude;
        }
        else
        {
            foreach (var state in preview.Rotations)
            {
                state.Clock?.Controller?.Remove();
            }
        }
        _rotationPreview = null;
        ShowRotationalAxis = preview.ShowAxes;
        ComposePhysicalAttitude();
        RotationPreviewChanged?.Invoke(this, EventArgs.Empty);
    }
}
