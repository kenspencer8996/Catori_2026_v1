using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class RobotRepositoryTests
{
    private readonly GlobalTestState _globalState;
    public RobotRepositoryTests(GlobalTestState globalState) => _globalState = globalState;

    [Fact]
    public async Task SaveAsync_round_trips_robot_and_json_poses()
    {
        using var db = await MachineLayoutDesignerRepositoryTests.CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);
        var locationId = await MachineLayoutDesignerRepositoryTests.CreateLocationAsync("Assembly");
        var repository = new RobotRepository();
        var robot = new RobotEntity
        {
            LocationId = locationId, RobotX = 25, RobotY = 35,
            Poses = new() { new RobotPoseEntity { PoseName = "Pickup", Pose = "[10,20,30,40]" } }
        };

        var id = await repository.SaveAsync(robot);
        var loaded = await repository.GetByLocationIdAsync(locationId);

        Assert.True(id > 0);
        Assert.NotNull(loaded);
        Assert.Equal(25, loaded.RobotX);
        Assert.Single(loaded.Poses);
        Assert.Equal("[10,20,30,40]", loaded.Poses[0].Pose);
    }

    [Fact]
    public async Task SaveAsync_replaces_poses_for_existing_location_robot()
    {
        using var db = await MachineLayoutDesignerRepositoryTests.CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);
        var locationId = await MachineLayoutDesignerRepositoryTests.CreateLocationAsync("Assembly");
        var repository = new RobotRepository();
        var firstId = await repository.SaveAsync(new RobotEntity { LocationId = locationId, Poses = new() { new() { PoseName = "Old", Pose = "[1]" } } });
        var secondId = await repository.SaveAsync(new RobotEntity { LocationId = locationId, Poses = new() { new() { PoseName = "New", Pose = "[2,3]" } } });
        var loaded = await repository.GetByLocationIdAsync(locationId);
        Assert.Equal(firstId, secondId);
        Assert.Single(loaded!.Poses);
        Assert.Equal("New", loaded.Poses[0].PoseName);
    }
}
