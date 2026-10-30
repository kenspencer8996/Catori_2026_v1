using System.Windows;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    /// <summary>Camera-projected aircraft bounds in control coordinates, excluding the transparent viewport.</summary>
    public Rect GetAircraftBounds()
    {
        Dispatcher.VerifyAccess();
        var bounds = AircraftModel.Bounds;
        if (bounds.IsEmpty || ActualWidth <= 0 || ActualHeight <= 0) return Rect.Empty;
        var look = FixedCamera.LookDirection;
        look.Normalize();
        var right = Vector3D.CrossProduct(look, FixedCamera.UpDirection);
        right.Normalize();
        var up = Vector3D.CrossProduct(right, look);
        up.Normalize();
        double scale = ActualWidth / FixedCamera.Width;
        var result = Rect.Empty;
        foreach (double x in new[] { bounds.X, bounds.X + bounds.SizeX })
        foreach (double y in new[] { bounds.Y, bounds.Y + bounds.SizeY })
        foreach (double z in new[] { bounds.Z, bounds.Z + bounds.SizeZ })
        {
            var relative = new Point3D(x, y, z) - FixedCamera.Position;
            result.Union(new Point(ActualWidth / 2 + Vector3D.DotProduct(relative, right) * scale,
                ActualHeight / 2 - Vector3D.DotProduct(relative, up) * scale));
        }
        return result;
    }
}
