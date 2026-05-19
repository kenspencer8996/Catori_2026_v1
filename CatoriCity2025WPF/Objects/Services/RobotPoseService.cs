namespace CatoriApp.Objects.Services
{
    public class RobotPoseService
    {
        private readonly RobotPoseRepository _repository;

        public RobotPoseService()
        {
            _repository = new RobotPoseRepository();
        }

        public async Task<List<RobotPoseViewModel>> GetByLocationIdAsync(long locationId)
        {
            var poses = await _repository.GetByLocationIdAsync(locationId);
            return poses.OrderBy(p => p.PoseIndex).Select(ToViewModel).ToList();
        }

        public async Task ReplaceForLocationAsync(long locationId, IList<RobotPoseViewModel> poses)
        {
            var entities = poses.Select(ToEntity).ToList();
            await _repository.ReplaceForLocationAsync(locationId, entities);

            for (int i = 0; i < entities.Count; i++)
                poses[i].RobotPoseId = entities[i].RobotPoseId;
        }

        private static RobotPoseEntity ToEntity(RobotPoseViewModel vm)
        {
            return new RobotPoseEntity
            {
                RobotPoseId = vm.RobotPoseId,
                LocationId = vm.LocationId,
                PoseIndex = vm.PoseIndex,
                PoseName = vm.PoseName,
                Joint1 = vm.Joint1,
                Joint2 = vm.Joint2,
                Joint3 = vm.Joint3,
                JointHand = vm.JointHand,
                DurationMilliseconds = vm.DurationMilliseconds
            };
        }

        private static RobotPoseViewModel ToViewModel(RobotPoseEntity entity)
        {
            return new RobotPoseViewModel
            {
                RobotPoseId = entity.RobotPoseId,
                LocationId = entity.LocationId,
                PoseIndex = entity.PoseIndex,
                PoseName = entity.PoseName,
                Joint1 = entity.Joint1,
                Joint2 = entity.Joint2,
                Joint3 = entity.Joint3,
                JointHand = entity.JointHand,
                DurationMilliseconds = entity.DurationMilliseconds
            };
        }
    }
}

