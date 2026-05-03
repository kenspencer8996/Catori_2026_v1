namespace CatoriServices.Objects.Entities
{
    public class FactoryLayoutEntity
    {
        public long FactoryLayoutId { get; set; }
        public long FactoryId { get; set; }
        public string LayoutName { get; set; } = "";
        public double CanvasWidth { get; set; } = 1920;
        public double CanvasHeight { get; set; } = 1080;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<FactoryLayoutItemEntity> Items { get; set; } = new();
    }
}
