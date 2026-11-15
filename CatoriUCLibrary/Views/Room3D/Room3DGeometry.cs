using System.Windows.Media;
using System.Windows.Media.Media3D;
using MeshBuilder = HelixToolkit.Geometry.MeshBuilder;
using System.Numerics;

namespace CatoriUCLibrary.Views.Room3D;

public static class Room3DGeometry
{
    public static Model3D Line(Point3D start, Point3D end, bool preview = false)
    {
        var mesh = new MeshBuilder(false, false);
        var lift = new Vector3D(0, 0, 0.04);
        if ((end - start).Length < 0.001)
        {
            return new Model3DGroup();
        }
        mesh.AddCylinder(Vector(start + lift), Vector(end + lift), 0.025f, 10);
        mesh.AddSphere(Vector(start + lift), 0.05f, 10, 8);
        mesh.AddSphere(Vector(end + lift), 0.05f, 10, 8);
        return Model(mesh, preview ? Brushes.DeepSkyBlue : Brushes.DarkOrange);
    }

    public static Model3D Hotspot(Point3D first, Point3D second, bool preview = false)
    {
        var mesh = new MeshBuilder(false, false);
        mesh.AddBox(new Vector3((float)((first.X + second.X) / 2), (float)((first.Y + second.Y) / 2), 0.025f), (float)Math.Max(0.01, Math.Abs(first.X - second.X)), (float)Math.Max(0.01, Math.Abs(first.Y - second.Y)), 0.02f);
        return Model(mesh, new SolidColorBrush(preview ? Color.FromArgb(150, 0, 190, 255) : Color.FromArgb(130, 0, 220, 160)));
    }

    private static Model3D Model(MeshBuilder mesh, Brush brush)
    {
        var material = new DiffuseMaterial(brush);
        var geometry = new MeshGeometry3D { Positions = new Point3DCollection(mesh.Positions.Select(point => new Point3D(point.X, point.Y, point.Z))), TriangleIndices = new Int32Collection(mesh.TriangleIndices) };
        var model = new GeometryModel3D(geometry, material) { BackMaterial = material };
        model.Freeze();
        return model;
    }

    private static Vector3 Vector(Point3D point)
    {
        return new Vector3((float)point.X, (float)point.Y, (float)point.Z);
    }
}
