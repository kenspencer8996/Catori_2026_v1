namespace CatoriServices.Objects.Entities.Locations
{
    public class LocationLayoutEntity
    {
        public int LocationId { get; set; }
        public string LayoutName { get; set; } = "";
        public double CanvasWidth { get; set; } = 1920;
        public double CanvasHeight { get; set; } = 1080;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<LocationLayoutItemEntity> Items { get; set; } = new();
    }
}

