using System;
namespace CatoriServices.Objects.Entities.Manufacturing
{
    /// <summary>
    /// Represents a Bill of Materials entry defining parent-child product relationships
    /// </summary>
    public class BillOfMaterialsEntity
    {
        public int BomId { get; set; }
        public int ParentProductId { get; set; }
        public int ComponentId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ScrapFactor { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Navigation properties (optional - for convenience)
        public ProductEntity? ParentProduct { get; set; }
   }
}

