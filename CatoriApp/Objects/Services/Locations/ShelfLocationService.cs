namespace CatoriApp.Objects.Services.Locations
{
    public class ShelfLocationService
    {
        private readonly ShelfLocationRepository _repository;

        public ShelfLocationService()
        {
            _repository = new ShelfLocationRepository();
        }

        /// <summary>
        /// Returns all shelf locations as view models.
        /// </summary>
        public async Task<List<ShelfLocationViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var results = new List<ShelfLocationViewModel>();
            var entities = await _repository.GetShelfLocationsAsync().ConfigureAwait(false);
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    var vm = new ShelfLocationViewModel();
                    vm.ToModel(entity);
                    results.Add(vm);
                }
            }
            return results;
        }

        /// <summary>
        /// Returns single shelf location by id as view model.
        /// </summary>
        public async Task<ShelfLocationViewModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetShelfLocationByIdAsync(id).ConfigureAwait(false);
            var vm = new ShelfLocationViewModel();
            if (entity != null && entity.ShelfLocationID != 0)
                vm.ToModel(entity);
            return vm;
        }

        /// <summary>
        /// Returns shelf locations for a specific shop item as view models.
        /// </summary>
        public async Task<List<ShelfLocationViewModel>> GetByShopItemIdAsync(int shopItemId, CancellationToken cancellationToken = default)
        {
            var results = new List<ShelfLocationViewModel>();
            var entities = await _repository.GetShelfLocationsByShopItemIdAsync(shopItemId).ConfigureAwait(false);
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    var vm = new ShelfLocationViewModel();
                    vm.ToModel(entity);
                    results.Add(vm);
                }
            }
            return results;
        }

        /// <summary>
        /// Returns shelf locations filtered by store type as view models.
        /// </summary>
        public async Task<List<ShelfLocationViewModel>> GetByStoreTypeAsync(string storeType, CancellationToken cancellationToken = default)
        {
            var results = new List<ShelfLocationViewModel>();
            var entities = await _repository.GetShelfLocationsByStoreTypeAsync(storeType).ConfigureAwait(false);
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    var vm = new ShelfLocationViewModel();
                    vm.ToModel(entity);
                    results.Add(vm);
                }
            }
            return results;
        }

        /// <summary>
        /// Upserts a shelf location. Repository.Upsert is async void in current pattern,
        /// so this method calls it and returns a completed Task.
        /// </summary>
        public Task UpsertAsync(ShelfLocationViewModel viewModel, CancellationToken cancellationToken = default)
        {
            if (viewModel == null) return Task.CompletedTask;
            ShelfLocationEntity entity = viewModel.GetEntity();
            _repository.Upsert(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a shelf location by id.
        /// </summary>
        public Task DeleteAsync(int shelfLocationId, CancellationToken cancellationToken = default)
        {
            _repository.Delete(shelfLocationId);
            return Task.CompletedTask;
        }
    }
}


