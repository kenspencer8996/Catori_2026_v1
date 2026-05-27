namespace CatoriApp.MachineLayoutDesigner.Objects.Services.Robots
{
    public class MachineCatalogService
    {
        private readonly MachineCatalogRepository _repository;

        public MachineCatalogService()
        {
            _repository = new MachineCatalogRepository();
        }

        public async Task<List<MachineDefinitionViewModel>> GetAllDefinitionsAsync()
            => (await _repository.GetAllDefinitionsAsync()).Select(ToViewModel).ToList();

        public async Task<MachineDefinitionViewModel?> GetDefinitionByIdAsync(long machineDefinitionId)
        {
            var entity = await _repository.GetDefinitionByIdAsync(machineDefinitionId);
            return entity == null ? null : ToViewModel(entity);
        }
        
        public async Task<MachineDefinitionViewModel?> GetDefinitionByNameAsync(string machineDefinitionName)
        {
            MachineDefinitionEntity? entity = null;
            try
            {
                entity = await _repository.GetDefinitionByNameAsync(machineDefinitionName);
            }
            catch (Exception ex)
            {
                throw;
            }
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<long> SaveDefinitionAsync(MachineDefinitionViewModel vm)
        {
            var entity = ToEntity(vm);
            var id = await _repository.SaveDefinitionAsync(entity);
            vm.MachineDefinitionId = id;

            return id;
        }

        public async Task<List<MachineInstanceViewModel>> GetAllInstancesAsync()
            => (await _repository.GetAllInstancesAsync()).Select(ToViewModel).ToList();

        public async Task<MachineInstanceViewModel?> GetInstanceByIdAsync(long machineInstanceId)
        {
            var entity = await _repository.GetInstanceByIdAsync(machineInstanceId);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<long> SaveInstanceAsync(MachineInstanceViewModel vm)
        {
            var entity = ToEntity(vm);
            var id = await _repository.SaveInstanceAsync(entity);
            vm.MachineInstanceId = id;
            for (int i = 0; i < entity.Segments.Count; i++)
            {
                vm.Segments[i].MachineInstanceId = id;
                vm.Segments[i].MachineInstanceSegmentId = entity.Segments[i].MachineInstanceSegmentId;
            }

            return id;
        }

        private static MachineDefinitionEntity ToEntity(MachineDefinitionViewModel vm)
        {
            return new MachineDefinitionEntity
            {
                MachineDefinitionId = vm.MachineDefinitionId,
                MachineType = vm.MachineType,
                MachineName = vm.MachineName,
                Description = vm.Description,
                DefaultWidth = vm.DefaultWidth,
                DefaultHeight = vm.DefaultHeight
            };
        }

        private static MachineInstanceEntity ToEntity(MachineInstanceViewModel vm)
        {
            return new MachineInstanceEntity
            {
                MachineInstanceId = vm.MachineInstanceId,
                MachineDefinitionId = vm.MachineDefinitionId,
                InstanceName = vm.InstanceName,
                DisplayName = vm.DisplayName,
                DefaultScale = vm.DefaultScale,
                DefaultWidth = vm.DefaultWidth,
                DefaultHeight = vm.DefaultHeight,
                Segments = vm.Segments.Select(ToEntity).ToList()
            };
        }

        private static MachineInstanceSegmentEntity ToEntity(MachineInstanceSegmentViewModel vm)
        {
            return new MachineInstanceSegmentEntity
            {
                MachineInstanceSegmentId = vm.MachineInstanceSegmentId,
                MachineInstanceId = vm.MachineInstanceId,
                SegmentIndex = vm.SegmentIndex,
                SegmentName = vm.SegmentName,
                Length = vm.Length,
                Width = vm.Width,
                InitialAngle = vm.InitialAngle,
                MinAngle = vm.MinAngle,
                MaxAngle = vm.MaxAngle,
                Overlap = vm.Overlap,
                Color = vm.Color,
                ImageName = vm.ImageName
            };
        }

        private static MachineDefinitionViewModel ToViewModel(MachineDefinitionEntity entity)
        {
            var vm = new MachineDefinitionViewModel
            {
                MachineDefinitionId = entity.MachineDefinitionId,
                MachineType = entity.MachineType,
                MachineName = entity.MachineName,
                Description = entity.Description,
                DefaultWidth = entity.DefaultWidth,
                DefaultHeight = entity.DefaultHeight
            };

            return vm;
        }

        private static MachineInstanceViewModel ToViewModel(MachineInstanceEntity entity)
        {
            var vm = new MachineInstanceViewModel
            {
                MachineInstanceId = entity.MachineInstanceId,
                MachineDefinitionId = entity.MachineDefinitionId,
                InstanceName = entity.InstanceName,
                DisplayName = entity.DisplayName,
                DefaultScale = entity.DefaultScale,
                DefaultWidth = entity.DefaultWidth,
                DefaultHeight = entity.DefaultHeight
            };

            foreach (var segment in entity.Segments.OrderBy(s => s.SegmentIndex))
                vm.Segments.Add(ToViewModel(segment));

            return vm;
        }

        private static MachineInstanceSegmentViewModel ToViewModel(MachineInstanceSegmentEntity entity)
        {
            return new MachineInstanceSegmentViewModel
            {
                MachineInstanceSegmentId = entity.MachineInstanceSegmentId,
                MachineInstanceId = entity.MachineInstanceId,
                SegmentIndex = entity.SegmentIndex,
                SegmentName = entity.SegmentName,
                Length = entity.Length,
                Width = entity.Width,
                InitialAngle = entity.InitialAngle,
                MinAngle = entity.MinAngle,
                MaxAngle = entity.MaxAngle,
                Overlap = entity.Overlap,
                Color = entity.Color,
                ImageName = entity.ImageName
            };
        }
    }
}
