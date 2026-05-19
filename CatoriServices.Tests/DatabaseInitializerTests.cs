using CatoriServices.Objects.database;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class DatabaseInitializerTests
{
    private readonly GlobalTestState _globalState;

    public DatabaseInitializerTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task LocationLayoutDatabaseInitializer_creates_live_layout_tables()
    {
        using var db = new SqliteTestDatabase();
        _globalState.UseDatabase(db.DatabasePath);

        await new LocationLayoutDatabaseInitializer("Data Source=" + db.DatabasePath).InitializeTablesAsync();

        Assert.True(await TableExistsAsync(db.DatabasePath, "Location"));
        Assert.True(await TableExistsAsync(db.DatabasePath, "LocationLayoutItem"));
        Assert.True(await TableExistsAsync(db.DatabasePath, "LocationLayoutPoint"));
    }

    [Fact]
    public async Task ManufacturingDatabaseInitializer_creates_manufacturing_tables()
    {
        using var db = new SqliteTestDatabase();
        _globalState.UseDatabase(db.DatabasePath);

        await new ManufacturingDatabaseInitializer("Data Source=" + db.DatabasePath).InitializeTablesAsync();

        Assert.True(await TableExistsAsync(db.DatabasePath, "Products"));
        Assert.True(await TableExistsAsync(db.DatabasePath, "Bill_of_Materials"));
        Assert.True(await TableExistsAsync(db.DatabasePath, "Inventory"));
    }

    private static async Task<bool> TableExistsAsync(string databasePath, string tableName)
    {
        await using var conn = new SqliteConnection("Data Source=" + databasePath);
        await conn.OpenAsync();
        await using var cmd = new SqliteCommand("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @Name", conn);
        cmd.Parameters.AddWithValue("@Name", tableName);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) == 1;
    }
}
