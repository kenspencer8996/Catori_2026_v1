using CatoriCity2025WPF.ViewModels;
using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
using System.Collections.Generic;

namespace CatoriCity2025WPF.Objects.Services
{
    /// <summary>
    /// Thin service that forwards calls to <see cref="BankRepository"/>.
    /// Mirrors pattern used by DepositService.
    /// </summary>
    public class BankService
    {
        private readonly BankRepository _repository;

        public BankService()
        {
            _repository = new BankRepository();
        }

        public List<BankViewModel> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List < BankViewModel > results = new List<BankViewModel>();
            var entities = _repository.GetBanksAsync().Result;
            foreach (var entity in entities)
            {
                BankViewModel viewModel = new BankViewModel();
                viewModel.ToModel(entity);
                results.Add(viewModel);
            }
            return results;
        }

        public BankViewModel GetByIdAsync(int bankId, CancellationToken cancellationToken = default)
        {

            BankViewModel results = new BankViewModel();
            var entitiy = _repository.GetBankByIdAsync(bankId).Result;
            results.ToModel(entitiy);
            
            return results;
        }

        /// <summary>
        /// Calls repository Upsert. Repository declares async void, so this method returns completed Task.
        /// If repository is changed to return Task, update this method to await/return that Task.
        /// </summary>
        public Task UpsertAsync(BankEntity entity, CancellationToken cancellationToken = default)
        {
            _repository.Upsert(entity);
            return Task.CompletedTask;
        }
    }
}
