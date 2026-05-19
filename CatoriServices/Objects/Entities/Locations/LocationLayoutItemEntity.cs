namespace CatoriServices.Objects.Entities.Locations
{
    public class LocationLayoutItemEntity
    {
        public long LocationLayoutItemId { get; set; }
        public long LocationId { get; set; }
        public string ItemName { get; set; } = "";
        public LocationLayoutItemType ItemType { get; set; } = LocationLayoutItemType.Conveyor;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RotationDegrees { get; set; }
        public int ZIndex { get; set; }
        public bool IsLocked { get; set; }
        public string? ImagePath { get; set; }
        public string? MetadataJson { get; set; }
        public List<LocationLayoutPointEntity> Points { get; set; } = new();
    }

    public enum LocationLayoutItemType
    {
        Conveyor,
        Table,
        Robot,
        Workstation,
        Storage,
        Decoration
    }
}

