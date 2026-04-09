namespace CatoriServices.Objects.Entities
{
    public class FactoryLayoutEntity
    {
        public long FactoryLayoutId { get; set; }
        public long FactoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<FactoryLayoutPointEntity> Points { get; set; } = new List<FactoryLayoutPointEntity>();

    }
}
