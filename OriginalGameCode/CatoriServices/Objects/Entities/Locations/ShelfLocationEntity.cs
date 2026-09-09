namespace CatoriServices.Objects.Entities.Locations
{
    public class ShelfLocationEntity
    {
        public int ShelfLocationID { get; set; }
        public string StoreType { get; set; } = "";
        public string Aisle { get; set; } = "";
        public string Shelf { get; set; } = "";
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double PositionZ { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int ShopItemId { get; set; } = 0;
    }
}

