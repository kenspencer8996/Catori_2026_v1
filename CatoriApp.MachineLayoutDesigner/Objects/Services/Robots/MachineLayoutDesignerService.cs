using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
namespace CatoriApp.MachineLayoutDesigner.Objects.Services.Robots
{
    public class MachineLayoutDesignerService
    {
        private readonly MachineLayoutDesignerRepository _designerRepository;
        private readonly RobotSequenceService _sequenceService;

        public MachineLayoutDesignerService()
        {
            _designerRepository = new MachineLayoutDesignerRepository();
            _sequenceService = new RobotSequenceService();
        }

        public async Task<MachineLayoutDesignerViewModel?> LoadSequenceAsync(string sequenceName)
        {
            var sequence = await _sequenceService.GetByNameAsync(sequenceName);
            if (sequence == null)
                return null;

            var entity = await _designerRepository.GetByLocationIdAsync(sequence.LocationId);
            var vm = entity == null ? new MachineLayoutDesignerViewModel() : ToViewModel(entity);

            vm.LocationId = sequence.LocationId;
            vm.Sequences.Add(sequence);
            vm.SelectedSequence = sequence;
            vm.SelectedPose = vm.Poses.FirstOrDefault();
            return vm;
        }

        public async Task<MachineLayoutDesignerViewModel?> LoadByLocationIdAsync(long locationId, string? preferredSequenceName = null)
        {
            var entity = await _designerRepository.GetByLocationIdAsync(locationId);
            var vm = entity == null ? new MachineLayoutDesignerViewModel { LocationId = locationId } : ToViewModel(entity);

            var sequences = await _sequenceService.GetByLocationIdAsync(locationId);
            foreach (var sequence in sequences)
                vm.Sequences.Add(sequence);

            var selected = !string.IsNullOrWhiteSpace(preferredSequenceName)
                ? vm.Sequences.FirstOrDefault(s => string.Equals(s.SequenceName, preferredSequenceName, StringComparison.OrdinalIgnoreCase))
                : null;

            vm.SelectedSequence = selected ?? vm.Sequences.FirstOrDefault() ?? vm.SelectedSequence;
            vm.SelectedSequence.LocationId = locationId;
            vm.SelectedPose = vm.Poses.FirstOrDefault();
            return vm;
        }

        public async Task<long> SaveSequenceAsync(MachineLayoutDesignerViewModel vm)
        {
            var entity = ToEntity(vm);
            var id = await _designerRepository.SaveAsync(entity);
            vm.MachineLayoutDesignerId = id;

            vm.SelectedSequence.LocationId = vm.LocationId;
            foreach (var pose in vm.Poses)
            {
                pose.LocationId = vm.LocationId;
                pose.RobotSequenceId = vm.RobotSequenceId;
            }

            await _sequenceService.SaveAsync(vm.SelectedSequence);
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



