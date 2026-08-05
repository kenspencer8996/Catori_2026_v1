namespace CatoriServices.Objects.Entities.Robots
{
    public class RobotPoseEntity
    {
        public long RobotPoseId { get; set; }
        public long LocationLayoutItemId { get; set; }
        public string PoseName { get; set; } = "";
        public string Pose { get; set; } = "[]";
    }
}

