using System.Text.Json;
using CatoriServices.Objects.database.Locations;
using CatoriServices.Objects.Entities.Locations;

namespace CatoriApp.MachineLayoutDesigner.Objects.Services.Robots
{
    public class RobotPoseService
    {
        private readonly LocationLayoutItemRepository _layoutItemRepository = new();
        private readonly RobotPoseRepository _poseRepository = new();

        public async Task<List<RobotPoseViewModel>> GetByLocationIdAsync(long locationId)
        {
            var robotItem = (await _layoutItemRepository.GetByLocationIdAsync(locationId))
                .FirstOrDefault(item => item.ItemType == LocationLayoutItemType.Robot);
            if (robotItem == null)
                return new();

            var result = new List<RobotPoseViewModel>();
            foreach (var entity in await _poseRepository.GetByLocationLayoutItemIdAsync(robotItem.LocationLayoutItemId))
            {
                var vm = new RobotPoseViewModel
                {
                    RobotPoseId = entity.RobotPoseId,
                    LocationLayoutItemId = robotItem.LocationLayoutItemId,
                    PoseIndex = result.Count,
                    PoseName = entity.PoseName
                };
                var angles = JsonSerializer.Deserialize<List<double>>(entity.Pose) ?? new();
                for (var i = 0; i < angles.Count; i++)
                    vm.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = i, Angle = angles[i] });
                result.Add(vm);
            }
            return result;
        }
    }
}
