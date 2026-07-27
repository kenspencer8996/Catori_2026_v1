namespace CatoriServices.Objects.Entities.Robots
{
    public class RobotEntity
    {
        public long RobotId { get; set; }
        public double RobotX { get; set; } = 300;
        public double RobotY { get; set; } = 200;
        public double RobotWidth { get; set; } = 100;
        public double RobotHeight { get; set; } = 100;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public long LocationId { get; set; }
        public List<RobotPoseEntity> Poses { get; set; } = new();
    }
}
