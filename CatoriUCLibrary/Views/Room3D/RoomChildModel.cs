namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomChildModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string ModelPath { get; set; } = "";
    public bool SourceYUp { get; set; } = true;
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Rotation { get; set; }
    public double Scale { get; set; } = 1;
    public Guid? LineId { get; set; }
}
