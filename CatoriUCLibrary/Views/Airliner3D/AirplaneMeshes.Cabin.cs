using System.Windows;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public sealed partial class AirplaneMeshes
{
    private static MeshGeometry3D CreateCockpit()
    {
        var mesh = new MeshGeometry3D();
        foreach (int side in new[] { -1, 1 })
        {
            Merge(mesh, CabinPatch(48, 61, 6, 62, side));
        }
        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreatePassengerWindows()
    {
        var mesh = new MeshGeometry3D();
        foreach (int side in new[] { -1, 1 })
        {
            foreach (double x in new[] { -43.0, -32, -21, -10, 1, 12, 23, 34 })
            {
                Merge(mesh, CabinPatch(x - 2.8, x + 2.8, 62, 86, side));
            }
        }
        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D Branding(int side)
    {
        // Reverse the longitudinal texture direction on the opposite side: both labels read normally.
        return side < 0 ? CabinPatch(-55, -24, 89, 119, side) : CabinPatch(-24, -55, 89, 119, side);
    }

    private static MeshGeometry3D CabinPatch(double x0, double x1, double phi0, double phi1, int side)
    {
        const int columns = 12;
        const int rows = 5;
        var mesh = new MeshGeometry3D();
        for (int column = 0; column <= columns; column++)
        {
            double u = (double)column / columns;
            double x = x0 + (x1 - x0) * u;
            double radius = Math.Sqrt(1 - x * x / (70 * 70));
            for (int row = 0; row <= rows; row++)
            {
                double v = (double)row / rows;
                double phi = (phi0 + (phi1 - phi0) * v) * Math.PI / 180;
                var point = new Point3D(x * 1.012, side * 10.5 * radius * Math.Sin(phi) * 1.012, 10 * radius * Math.Cos(phi) * 1.012);
                var normal = new Vector3D(point.X / (70 * 70), point.Y / (10.5 * 10.5), point.Z / 100);
                normal.Normalize();
                mesh.Positions.Add(point);
                mesh.Normals.Add(normal);
                mesh.TextureCoordinates.Add(new Point(u, v));
            }
        }
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                int a = column * (rows + 1) + row;
                int b = a + rows + 1;
                CabinTriangle(mesh, a, b, b + 1);
                CabinTriangle(mesh, a, b + 1, a + 1);
            }
        }
        mesh.Freeze();
        return mesh;
    }

    private static void CabinTriangle(MeshGeometry3D mesh, int a, int b, int c)
    {
        var normal = Vector3D.CrossProduct(mesh.Positions[b] - mesh.Positions[a], mesh.Positions[c] - mesh.Positions[a]);
        mesh.TriangleIndices.Add(a);
        if (Vector3D.DotProduct(normal, mesh.Normals[a]) > 0)
        {
            mesh.TriangleIndices.Add(b);
            mesh.TriangleIndices.Add(c);
        }
        else
        {
            mesh.TriangleIndices.Add(c);
            mesh.TriangleIndices.Add(b);
        }
    }
}
