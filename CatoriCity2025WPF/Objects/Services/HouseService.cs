using CityAppServices.Objects.database;

namespace CatoriCity2025WPF.Objects.Services
{
    public class HouseService
    {
        private readonly HouseRepository _repository;

        public HouseService()
        {
            _repository = new HouseRepository();
        }

        /// <summary>
        /// Returns all houses as ViewModels.
        /// </summary>
        public async Task<List<HouseViewModel>> GetHousesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _repository.GetHousesAsync().ConfigureAwait(false);
                return ConvertToViewModels(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns all houses synchronously.
        /// </summary>
        public List<HouseViewModel> GetHouses()
        {
            List<HouseViewModel> results = new List<HouseViewModel>();
            try
            {
                var entities = _repository.GetHousesAsync().Result;
                results = ConvertToViewModels(entities);
            }
            catch (Exception)
            {
                throw;
            }
            return results;
        }

        /// <summary>
        /// Returns a single house by id.
        /// </summary>
        public async Task<HouseViewModel> GetHouseByIdAsync(int houseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetHouseByIdAsync(houseId).ConfigureAwait(false);
                return ConvertToViewModel(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a single house by id synchronously.
        /// </summary>
        public HouseViewModel GetHouseById(int houseId)
        {
            HouseViewModel result = new HouseViewModel();
            try
            {
                var entity = _repository.GetHouseByIdAsync(houseId).Result;
                if (entity != null && entity.HouseId > 0)
                {
                    result = ConvertToViewModel(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Returns a single house by name.
        /// </summary>
        public async Task<HouseViewModel> GetHouseByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _repository.GetHouseByNameAsync(name).ConfigureAwait(false);
                return ConvertToViewModel(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a single house by name synchronously.
        /// </summary>
        public HouseViewModel GetHouseByName(string name)
        {
            HouseViewModel result = new HouseViewModel();
            try
            {
                var entity = _repository.GetHouseByNameAsync(name).Result;
                if (entity != null && entity.HouseId > 0)
                {
                    result = ConvertToViewModel(entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Upserts a house from ViewModel.
        /// </summary>
        public Task UpsertAsync(HouseViewModel viewModel, CancellationToken cancellationToken = default)
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
        /// Upserts a house from ViewModel synchronously.
        /// </summary>
        public void Upsert(HouseViewModel viewModel)
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
        }

        /// <summary>
        /// Upserts a house entity.
        /// </summary>
        public Task UpsertAsync(HouseEntity entity, CancellationToken cancellationToken = default)
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
        /// Deletes a house by id.
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
        /// Deletes a house by id synchronously.
        /// </summary>
        public void Delete(int id)
        {
            try
            {
                _repository.DeleteAsync(id).Wait();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Converts an entity to a ViewModel.
        /// </summary>
        private HouseViewModel ConvertToViewModel(HouseEntity entity)
        {
            if (entity == null || entity.HouseId == 0)
                return new HouseViewModel();

            HouseViewModel viewModel = new HouseViewModel();
            viewModel.ToModel(entity);
            return viewModel;
        }

        /// <summary>
        /// Converts a list of entities to ViewModels.
        /// </summary>
        private List<HouseViewModel> ConvertToViewModels(List<HouseEntity> entities)
        {
            List<HouseViewModel> viewModels = new List<HouseViewModel>();
            foreach (var entity in entities)
            {
                HouseViewModel viewModel = new HouseViewModel();
                viewModel.ToModel(entity);
                viewModels.Add(viewModel);
            }
            return viewModels;
        }
    }
}