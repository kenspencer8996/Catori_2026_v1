namespace CatoriServices.Objects.Entities
{
    public class FactoryEntity
    {
        public long FactoryId { get; set; }
        public long? BusinessId { get; set; }
        public string FactoryName { get; set; } = "";
        public string? BackgroundImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
