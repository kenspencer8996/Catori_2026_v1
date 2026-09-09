namespace CatoriApp.Game.Objects.Services.Manufacturing
{
    public class MachineTypeService
    {
        private readonly MachineTypeRepository _repository;

        public MachineTypeService()
        {
            _repository = new MachineTypeRepository();
        }

        public async Task<MachineTypeViewModel?> GetByIdAsync(int machineTypeId)
        {
            var entity = await _repository.GetByIdAsync(machineTypeId);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<MachineTypeViewModel?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var entity = await _repository.GetByNameAsync(name.Trim());
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<List<MachineTypeViewModel>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return entities
                .Select(ToViewModel)
                .ToList();
        }

        public async Task<int> SaveAsync(MachineTypeViewModel vm)
        {
            Validate(vm);

            var entity = ToEntity(vm);

            if (entity.MachineTypeId <= 0)
            {
                int newId = await _repository.InsertAsync(entity);
                vm.MachineTypeId = newId;
                return newId;
            }

            bool updated = await _repository.UpdateAsync(entity);

            if (!updated)
                throw new InvalidOperationException($"Machine type {entity.MachineTypeId} was not updated.");

            return entity.MachineTypeId;
        }

        public async Task<bool> DeleteAsync(MachineTypeViewModel vm)
        {
            if (vm.MachineTypeId <= 0)
                return false;

            return await _repository.DeleteAsync(vm.MachineTypeId);
        }

        public async Task<bool> DeleteAsync(int machineTypeId)
        {
            if (machineTypeId <= 0)
                return false;

            return await _repository.DeleteAsync(machineTypeId);
        }

        private static MachineTypeViewModel ToViewModel(MachineTypeEntity entity)
        {
            return new MachineTypeViewModel
            {
                MachineTypeId = entity.MachineTypeId,
                Name = entity.Name
            };
        }

        private static MachineTypeEntity ToEntity(MachineTypeViewModel vm)
        {
            return new MachineTypeEntity
            {
                MachineTypeId = vm.MachineTypeId,
                Name = vm.Name?.Trim() ?? ""
            };
        }

        private static void Validate(MachineTypeViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Name))
                throw new InvalidOperationException("Machine type name is required.");
        }
    }
}


