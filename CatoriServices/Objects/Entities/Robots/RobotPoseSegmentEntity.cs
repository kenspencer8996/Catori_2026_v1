namespace CatoriServices.Objects.Entities.Robots
{
    public class RobotPoseSegmentEntity
    {
        public long RobotPoseSegmentId { get; set; }
        public long RobotPoseId { get; set; }
        public int SegmentIndex { get; set; }
        public double Angle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
