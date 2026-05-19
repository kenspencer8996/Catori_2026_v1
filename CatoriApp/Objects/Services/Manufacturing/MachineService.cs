namespace CatoriApp.Objects.Services.Manufacturing
{
    public class MachineService
    {
        private readonly MachineRepository _repository;

        public MachineService()
        {
            _repository = new MachineRepository();
        }

        public async Task<MachineViewModel?> GetByIdAsync(int machineId)
        {
            var entity = await _repository.GetByIdAsync(machineId);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<List<MachineViewModel>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<List<MachineViewModel>> GetByMachineTypeIdAsync(int machineTypeId)
        {
            var entities = await _repository.GetByMachineTypeIdAsync(machineTypeId);

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<int> SaveAsync(MachineViewModel vm)
        {
            Validate(vm);

            var entity = ToEntity(vm);

            if (entity.MachineId <= 0)
            {
                int newId = await _repository.InsertAsync(entity);
                vm.MachineId = newId;
                return newId;
            }

            bool updated = await _repository.UpdateAsync(entity);

            if (!updated)
                throw new InvalidOperationException($"Machine {entity.MachineId} was not updated.");

            return entity.MachineId;
        }

        public async Task<bool> DeleteAsync(MachineViewModel vm)
        {
            if (vm.MachineId <= 0)
                return false;

            return await _repository.DeleteAsync(vm.MachineId);
        }

        public async Task<bool> DeleteAsync(int machineId)
        {
            if (machineId <= 0)
                return false;

            return await _repository.DeleteAsync(machineId);
        }

        private static MachineViewModel ToViewModel(MachineEntity entity)
        {
            return new MachineViewModel
            {
                MachineId = entity.MachineId,
                MachineTypeId = entity.MachineTypeId,
                Name = entity.Name,
                ImagePath = entity.ImagePath,
                ControlTypeName = entity.ControlTypeName,
                Description = entity.Description
            };
        }

        private static MachineEntity ToEntity(MachineViewModel vm)
        {
            return new MachineEntity
            {
                MachineId = vm.MachineId,
                MachineTypeId = vm.MachineTypeId,
                Name = vm.Name?.Trim() ?? "",
                ImagePath = string.IsNullOrWhiteSpace(vm.ImagePath) ? null : vm.ImagePath.Trim(),
                ControlTypeName = string.IsNullOrWhiteSpace(vm.ControlTypeName) ? null : vm.ControlTypeName.Trim(),
                Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim()
            };
        }

        private static void Validate(MachineViewModel vm)
        {
            if (vm.MachineTypeId <= 0)
                throw new InvalidOperationException("Machine type is required.");

            if (string.IsNullOrWhiteSpace(vm.Name))
                throw new InvalidOperationException("Machine name is required.");
        }
    }
}


