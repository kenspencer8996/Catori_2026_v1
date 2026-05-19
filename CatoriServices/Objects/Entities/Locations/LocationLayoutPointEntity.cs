namespace CatoriServices.Objects.Entities.Locations
{
    public class LocationLayoutPointEntity
    {
        public long LocationLayoutPointId { get; set; }
        public long LocationLayoutItemId { get; set; }
        public long LocationId { get; set; }
        public int PointIndex { get; set; }
        public string? PointRole { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public LocationLayoutSegmentKind SegmentKind { get; set; } = LocationLayoutSegmentKind.Line;
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

    public enum LocationLayoutSegmentKind
    {
        Line,
        QuadraticBezier,
        CubicBezier
    }
}

