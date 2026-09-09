namespace CatoriApp.Game.Objects.Services.Manufacturing
{
    public class ProductService
    {
        private ProductRepository _repository;

        public ProductService()
        {
            _repository = new ProductRepository();
        }

        public async Task<ProductViewModel?> GetByIdAsync(int productId)
        {
            var entity = await _repository.GetByIdAsync(productId);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<ProductViewModel?> GetByCodeAsync(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                return null;

            var entity = await _repository.GetByCodeAsync(productCode.Trim());
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<List<ProductViewModel>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<List<ProductViewModel>> GetByTypeAsync(ProductType productType)
        {
            var entities = await _repository.GetByTypeAsync(productType);

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<int> SaveAsync(ProductViewModel vm)
        {
            Validate(vm);

            var entity = ToEntity(vm);

            if (entity.ProductId <= 0)
            {
                if (entity.CreatedAt == default)
                    entity.CreatedAt = DateTime.Now;

                int newId = await _repository.InsertAsync(entity);
                vm.ProductId = newId;
                return newId;
            }

            bool updated = await _repository.UpdateAsync(entity);

            if (!updated)
                throw new InvalidOperationException($"Product {entity.ProductId} was not updated.");

            return entity.ProductId;
        }

        public async Task<bool> DeleteAsync(ProductViewModel vm)
        {
            if (vm.ProductId <= 0)
                return false;

            return await _repository.DeleteAsync(vm.ProductId);
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            if (productId <= 0)
                return false;

            return await _repository.DeleteAsync(productId);
        }

        private static ProductViewModel ToViewModel(ProductEntity entity)
        {
            return new ProductViewModel
            {
                ProductId = entity.ProductId,
                ProductName = entity.ProductName,
                ProductCode = entity.ProductCode,
                ProductType = entity.ProductType,
                UnitOfMeasure = entity.UnitOfMeasure,
                CostPerUnit = entity.CostPerUnit,
                CreatedAt = entity.CreatedAt
            };
        }

        private static ProductEntity ToEntity(ProductViewModel vm)
        {
            return new ProductEntity
            {
                ProductId = vm.ProductId,
                ProductName = vm.ProductName?.Trim() ?? "",
                ProductCode = vm.ProductCode?.Trim() ?? "",
                ProductType = vm.ProductType,
                UnitOfMeasure = vm.UnitOfMeasure?.Trim() ?? "pcs",
                CostPerUnit = vm.CostPerUnit,
                CreatedAt = vm.CreatedAt == default ? DateTime.Now : vm.CreatedAt
            };
        }

        private static void Validate(ProductViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ProductName))
                throw new InvalidOperationException("Product name is required.");

            if (string.IsNullOrWhiteSpace(vm.ProductCode))
                throw new InvalidOperationException("Product code is required.");

            //if (string.IsNullOrWhiteSpace(vm.ProductType))
            //    throw new InvalidOperationException("Product type is required.");

            //if (!Enum.TryParse<ProductType>(vm.ProductType, out _))
            //    throw new InvalidOperationException("Product type must be Finished or Component.");

            if (string.IsNullOrWhiteSpace(vm.UnitOfMeasure))
                throw new InvalidOperationException("Unit of measure is required.");

            if (vm.CostPerUnit < 0)
                throw new InvalidOperationException("Cost per unit cannot be negative.");
        }
    }
}
