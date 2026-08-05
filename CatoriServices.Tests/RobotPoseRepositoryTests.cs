using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class RobotPoseRepositoryTests
{
    private readonly GlobalTestState _globalState;
    public RobotPoseRepositoryTests(GlobalTestState globalState) => _globalState = globalState;

    [Fact]
    public async Task ReplaceAsync_round_trips_json_poses_for_layout_item()
    {
        using var db = await MachineLayoutDesignerRepositoryTests.CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);
        var locationId = await MachineLayoutDesignerRepositoryTests.CreateLocationAsync("Assembly");
        var itemRepository = new LocationLayoutItemRepository();
        var itemId = await itemRepository.InsertAsync(new LocationLayoutItemEntity
        {
            LocationId = locationId,
            ItemName = "Robot",
            ItemType = LocationLayoutItemType.Robot
        });
        var repository = new RobotPoseRepository();

        await repository.ReplaceAsync(itemId, new[]
        {
            new RobotPoseEntity { PoseName = "Pickup", Pose = "[10,20,30,40]" }
        });
        var loaded = await repository.GetByLocationLayoutItemIdAsync(itemId);

        Assert.Single(loaded);
        Assert.Equal(itemId, loaded[0].LocationLayoutItemId);
        Assert.Equal("[10,20,30,40]", loaded[0].Pose);
    }

    [Fact]
    public async Task ReplaceAsync_replaces_existing_layout_item_poses()
    {
        using var db = await MachineLayoutDesignerRepositoryTests.CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);
        var locationId = await MachineLayoutDesignerRepositoryTests.CreateLocationAsync("Assembly");
        var itemRepository = new LocationLayoutItemRepository();
        var itemId = await itemRepository.InsertAsync(new LocationLayoutItemEntity
        {
            LocationId = locationId,
            ItemName = "Robot",
            ItemType = LocationLayoutItemType.Robot
        });
        var repository = new RobotPoseRepository();

        await repository.ReplaceAsync(itemId, new[] { new RobotPoseEntity { PoseName = "Old", Pose = "[1]" } });
        await repository.ReplaceAsync(itemId, new[] { new RobotPoseEntity { PoseName = "New", Pose = "[2,3]" } });
        var loaded = await repository.GetByLocationLayoutItemIdAsync(itemId);

        Assert.Single(loaded);
        Assert.Equal("New", loaded[0].PoseName);
    }
}