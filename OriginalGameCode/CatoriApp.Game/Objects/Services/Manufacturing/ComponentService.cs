public class ComponentService
{
    private readonly ComponentRepository _repository;

    public ComponentService()
    {
        _repository = new ComponentRepository();
    }

    public async Task<ComponentViewModel?> GetByIdAsync(int componentId)
    {
        var entity = await _repository.GetByIdAsync(componentId);
        return entity == null ? null : ToViewModel(entity);
    }

    public async Task<List<ComponentViewModel>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();

        return entities
            .Select(ToViewModel)
            .ToList();
    }

    public async Task<int> SaveAsync(ComponentViewModel vm)
    {
        Validate(vm);

        var entity = ToEntity(vm);

        if (entity.ComponentId <= 0)
        {
            int newId = await _repository.InsertAsync(entity);
            vm.ComponentId = newId;
            return newId;
        }

        bool updated = await _repository.UpdateAsync(entity);

        if (!updated)
            throw new InvalidOperationException($"Component {entity.ComponentId} was not updated.");

        return entity.ComponentId;
    }

    public async Task<bool> DeleteAsync(ComponentViewModel vm)
    {
        if (vm.ComponentId <= 0)
            return false;

        return await _repository.DeleteAsync(vm.ComponentId);
    }

    public async Task<bool> DeleteAsync(int componentId)
    {
        if (componentId <= 0)
            return false;

        return await _repository.DeleteAsync(componentId);
    }

    private static ComponentViewModel ToViewModel(ComponentEntity entity)
    {
        return new ComponentViewModel
        {
            ComponentId = entity.ComponentId,
            ComponentName = entity.ComponentName,
            Quantity = entity.Quantity
        };
    }

    private static ComponentEntity ToEntity(ComponentViewModel vm)
    {
        return new ComponentEntity
        {
            ComponentId = vm.ComponentId,
            ComponentName = vm.ComponentName?.Trim() ?? "",
            Quantity = vm.Quantity
        };
    }

    private static void Validate(ComponentViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.ComponentName))
            throw new InvalidOperationException("Component name is required.");

        if (vm.Quantity < 0)
            throw new InvalidOperationException("Quantity cannot be negative.");
    }
}