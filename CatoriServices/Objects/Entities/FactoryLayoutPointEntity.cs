namespace CatoriServices.Objects.Entities
{
    public class FactoryLayoutPointEntity
    {
        public long FactoryLayoutPointId { get; set; }
        public long FactoryLayoutItemId { get; set; }
        public long FactoryLayoutId { get; set; }
        public int PointIndex { get; set; }
        public string? PointRole { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public FactoryLayoutSegmentKind SegmentKind { get; set; } = FactoryLayoutSegmentKind.Line;
        public double? Control1X { get; set; }
        public double? Control1Y { get; set; }
        public double? Control2X { get; set; }
        public double? Control2Y { get; set; }
        public double? RotationDegrees { get; set; }

        public double XLoc
        {
            get => X;
            set => X = value;
        }

        public double YLoc
        {
            get => Y;
            set => Y = value;
        }

        public double XLocEnd { get; set; }
        public double YLocEnd { get; set; }

        public string PointType
        {
            get => PointRole ?? "";
            set => PointRole = value;
        }
    }

    public enum FactoryLayoutSegmentKind
    {
        Line,
        QuadraticBezier,
        CubicBezier
    }
}
