namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomNamedLine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }
    public double Duration { get; set; } = 5;
}
