using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public sealed partial class AirplaneMeshes
{
    public static MeshGeometry3D CreateFlap(int side)
    {
        double Rear(double y) => -15 - 8 * (y - 6) / 59;
        return Prism(
            new[]
            {
                new Point3D(Rear(12) + 11, side * 12, 1.3),
                new Point3D(Rear(53) + 11, side * 53, 1.3),
                new Point3D(Rear(53), side * 53, 1.3),
                new Point3D(Rear(12), side * 12, 1.3),
            },
            new Vector3D(0, 0, 1.2)
        );
    }
}
