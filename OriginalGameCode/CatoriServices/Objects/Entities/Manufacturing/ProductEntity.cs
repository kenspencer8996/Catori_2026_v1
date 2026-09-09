using System;
namespace CatoriServices.Objects.Entities.Manufacturing
{
    /// <summary>
    /// Represents a product or component in the manufacturing system
    /// </summary>
    public class ProductEntity
    {
        public int ProductId { get; set; }
        public int PartId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty; // supplied by the linked Part
        public decimal CostPerUnit { get; set; } // supplied by the linked Part
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Product type enumeration
    /// </summary>
    public enum ProductType
    {
        Finished,
        Component
    }
}

