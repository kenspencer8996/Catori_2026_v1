using CatoriServices.Objects.Entities;
namespace CatoriCity2025WPF.Objects.Services
{
    public class PersonProductsOwnedService
    {
        private readonly PersonProductsOwnedRepository _repository;

        public PersonProductsOwnedService()
        {
            _repository = new PersonProductsOwnedRepository();
        }
        /// <summary>
        /// Returns all PersonProductsOwned records as ViewModels.
        /// </summary>
        public async Task<List<PersonProductsOwnedViewModel>> GetByPersonIdWithShopItemDetailsAsync(int personId)
        {
            ShopItemService shopItemService = new ShopItemService();    
            List<PersonProductsOwnedViewModel> results = new List<PersonProductsOwnedViewModel>();
            try
            {
                var shopItems = await shopItemService.GetAllAsync().ConfigureAwait(false);
                var entities = await _repository.GetByPersonIdWithShopItemDetailsAsync(personId).ConfigureAwait(false);
                foreach(var entity in entities)
                {
                    var product = shopItems.Where(si => si.ShopItemId == entity.ShopItemId).FirstOrDefault();  
                    PersonProductsOwnedViewModel model = new PersonProductsOwnedViewModel();
                    model.ToModel(entity);
                    model.ImageName = product.ImageName;
                    results.Add(model);
                }
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Returns all PersonProductsOwned records as ViewModels.
        /// </summary>
        public async Task<List<PersonProductsOwnedViewModel>> GetAllAsync()
        {
            List<PersonProductsOwnedViewModel> results = new List<PersonProductsOwnedViewModel>();
            try
            {
                var entities = await _repository.GetAllAsync().ConfigureAwait(false);
                foreach (var entity in entities)
                {
                    PersonProductsOwnedViewModel model = new PersonProductsOwnedViewModel();
                    model.ToModel(entity);
                    results.Add(model);
                }
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Returns all PersonProductsOwned records as ViewModels.
        /// </summary>
        public async Task<List<PersonProductsOwnedViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetAllAsync().ConfigureAwait(false);
                return ConvertToViewModels(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns all products owned by a specific person.
        /// </summary>
        public async Task<List<PersonProductsOwnedViewModel>> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetByPersonIdAsync(personId).ConfigureAwait(false);
                return ConvertToViewModels(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a single PersonProductsOwned record by id.
        /// </summary>
        public async Task<PersonProductsOwnedViewModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id).ConfigureAwait(false);
                return ConvertToViewModel(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a specific product owned by a person.
        /// </summary>
        public async Task<PersonProductsOwnedViewModel> GetByPersonAndShopItemAsync(int personId, int shopItemId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetByPersonAndShopItemAsync(personId, shopItemId).ConfigureAwait(false);
                return ConvertToViewModel(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Upserts a PersonProductsOwned record.
        /// </summary>
        public Task UpsertAsync(PersonProductsOwnedViewModel viewModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = viewModel.GetEntity();
                _repository.Upsert(entity);
            }
            catch (Exception)
            {
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Upserts a PersonProductsOwned entity.
        /// </summary>
        public Task UpsertAsync(PersonProductsOwnedEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _repository.Upsert(entity);
            }
            catch (Exception)
            {
                throw;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a PersonProductsOwned record by id.
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _repository.DeleteAsync(id).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes a specific product for a person.
        /// </summary>
        public async Task DeleteByPersonAndShopItemAsync(int personId, int shopItemId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _repository.DeleteByPersonAndShopItemAsync(personId, shopItemId).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates the quantity for a specific product owned by a person.
        /// </summary>
        public async Task UpdateQuantityAsync(int personId, int shopItemId, int newQuantity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _repository.UpdateQuantityAsync(personId, shopItemId, newQuantity).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds quantity to an existing product or creates a new record if it doesn't exist.
        /// </summary>
        public async Task AddQuantityAsync(int personId, int shopItemId, int quantityToAdd, CancellationToken cancellationToken = default)
        {
            try
            {
                await _repository.AddQuantityAsync(personId, shopItemId, quantityToAdd).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Converts an entity to a ViewModel.
        /// </summary>
        private PersonProductsOwnedViewModel ConvertToViewModel(PersonProductsOwnedEntity entity)
        {
            if (entity == null || entity.PersonProductsOwnedId == 0)
                return new PersonProductsOwnedViewModel();

            PersonProductsOwnedViewModel viewModel = new PersonProductsOwnedViewModel();
            viewModel.ToModel(entity);
            return viewModel;
        }

        /// <summary>
        /// Converts a list of entities to ViewModels.
        /// </summary>
        private List<PersonProductsOwnedViewModel> ConvertToViewModels(List<PersonProductsOwnedEntity> entities)
        {
            List<PersonProductsOwnedViewModel> viewModels = new List<PersonProductsOwnedViewModel>();
            foreach (var entity in entities)
            {
                PersonProductsOwnedViewModel viewModel = new PersonProductsOwnedViewModel();
                viewModel.ToModel(entity);
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

    }
}
