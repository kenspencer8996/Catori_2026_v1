namespace CatoriServices.Objects.Entities
{
    public class FactoryPartRouteEntity
    {
        public long FactoryPartRouteId { get; set; }
        public long FactoryLayoutId { get; set; }
        public string RouteName { get; set; } = "";
        public int? ProductId { get; set; }
        public long? FromItemId { get; set; }
        public long? ToItemId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<FactoryPartRoutePointEntity> Points { get; set; } = new();
    }
}
