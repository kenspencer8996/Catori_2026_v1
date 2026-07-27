using System.Text.Json;

namespace CatoriApp.MachineLayoutDesigner.Objects.Services.Robots
{
    public class RobotPoseService
    {
        private readonly RobotRepository _repository = new();

        public async Task<List<RobotPoseViewModel>> GetByLocationIdAsync(long locationId)
        {
            var robot = await _repository.GetByLocationIdAsync(locationId);
            if (robot == null) return new();
            var result = new List<RobotPoseViewModel>();
            foreach (var entity in robot.Poses)
            {
                var vm = new RobotPoseViewModel { RobotPoseId = entity.RobotPoseId, RobotId = robot.RobotId, PoseIndex = result.Count, PoseName = entity.PoseName };
                var angles = JsonSerializer.Deserialize<List<double>>(entity.Pose) ?? new();
                for (var i = 0; i < angles.Count; i++) vm.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = i, Angle = angles[i] });
                result.Add(vm);
            }
            return result;
        }
    }
}
