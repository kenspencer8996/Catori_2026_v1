namespace CatoriApp.Game.Objects.Services.Locations
{
    public class LocationLayoutMachineService
    {
        private readonly LocationLayoutMachineRepository _repository;

        public LocationLayoutMachineService()
        {
            _repository = new LocationLayoutMachineRepository();
        }

        public async Task<LocationLayoutMachineViewModel?> GetByIdAsync(int locationLayoutMachineId)
        {
            var entity = await _repository.GetByIdAsync(locationLayoutMachineId);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<List<LocationLayoutMachineViewModel>> GetByLocationIdAsync(int locationId)
        {
            var entities = await _repository.GetByLocationIdAsync(locationId);

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<List<LocationLayoutMachineViewModel>> GetByMachineIdAsync(int machineId)
        {
            var entities = await _repository.GetByMachineIdAsync(machineId);

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<int> SaveAsync(LocationLayoutMachineViewModel vm)
        {
            Validate(vm);

            var entity = ToEntity(vm);

            if (entity.LocationLayoutMachineId <= 0)
            {
                int newId = await _repository.InsertAsync(entity);
                vm.LocationLayoutMachineId = newId;
                return newId;
            }

            bool updated = await _repository.UpdateAsync(entity);

            if (!updated)
                throw new InvalidOperationException($"Location layout machine {entity.LocationLayoutMachineId} was not updated.");

            return entity.LocationLayoutMachineId;
        }

        public async Task<bool> DeleteAsync(LocationLayoutMachineViewModel vm)
        {
            if (vm.LocationLayoutMachineId <= 0)
                return false;

            return await _repository.DeleteAsync(vm.LocationLayoutMachineId);
        }

        public async Task<bool> DeleteAsync(int locationLayoutMachineId)
        {
            if (locationLayoutMachineId <= 0)
                return false;

            return await _repository.DeleteAsync(locationLayoutMachineId);
        }

        public async Task<int> DeleteByLocationIdAsync(int locationId)
        {
            if (locationId <= 0)
                return 0;

            return await _repository.DeleteByLocationIdAsync(locationId);
        }

        private static LocationLayoutMachineViewModel ToViewModel(LocationLayoutMachineEntity entity)
        {
            return new LocationLayoutMachineViewModel
            {
                LocationLayoutMachineId = entity.LocationLayoutMachineId,
                LocationId = entity.LocationId,
                MachineId = entity.MachineId,
                X = entity.X,
                Y = entity.Y,
                Width = entity.Width,
                Height = entity.Height,
                Rotation = entity.Rotation,
                ZIndex = entity.ZIndex,
                IsEnabled = entity.IsEnabled
            };
        }

        private static LocationLayoutMachineEntity ToEntity(LocationLayoutMachineViewModel vm)
        {
            return new LocationLayoutMachineEntity
            {
                LocationLayoutMachineId = vm.LocationLayoutMachineId,
                LocationId = vm.LocationId,
                MachineId = vm.MachineId,
                X = vm.X,
                Y = vm.Y,
                Width = vm.Width,
                Height = vm.Height,
                Rotation = vm.Rotation,
                ZIndex = vm.ZIndex,
                IsEnabled = vm.IsEnabled
            };
        }

        private static void Validate(LocationLayoutMachineViewModel vm)
        {
            if (vm.LocationId <= 0)
                throw new InvalidOperationException("Location is required.");

            if (vm.MachineId <= 0)
                throw new InvalidOperationException("Machine is required.");
        }
    }
}


