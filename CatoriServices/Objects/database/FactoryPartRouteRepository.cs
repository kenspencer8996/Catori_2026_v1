using CatoriServices.Objects.Entities;

namespace CatoriServices.Objects.database
{
    public class FactoryPartRouteRepository
    {
        public Task<FactoryPartRouteEntity?> GetByIdAsync(long routeId)
            => Task.FromResult<FactoryPartRouteEntity?>(null);

        public Task<List<FactoryPartRouteEntity>> GetByLayoutIdAsync(long factoryLayoutId)
            => Task.FromResult(new List<FactoryPartRouteEntity>());

        public Task<int> InsertAsync(FactoryPartRouteEntity route)
            => throw new NotSupportedException("Factory part routes are not present in the live database schema.");

        public Task<bool> UpdateAsync(FactoryPartRouteEntity route)
            => throw new NotSupportedException("Factory part routes are not present in the live database schema.");

        public Task<bool> DeleteAsync(long routeId)
            => Task.FromResult(false);

        public Task<List<FactoryPartRoutePointEntity>> GetPointsAsync(long routeId)
            => Task.FromResult(new List<FactoryPartRoutePointEntity>());

        public Task<int> InsertPointAsync(FactoryPartRoutePointEntity point)
            => throw new NotSupportedException("Factory part route points are not present in the live database schema.");

        public Task<bool> ReplacePointsAsync(long routeId, IEnumerable<FactoryPartRoutePointEntity> points)
            => throw new NotSupportedException("Factory part route points are not present in the live database schema.");
    }
}
