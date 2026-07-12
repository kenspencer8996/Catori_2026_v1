namespace CatoriServices.Objects.Entities.Production
{
    public class LocationUnlockRuleEntity
    {
        public long LocationUnlockRuleId { get; set; }
        public long LocationId { get; set; }
        public double RequiredMoney { get; set; }
        public long? RequiredProductIdLocationId { get; set; }
        public long RequiredProductCount { get; set; }
        public string? UnlockDescription { get; set; }
    }
}