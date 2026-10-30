using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

/// <summary>Only mesh data is built in code. The visual tree, lights, materials and transforms are XAML.</summary>
public sealed partial class AirplaneMeshes
{
    public MeshGeometry3D Fuselage { get; } = Ellipsoid(new Point3D(0, 0, 0), 70, 10.5, 10);
    public MeshGeometry3D Cockpit { get; } = CreateCockpit();
    public MeshGeometry3D PassengerWindows { get; } = CreatePassengerWindows();
    public MeshGeometry3D LeftBrand { get; } = Branding(-1);
    public MeshGeometry3D RightBrand { get; } = Branding(1);
    public MeshGeometry3D Wings { get; } = CreateWings();
    public MeshGeometry3D Tail { get; } = CreateTail();
    public MeshGeometry3D Engines { get; } = CreateEngines();
    public MeshGeometry3D Fin { get; } = Prism(new[] { new Point3D(-64, -1.5, 3), new Point3D(-45, -1.5, 3), new Point3D(-59, -1.5, 25), new Point3D(-67, -1.5, 25) }, new Vector3D(0, 3, 0));
    public MeshGeometry3D Gear { get; } = CreateGear();
    public MeshGeometry3D Wheels { get; } = CreateWheels();
    public MeshGeometry3D Propeller { get; } = CreatePropeller();
    public MeshGeometry3D Spinners { get; } = CreateSpinners();

    private static MeshGeometry3D CreateWings()
    {
        var mesh = new MeshGeometry3D();
        foreach (int side in new[] { -1, 1 })
        {
            Merge(mesh, Prism(new[] { new Point3D(16, side * 6, -1), new Point3D(-6, side * 65, -1), new Point3D(-23, side * 65, -1), new Point3D(-15, side * 6, -1) }, new Vector3D(0, 0, 3.5)));
        }
        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreateTail()
    {
        return Prism(new[] { new Point3D(-51, -25, 2), new Point3D(-51, 25, 2), new Point3D(-66, 27, 2), new Point3D(-68, -27, 2) }, new Vector3D(0, 0, 2));
    }

    private static MeshGeometry3D CreateEngines()
    {
        var mesh = new MeshGeometry3D();
        Merge(mesh, Ellipsoid(new Point3D(12, -31, -2), 19, 6.5, 6.5));
        Merge(mesh, Ellipsoid(new Point3D(12, 31, -2), 19, 6.5, 6.5));
        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreateSpinners()
    {
        var mesh = new MeshGeometry3D();
        Merge(mesh, Ellipsoid(new Point3D(33, -31, -2), 5, 3, 3));
        Merge(mesh, Ellipsoid(new Point3D(33, 31, -2), 5, 3, 3));
        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreatePropeller()
    {
        var mesh = new MeshGeometry3D();
        Merge(mesh, Prism(new[] { new Point3D(-.7, -16, -1.8), new Point3D(-.7, 16, -1.8), new Point3D(-.7, 16, 1.8), new Point3D(-.7, -16, 1.8) }, new Vector3D(1.4, 0, 0)));
        Merge(mesh, Prism(new[] { new Point3D(-.7, -1.8, -16), new Point3D(-.7, 1.8, -16), new Point3D(-.7, 1.8, 16), new Point3D(-.7, -1.8, 16) }, new Vector3D(1.4, 0, 0)));
        mesh.Freeze();
        return mesh;
    }

    public static MeshGeometry3D CreateGearStrut(double x, double y) => Ellipsoid(new Point3D(x, y, -10), 1.2, 1.2, 7);
    public static MeshGeometry3D CreateGearWheel(double x, double y) => Ellipsoid(new Point3D(x, y, -16), 4, 2, 4);

    private static MeshGeometry3D CreateGear()
    {
        var mesh = new MeshGeometry3D();
        foreach (var center in new[] { new Point3D(-7, -18, -10), new Point3D(-7, 18, -10), new Point3D(45, 0, -10) })
        {
            Merge(mesh, Ellipsoid(center, 1.2, 1.2, 7));
        }
        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreateWheels()
    {
        var mesh = new MeshGeometry3D();
        foreach (var center in new[] { new Point3D(-7, -18, -16), new Point3D(-7, 18, -16), new Point3D(45, 0, -16) })
        {
            Merge(mesh, Ellipsoid(center, 4, 2, 4));
        }
        mesh.Freeze();
        return mesh;
    }

    public static MeshGeometry3D Ellipsoid(Point3D center, double rx, double ry, double rz)
    {
        const int rings = 18;
        const int slices = 32;
        var mesh = new MeshGeometry3D();
        for (int ring = 0; ring <= rings; ring++)
        {
            double latitude = Math.PI * ring / rings;
            for (int slice = 0; slice <= slices; slice++)
            {
                double longitude = 2 * Math.PI * slice / slices;
                double x = Math.Sin(latitude) * Math.Cos(longitude);
                double y = Math.Sin(latitude) * Math.Sin(longitude);
                double z = Math.Cos(latitude);
                mesh.Positions.Add(new Point3D(center.X + rx * x, center.Y + ry * y, center.Z + rz * z));
                var normal = new Vector3D(x / rx, y / ry, z / rz);
                normal.Normalize();
                mesh.Normals.Add(normal);
            }
        }
        for (int ring = 0; ring < rings; ring++)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                int a = ring * (slices + 1) + slice;
                int b = a + slices + 1;
                mesh.TriangleIndices.Add(a);
                mesh.TriangleIndices.Add(b);
                mesh.TriangleIndices.Add(a + 1);
                mesh.TriangleIndices.Add(a + 1);
                mesh.TriangleIndices.Add(b);
                mesh.TriangleIndices.Add(b + 1);
            }
        }
        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D Prism(Point3D[] polygon, Vector3D thickness)
    {
        var mesh = new MeshGeometry3D();
        for (int index = 1; index < polygon.Length - 1; index++)
        {
            Triangle(mesh, polygon[0], polygon[index + 1], polygon[index]);
            Triangle(mesh, polygon[0] + thickness, polygon[index] + thickness, polygon[index + 1] + thickness);
        }
        for (int index = 0; index < polygon.Length; index++)
        {
            var a = polygon[index];
            var b = polygon[(index + 1) % polygon.Length];
            Triangle(mesh, a, b, b + thickness);
            Triangle(mesh, a, b + thickness, a + thickness);
        }
        mesh.Freeze();
        return mesh;
    }

    private static void Triangle(MeshGeometry3D mesh, Point3D a, Point3D b, Point3D c)
    {
        int start = mesh.Positions.Count;
        var normal = Vector3D.CrossProduct(b - a, c - a);
        normal.Normalize();
        mesh.Positions.Add(a);
        mesh.Positions.Add(b);
        mesh.Positions.Add(c);
        mesh.Normals.Add(normal);
        mesh.Normals.Add(normal);
        mesh.Normals.Add(normal);
        mesh.TriangleIndices.Add(start);
        mesh.TriangleIndices.Add(start + 1);
        mesh.TriangleIndices.Add(start + 2);
    }

    private static void Merge(MeshGeometry3D target, MeshGeometry3D source)
    {
        int offset = target.Positions.Count;
        foreach (var point in source.Positions)
        {
            target.Positions.Add(point);
        }
        foreach (var normal in source.Normals)
        {
            target.Normals.Add(normal);
        }
        foreach (int index in source.TriangleIndices)
        {
            target.TriangleIndices.Add(index + offset);
        }
    }
}
