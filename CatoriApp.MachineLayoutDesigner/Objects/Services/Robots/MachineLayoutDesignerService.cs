using System.Text.Json;

namespace CatoriApp.MachineLayoutDesigner.Objects.Services.Robots
{
    public class MachineLayoutDesignerService
    {
        private readonly MachineLayoutDesignerRepository _designerRepository = new();
        private readonly RobotRepository _robotRepository = new();

        public async Task<MachineLayoutDesignerViewModel?> LoadByLocationIdAsync(long locationId, string? unused = null)
        {
            var layout = await _designerRepository.GetByLocationIdAsync(locationId);
            var robot = await _robotRepository.GetByLocationIdAsync(locationId);
            var vm = new MachineLayoutDesignerViewModel { LocationId = locationId };
            if (layout != null)
            {
                vm.MachineLayoutDesignerId = layout.MachineLayoutDesignerId;
                vm.SelectionX = layout.SelectionX; vm.SelectionY = layout.SelectionY;
                vm.SelectionWidth = layout.SelectionWidth; vm.SelectionHeight = layout.SelectionHeight;
            }
            if (robot != null)
            {
                vm.RobotId = robot.RobotId; 
                vm.RobotX = robot.RobotX; 
                vm.RobotY = robot.RobotY;
                vm.RobotWidth = robot.RobotWidth; 
                vm.RobotHeight = robot.RobotHeight;
                foreach (var pose in robot.Poses)
                {
                    var poseVm = new RobotPoseViewModel { RobotPoseId = pose.RobotPoseId, RobotId = robot.RobotId, PoseIndex = vm.Poses.Count, PoseName = pose.PoseName };
                    var angles = JsonSerializer.Deserialize<List<double>>(pose.Pose) ?? new();
                    for (var i = 0; i < angles.Count; i++) poseVm.Segments.Add(new RobotPoseSegmentViewModel { SegmentIndex = i, Angle = angles[i] });
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
                MachineLayoutDesignerId = vm.MachineLayoutDesignerId, LocationId = vm.LocationId, SelectionX = vm.SelectionX,
                SelectionY = vm.SelectionY, SelectionWidth = vm.SelectionWidth, SelectionHeight = vm.SelectionHeight
            });
            var robot = new RobotEntity
            {
                RobotId = vm.RobotId, LocationId = vm.LocationId, RobotX = vm.RobotX, RobotY = vm.RobotY,
                RobotWidth = vm.RobotWidth, RobotHeight = vm.RobotHeight,
                Poses = vm.Poses.Select(p => new RobotPoseEntity
                {
                    RobotPoseId = p.RobotPoseId, RobotId = vm.RobotId, PoseName = p.PoseName,
                    Pose = JsonSerializer.Serialize(p.Segments.OrderBy(s => s.SegmentIndex).Select(s => s.Angle))
                }).ToList()
            };
            vm.RobotId = await _robotRepository.SaveAsync(robot);
            return vm.RobotId;
        }
    }
}
