using CatoriServices.Objects.database;
using CatoriServices.Objects.database.Production;
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

    [Fact]
    public async Task RuntimeProductionSchemaMigrator_preserves_legacy_products_and_converts_bom()
    {
        using var db=new SqliteTestDatabase();
        await using(var connection=new SqliteConnection("Data Source="+db.DatabasePath))
        {
            await connection.OpenAsync();
            await using var command=connection.CreateCommand();
            command.CommandText=@"CREATE TABLE Products(product_id INTEGER PRIMARY KEY,product_name TEXT NOT NULL,product_code TEXT NOT NULL,product_type TEXT NOT NULL,unit_of_measure TEXT,cost_per_unit REAL,created_at TEXT);
CREATE TABLE Bill_of_Materials(bom_id INTEGER PRIMARY KEY,parent_product_id INTEGER NOT NULL,component_id INTEGER NOT NULL,quantity REAL NOT NULL,scrap_factor REAL NOT NULL,effective_date TEXT,expiry_date TEXT);
CREATE TABLE LocationLayoutItem(LocationLayoutItemId INTEGER PRIMARY KEY,LocationId INTEGER NOT NULL,ItemName TEXT NOT NULL,ItemType TEXT NOT NULL,ItemDataJson TEXT);
INSERT INTO Products VALUES(1,'Finished Saw','SAW','Finished','each',20,datetime('now'));
INSERT INTO Products VALUES(2,'Saw Handle','HANDLE','Component','each',4,datetime('now'));
INSERT INTO Bill_of_Materials VALUES(1,1,2,1,0,date('now'),NULL);";
            await command.ExecuteNonQueryAsync();
        }

        RuntimeProductionSchemaMigrator.Migrate(db.DatabasePath);

        await using var verify=new SqliteConnection("Data Source="+db.DatabasePath);await verify.OpenAsync();
        Assert.Equal(2,await ScalarIntAsync(verify,"SELECT COUNT(*) FROM Parts;"));
        Assert.Equal(2,await ScalarIntAsync(verify,"SELECT COUNT(*) FROM Products WHERE part_id IS NOT NULL;"));
        Assert.Equal(1,await ScalarIntAsync(verify,"SELECT COUNT(*) FROM Bill_of_Materials WHERE parent_part_id IS NOT NULL AND child_part_id IS NOT NULL;"));
        Assert.True(await TableExistsAsync(db.DatabasePath,"Bill_of_Materials_Legacy"));
        Assert.True(await TableExistsAsync(db.DatabasePath,"LocationProductionAssetPart"));
        Assert.True(await TableExistsAsync(db.DatabasePath,"RobotPathAssignment"));
    }

    private static async Task<int> ScalarIntAsync(SqliteConnection connection,string sql)
    { await using var command=new SqliteCommand(sql,connection);return Convert.ToInt32(await command.ExecuteScalarAsync()); }

    private static async Task<bool> TableExistsAsync(string databasePath, string tableName)
    {
        await using var conn = new SqliteConnection("Data Source=" + databasePath);
        await conn.OpenAsync();
        await using var cmd = new SqliteCommand("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @Name", conn);
        cmd.Parameters.AddWithValue("@Name", tableName);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) == 1;
    }
}
