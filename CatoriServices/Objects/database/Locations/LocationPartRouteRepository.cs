using CatoriServices.Objects.Entities;
namespace CatoriServices.Objects.database.Locations
{
    public class LocationPartRouteRepository
    {
        public Task<LocationPartRouteEntity?> GetByIdAsync(long routeId)
            => Task.FromResult<LocationPartRouteEntity?>(null);

        public Task<List<LocationPartRouteEntity>> GetByLayoutIdAsync(long LocationId)
            => Task.FromResult(new List<LocationPartRouteEntity>());

        public Task<int> InsertAsync(LocationPartRouteEntity route)
            => throw new NotSupportedException("Location part routes are not present in the live database schema.");

        public Task<bool> UpdateAsync(LocationPartRouteEntity route)
            => throw new NotSupportedException("Location part routes are not present in the live database schema.");

        public Task<bool> DeleteAsync(long routeId)
            => Task.FromResult(false);

        public Task<List<LocationPartRoutePointEntity>> GetPointsAsync(long routeId)
            => Task.FromResult(new List<LocationPartRoutePointEntity>());

        public Task<int> InsertPointAsync(LocationPartRoutePointEntity point)
            => throw new NotSupportedException("Location part route points are not present in the live database schema.");

        public Task<bool> ReplacePointsAsync(long routeId, IEnumerable<LocationPartRoutePointEntity> points)
            => throw new NotSupportedException("Location part route points are not present in the live database schema.");
    }
}

