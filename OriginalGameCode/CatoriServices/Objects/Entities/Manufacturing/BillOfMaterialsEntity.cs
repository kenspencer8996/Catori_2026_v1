using System;
namespace CatoriServices.Objects.Entities.Manufacturing
{
    /// <summary>
    /// Represents a Bill of Materials entry defining parent-child product relationships
    /// </summary>
    public class BillOfMaterialsEntity
    {
        public int BomId { get; set; }
        public int ParentPartId { get; set; }
        public int ChildPartId { get; set; }
        [Obsolete("Use ParentPartId.")]
        public int ParentProductId { get => ParentPartId; set => ParentPartId = value; }
        [Obsolete("Use ChildPartId.")]
        public int ComponentId { get => ChildPartId; set => ChildPartId = value; }
        public decimal Quantity { get; set; }
        public decimal ScrapFactor { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Navigation properties (optional - for convenience)
        public PartEntity? ParentPart { get; set; }
   }
}

