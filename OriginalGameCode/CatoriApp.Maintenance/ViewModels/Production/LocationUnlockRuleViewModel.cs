namespace CatoriApp.Maintenance.ViewModels.Production
{
    public class LocationUnlockRuleViewModel : ViewmodelBase
    {
        private long _locationUnlockRuleId;
        private long _locationId;
        private double _requiredMoney;
        private long? _requiredProductIdLocationId;
        private long _requiredProductCount;
        private string? _unlockDescription;

        public long LocationUnlockRuleId { get => _locationUnlockRuleId; set => SetProperty(ref _locationUnlockRuleId, value); }
        public long LocationId { get => _locationId; set => SetProperty(ref _locationId, value); }
        public double RequiredMoney { get => _requiredMoney; set => SetProperty(ref _requiredMoney, value); }
        public long? RequiredProductIdLocationId { get => _requiredProductIdLocationId; set => SetProperty(ref _requiredProductIdLocationId, value); }
        public long RequiredProductCount { get => _requiredProductCount; set => SetProperty(ref _requiredProductCount, value); }
        public string? UnlockDescription { get => _unlockDescription; set => SetProperty(ref _unlockDescription, value); }
    }
}