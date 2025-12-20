using CatoriCity2025WPF.ViewModels;
using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects.Services
{
    public class DepositService
    {
        private readonly DepositRepository _repository;

        public DepositService()
        {
            _repository = new DepositRepository();
        }
       

        /// <summary>
        /// Async: returns all deposits using async/await.
        /// </summary>
        public async Task<List<DepositViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var depositEntities = await _repository.GetDepositsAsync().ConfigureAwait(false);
                return ConvertToViewModels(depositEntities);
            }
            catch (Exception)
            {
                // preserve original behavior of throwing to caller
                throw;
            }
        }


        /// <summary>
        /// Returns all deposits.
        /// </summary>
        public List<DepositViewModel> GetAll(CancellationToken cancellationToken = default)
        {
            List<DepositViewModel> results = new List<DepositViewModel>();
            try
            {
                var depositsentities = _repository.GetDepositsAsync().Result;
                results = ConvertToViewModels(depositsentities);
            }
            catch (Exception ex)
            {
                throw;
            }
            return results;
            // repository already exposes async method
        }
  
        public List<DepositViewModel> GetDepositsForPersonAsync(int personid)
        {
            List<DepositViewModel> deposits = new List<DepositViewModel>();
            var depositsentities = _repository.GetDepositsForPersonAsync(personid).Result;
            deposits = ConvertToViewModels(depositsentities);
            return deposits;
        }
        
        private DepositViewModel ConvertToViewModel(DepositEntity entity)
        {
            DepositViewModel viewModel = new DepositViewModel();
            viewModel.ToDepositViewModel(entity);
            return viewModel;
        }
        private List<DepositViewModel> ConvertToViewModels(List<DepositEntity> entities)
        {
            List<DepositViewModel> viewModels = new List<DepositViewModel>();
            foreach (var entity in entities)
            {
                DepositViewModel viewModel = new DepositViewModel();
                viewModel.ToDepositViewModel(entity);
                viewModels.Add(viewModel);
            }
            return viewModels;
        }   
        /// <summary>
        /// Returns a single deposit by id.
        /// </summary>
        public DepositViewModel GetByIdAsync(int depositId, CancellationToken cancellationToken = default)
        {
            var resultEntity = _repository.GetDepositByIdAsync(depositId);
            DepositViewModel viewModel = new DepositViewModel();    
            if (resultEntity != null && resultEntity.Result != null)
            {
                 viewModel.ToDepositViewModel(resultEntity.Result);
            }
            return viewModel;
        }

        /// <summary>
        /// Upserts a deposit.
        /// Note: <see cref="DepositRepository.Upsert(DepositEntity)"/> is declared async void in the repo.
        /// This method calls the repository and returns immediately. If you need to wait until DB work
        /// completes, consider updating the repository to return a Task.
        /// </summary>
        public Task UpsertAsync(DepositEntity entity, CancellationToken cancellationToken = default)
        {
            // repository.Upsert is async void; call it and return a completed Task.
            // If the repository is changed to return Task, replace this with 'return _repository.UpsertAsync(entity);'
            _repository.Upsert(entity);
            return Task.CompletedTask;
        }
    }
}
