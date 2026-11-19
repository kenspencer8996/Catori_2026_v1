using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Room3D;

internal sealed class ShelfItem
{
    public Model3D Source { get; }
    public Model3DGroup Visual { get; }
    public Rect3D Bounds { get; }
    public int Level { get; }

    public ShelfItem(Model3D source, Model3DGroup visual, Rect3D bounds, int level)
    {
        Source = source;
        Level = level;
        Bounds = bounds;
        Visual = visual;
    }
}
