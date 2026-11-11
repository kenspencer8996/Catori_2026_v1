using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    // These animatable objects are input channels, not independently applied visual transforms.
    private readonly AxisAngleRotation3D YawRotation = new(new Vector3D(0, 0, 1), 0);
    private readonly AxisAngleRotation3D PitchRotation = new(new Vector3D(0, 1, 0), 0);
    private readonly AxisAngleRotation3D RollRotation = new(new Vector3D(1, 0, 0), 0);
    // Retained only for the legacy preview snapshot plumbing until Phase 4.
    private readonly AxisAngleRotation3D ManualRollRotation = new(new Vector3D(1, 0, 0), 0);
    private readonly AxisAngleRotation3D ManualPitchRotation = new(new Vector3D(0, 1, 0), 0);
    private bool _hasAuthoredAttitude;

    /// <summary>Physical yaw, positive right turn: +X at 0, -Y at 90, -X at 180, +Y at 270.</summary>
    public double Yaw
    {
        get => NormalizeHeading(-YawRotation.Angle);
        set => SetHeading(value);
    }

    /// <summary>Compatibility alias for physical Yaw; this is not a screen bearing.</summary>
    public double Heading
    {
        get => Yaw;
        set => SetHeading(value);
    }

    /// <summary>Positive raises the nose; negative lowers the nose.</summary>
    public double Pitch
    {
        get => -PitchRotation.Angle;
        set => SetPitch(value);
    }

    /// <summary>Positive banks right; negative banks left.</summary>
    public double Roll
    {
        get => RollRotation.Angle;
        set => SetRoll(value);
    }

    private void InitializePhysicalAttitude()
    {
        YawRotation.Changed += PhysicalAttitudeChanged;
        PitchRotation.Changed += PhysicalAttitudeChanged;
        RollRotation.Changed += PhysicalAttitudeChanged;
        ComposePhysicalAttitude();
    }

    private void PhysicalAttitudeChanged(object? sender, EventArgs e)
    {
        ComposePhysicalAttitude();
    }

    private Matrix3D PhysicalOrientationMatrix()
    {
        // Exact physical branch of AirlinerAttitudePrototype.ComposeAttitude.
        var matrix = Matrix3D.Identity;
        matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), Roll));
        matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), -Pitch));
        matrix.Rotate(new Quaternion(new Vector3D(0, 0, -1), Yaw));
        return matrix;
    }

    private void ComposePhysicalAttitude()
    {
        if (PhysicalAttitudeTransform is null)
        {
            return;
        }
        PhysicalAttitudeTransform.Matrix = PhysicalOrientationMatrix();
        UpdateRotationalAxis();
    }

    private void SetAttitudeChannel(AxisAngleRotation3D channel, double target, TimeSpan duration)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(target) || duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(target));
        }
        _hasAuthoredAttitude = true;
        AnimateAdjustment(channel, target, duration);
    }

    private void ApplyAutomaticAttitude(double screenHeading, double? bank = null)
    {
        if (_hasAuthoredAttitude || IsRotationPreviewActive)
        {
            return;
        }
        // Input conversion only: the existing path sampler reports a screen tangent.
        // World mapping is unchanged; physical yaw is the negative of legacy world yaw.
        YawRotation.Angle = SceneProjection.ToYaw(screenHeading);
        if (bank.HasValue)
        {
            RollRotation.Angle = bank.Value;
        }
    }

    private void ResetAutomaticAttitude()
    {
        if (_hasAuthoredAttitude)
        {
            return;
        }
        CancelKeyframeHeading();
        ApplyPlaybackAnimation(RollRotation, AxisAngleRotation3D.AngleProperty, null);
        _hasAuthoredAttitude = false;
        _hasKeyframeHeading = false;
        ApplyPlaybackAnimation(PitchRotation, AxisAngleRotation3D.AngleProperty, null);
        PitchRotation.Angle = 0;
    }
}
