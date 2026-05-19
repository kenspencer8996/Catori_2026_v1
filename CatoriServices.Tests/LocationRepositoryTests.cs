using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class LocationRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public LocationRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task InsertAsync_get_update_delete_round_trips_location()
    {
        using var db = await CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new LocationRepository();

        var id = await repository.InsertAsync(new LocationEntity
        {
            LocationName = "Main Location"
        });

        var inserted = await repository.GetByIdAsync(id);
        Assert.NotNull(inserted);
        Assert.Equal("Main Location", inserted.LocationName);

        inserted.LocationName = "Updated Location";
        await repository.UpdateAsync(inserted);

        var updated = await repository.GetByIdAsync(id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Location", updated.LocationName);

        await repository.DeleteAsync(id);

        Assert.Null(await repository.GetByIdAsync(id));
    }

    [Fact]
    public async Task GetAllAsync_orders_by_location_name()
    {
        using var db = await CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new LocationRepository();

        await repository.InsertAsync(new LocationEntity { LocationName = "Zeta" });
        await repository.InsertAsync(new LocationEntity { LocationName = "Alpha" });

        var factories = await repository.GetAllAsync();

        Assert.Collection(
            factories,
            first => Assert.Equal("Alpha", first.LocationName),
            second => Assert.Equal("Zeta", second.LocationName));
    }

    [Fact]
    public async Task GetActiveLayoutByLocationIdAsync_maps_live_location_row_as_layout()
    {
        using var db = await CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var locationId = await new LocationRepository().InsertAsync(new LocationEntity { LocationName = "North Plant" });
        var repository = new LocationRepository();

        var layout = await repository.GetActiveLayoutByLocationIdAsync(locationId);

        Assert.NotNull(layout);
        Assert.Equal(locationId, layout.LocationId);
        Assert.Equal(locationId, layout.LocationId);
        Assert.Equal("North Plant Layout", layout.LayoutName);
        Assert.True(layout.IsActive);
        Assert.Equal(1920, layout.CanvasWidth);
        Assert.Equal(1080, layout.CanvasHeight);
    }

    internal static Task<SqliteTestDatabase> CreateLocationLayoutDatabaseAsync()
        => TestDatabaseScripts.CreateFromScriptAsync("CatoriServices", "Database", "Scripts", "Location", "CreateLocationLayoutTables.sql");
}
