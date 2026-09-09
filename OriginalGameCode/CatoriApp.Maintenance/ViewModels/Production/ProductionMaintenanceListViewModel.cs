namespace CatoriApp.Maintenance.ViewModels.Production
{
    public class ProductionMaintenanceListViewModel : ViewmodelBase
    {
        private FactoryCapabilityViewModel? _selectedFactoryCapability;
        private ProductProduceRequirementViewModel? _selectedProductProduceRequirement;
        private LocationUnlockRuleViewModel? _selectedLocationUnlockRule;
        private string _statusMessage = "";

        public ObservableCollection<FactoryCapabilityViewModel> FactoryCapabilities { get; } = new();
        public ObservableCollection<ProductProduceRequirementViewModel> ProductProduceRequirements { get; } = new();
        public ObservableCollection<LocationUnlockRuleViewModel> LocationUnlockRules { get; } = new();

        public FactoryCapabilityViewModel? SelectedFactoryCapability { get => _selectedFactoryCapability; set => SetProperty(ref _selectedFactoryCapability, value); }
        public ProductProduceRequirementViewModel? SelectedProductProduceRequirement { get => _selectedProductProduceRequirement; set => SetProperty(ref _selectedProductProduceRequirement, value); }
        public LocationUnlockRuleViewModel? SelectedLocationUnlockRule { get => _selectedLocationUnlockRule; set => SetProperty(ref _selectedLocationUnlockRule, value); }
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    }
}