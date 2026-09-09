using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class LearnedStepRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public LearnedStepRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task LearnedStepRepository_round_trips_step()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE LearnedStep (
                LearnedStepId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                StepNumber INTEGER NOT NULL,
                DisplayName TEXT NOT NULL,
                IsComplete INTEGER NOT NULL,
                TreasureStep TEXT NOT NULL,
                ParentName TEXT NOT NULL
            );
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new LearnedStepRepository();
        var id = await repository.InsertAsync(new LearnedStepEntity
        {
            Name = "StepA",
            StepNumber = 2,
            DisplayName = "Step A",
            IsComplete = false,
            TreasureStep = "TreasureA",
            ParentName = "Root"
        });

        var loaded = await repository.GetByIdAsync(id);
        Assert.NotNull(loaded);
        Assert.Equal("StepA", loaded.Name);

        loaded.IsComplete = true;
        Assert.Equal(1, await repository.UpdateAsync(loaded));

        var updated = await repository.GetByIdAsync(id);
        Assert.NotNull(updated);
        Assert.True(updated.IsComplete);
        Assert.Equal(1, await repository.DeleteAsync(id));
        Assert.Null(await repository.GetByIdAsync(id));
    }
}

