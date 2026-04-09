namespace CatoriServices.Objects.Entities
{
    public class FactoryLayoutPointEntity
    {
        public long FactoryLayoutPointId { get; set; }
        public long FactoryLayoutId { get; set; }
        public long PointIndex { get; set; }
        public string PointType { get; set; }   
        public double XLoc { get; set; }
        public double YLoc { get; set; }
        public double XLocEnd { get; set; }
        public double YLocEnd { get; set; }
    }
}
