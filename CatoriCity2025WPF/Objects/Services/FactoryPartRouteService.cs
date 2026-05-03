using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects.Services
{
    public class FactoryPartRouteService
    {
        private readonly FactoryPartRouteRepository _repository;

        public FactoryPartRouteService()
        {
            _repository = new FactoryPartRouteRepository();
        }

        public async Task<List<FactoryPartRouteViewModel>> GetByLayoutIdAsync(long factoryLayoutId)
        {
            var routes = await _repository.GetByLayoutIdAsync(factoryLayoutId);
            var results = new List<FactoryPartRouteViewModel>();

            foreach (var route in routes)
            {
                var vm = ToViewModel(route);
                var points = await _repository.GetPointsAsync(route.FactoryPartRouteId);
                foreach (var point in points)
                    vm.Points.Add(ToViewModel(point));

                results.Add(vm);
            }

            return results;
        }

        public async Task<long> SaveAsync(FactoryPartRouteViewModel vm)
        {
            Validate(vm);

            var entity = ToEntity(vm);
            if (entity.FactoryPartRouteId <= 0)
                vm.FactoryPartRouteId = await _repository.InsertAsync(entity);
            else
                await _repository.UpdateAsync(entity);

            var points = vm.Points
                .OrderBy(p => p.PointIndex)
                .Select(ToEntity)
                .ToList();

            await _repository.ReplacePointsAsync(vm.FactoryPartRouteId, points);
            return vm.FactoryPartRouteId;
        }

        public async Task<bool> DeleteAsync(long routeId)
        {
            if (routeId <= 0)
                return false;

            return await _repository.DeleteAsync(routeId);
        }

        private static void Validate(FactoryPartRouteViewModel vm)
        {
            if (vm.FactoryLayoutId <= 0)
                throw new InvalidOperationException("Factory layout is required.");

            if (string.IsNullOrWhiteSpace(vm.RouteName))
                throw new InvalidOperationException("Route name is required.");
        }

        private static FactoryPartRouteEntity ToEntity(FactoryPartRouteViewModel vm)
        {
            return new FactoryPartRouteEntity
            {
                FactoryPartRouteId = vm.FactoryPartRouteId,
                FactoryLayoutId = vm.FactoryLayoutId,
                RouteName = vm.RouteName.Trim(),
                ProductId = vm.ProductId,
                FromItemId = vm.FromItemId,
                ToItemId = vm.ToItemId,
                IsActive = vm.IsActive,
                CreatedAt = vm.CreatedAt == default ? DateTime.Now : vm.CreatedAt
            };
        }

        private static FactoryPartRoutePointEntity ToEntity(FactoryPartRoutePointViewModel vm)
        {
            return new FactoryPartRoutePointEntity
            {
                FactoryPartRoutePointId = vm.FactoryPartRoutePointId,
                FactoryPartRouteId = vm.FactoryPartRouteId,
                PointIndex = vm.PointIndex,
                X = vm.X,
                Y = vm.Y,
                Z = vm.Z,
                SecondsFromStart = vm.SecondsFromStart
            };
        }

        private static FactoryPartRouteViewModel ToViewModel(FactoryPartRouteEntity entity)
        {
            return new FactoryPartRouteViewModel
            {
                FactoryPartRouteId = entity.FactoryPartRouteId,
                FactoryLayoutId = entity.FactoryLayoutId,
                RouteName = entity.RouteName,
                ProductId = entity.ProductId,
                FromItemId = entity.FromItemId,
                ToItemId = entity.ToItemId,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        private static FactoryPartRoutePointViewModel ToViewModel(FactoryPartRoutePointEntity entity)
        {
            return new FactoryPartRoutePointViewModel
            {
                FactoryPartRoutePointId = entity.FactoryPartRoutePointId,
                FactoryPartRouteId = entity.FactoryPartRouteId,
                PointIndex = entity.PointIndex,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                SecondsFromStart = entity.SecondsFromStart
            };
        }
    }
}
