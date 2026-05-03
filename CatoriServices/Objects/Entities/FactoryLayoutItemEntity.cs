namespace CatoriServices.Objects.Entities
{
    public class FactoryLayoutItemEntity
    {
        public long FactoryLayoutItemId { get; set; }
        public long FactoryLayoutId { get; set; }
        public string ItemName { get; set; } = "";
        public FactoryLayoutItemType ItemType { get; set; } = FactoryLayoutItemType.Conveyor;
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
        public List<FactoryLayoutPointEntity> Points { get; set; } = new();
    }

    public enum FactoryLayoutItemType
    {
        Conveyor,
        Table,
        Robot,
        Workstation,
        Storage,
        Decoration
    }
}
