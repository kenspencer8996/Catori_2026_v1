using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace CatoriUCLibrary.Views.Room3D;

public static class Room3DFloorProjection
{
    public static Point3D? FloorPoint(Viewport3D viewport, Point point)
    {
        if (point.X < 0 || point.Y < 0 || point.X > viewport.ActualWidth || point.Y > viewport.ActualHeight)
        {
            return null;
        }
        var result = viewport.UnProject(point, new Point3D(), new Vector3D(0, 0, 1));
        if (!result.HasValue || !double.IsFinite(result.Value.X) || !double.IsFinite(result.Value.Y) || viewport.Camera is not ProjectionCamera camera)
        {
            return null;
        }
        var position = camera.Transform.Transform(camera.Position);
        var forward = camera.Transform.Transform(camera.LookDirection);
        forward.Normalize();
        double depth = Vector3D.DotProduct(result.Value - position, forward);
        return double.IsFinite(depth) && depth > camera.NearPlaneDistance && depth < camera.FarPlaneDistance ? new Point3D(result.Value.X, result.Value.Y, 0) : null;
    }
}
