using CatoriServices.Objects.Entities;

namespace CatoriServices.Objects.database
{
    public interface IFactoryRepository
    {
        Task<FactoryEntity?> GetByIdAsync(int factoryId);
        Task<List<FactoryEntity>> GetAllAsync();
        Task<List<FactoryEntity>> GetByBusinessIdAsync(int businessId);

        Task<int> InsertAsync(FactoryEntity factory);
        Task UpdateAsync(FactoryEntity factory);
        Task DeleteAsync(int factoryId);
    }
}
