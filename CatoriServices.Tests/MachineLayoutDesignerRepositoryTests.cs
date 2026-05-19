using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class MachineLayoutDesignerRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public MachineLayoutDesignerRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task SaveAsync_inserts_and_loads_machine_layout_designer_by_name()
    {
        using var db = await CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var locationId = await CreateLocationAsync("Location A");
        var repository = new MachineLayoutDesignerRepository();

        var id = await repository.SaveAsync(new MachineLayoutDesignerEntity
        {
            LocationId = locationId,
            SequenceName = "PickupPartA",
            SelectionX = 10,
            SelectionY = 20,
            SelectionWidth = 300,
            SelectionHeight = 200,
            RobotX = 45,
            RobotY = 55,
            RobotWidth = 125,
            RobotHeight = 135
        });

        var loaded = await repository.GetByNameAsync("PickupPartA");

        Assert.True(id > 0);
        Assert.NotNull(loaded);
        Assert.Equal(id, loaded.MachineLayoutDesignerId);
        Assert.Equal(locationId, loaded.LocationId);
        Assert.Equal(10, loaded.SelectionX);
        Assert.Equal(20, loaded.SelectionY);
        Assert.Equal(300, loaded.SelectionWidth);
        Assert.Equal(200, loaded.SelectionHeight);
        Assert.Equal(45, loaded.RobotX);
        Assert.Equal(55, loaded.RobotY);
        Assert.Equal(125, loaded.RobotWidth);
        Assert.Equal(135, loaded.RobotHeight);
    }

    [Fact]
    public async Task SaveAsync_updates_existing_machine_layout_designer_with_same_name()
    {
        using var db = await CreateMachineLayoutDesignerDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var locationId = await CreateLocationAsync("Location B");
        var repository = new MachineLayoutDesignerRepository();

        var firstId = await repository.SaveAsync(new MachineLayoutDesignerEntity
        {
            LocationId = locationId,
            SequenceName = "SharedSequence",
            RobotX = 100
        });

        var secondId = await repository.SaveAsync(new MachineLayoutDesignerEntity
        {
            LocationId = locationId,
            SequenceName = "SharedSequence",
            RobotX = 250
        });

        var all = await repository.GetAllAsync();
        var loaded = await repository.GetByNameAsync("SharedSequence");

        Assert.Equal(firstId, secondId);
        Assert.Single(all);
        Assert.NotNull(loaded);
        Assert.Equal(locationId, loaded.LocationId);
        Assert.Equal(250, loaded.RobotX);
    }

    internal static async Task<SqliteTestDatabase> CreateMachineLayoutDesignerDatabaseAsync()
    {
        var db = await LocationRepositoryTests.CreateLocationLayoutDatabaseAsync();
        var scriptPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "CatoriServices",
            "Database",
            "Scripts",
            "MachineLayoutDesigner",
            "CreateMachineLayoutDesignerTables.sql"));

        await db.ExecuteScriptAsync(await File.ReadAllTextAsync(scriptPath));
        return db;
    }

    internal static async Task<long> CreateLocationAsync(string locationName)
    {
        var locationRepository = new LocationRepository();
        return await locationRepository.InsertAsync(new LocationEntity { LocationName = locationName });
    }
}
