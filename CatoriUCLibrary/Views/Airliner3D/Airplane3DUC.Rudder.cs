namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    /// <summary>Rudder deflection relative to the fuselage. Negative moves the trailing edge to aircraft left.</summary>
    public double RudderDeflectionDegrees => RudderRotation.Angle;

    private void UpdateRudder()
    {
        // Native +Y is aircraft left. Negative rotation about the upward swept hinge
        // moves the aft edge toward +Y, matching negative (left) physical bank.
        RudderRotation.Angle = Math.Clamp(Roll, -25, 25);
    }
}
