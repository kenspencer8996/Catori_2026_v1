public class FactoryLayoutObjectPointEntity
{
    public long LayoutObjectPointId { get; set; }
    public long LayoutObjectId { get; set; }

    public int PointIndex { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public string? PointRole { get; set; }
}