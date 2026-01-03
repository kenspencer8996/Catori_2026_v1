namespace CatoriServices.Objects.Entities
{
    public class ShopItemEntity
    {
        public int ShopItemId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        // Geometry / layout
        public double Height { get; set; }
        public double Width { get; set; }

        // Placement / transform
        public double RotationDegree { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

    }
}
