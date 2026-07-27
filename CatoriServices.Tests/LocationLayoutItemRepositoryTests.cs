using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class LocationLayoutItemRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public LocationLayoutItemRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task InsertAsync_creates_layout_object_and_anchor_point()
    {
        using var db = await LocationRepositoryTests.CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var locationId = await new LocationRepository().InsertAsync(new LocationEntity { LocationName = "Layout Location" });
        var repository = new LocationLayoutItemRepository();

        var itemId = await repository.InsertAsync(new LocationLayoutItemEntity
        {
            LocationId = locationId,
            ItemName = "Robot A",
            ItemType = LocationLayoutItemType.Robot,
            MajorItemType = "Production",
            X = 125.5,
            Y = 240.25,
            ZIndex = 12,
            IsLocked = true,
            WpfPath = "M 10,20 L 30,40",
            MetadataJson = "{\"station\":\"A\"}"
        });

        var loaded = await repository.GetByIdAsync(itemId);

        Assert.NotNull(loaded);
        Assert.Equal(locationId, loaded.LocationId);
        Assert.Equal("Robot A", loaded.ItemName);
        Assert.Equal(LocationLayoutItemType.Robot, loaded.ItemType);
        Assert.Equal("Production", loaded.MajorItemType);
        Assert.Equal(125.5, loaded.X);
        Assert.Equal(240.25, loaded.Y);
        Assert.Equal(12, loaded.ZIndex);
        Assert.True(loaded.IsLocked);
        Assert.Equal("M 10,20 L 30,40", loaded.WpfPath);
        Assert.Equal("{\"station\":\"A\"}", loaded.MetadataJson);
    }

    [Fact]
    public async Task UpdateAsync_updates_layout_object_and_anchor_point()
    {
        using var db = await LocationRepositoryTests.CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var locationId = await new LocationRepository().InsertAsync(new LocationEntity { LocationName = "Layout Location" });
        var repository = new LocationLayoutItemRepository();
        var itemId = await repository.InsertAsync(new LocationLayoutItemEntity
        {
            LocationId = locationId,
            ItemName = "Table A",
            ItemType = LocationLayoutItemType.Table,
            X = 10,
            Y = 20
        });

        var updated = await repository.UpdateAsync(new LocationLayoutItemEntity
        {
            LocationLayoutItemId = itemId,
            LocationId = locationId,
            ItemName = "Table B",
            ItemType = LocationLayoutItemType.Workstation,
            MajorItemType = "Assembly",
            X = 30,
            Y = 40,
            ZIndex = 5,
            IsLocked = false,
            WpfPath = "M 1,2 L 3,4",
            MetadataJson = "updated"
        });

        var loaded = await repository.GetByIdAsync(itemId);

        Assert.True(updated);
        Assert.NotNull(loaded);
        Assert.Equal("Table B", loaded.ItemName);
        Assert.Equal(LocationLayoutItemType.Workstation, loaded.ItemType);
        Assert.Equal("Assembly", loaded.MajorItemType);
        Assert.Equal(30, loaded.X);
        Assert.Equal(40, loaded.Y);
        Assert.Equal(5, loaded.ZIndex);
        Assert.False(loaded.IsLocked);
        Assert.Equal("M 1,2 L 3,4", loaded.WpfPath);
        Assert.Equal("updated", loaded.MetadataJson);
    }
}
