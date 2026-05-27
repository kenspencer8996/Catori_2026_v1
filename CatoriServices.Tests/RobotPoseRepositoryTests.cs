using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class RobotPoseRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public RobotPoseRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task ReplaceForLocationAsync_replaces_existing_pose_sequence_in_order()
    {
        using var db = await MachineLayoutDesignerRepositoryTests.CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var locationId = await MachineLayoutDesignerRepositoryTests.CreateLocationAsync("Assembly Location");
        var poseRepository = new RobotPoseRepository();

        await poseRepository.ReplaceForLocationAsync(locationId, new List<RobotPoseEntity>
        {
            new() { PoseName = "OldPose", Joint1 = 1 }
        });

        var replacement = new List<RobotPoseEntity>
        {
            new() { PoseName = "PickupPartA", Joint1 = 10, Joint2 = 20, Joint3 = 30, JointEnd = 40, DurationMilliseconds = 500 },
            new() { PoseName = "DropPartA", Joint1 = 50, Joint2 = 60, Joint3 = 70, JointEnd = 80, DurationMilliseconds = 900 }
        };

        await poseRepository.ReplaceForLocationAsync(locationId, replacement);
        var loaded = await poseRepository.GetByLocationIdAsync(locationId);

        Assert.Equal(2, loaded.Count);
        Assert.All(loaded, pose => Assert.Equal(locationId, pose.LocationId));
        Assert.Equal("PickupPartA", loaded[0].PoseName);
        Assert.Equal(0, loaded[0].PoseIndex);
        Assert.Equal(10, loaded[0].Joint1);
        Assert.Equal(20, loaded[0].Joint2);
        Assert.Equal(30, loaded[0].Joint3);
        Assert.Equal(40, loaded[0].JointEnd);
        Assert.Equal(500, loaded[0].DurationMilliseconds);
        Assert.Equal("DropPartA", loaded[1].PoseName);
        Assert.Equal(1, loaded[1].PoseIndex);
    }

    [Fact]
    public async Task GetByLocationIdAsync_does_not_return_poses_for_other_factories()
    {
        using var db = await MachineLayoutDesignerRepositoryTests.CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var firstLocationId = await MachineLayoutDesignerRepositoryTests.CreateLocationAsync("First");
        var secondLocationId = await MachineLayoutDesignerRepositoryTests.CreateLocationAsync("Second");
        var poseRepository = new RobotPoseRepository();

        await poseRepository.ReplaceForLocationAsync(firstLocationId, new List<RobotPoseEntity>
        {
            new() { PoseName = "FirstPose", Joint1 = 11 }
        });
        await poseRepository.ReplaceForLocationAsync(secondLocationId, new List<RobotPoseEntity>
        {
            new() { PoseName = "SecondPose", Joint1 = 22 }
        });

        var firstPoses = await poseRepository.GetByLocationIdAsync(firstLocationId);

        Assert.Single(firstPoses);
        Assert.Equal("FirstPose", firstPoses[0].PoseName);
        Assert.Equal(11, firstPoses[0].Joint1);
    }
}
