namespace CatoriServices.Objects.Entities.Robots
{
    public class MachineInstanceSegmentEntity
    {
        public long MachineInstanceSegmentId { get; set; }
        public long MachineInstanceId { get; set; }
        public int SegmentIndex { get; set; }
        public string SegmentName { get; set; } = "";
        public double Length { get; set; } = 124;
        public double Width { get; set; } = 40;
        public double InitialAngle { get; set; }
        public double MinAngle { get; set; } = -180;
        public double MaxAngle { get; set; } = 180;
        public double Overlap { get; set; } = 16;
        public string Color { get; set; } = "";
        public string ImageName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
