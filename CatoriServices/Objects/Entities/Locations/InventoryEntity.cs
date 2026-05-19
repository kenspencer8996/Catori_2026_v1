using System;
namespace CatoriServices.Objects.Entities.Locations
{
    /// <summary>
    /// Represents inventory stock levels for products
    /// </summary>
    public class InventoryEntity
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string Location { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public DateTime LastUpdated { get; set; }

        // Navigation property (optional - for convenience)
        public ProductEntity? Product { get; set; }
    }
}

