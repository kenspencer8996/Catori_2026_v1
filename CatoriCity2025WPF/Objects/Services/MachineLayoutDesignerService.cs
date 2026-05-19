using CatoriApp.ViewModels;
using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriApp.Objects.Services
{
    public class MachineLayoutDesignerService
    {
        private readonly MachineLayoutDesignerRepository _designerRepository;
        private readonly RobotPoseService _poseService;

        public MachineLayoutDesignerService()
        {
            _designerRepository = new MachineLayoutDesignerRepository();
            _poseService = new RobotPoseService();
        }

        public async Task<MachineLayoutDesignerViewModel?> LoadSequenceAsync(string sequenceName)
        {
            var entity = await _designerRepository.GetByNameAsync(sequenceName);
            if (entity == null)
                return null;

            var vm = ToViewModel(entity);
            var poses = await _poseService.GetByLocationIdAsync(vm.LocationId);
            foreach (var pose in poses)
                vm.Poses.Add(pose);

            vm.SelectedPose = vm.Poses.FirstOrDefault();
            return vm;
        }

        public async Task<long> SaveSequenceAsync(MachineLayoutDesignerViewModel vm)
        {
            var entity = ToEntity(vm);
            var id = await _designerRepository.SaveAsync(entity);
            vm.MachineLayoutDesignerId = id;

            foreach (var pose in vm.Poses)
                pose.LocationId = vm.LocationId;

            await _poseService.ReplaceForLocationAsync(vm.LocationId, vm.Poses);
            return id;
        }

        private static MachineLayoutDesignerEntity ToEntity(MachineLayoutDesignerViewModel vm)
        {
            return new MachineLayoutDesignerEntity
            {
                MachineLayoutDesignerId = vm.MachineLayoutDesignerId,
                LocationId = vm.LocationId,
                SequenceName = vm.SequenceName,
                SelectionX = vm.SelectionX,
                SelectionY = vm.SelectionY,
                SelectionWidth = vm.SelectionWidth,
                SelectionHeight = vm.SelectionHeight,
                RobotX = vm.RobotX,
                RobotY = vm.RobotY,
                RobotWidth = vm.RobotWidth,
                RobotHeight = vm.RobotHeight
            };
        }

        private static MachineLayoutDesignerViewModel ToViewModel(MachineLayoutDesignerEntity entity)
        {
            return new MachineLayoutDesignerViewModel
            {
                MachineLayoutDesignerId = entity.MachineLayoutDesignerId,
                LocationId = entity.LocationId,
                SequenceName = entity.SequenceName,
                SelectionX = entity.SelectionX,
                SelectionY = entity.SelectionY,
                SelectionWidth = entity.SelectionWidth,
                SelectionHeight = entity.SelectionHeight,
                RobotX = entity.RobotX,
                RobotY = entity.RobotY,
                RobotWidth = entity.RobotWidth,
                RobotHeight = entity.RobotHeight
            };
        }
    }
}

