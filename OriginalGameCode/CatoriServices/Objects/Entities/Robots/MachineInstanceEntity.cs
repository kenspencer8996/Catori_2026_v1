namespace CatoriServices.Objects.Entities.Robots
{
    public class MachineInstanceEntity
    {
        public long MachineInstanceId { get; set; }
        public long MachineDefinitionId { get; set; }
        public string InstanceName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public double DefaultScale { get; set; } = 1;
        public double DefaultWidth { get; set; } = 100;
        public double DefaultHeight { get; set; } = 100;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List<MachineInstanceSegmentEntity> Segments { get; set; } = new();
    }
}
