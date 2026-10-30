using System.Windows;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

/// <summary>Matches the fixed camera in Airplane3DUC.xaml. No 3D authoring coordinates are required.</summary>
public static class SceneProjection
{
    public const double Width = 1200;
    public const double Height = 680;
    public const double GroundCompression = .8;

    public static Point3D ToWorld(Point point)
    {
        return new Point3D(point.X - Width / 2, (Height / 2 - point.Y) / GroundCompression, 0);
    }

    public static Point ToScreen(Point3D point)
    {
        return new Point(point.X + Width / 2, Height / 2 - GroundCompression * point.Y - .6 * point.Z);
    }

    public static double ToYaw(double screenHeading)
    {
        double radians = screenHeading * Math.PI / 180;
        return Math.Atan2(-Math.Sin(radians) / GroundCompression, Math.Cos(radians)) * 180 / Math.PI;
    }
}
