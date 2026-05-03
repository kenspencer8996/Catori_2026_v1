namespace CatoriServices.Objects.Entities
{
    public class FactoryAssemblyRobotEntity
    {
        public long FactoryAssemblyRobotId { get; set; }
        public long FactoryLayoutId { get; set; }
        public long FactoryLayoutItemId { get; set; }
        public string Name { get; set; } = "";
        public string RobotType { get; set; } = "Arm";
        public long Xloc { get; set; }
        public long Yloc { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RotationDegrees { get; set; }
    }
}
