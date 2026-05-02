public class FactoryLayoutObjectEntity
{
    public long LayoutObjectId { get; set; }
    public long FactoryId { get; set; }

    public string ObjectName { get; set; } = "";
    public string ObjectType { get; set; } = "";

    public string? ImagePath { get; set; }
    public int ZIndex { get; set; }
    public bool IsInteractive { get; set; } = true;
    public bool IsVisible { get; set; } = true;

    public string? Notes { get; set; }

    public List<FactoryLayoutObjectPointEntity> Points { get; set; } = new();
}