namespace CatoriServices.Objects.Entities
{
    public class FactoryPartRoutePointEntity
    {
        public long FactoryPartRoutePointId { get; set; }
        public long FactoryPartRouteId { get; set; }
        public int PointIndex { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double SecondsFromStart { get; set; }
    }
}
