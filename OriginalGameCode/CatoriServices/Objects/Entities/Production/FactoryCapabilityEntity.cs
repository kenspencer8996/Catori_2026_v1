namespace CatoriServices.Objects.Entities.Production
{
    public class FactoryCapabilityEntity
    {
        public long FactoryCapabilityId { get; set; }
        public long LocationId { get; set; }
        public string CapabilityType { get; set; } = "";
        public string? CapabilityName { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}