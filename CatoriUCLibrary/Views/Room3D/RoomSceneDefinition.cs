namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomSceneDefinition
{
    public List<RoomChildModel> Models { get; set; } = new();
    public List<RoomNamedLine> Lines { get; set; } = new();
    public List<RoomFloorHotspot> Hotspots { get; set; } = new();
}
