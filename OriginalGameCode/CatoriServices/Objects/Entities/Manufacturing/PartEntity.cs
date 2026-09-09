using System;

namespace CatoriServices.Objects.Entities.Manufacturing
{
    public class PartEntity
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string UnitOfMeasure { get; set; } = "pcs";
        public decimal CostPerUnit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
