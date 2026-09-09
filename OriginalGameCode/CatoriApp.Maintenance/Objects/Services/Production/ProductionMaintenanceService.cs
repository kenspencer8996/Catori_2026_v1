using CatoriApp.Maintenance.ViewModels.Production;
using CatoriServices.Objects.database.Production;
using CatoriServices.Objects.Entities.Production;

namespace CatoriApp.Maintenance.Objects.Services.Production
{
    public class ProductionMaintenanceService
    {
        private readonly ProductionMaintenanceRepository _repository = new();

        public async Task<List<FactoryCapabilityViewModel>> GetAllFactoryCapabilitiesAsync()
        {
            return (await _repository.GetAllFactoryCapabilitiesAsync()).Select(ToViewModel).ToList();
        }

        public async Task<long> SaveFactoryCapabilityAsync(FactoryCapabilityViewModel vm)
        {
            ValidateFactoryCapability(vm);
            var entity = ToEntity(vm);
            var id = await _repository.SaveFactoryCapabilityAsync(entity);
            vm.FactoryCapabilityId = id;
            return id;
        }

        public async Task<List<ProductProduceRequirementViewModel>> GetAllProductProduceRequirementsAsync()
        {
            return (await _repository.GetAllProductProduceRequirementsAsync()).Select(ToViewModel).ToList();
        }

        public async Task<long> SaveProductProduceRequirementAsync(ProductProduceRequirementViewModel vm)
        {
            ValidateProductProduceRequirement(vm);
            var entity = ToEntity(vm);
            var id = await _repository.SaveProductProduceRequirementAsync(entity);
            vm.ProductProduceRequirementIdId = id;
            return id;
        }

        public async Task<List<LocationUnlockRuleViewModel>> GetAllLocationUnlockRulesAsync()
        {
            return (await _repository.GetAllLocationUnlockRulesAsync()).Select(ToViewModel).ToList();
        }

        public async Task<long> SaveLocationUnlockRuleAsync(LocationUnlockRuleViewModel vm)
        {
            ValidateLocationUnlockRule(vm);
            var entity = ToEntity(vm);
            var id = await _repository.SaveLocationUnlockRuleAsync(entity);
            vm.LocationUnlockRuleId = id;
            return id;
        }

        private static FactoryCapabilityViewModel ToViewModel(FactoryCapabilityEntity entity)
        {
            return new FactoryCapabilityViewModel
            {
                FactoryCapabilityId = entity.FactoryCapabilityId,
                LocationId = entity.LocationId,
                CapabilityType = entity.CapabilityType,
                CapabilityName = entity.CapabilityName,
                IsEnabled = entity.IsEnabled
            };
        }

        private static ProductProduceRequirementViewModel ToViewModel(ProductProduceRequirementEntity entity)
        {
            return new ProductProduceRequirementViewModel
            {
                ProductProduceRequirementIdId = entity.ProductProduceRequirementIdId,
                ProductId = entity.ProductId,
                RequiredCapabilityType = entity.RequiredCapabilityType,
                RequiredCapabilityName = entity.RequiredCapabilityName,
                MinimumFactoryLevel = entity.MinimumFactoryLevel
            };
        }

        private static LocationUnlockRuleViewModel ToViewModel(LocationUnlockRuleEntity entity)
        {
            return new LocationUnlockRuleViewModel
            {
                LocationUnlockRuleId = entity.LocationUnlockRuleId,
                LocationId = entity.LocationId,
                RequiredMoney = entity.RequiredMoney,
                RequiredProductIdLocationId = entity.RequiredProductIdLocationId,
                RequiredProductCount = entity.RequiredProductCount,
                UnlockDescription = entity.UnlockDescription
            };
        }

        private static FactoryCapabilityEntity ToEntity(FactoryCapabilityViewModel vm)
        {
            return new FactoryCapabilityEntity
            {
                FactoryCapabilityId = vm.FactoryCapabilityId,
                LocationId = vm.LocationId,
                CapabilityType = vm.CapabilityType.Trim(),
                CapabilityName = string.IsNullOrWhiteSpace(vm.CapabilityName) ? null : vm.CapabilityName.Trim(),
                IsEnabled = vm.IsEnabled
            };
        }

        private static ProductProduceRequirementEntity ToEntity(ProductProduceRequirementViewModel vm)
        {
            return new ProductProduceRequirementEntity
            {
                ProductProduceRequirementIdId = vm.ProductProduceRequirementIdId,
                ProductId = vm.ProductId,
                RequiredCapabilityType = vm.RequiredCapabilityType.Trim(),
                RequiredCapabilityName = string.IsNullOrWhiteSpace(vm.RequiredCapabilityName) ? null : vm.RequiredCapabilityName.Trim(),
                MinimumFactoryLevel = vm.MinimumFactoryLevel
            };
        }

        private static LocationUnlockRuleEntity ToEntity(LocationUnlockRuleViewModel vm)
        {
            return new LocationUnlockRuleEntity
            {
                LocationUnlockRuleId = vm.LocationUnlockRuleId,
                LocationId = vm.LocationId,
                RequiredMoney = vm.RequiredMoney,
                RequiredProductIdLocationId = vm.RequiredProductIdLocationId,
                RequiredProductCount = vm.RequiredProductCount,
                UnlockDescription = string.IsNullOrWhiteSpace(vm.UnlockDescription) ? null : vm.UnlockDescription.Trim()
            };
        }

        private static void ValidateFactoryCapability(FactoryCapabilityViewModel vm)
        {
            if (vm.LocationId <= 0)
                throw new InvalidOperationException("Location id is required.");
            if (string.IsNullOrWhiteSpace(vm.CapabilityType))
                throw new InvalidOperationException("Capability type is required.");
        }

        private static void ValidateProductProduceRequirement(ProductProduceRequirementViewModel vm)
        {
            if (vm.ProductId <= 0)
                throw new InvalidOperationException("Product id is required.");
            if (string.IsNullOrWhiteSpace(vm.RequiredCapabilityType))
                throw new InvalidOperationException("Required capability type is required.");
            if (vm.MinimumFactoryLevel < 1)
                throw new InvalidOperationException("Minimum factory level must be 1 or greater.");
        }

        private static void ValidateLocationUnlockRule(LocationUnlockRuleViewModel vm)
        {
            if (vm.LocationId <= 0)
                throw new InvalidOperationException("Location id is required.");
            if (vm.RequiredMoney < 0)
                throw new InvalidOperationException("Required money cannot be negative.");
            if (vm.RequiredProductCount < 0)
                throw new InvalidOperationException("Required product count cannot be negative.");
        }
    }
}