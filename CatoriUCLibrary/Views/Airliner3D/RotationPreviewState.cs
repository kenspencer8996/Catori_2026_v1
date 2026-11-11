using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

internal sealed class RotationPreviewState
{
    public required RotationClockState[] Rotations { get; init; }
    public bool ShowAxes { get; init; }
    public bool KeyframeHeading { get; init; }
    public bool AuthoredAttitude { get; init; }
    public Matrix3D Orientation { get; init; }
    public bool Changed { get; set; }
}
