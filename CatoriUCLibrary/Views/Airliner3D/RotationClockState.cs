using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

internal sealed class RotationClockState
{
    public required AxisAngleRotation3D Rotation { get; init; }
    public double BaseAngle { get; init; }
    public double CurrentAngle { get; init; }
    public TimeSpan ClockTime { get; init; }
    public AnimationClock? Clock { get; init; }
}
