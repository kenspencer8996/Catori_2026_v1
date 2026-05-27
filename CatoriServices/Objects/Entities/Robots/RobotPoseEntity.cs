namespace CatoriServices.Objects.Entities.Robots
{
    public class RobotPoseEntity
    {
        public long RobotPoseId { get; set; }
        public long RobotSequenceId { get; set; }
        public long LocationId { get; set; }
        public int PoseIndex { get; set; }
        public string PoseName { get; set; } = "";
        public double Joint1 { get; set; }
        public double Joint2 { get; set; }
        public double Joint3 { get; set; }
        public double JointEnd { get; set; }
        public int DurationMilliseconds { get; set; } = 600;
        public List<RobotPoseSegmentEntity> Segments { get; set; } = new();
    }
}

