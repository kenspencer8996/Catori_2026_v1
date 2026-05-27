namespace CatoriApp.MachineLayoutDesigner.Objects.Services.Robots
{
    public class RobotSequenceService
    {
        private readonly RobotSequenceRepository _repository;

        public RobotSequenceService()
        {
            _repository = new RobotSequenceRepository();
        }

        public async Task<List<RobotSequenceViewModel>> GetAllAsync()
            => (await _repository.GetAllAsync()).Select(ToViewModel).ToList();

        public async Task<RobotSequenceViewModel?> GetByIdAsync(long robotSequenceId)
        {
            var entity = await _repository.GetByIdAsync(robotSequenceId);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<List<RobotSequenceViewModel>> GetByLocationIdAsync(long locationId)
            => (await _repository.GetByLocationIdAsync(locationId)).Select(ToViewModel).ToList();

        public async Task<RobotSequenceViewModel?> GetByNameAsync(string sequenceName)
        {
            var entity = await _repository.GetByNameAsync(sequenceName);
            return entity == null ? null : ToViewModel(entity);
        }

        public async Task<long> SaveAsync(RobotSequenceViewModel vm)
        {
            var entity = ToEntity(vm);
            var id = await _repository.SaveAsync(entity);
            vm.RobotSequenceId = id;

            for (int i = 0; i < entity.Poses.Count; i++)
            {
                vm.Poses[i].RobotPoseId = entity.Poses[i].RobotPoseId;
                vm.Poses[i].RobotSequenceId = id;

                for (int j = 0; j < entity.Poses[i].Segments.Count && j < vm.Poses[i].Segments.Count; j++)
                {
                    vm.Poses[i].Segments[j].RobotPoseId = entity.Poses[i].RobotPoseId;
                    vm.Poses[i].Segments[j].RobotPoseSegmentId = entity.Poses[i].Segments[j].RobotPoseSegmentId;
                }
            }

            return id;
        }

        public async Task<bool> UpdateAsync(RobotSequenceViewModel vm)
            => await _repository.UpdateAsync(ToEntity(vm));

        private static RobotSequenceEntity ToEntity(RobotSequenceViewModel vm)
        {
            return new RobotSequenceEntity
            {
                RobotSequenceId = vm.RobotSequenceId,
                LocationId = vm.LocationId,
                MachineInstanceId = vm.MachineInstanceId,
                SequenceName = vm.SequenceName,
                RobotX = vm.RobotX,
                RobotY = vm.RobotY,
                RobotWidth = vm.RobotWidth,
                RobotHeight = vm.RobotHeight,
                Poses = vm.Poses.Select(ToPoseEntity).ToList()
            };
        }

        private static RobotPoseEntity ToPoseEntity(RobotPoseViewModel vm)
        {
            var entity = new RobotPoseEntity
            {
                RobotPoseId = vm.RobotPoseId,
                RobotSequenceId = vm.RobotSequenceId,
                LocationId = vm.LocationId,
                PoseIndex = vm.PoseIndex,
                PoseName = vm.PoseName,
                Joint1 = vm.Joint1,
                Joint2 = vm.Joint2,
                Joint3 = vm.Joint3,
                JointEnd = vm.JointEnd,
                DurationMilliseconds = vm.DurationMilliseconds
            };

            foreach (var segment in vm.Segments.OrderBy(s => s.SegmentIndex))
                entity.Segments.Add(ToPoseSegmentEntity(segment));

            return entity;
        }

        private static RobotPoseSegmentEntity ToPoseSegmentEntity(RobotPoseSegmentViewModel vm)
        {
            return new RobotPoseSegmentEntity
            {
                RobotPoseSegmentId = vm.RobotPoseSegmentId,
                RobotPoseId = vm.RobotPoseId,
                SegmentIndex = vm.SegmentIndex,
                Angle = vm.Angle
            };
        }

        private static RobotSequenceViewModel ToViewModel(RobotSequenceEntity entity)
        {
            var vm = new RobotSequenceViewModel
            {
                RobotSequenceId = entity.RobotSequenceId,
                LocationId = entity.LocationId,
                MachineInstanceId = entity.MachineInstanceId,
                SequenceName = entity.SequenceName,
                RobotX = entity.RobotX,
                RobotY = entity.RobotY,
                RobotWidth = entity.RobotWidth,
                RobotHeight = entity.RobotHeight
            };

            foreach (var pose in entity.Poses.OrderBy(p => p.PoseIndex))
                vm.Poses.Add(ToPoseViewModel(pose));

            vm.SelectedPose = vm.Poses.FirstOrDefault();
            return vm;
        }

        private static RobotPoseViewModel ToPoseViewModel(RobotPoseEntity entity)
        {
            var vm = new RobotPoseViewModel
            {
                RobotPoseId = entity.RobotPoseId,
                RobotSequenceId = entity.RobotSequenceId,
                LocationId = entity.LocationId,
                PoseIndex = entity.PoseIndex,
                PoseName = entity.PoseName,
                Joint1 = entity.Joint1,
                Joint2 = entity.Joint2,
                Joint3 = entity.Joint3,
                JointEnd = entity.JointEnd,
                DurationMilliseconds = entity.DurationMilliseconds
            };

            foreach (var segment in entity.Segments.OrderBy(s => s.SegmentIndex))
                vm.Segments.Add(ToPoseSegmentViewModel(segment));

            return vm;
        }

        private static RobotPoseSegmentViewModel ToPoseSegmentViewModel(RobotPoseSegmentEntity entity)
        {
            return new RobotPoseSegmentViewModel
            {
                RobotPoseSegmentId = entity.RobotPoseSegmentId,
                RobotPoseId = entity.RobotPoseId,
                SegmentIndex = entity.SegmentIndex,
                Angle = entity.Angle
            };
        }
    }
}
