using System.Windows;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public static class RotationTrackball
{
    public static bool TryProject(Vector displacement, Vector3D axis, Vector3D look, Vector3D cameraUp, out Vector3D vector)
    {
        // Diagnostic arms extend 100–155 local units. This sphere encloses them
        // and follows layout zoom through the captured screen-to-control transform.
        double x = displacement.X / 180;
        double y = -displacement.Y / 180;
        double radius = x * x + y * y;
        double z = radius <= 0.5 ? Math.Sqrt(1 - radius) : 0.5 / Math.Sqrt(radius);
        look.Normalize();
        axis.Normalize();
        var right = Vector3D.CrossProduct(look, cameraUp);
        right.Normalize();
        var up = Vector3D.CrossProduct(right, look);
        up.Normalize();
        vector = right * x + up * y - look * z;
        vector -= axis * Vector3D.DotProduct(vector, axis);
        if (vector.LengthSquared < 0.000001)
        {
            return false;
        }
        vector.Normalize();
        return true;
    }

    public static double Angle(Vector3D previous, Vector3D current, Vector3D axis)
    {
        return Math.Atan2(Vector3D.DotProduct(axis, Vector3D.CrossProduct(previous, current)),
            Vector3D.DotProduct(previous, current)) * 180 / Math.PI;
    }
}
