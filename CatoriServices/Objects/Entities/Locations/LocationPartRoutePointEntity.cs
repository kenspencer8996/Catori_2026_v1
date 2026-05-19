namespace CatoriServices.Objects.Entities.Locations
{
    public class LocationPartRoutePointEntity
    {
        public long LocationPartRoutePointId { get; set; }
        public long LocationPartRouteId { get; set; }
        public int PointIndex { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double SecondsFromStart { get; set; }
    }
}

