using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class LocationLayoutPointRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public LocationLayoutPointRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task InsertAsync_and_get_by_item_id_return_points_in_point_index_order()
    {
        using var db = await LocationRepositoryTests.CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var itemId = await CreateLayoutItemAsync();
        var repository = new LocationLayoutPointRepository();

        await repository.InsertAsync(new LocationLayoutPointEntity { LocationLayoutItemId = itemId, PointIndex = 2, X = 200, Y = 220, PointRole = "End" });
        await repository.InsertAsync(new LocationLayoutPointEntity { LocationLayoutItemId = itemId, PointIndex = 1, X = 100, Y = 120, PointRole = "Start" });

        var points = await repository.GetByItemIdAsync(itemId);

        Assert.Collection(
            points,
            first =>
            {
                Assert.Equal(1, first.PointIndex);
                Assert.Equal("Start", first.PointRole);
                Assert.Equal(100, first.X);
                Assert.Equal(120, first.Y);
            },
            second =>
            {
                Assert.Equal(2, second.PointIndex);
                Assert.Equal("End", second.PointRole);
                Assert.Equal(200, second.X);
                Assert.Equal(220, second.Y);
            });
    }

    [Fact]
    public async Task UpdateAsync_and_delete_by_item_id_update_point_storage()
    {
        using var db = await LocationRepositoryTests.CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var itemId = await CreateLayoutItemAsync();
        var repository = new LocationLayoutPointRepository();
        var pointId = await repository.InsertAsync(new LocationLayoutPointEntity
        {
            LocationLayoutItemId = itemId,
            PointIndex = 1,
            X = 10,
            Y = 20,
            PointRole = "Start"
        });

        var updated = await repository.UpdateAsync(new LocationLayoutPointEntity
        {
            LocationLayoutPointId = pointId,
            LocationLayoutItemId = itemId,
            PointIndex = 3,
            X = 30,
            Y = 40,
            PointRole = "Mid"
        });
        var pointsAfterUpdate = await repository.GetByItemIdAsync(itemId);

        Assert.True(updated);
        var point = Assert.Single(pointsAfterUpdate);
        Assert.Equal(3, point.PointIndex);
        Assert.Equal(30, point.X);
        Assert.Equal(40, point.Y);
        Assert.Equal("Mid", point.PointRole);

        Assert.True(await repository.DeleteByItemIdAsync(itemId));
        Assert.Empty(await repository.GetByItemIdAsync(itemId));
    }

    private static async Task<int> CreateLayoutItemAsync()
    {
        var locationId = await new LocationRepository().InsertAsync(new LocationEntity { LocationName = "Point Location" });
        return await new LocationLayoutItemRepository().InsertAsync(new LocationLayoutItemEntity
        {
            LocationId = locationId,
            ItemName = "Conveyor",
            ItemType = LocationLayoutItemType.Conveyor
        });
    }
}
