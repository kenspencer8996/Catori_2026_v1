
namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class ShopItemRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public ShopItemRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task GetShopItemsAsync_reads_shop_items_without_xy_columns()
    {
        using var db = await CreateShopItemDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new ShopItemRepository();

        var items = await repository.GetShopItemsAsync();

        Assert.Collection(
            items,
            first =>
            {
                Assert.Equal(1, first.ShopItemId);
                Assert.Equal("Robot Arm", first.Name);
                Assert.Equal("Location", first.StoreType);
                Assert.Equal(49.95m, first.Price);
                Assert.Equal(48, first.Height);
                Assert.Equal(64, first.Width);
                Assert.Equal(15, first.RotationDegree);
            },
            second =>
            {
                Assert.Equal(2, second.ShopItemId);
                Assert.Equal("Table", second.Name);
                Assert.Equal("Location", second.StoreType);
            });
    }

    [Fact]
    public async Task GetShopItemByNameAsync_reads_matching_shop_item()
    {
        using var db = await CreateShopItemDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new ShopItemRepository();

        var item = await repository.GetShopItemByNameAsync("Table");

        Assert.Equal(2, item.ShopItemId);
        Assert.Equal("Table", item.Name);
        Assert.Equal("table.png", item.ImageName);
        Assert.Equal("C:\\Images\\table.png", item.FilePath);
        Assert.Equal(25.50m, item.Price);
    }

    private static async Task<SqliteTestDatabase> CreateShopItemDatabaseAsync()
    {
        var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE ShopItem (
                ShopItemId INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                StoreType TEXT NOT NULL,
                FilePath TEXT NOT NULL,
                Description TEXT NOT NULL,
                ImageName TEXT NOT NULL,
                Price NUMERIC NOT NULL,
                Height REAL NOT NULL,
                RotationDegree REAL NOT NULL,
                Width REAL NOT NULL,
                Quantity INTEGER NOT NULL DEFAULT 0
            );

            INSERT INTO ShopItem
                (ShopItemId, Name, StoreType, FilePath, Description, ImageName, Price, Height, RotationDegree, Width, Quantity)
            VALUES
                (1, 'Robot Arm', 'Location', 'C:\Images\robot.png', 'Robot item', 'robot.png', 49.95, 48, 15, 64, 1),
                (2, 'Table', 'Location', 'C:\Images\table.png', 'Table item', 'table.png', 25.50, 32, 0, 72, 1);
            """);

        return db;
    }
}

