using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class ManufacturingRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public ManufacturingRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task ProductRepository_round_trips_product_and_filters_by_type()
    {
        using var db = await CreateManufacturingDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new ProductRepository();
        var id = await repository.InsertAsync(new ProductEntity
        {
            ProductName = "Finished Widget",
            ProductCode = "FW-100",
            ProductType = ProductType.Finished,
            UnitOfMeasure = "each",
            CostPerUnit = 12.50m,
            CreatedAt = new DateTime(2026, 1, 2, 3, 4, 5)
        });
        await repository.InsertAsync(new ProductEntity
        {
            ProductName = "Component Gear",
            ProductCode = "CG-100",
            ProductType = ProductType.Component,
            UnitOfMeasure = "each",
            CostPerUnit = 2.25m,
            CreatedAt = new DateTime(2026, 1, 2)
        });

        var byId = await repository.GetByIdAsync(id);
        var byCode = await repository.GetByCodeAsync("FW-100");
        var finished = await repository.GetByTypeAsync(ProductType.Finished);

        Assert.NotNull(byId);
        Assert.Equal("Finished Widget", byId.ProductName);
        Assert.NotNull(byCode);
        Assert.Equal(id, byCode.ProductId);
        var onlyFinished = Assert.Single(finished);
        Assert.Equal(ProductType.Finished, onlyFinished.ProductType);
    }

    [Fact]
    public async Task BillOfMaterialsRepository_returns_current_components_for_product()
    {
        using var db = await CreateManufacturingDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var products = new ProductRepository();
        var parentId = await products.InsertAsync(NewProduct("Assembly", "ASM-1", ProductType.Finished));
        var currentComponentId = await products.InsertAsync(NewProduct("Bolt", "BOLT-1", ProductType.Component));
        var expiredComponentId = await products.InsertAsync(NewProduct("Old Bolt", "BOLT-OLD", ProductType.Component));
        var repository = new BillOfMaterialsRepository();

        await repository.InsertAsync(new BillOfMaterialsEntity
        {
            ParentProductId = parentId,
            ComponentId = currentComponentId,
            Quantity = 4,
            ScrapFactor = 1,
            EffectiveDate = new DateTime(2026, 1, 1)
        });
        await repository.InsertAsync(new BillOfMaterialsEntity
        {
            ParentProductId = parentId,
            ComponentId = expiredComponentId,
            Quantity = 2,
            ScrapFactor = 0,
            EffectiveDate = new DateTime(2025, 1, 1),
            ExpiryDate = new DateTime(2025, 12, 31)
        });

        var currentComponents = await repository.GetComponentsForProductAsync(parentId, new DateTime(2026, 5, 7));
        var usage = await repository.GetUsageForComponentAsync(currentComponentId, new DateTime(2026, 5, 7));

        var component = Assert.Single(currentComponents);
        Assert.Equal(currentComponentId, component.ComponentId);
        Assert.Single(usage);
    }

    [Fact]
    public async Task LocationInventoryRepository_adjusts_and_totals_inventory()
    {
        using var db = await CreateManufacturingDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var productId = await new ProductRepository().InsertAsync(NewProduct("Part", "PART-1", ProductType.Component));
        var repository = new LocationInventoryRepository();
        var inventoryId = await repository.InsertAsync(new InventoryEntity
        {
            ProductId = productId,
            Location = "Warehouse A",
            QuantityOnHand = 10,
            LastUpdated = new DateTime(2026, 5, 7)
        });

        Assert.True(await repository.AdjustQuantityAsync(productId, "Warehouse A", 2.5m));

        var loaded = await repository.GetByIdAsync(inventoryId);
        var total = await repository.GetTotalQuantityForProductAsync(productId);

        Assert.NotNull(loaded);
        Assert.Equal(12.5m, loaded.QuantityOnHand);
        Assert.Equal(12.5m, total);
    }

    [Fact]
    public async Task ComponentRepository_round_trips_component_against_its_component_table()
    {
        using var db = await CreateManufacturingDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var repository = new ComponentRepository();
        var id = await repository.InsertAsync(new ComponentEntity
        {
            ComponentName = "Screw",
            Quantity = 100
        });

        var loaded = await repository.GetByIdAsync(id);
        Assert.NotNull(loaded);
        Assert.Equal("Screw", loaded.ComponentName);
        Assert.Equal(100, loaded.Quantity);

        loaded.ComponentName = "Machine Screw";
        loaded.Quantity = 125;
        Assert.True(await repository.UpdateAsync(loaded));

        var updated = await repository.GetByIdAsync(id);
        Assert.NotNull(updated);
        Assert.Equal("Machine Screw", updated.ComponentName);
        Assert.Equal(125, updated.Quantity);
    }

    internal static async Task<SqliteTestDatabase> CreateManufacturingDatabaseAsync()
    {
        var db = await TestDatabaseScripts.CreateFromScriptAsync("CatoriServices", "Database", "Scripts", "Manufacturing", "CreateManufacturingTables.sql");
        await db.ExecuteScriptAsync("""
            CREATE TABLE IF NOT EXISTS Components (
                component_id INTEGER PRIMARY KEY AUTOINCREMENT,
                component_name TEXT NOT NULL,
                quantity REAL NOT NULL
            );
            """);

        return db;
    }

    private static ProductEntity NewProduct(string name, string code, ProductType type)
        => new()
        {
            ProductName = name,
            ProductCode = code,
            ProductType = type,
            UnitOfMeasure = "each",
            CostPerUnit = 1,
            CreatedAt = new DateTime(2026, 5, 7)
        };
}
