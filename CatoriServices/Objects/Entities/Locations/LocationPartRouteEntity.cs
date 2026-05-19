namespace CatoriServices.Objects.Entities.Locations
{
    public class LocationPartRouteEntity
    {
        public long LocationPartRouteId { get; set; }
        public long LocationId { get; set; }
        public string RouteName { get; set; } = "";
        public int? ProductId { get; set; }
        public long? FromItemId { get; set; }
        public long? ToItemId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<LocationPartRoutePointEntity> Points { get; set; } = new();
    }
}

