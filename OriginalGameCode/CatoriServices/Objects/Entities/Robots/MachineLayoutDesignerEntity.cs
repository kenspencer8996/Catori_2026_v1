namespace CatoriServices.Objects.Entities.Robots
{
    public class MachineLayoutDesignerEntity
    {
        public long MachineLayoutDesignerId { get; set; }
        public long LocationId { get; set; }
        public double SelectionX { get; set; }
        public double SelectionY { get; set; }
        public double SelectionWidth { get; set; }
        public double SelectionHeight { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

