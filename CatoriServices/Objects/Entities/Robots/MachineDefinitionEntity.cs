namespace CatoriServices.Objects.Entities.Robots
{
    public class MachineDefinitionEntity
    {
        public long MachineDefinitionId { get; set; }
        public string MachineType { get; set; } = "";
        public string MachineName { get; set; } = "";
        public string Description { get; set; } = "";
        public double DefaultWidth { get; set; } = 100;
        public double DefaultHeight { get; set; } = 100;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
