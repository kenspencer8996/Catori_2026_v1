namespace CatoriServices.Objects.Entities.Robots
{
    public class RobotSequenceEntity
    {
        public long RobotSequenceId { get; set; }
        public long LocationId { get; set; }
        public long MachineInstanceId { get; set; }
        public string SequenceName { get; set; } = "";
        public double RobotX { get; set; } = 300;
        public double RobotY { get; set; } = 200;
        public double RobotWidth { get; set; } = 100;
        public double RobotHeight { get; set; } = 100;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List<RobotPoseEntity> Poses { get; set; } = new();
    }
}
