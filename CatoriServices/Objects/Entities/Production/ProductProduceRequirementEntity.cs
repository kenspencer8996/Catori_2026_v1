namespace CatoriServices.Objects.Entities.Production
{
    public class ProductProduceRequirementEntity
    {
        public long ProductProduceRequirementIdId { get; set; }
        public long ProductId { get; set; }
        public string RequiredCapabilityType { get; set; } = "";
        public string? RequiredCapabilityName { get; set; }
        public long MinimumFactoryLevel { get; set; } = 1;
    }
}