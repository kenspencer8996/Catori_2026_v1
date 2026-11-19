namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomVisualDescriptor
{
    public string Name { get; }
    public string FullName { get; }

    public RoomVisualDescriptor(string name, string fullName)
    {
        Name = name;
        FullName = fullName;
    }
}
