namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    /// <summary>Flying-tail deflection relative to the fuselage. Positive raises its trailing edge.</summary>
    public double ElevatorDeflectionDegrees => ElevatorRotation.Angle;

    private void UpdateElevator()
    {
        // The nose points along +X. A positive local +Y rotation raises the aft (-X) edge.
        // Follow the animated physical pitch channel without a second animation clock.
        ElevatorRotation.Angle = Math.Clamp(Pitch, -25, 25);
    }
}
