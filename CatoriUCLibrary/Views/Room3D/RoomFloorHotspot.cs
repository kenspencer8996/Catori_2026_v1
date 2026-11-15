namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomFloorHotspot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Depth { get; set; }
    public long TargetSceneId { get; set; }
}
