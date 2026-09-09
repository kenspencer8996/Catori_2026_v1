namespace CatoriServices.Objects.Entities.Manufacturing
{
    public class MachineEntity
    {
        public int MachineId { get; set; }
        public int MachineTypeId { get; set; }
        public string Name { get; set; } = "";
        public string? ImagePath { get; set; }
        public string? ControlTypeName { get; set; }
        public string? Description { get; set; }
    }
}

