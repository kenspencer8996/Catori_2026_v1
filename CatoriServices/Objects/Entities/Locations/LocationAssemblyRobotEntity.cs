namespace CatoriServices.Objects.Entities.Locations
{
    public class LocationAssemblyRobotEntity
    {
        public long LocationAssemblyRobotId { get; set; }
        public long LocationId { get; set; }
        public long LocationLayoutItemId { get; set; }
        public string Name { get; set; } = "";
        public string RobotType { get; set; } = "Arm";
        public long Xloc { get; set; }
        public long Yloc { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RotationDegrees { get; set; }
    }
}

