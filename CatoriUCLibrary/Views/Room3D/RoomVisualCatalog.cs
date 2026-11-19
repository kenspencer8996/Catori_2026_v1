using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Room3D;

public static class RoomVisualCatalog
{
    public const string RoomType = "CatoriUCLibrary.Controls.Room3D";
    public const string ShelfType = "CatoriUCLibrary.Views.Room3D.Shelf3D";
    public const string SimpleShelfType = "CatoriUCLibrary.Views.Room3D.SimpleShelf3D";
    public static IReadOnlyList<RoomVisualDescriptor> Types { get; } = Array.AsReadOnly(new[]
    {
        new RoomVisualDescriptor("Room3D", RoomType),
        new RoomVisualDescriptor("Shelf3D", ShelfType),
        new RoomVisualDescriptor("SimpleShelf3D", SimpleShelfType)
    });

    public static Model3DGroup Create(string typeName, IEnumerable<RoomShelfContent>? contents = null, RoomConstructionSettings? room = null)
    {
        if (typeName == RoomType)
        {
            room ??= new RoomConstructionSettings();
            var visual = new Controls.Room3D
            {
                Width = room.Width,
                Length = room.Length,
                Height = room.Height,
                ShowNorthWall = room.ShowNorthWall,
                ShowSouthWall = room.ShowSouthWall,
                ShowEastWall = room.ShowEastWall,
                ShowWestWall = room.ShowWestWall,
                Door = room.Door
            };
            return Snapshot(visual);
        }
        Shelf3D shelf = typeName switch
        {
            ShelfType => new Shelf3D(),
            SimpleShelfType => new SimpleShelf3D(),
            _ => throw new ArgumentException("Unknown library 3D object: " + typeName)
        };
        if (contents is not null)
        {
            foreach (var entry in contents)
            {
                shelf.AddItem(new Room3DModelLoader().Load(entry.ModelPath, entry.SourceYUp, 1), entry.Level);
            }
        }
        return Snapshot(shelf);
    }

    private static Model3DGroup Snapshot(ModelVisual3D visual)
    {
        var result = new Model3DGroup { Transform = visual.Transform.CloneCurrentValue() };
        if (visual.Content is not null)
        {
            result.Children.Add(visual.Content.CloneCurrentValue());
        }
        return result;
    }
}
