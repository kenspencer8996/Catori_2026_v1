using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private void ExecuteAttitudeKeyframe(KeyframeActionDefinition action)
    {
        ValidateAngle(action.Angle);
        if (!double.IsFinite(action.Duration) || action.Duration < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(action.Duration), "Attitude duration must be finite and nonnegative.");
        }
        TimeSpan duration = TimeSpan.FromSeconds(action.Duration);
        AxisAngleRotation3D channel;
        double target;
        switch (action.Action)
        {
            case ActionEnum.SetHeading:
                SetHeading(action.Angle, duration, action.TurnDirection);
                return;
            case ActionEnum.BankLeft:
                channel = RollRotation;
                target = -Math.Abs(action.Angle);
                break;
            case ActionEnum.BankRight:
                channel = RollRotation;
                // Preserve signed legacy values; new positive magnitudes bank right.
                target = action.Angle;
                break;
            case ActionEnum.InvertLeft:
                channel = RollRotation;
                target = -180;
                break;
            case ActionEnum.InvertRight:
                channel = RollRotation;
                target = 180;
                break;
            case ActionEnum.NoseUp:
                channel = PitchRotation;
                // Internal pitch channel is the negative of public physical pitch.
                target = -action.Angle;
                break;
            case ActionEnum.NoseDown:
                channel = PitchRotation;
                target = Math.Abs(action.Angle);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action.Action));
        }
        SetAttitudeChannel(channel, target, duration);
    }
}
