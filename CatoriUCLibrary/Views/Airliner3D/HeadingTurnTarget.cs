namespace CatoriUCLibrary.Views.Airliner3D;

public static class HeadingTurnTarget
{
    /// <summary>Resolve a physical heading into the continuous internal -Z yaw channel.</summary>
    public static double Resolve(double currentRotationAngle, double heading, HeadingTurnDirection direction)
    {
        if (!double.IsFinite(currentRotationAngle) || !double.IsFinite(heading) || !Enum.IsDefined(direction))
        {
            throw new ArgumentOutOfRangeException(nameof(direction), "Heading and current angle must be finite and turn direction must be defined.");
        }
        if (direction == HeadingTurnDirection.Shortest)
        {
            // Preserve the existing shortest-turn and 180-degree tie behavior exactly.
            return AngleMath.Unwrap(currentRotationAngle, -heading);
        }
        double currentHeading = ((-currentRotationAngle % 360) + 360) % 360;
        double targetHeading = ((heading % 360) + 360) % 360;
        double clockwise = (targetHeading - currentHeading + 360) % 360;
        // An already matching heading stays still, rather than requesting an extra revolution.
        double delta = direction == HeadingTurnDirection.Clockwise || clockwise == 0 ? clockwise : clockwise - 360;
        return currentRotationAngle - delta;
    }
}
