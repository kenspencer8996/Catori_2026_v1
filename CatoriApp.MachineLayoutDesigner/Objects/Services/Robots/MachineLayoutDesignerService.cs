using System.Text.Json;
using CatoriServices.Objects.database.Locations;
using CatoriServices.Objects.Entities.Locations;

namespace CatoriApp.MachineLayoutDesigner.Objects.Services.Robots
{
    public class MachineLayoutDesignerService
    {
        private readonly MachineLayoutDesignerRepository _designerRepository = new();
        private readonly LocationLayoutItemRepository _layoutItemRepository = new();
        private readonly RobotPoseRepository _poseRepository = new();

        public async Task<MachineLayoutDesignerViewModel?> LoadByLocationIdAsync(long locationId, string? unused = null)
        {
            var layout = await _designerRepository.GetByLocationIdAsync(locationId);
            var robotItem = (await _layoutItemRepository.GetByLocationIdAsync(locationId))
                .FirstOrDefault(item => item.ItemType == LocationLayoutItemType.Robot);
            var vm = new MachineLayoutDesignerViewModel { LocationId = locationId };

            if (layout != null)
            {
                vm.MachineLayoutDesignerId = layout.MachineLayoutDesignerId;
                vm.SelectionX = layout.SelectionX;
                vm.SelectionY = layout.SelectionY;
                vm.SelectionWidth = layout.SelectionWidth;
                vm.SelectionHeight = layout.SelectionHeight;
            }

            if (robotItem != null)
            {
                vm.LocationLayoutItemId = robotItem.LocationLayoutItemId;
                vm.RobotX = robotItem.X;
                vm.RobotY = robotItem.Y;
                vm.RobotWidth = robotItem.Width;
                vm.RobotHeight = robotItem.Height;

                foreach (var pose in await _poseRepository.GetByLocationLayoutItemIdAsync(robotItem.LocationLayoutItemId))
                {
                    var poseVm = new RobotPoseViewModel
                    {
                        RobotPoseId = pose.RobotPoseId,
                        LocationLayoutItemId = robotItem.LocationLayoutItemId,
                        PoseIndex = vm.Poses.Count,
                        PoseName = pose.PoseName
                    };
                    var angles = JsonSerializer.Deserialize<List<double>>(pose.Pose) ?? new();
                    for (var i = 0; i < angles.Count; i++)
                        poseVm.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = i, Angle = angles[i] });
                    vm.Poses.Add(poseVm);
                }
                vm.SelectedPose = vm.Poses.FirstOrDefault();
            }
            return vm;
        }

        public async Task<long> SaveSequenceAsync(MachineLayoutDesignerViewModel vm)
        {
            vm.MachineLayoutDesignerId = await _designerRepository.SaveAsync(new MachineLayoutDesignerEntity
            {
                MachineLayoutDesignerId = vm.MachineLayoutDesignerId,
                LocationId = vm.LocationId,
                SelectionX = vm.SelectionX,
                SelectionY = vm.SelectionY,
                SelectionWidth = vm.SelectionWidth,
                SelectionHeight = vm.SelectionHeight
            });

            var robotItem = vm.LocationLayoutItemId > 0
                ? await _layoutItemRepository.GetByIdAsync(vm.LocationLayoutItemId)
                : null;
            robotItem ??= new LocationLayoutItemEntity
            {
                LocationId = vm.LocationId,
                ItemName = "Robot",
                ItemType = LocationLayoutItemType.Robot,
                MajorItemType = "Robot"
            };
            robotItem.X = vm.RobotX;
            robotItem.Y = vm.RobotY;
            robotItem.Width = vm.RobotWidth;
            robotItem.Height = vm.RobotHeight;

            if (robotItem.LocationLayoutItemId <= 0)
                robotItem.LocationLayoutItemId = await _layoutItemRepository.InsertAsync(robotItem);
            else
                await _layoutItemRepository.UpdateAsync(robotItem);

            vm.LocationLayoutItemId = robotItem.LocationLayoutItemId;
            var poses = vm.Poses.Select(p => new RobotPoseEntity
            {
                RobotPoseId = p.RobotPoseId,
                LocationLayoutItemId = robotItem.LocationLayoutItemId,
                PoseName = p.PoseName,
                Pose = JsonSerializer.Serialize(p.Segments.OrderBy(s => s.SegmentIndex).Select(s => s.Angle))
            }).ToList();
            await _poseRepository.ReplaceAsync(robotItem.LocationLayoutItemId, poses);
            return robotItem.LocationLayoutItemId;
        }
    }
}
