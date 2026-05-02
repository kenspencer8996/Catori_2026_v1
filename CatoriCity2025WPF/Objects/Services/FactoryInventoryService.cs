namespace CatoriServices.Services
{
    public class FactoryInventoryService
    {
        private readonly FactoryInventoryRepository _repository;

        public FactoryInventoryService()
        {
            _repository = new FactoryInventoryRepository();
        }

        public async Task<InventoryItemViewModel?> GetByIdAsync(int inventoryId)
        {
            var entity = await _repository.GetByIdAsync(inventoryId);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<InventoryItemViewModel?> GetByProductAndLocationAsync(
            int productId,
            string location)
        {
            if (productId <= 0 || string.IsNullOrWhiteSpace(location))
                return null;

            var entity = await _repository.GetByProductAndLocationAsync(productId, location.Trim());
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<List<InventoryItemViewModel>> GetByProductAsync(int productId)
        {
            var entities = await _repository.GetByProductAsync(productId);

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<List<InventoryItemViewModel>> GetByLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return new List<InventoryItemViewModel>();

            var entities = await _repository.GetByLocationAsync(location.Trim());

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<decimal> GetTotalQuantityForProductAsync(int productId)
        {
            if (productId <= 0)
                return 0;

            return await _repository.GetTotalQuantityForProductAsync(productId);
        }

        public async Task<List<InventoryItemViewModel>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<int> SaveAsync(InventoryItemViewModel vm)
        {
            Validate(vm);

            var entity = ToEntity(vm);
            entity.LastUpdated = DateTime.Now;

            if (entity.InventoryId <= 0)
            {
                int newId = await _repository.InsertAsync(entity);
                vm.InventoryId = newId;
                vm.LastUpdated = entity.LastUpdated;
                return newId;
            }

            bool updated = await _repository.UpdateAsync(entity);

            if (!updated)
                throw new InvalidOperationException($"Inventory item {entity.InventoryId} was not updated.");

            vm.LastUpdated = entity.LastUpdated;
            return entity.InventoryId;
        }

        public async Task<bool> AdjustQuantityAsync(
            int productId,
            string location,
            decimal quantityChange)
        {
            if (productId <= 0)
                throw new InvalidOperationException("Product is required.");

            if (string.IsNullOrWhiteSpace(location))
                throw new InvalidOperationException("Location is required.");

            return await _repository.AdjustQuantityAsync(
                productId,
                location.Trim(),
                quantityChange);
        }

        public async Task<bool> DeleteAsync(InventoryItemViewModel vm)
        {
            if (vm.InventoryId <= 0)
                return false;

            return await _repository.DeleteAsync(vm.InventoryId);
        }

        public async Task<bool> DeleteAsync(int inventoryId)
        {
            if (inventoryId <= 0)
                return false;

            return await _repository.DeleteAsync(inventoryId);
        }

        private static InventoryItemViewModel ToViewModel(InventoryEntity entity)
        {
            return new InventoryItemViewModel
            {
                InventoryId = entity.InventoryId,
                ProductId = entity.ProductId,
                Location = entity.Location,
                QuantityOnHand = entity.QuantityOnHand,
                LastUpdated = entity.LastUpdated
            };
        }

        private static InventoryEntity ToEntity(InventoryItemViewModel vm)
        {
            return new InventoryEntity
            {
                InventoryId = vm.InventoryId,
                ProductId = vm.ProductId,
                Location = vm.Location?.Trim() ?? "",
                QuantityOnHand = vm.QuantityOnHand,
                LastUpdated = vm.LastUpdated == default ? DateTime.Now : vm.LastUpdated
            };
        }

        private static void Validate(InventoryItemViewModel vm)
        {
            if (vm.ProductId <= 0)
                throw new InvalidOperationException("Product is required.");

            if (string.IsNullOrWhiteSpace(vm.Location))
                throw new InvalidOperationException("Location is required.");

            if (vm.QuantityOnHand < 0)
                throw new InvalidOperationException("Quantity on hand cannot be negative.");
        }
    }
}