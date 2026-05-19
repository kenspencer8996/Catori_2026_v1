namespace CatoriServices.Objects.Entities.Robots
{
    public class MachineLayoutDesignerEntity
    {
        public long MachineLayoutDesignerId { get; set; }
        public long LocationId { get; set; }
        public string SequenceName { get; set; } = "";
        public double SelectionX { get; set; }
        public double SelectionY { get; set; }
        public double SelectionWidth { get; set; }
        public double SelectionHeight { get; set; }
        public double RobotX { get; set; } = 300;
        public double RobotY { get; set; } = 200;
        public double RobotWidth { get; set; } = 100;
        public double RobotHeight { get; set; } = 100;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

