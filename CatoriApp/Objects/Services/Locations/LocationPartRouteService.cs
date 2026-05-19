using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
namespace CatoriApp.Objects.Services.Locations
{
    public class LocationPartRouteService
    {
        private readonly LocationPartRouteRepository _repository;

        public LocationPartRouteService()
        {
            _repository = new LocationPartRouteRepository();
        }

        public async Task<List<LocationPartRouteViewModel>> GetByLayoutIdAsync(long LocationId)
        {
            var routes = await _repository.GetByLayoutIdAsync(LocationId);
            var results = new List<LocationPartRouteViewModel>();

            foreach (var route in routes)
            {
                var vm = ToViewModel(route);
                var points = await _repository.GetPointsAsync(route.LocationPartRouteId);
                foreach (var point in points)
                    vm.Points.Add(ToViewModel(point));

                results.Add(vm);
            }

            return results;
        }

        public async Task<long> SaveAsync(LocationPartRouteViewModel vm)
        {
            Validate(vm);

            var entity = ToEntity(vm);
            if (entity.LocationPartRouteId <= 0)
                vm.LocationPartRouteId = await _repository.InsertAsync(entity);
            else
                await _repository.UpdateAsync(entity);

            var points = vm.Points
                .OrderBy(p => p.PointIndex)
                .Select(ToEntity)
                .ToList();

            await _repository.ReplacePointsAsync(vm.LocationPartRouteId, points);
            return vm.LocationPartRouteId;
        }

        public async Task<bool> DeleteAsync(long routeId)
        {
            if (routeId <= 0)
                return false;

            return await _repository.DeleteAsync(routeId);
        }

        private static void Validate(LocationPartRouteViewModel vm)
        {
            if (vm.LocationId <= 0)
                throw new InvalidOperationException("Location layout is required.");

            if (string.IsNullOrWhiteSpace(vm.RouteName))
                throw new InvalidOperationException("Route name is required.");
        }

        private static LocationPartRouteEntity ToEntity(LocationPartRouteViewModel vm)
        {
            return new LocationPartRouteEntity
            {
                LocationPartRouteId = vm.LocationPartRouteId,
                LocationId = vm.LocationId,
                RouteName = vm.RouteName.Trim(),
                ProductId = vm.ProductId,
                FromItemId = vm.FromItemId,
                ToItemId = vm.ToItemId,
                IsActive = vm.IsActive,
                CreatedAt = vm.CreatedAt == default ? DateTime.Now : vm.CreatedAt
            };
        }

        private static LocationPartRoutePointEntity ToEntity(LocationPartRoutePointViewModel vm)
        {
            return new LocationPartRoutePointEntity
            {
                LocationPartRoutePointId = vm.LocationPartRoutePointId,
                LocationPartRouteId = vm.LocationPartRouteId,
                PointIndex = vm.PointIndex,
                X = vm.X,
                Y = vm.Y,
                Z = vm.Z,
                SecondsFromStart = vm.SecondsFromStart
            };
        }

        private static LocationPartRouteViewModel ToViewModel(LocationPartRouteEntity entity)
        {
            return new LocationPartRouteViewModel
            {
                LocationPartRouteId = entity.LocationPartRouteId,
                LocationId = entity.LocationId,
                RouteName = entity.RouteName,
                ProductId = entity.ProductId,
                FromItemId = entity.FromItemId,
                ToItemId = entity.ToItemId,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        private static LocationPartRoutePointViewModel ToViewModel(LocationPartRoutePointEntity entity)
        {
            return new LocationPartRoutePointViewModel
            {
                LocationPartRoutePointId = entity.LocationPartRoutePointId,
                LocationPartRouteId = entity.LocationPartRouteId,
                PointIndex = entity.PointIndex,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                SecondsFromStart = entity.SecondsFromStart
            };
        }
    }
}


