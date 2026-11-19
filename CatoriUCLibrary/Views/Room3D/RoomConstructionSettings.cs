namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomConstructionSettings
{
    public RoomDoorSettings? Door { get; set; }
    public double Width { get; set; } = 8;
    public double Length { get; set; } = 6;
    public double Height { get; set; } = 3;
    public bool ShowNorthWall { get; set; } = true;
    public bool ShowSouthWall { get; set; } = true;
    public bool ShowEastWall { get; set; } = true;
    public bool ShowWestWall { get; set; } = true;
}
