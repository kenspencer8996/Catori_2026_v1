namespace CatoriServices.Objects.Entities.Locations
{
    public class LocationLayoutMachineEntity
    {
        public int LocationLayoutMachineId { get; set; }
        public int LocationId { get; set; }
        public int MachineId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double Rotation { get; set; }
        public int ZIndex { get; set; } = 100;
        public bool IsEnabled { get; set; } = true;
    }
}

