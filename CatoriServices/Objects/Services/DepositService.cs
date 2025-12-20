using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatoriServices.Objects.Services
{
    public class DepositService
    {
        private readonly DepositRepository _repository;

        public DepositService()
        {
            _repository = new DepositRepository();
        }

        /// <summary>
        /// Returns all deposits.
        /// </summary>
        public Task<List<DepositEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // repository already exposes async method
           
            var deposits = _repository.GetDepositsAsync();
            return deposits;
        }

        /// <summary>
        /// Returns a single deposit by id.
        /// </summary>
        public Task<DepositEntity> GetByIdAsync(int depositId, CancellationToken cancellationToken = default)
        {
            return _repository.GetDepositByIdAsync(depositId);
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
