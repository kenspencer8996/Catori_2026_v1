namespace CatoriCity2025WPF.Objects.Services
{
    public class FactoryService 
    {
        private readonly IFactoryRepository _repository;

        public FactoryService(IFactoryRepository repository)
        {
            _repository = repository;
        }

        public Task<FactoryEntity?> GetFactoryAsync(int id)
            => _repository.GetByIdAsync(id);

        public Task<List<FactoryEntity>> GetFactoriesAsync()
            => _repository.GetAllAsync();

        public Task<List<FactoryEntity>> GetFactoriesByBusinessAsync(int businessId)
            => _repository.GetByBusinessIdAsync(businessId);

        public Task<int> CreateFactoryAsync(FactoryEntity factory)
            => _repository.InsertAsync(factory);

        public Task UpdateFactoryAsync(FactoryEntity factory)
            => _repository.UpdateAsync(factory);

        public Task DeleteFactoryAsync(int id)
            => _repository.DeleteAsync(id);
    }

}
