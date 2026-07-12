namespace CatoriApp.Maintenance.ViewModels.Production
{
    public class ProductProduceRequirementViewModel : ViewmodelBase
    {
        private long _productProduceRequirementIdId;
        private long _productId;
        private string _requiredCapabilityType = "";
        private string? _requiredCapabilityName;
        private long _minimumFactoryLevel = 1;

        public long ProductProduceRequirementIdId { get => _productProduceRequirementIdId; set => SetProperty(ref _productProduceRequirementIdId, value); }
        public long ProductId { get => _productId; set => SetProperty(ref _productId, value); }
        public string RequiredCapabilityType { get => _requiredCapabilityType; set => SetProperty(ref _requiredCapabilityType, value ?? ""); }
        public string? RequiredCapabilityName { get => _requiredCapabilityName; set => SetProperty(ref _requiredCapabilityName, value); }
        public long MinimumFactoryLevel { get => _minimumFactoryLevel; set => SetProperty(ref _minimumFactoryLevel, value); }
    }
}