using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class MachineLayoutDesignerRepositoryTests
{
    private readonly GlobalTestState _globalState;
    public MachineLayoutDesignerRepositoryTests(GlobalTestState globalState) => _globalState = globalState;

    [Fact]
    public async Task SaveAsync_round_trips_selection_by_location()
    {
        using var db = await CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);
        var locationId = await CreateLocationAsync("Location A");
        var repository = new MachineLayoutDesignerRepository();
        var id = await repository.SaveAsync(new MachineLayoutDesignerEntity { LocationId = locationId, SelectionX = 10, SelectionY = 20, SelectionWidth = 300, SelectionHeight = 200 });
        var loaded = await repository.GetByLocationIdAsync(locationId);
        Assert.True(id > 0);
        Assert.NotNull(loaded);
        Assert.Equal(10, loaded.SelectionX);
        Assert.Equal(300, loaded.SelectionWidth);
    }

    internal static async Task<SqliteTestDatabase> CreateMachineLayoutDesignerDatabaseAsync()
    {
        var db = await LocationRepositoryTests.CreateLocationLayoutDatabaseAsync();
        var scriptPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CatoriServices", "Database", "Scripts", "MachineLayoutDesigner", "CreateMachineLayoutDesignerTables.sql"));
        await db.ExecuteScriptAsync(await File.ReadAllTextAsync(scriptPath));
        return db;
    }

    internal static async Task<long> CreateLocationAsync(string locationName)
    {
        var locationRepository = new LocationRepository();
        return await locationRepository.InsertAsync(new LocationEntity { LocationName = locationName });
    }
}
