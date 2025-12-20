using CatoriCity2025WPF.ViewModels;
using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects.Services
{
    /// <summary>
    /// Thin service forwarding calls to <see cref="BankCustomerFundsRepository"/>.
    /// Mirrors existing service pattern (DepositService / BankService).
    /// </summary>
    public class BankCustomerFundsService
    {
        private readonly BankCustomerFundsRepository _repository;

        public BankCustomerFundsService()
        {
            _repository = new BankCustomerFundsRepository();
        }

        public List<BankCustomerFundsViewModel> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List < BankCustomerFundsViewModel > results = new List<BankCustomerFundsViewModel>();
            var entities = _repository.GetBankCustomerFundsAsync().Result;
            foreach (var item in entities)
            {
                BankCustomerFundsViewModel model = new BankCustomerFundsViewModel();
                model.ToModel(item);
                results.Add(model);
            }
            return results;
        }

        public BankCustomerFundsViewModel GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BankCustomerFundsViewModel result = new BankCustomerFundsViewModel();
            var entity = _repository.GetBankCustomerFundsByIdAsync(id).Result;
            result.ToModel(entity);
           
            return result;
        }

        /// <summary>
        /// Calls repository Upsert. Repository.Upsert is async void in existing pattern,
        /// so this method calls it and returns a completed Task. If repository is changed
        /// to return Task, update this method to await/return that Task.
        /// </summary>
        public Task UpsertAsync(BankCustomerFundsEntity entity, CancellationToken cancellationToken = default)
        {
            _repository.Upsert(entity);
            return Task.CompletedTask;
        }
    }
}
