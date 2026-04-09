namespace CatoriServices.Objects.Entities
{
    public class FactoryAssemblyRobotEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long FactoryLayoutId { get; set; }
        public string FactoryInteriorName { get; set; } = string.Empty;
        public string RobotType { get; set; } = string.Empty;
        public double Xloc { get; set; }
        public double Yloc { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
