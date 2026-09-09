namespace CatoriApp.Maintenance.ViewModels.Production
{
    public class FactoryCapabilityViewModel : ViewmodelBase
    {
        private long _factoryCapabilityId;
        private long _locationId;
        private string _capabilityType = "";
        private string? _capabilityName;
        private bool _isEnabled = true;

        public long FactoryCapabilityId { get => _factoryCapabilityId; set => SetProperty(ref _factoryCapabilityId, value); }
        public long LocationId { get => _locationId; set => SetProperty(ref _locationId, value); }
        public string CapabilityType { get => _capabilityType; set => SetProperty(ref _capabilityType, value ?? ""); }
        public string? CapabilityName { get => _capabilityName; set => SetProperty(ref _capabilityName, value); }
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }
    }
}