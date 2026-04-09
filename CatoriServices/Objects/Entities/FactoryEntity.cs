namespace CatoriServices.Objects.Entities
{
    public class FactoryEntity
    {
        public int FactoryId { get; set; }
        public int BusinessId { get; set; }
        public string InteriorName { get; set; } = string.Empty;
        public string? FactoryName { get; set; }
        public string? Description { get; set; }
    }
}
