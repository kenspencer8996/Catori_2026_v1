using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public sealed partial class AirplaneMeshes
{
    public MeshGeometry3D Rudder { get; } = CreateRudder();

    private static MeshGeometry3D CreateFin()
    {
        return Prism(
            new[]
            {
                new Point3D(-60.2, -1.5, 3),
                new Point3D(-45, -1.5, 3),
                new Point3D(-59, -1.5, 25),
                new Point3D(-65.4, -1.5, 25),
            },
            new Vector3D(0, 3, 0));
    }

    private static MeshGeometry3D CreateRudder()
    {
        // The aft 20% of each local chord gives exactly 20% of the original fin side area.
        return Prism(
            new[]
            {
                new Point3D(-64, -1.5, 3),
                new Point3D(-60.2, -1.5, 3),
                new Point3D(-65.4, -1.5, 25),
                new Point3D(-67, -1.5, 25),
            },
            new Vector3D(0, 3, 0));
    }
}
