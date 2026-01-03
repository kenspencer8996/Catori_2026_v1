using CatoriServices.Data;

namespace CatoriCity2025WPF.Objects.Services
{
    internal class ShopItemService
    {

        private readonly ShopItemRepository _repository;

        public ShopItemService()
        {
            _repository = new ShopItemRepository();
        }

        public async Task<List<ShopItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var results = new List<ShopItemViewModel>();
            var entities = await _repository.GetShopItemsAsync().ConfigureAwait(false);
            if (entities != null)
            {
                foreach (var e in entities)
                {
                    var vm = new ShopItemViewModel();
                    vm.ToModel(e);
                    results.Add(vm);
                }
            }
            return results;
        }

        public async Task<ShopItemViewModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetShopItemByIdAsync(id).ConfigureAwait(false);
            var vm = new ShopItemViewModel();
            if (entity != null)
                vm.ToModel(entity);
            return vm;
        }

        public async Task<ShopItemViewModel> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetShopItemByNameAsync(name).ConfigureAwait(false);
            var vm = new ShopItemViewModel();
            if (entity != null)
                vm.ToModel(entity);
            return vm;
        }

        /// <summary>
        /// Upserts a shop item. Repository.Upsert is fire-and-forget in current pattern,
        /// so this method invokes it and returns a completed Task.
        /// </summary>
        public Task UpsertAsync(ShopItemViewModel viewModel, CancellationToken cancellationToken = default)
        {
            if (viewModel == null) return Task.CompletedTask;
            ShopItemEntity entity = viewModel.GetEntity();
            _repository.Upsert(entity);
            return Task.CompletedTask;
        }
    }
}


