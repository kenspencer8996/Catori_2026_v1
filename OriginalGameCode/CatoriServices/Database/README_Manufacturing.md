# Manufacturing Database - SQLite Tables

This document describes the manufacturing database schema and provides usage examples for the Products, Bill of Materials, and Inventory tables.

## Overview

The manufacturing database consists of three main tables:

1. **Products** - Stores finished goods and components
2. **Bill_of_Materials** - Defines parent-child relationships between products
3. **Inventory** - Tracks stock levels by location

## Database Schema

### Products Table

| Field Name      | Data Type         | Description                           |
|-----------------|-------------------|---------------------------------------|
| product_id      | INTEGER (PK)      | Unique identifier (auto-increment)    |
| product_name    | TEXT              | Name of the product/component         |
| product_code    | TEXT (UNIQUE)     | SKU or internal code                  |
| product_type    | TEXT (ENUM)       | 'Finished' or 'Component'             |
| unit_of_measure | TEXT              | e.g., pcs, kg, m                      |
| cost_per_unit   | REAL              | Cost of one unit                      |
| created_at      | TEXT (DATETIME)   | Record creation timestamp             |

**Indexes:**
- `idx_products_code` on `product_code`
- `idx_products_type` on `product_type`

### Bill_of_Materials Table

| Field Name         | Data Type       | Description                              |
|--------------------|-----------------|------------------------------------------|
| bom_id             | INTEGER (PK)    | Unique BOM record ID (auto-increment)    |
| parent_product_id  | INTEGER (FK)    | Finished product ID                      |
| component_id       | INTEGER (FK)    | Component product ID                     |
| quantity           | REAL            | Quantity of component needed             |
| scrap_factor       | REAL            | % extra to account for waste (0-100)     |
| effective_date     | TEXT (DATE)     | Start date for BOM validity              |
| expiry_date        | TEXT (DATE)     | End date for BOM validity (nullable)     |

**Indexes:**
- `idx_bom_parent` on `parent_product_id`
- `idx_bom_component` on `component_id`
- `idx_bom_dates` on `effective_date, expiry_date`
- `idx_bom_unique` (UNIQUE) on `parent_product_id, component_id, effective_date`

**Constraints:**
- Prevents parent and component from being the same product
- Ensures expiry_date >= effective_date
- Quantity must be positive
- Scrap factor must be 0-100%

### Inventory Table

| Field Name       | Data Type       | Description                              |
|------------------|-----------------|------------------------------------------|
| inventory_id     | INTEGER (PK)    | Unique inventory record (auto-increment) |
| product_id       | INTEGER (FK)    | Product/component ID                     |
| location         | TEXT            | Warehouse or storage location            |
| quantity_on_hand | REAL            | Current stock quantity                   |
| last_updated     | TEXT (DATETIME) | Last update timestamp                    |

**Indexes:**
- `idx_inventory_product` on `product_id`
- `idx_inventory_location` on `location`
- `idx_inventory_unique` (UNIQUE) on `product_id, location`

**Constraints:**
- Quantity must be non-negative
- One inventory record per product per location

## Setup and Initialization

### 1. Initialize Tables

```csharp
using CatoriServices.Objects.database;

// Using your existing connection string pattern
string connectionString = $"Data Source={GlobalServices.Database}";

var initializer = new ManufacturingDatabaseInitializer(connectionString);
await initializer.InitializeTablesAsync();
```

### 2. Insert Sample Data (Optional)

```csharp
await initializer.InsertSampleDataAsync();
```

## Usage Examples

### Working with Products

```csharp
using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

string connectionString = $"Data Source={GlobalServices.Database}";
var productRepo = new ProductRepository(connectionString);

// Create a new finished product
var finishedProduct = new ProductEntity
{
    ProductName = "Custom Widget",
    ProductCode = "CW-100",
    ProductType = ProductType.Finished,
    UnitOfMeasure = "pcs",
    CostPerUnit = 50.00m,
    CreatedAt = DateTime.Now
};
int productId = await productRepo.InsertAsync(finishedProduct);

// Get product by ID
var product = await productRepo.GetByIdAsync(productId);

// Get product by code
var productByCode = await productRepo.GetByCodeAsync("CW-100");

// Get all finished products
var finishedProducts = await productRepo.GetByTypeAsync(ProductType.Finished);

// Get all components
var components = await productRepo.GetByTypeAsync(ProductType.Component);

// Update product
product.CostPerUnit = 55.00m;
await productRepo.UpdateAsync(product);

// Delete product
await productRepo.DeleteAsync(productId);
```

### Working with Bill of Materials

```csharp
var bomRepo = new BillOfMaterialsRepository(connectionString);

// Create BOM entry (Widget needs 2x Component A)
var bomEntry = new BillOfMaterialsEntity
{
    ParentProductId = 1,  // Finished Widget
    ComponentId = 2,      // Component A
    Quantity = 2.0m,
    ScrapFactor = 5.0m,   // 5% waste factor
    EffectiveDate = DateTime.Now,
    ExpiryDate = null     // No expiry (current BOM)
};
int bomId = await bomRepo.InsertAsync(bomEntry);

// Get all components needed for a product
var components = await bomRepo.GetComponentsForProductAsync(parentProductId: 1);

// Get all products that use a specific component
var usages = await bomRepo.GetUsageForComponentAsync(componentId: 2);

// Get BOM as of specific date
var historicalBom = await bomRepo.GetComponentsForProductAsync(
    parentProductId: 1, 
    asOfDate: new DateTime(2024, 1, 1)
);

// Calculate total material needed (with scrap)
foreach (var component in components)
{
    decimal netQuantity = component.Quantity;
    decimal scrapQuantity = netQuantity * (component.ScrapFactor / 100m);
    decimal totalNeeded = netQuantity + scrapQuantity;

    Console.WriteLine($"Component {component.ComponentId}: " +
                     $"{netQuantity} + {scrapQuantity} scrap = {totalNeeded} total");
}
```

### Working with Inventory

```csharp
var inventoryRepo = new InventoryRepository(connectionString);

// Add initial inventory
var inventory = new InventoryEntity
{
    ProductId = 1,
    Location = "Warehouse A",
    QuantityOnHand = 1000.0m,
    LastUpdated = DateTime.Now
};
int inventoryId = await inventoryRepo.InsertAsync(inventory);

// Get inventory for a specific product at a location
var stock = await inventoryRepo.GetByProductAndLocationAsync(
    productId: 1, 
    location: "Warehouse A"
);

// Get all inventory locations for a product
var allLocations = await inventoryRepo.GetByProductAsync(productId: 1);

// Get total quantity across all locations
decimal totalQty = await inventoryRepo.GetTotalQuantityForProductAsync(productId: 1);

// Adjust inventory (add or subtract)
await inventoryRepo.AdjustQuantityAsync(
    productId: 1,
    location: "Warehouse A",
    quantityChange: -50.0m  // Negative for consumption, positive for receipt
);

// Get all inventory at a location
var warehouseStock = await inventoryRepo.GetByLocationAsync("Warehouse A");
```

## Advanced Scenarios

### Material Requirements Planning (MRP)

```csharp
// Calculate materials needed to build X units of a product
async Task<Dictionary<int, decimal>> CalculateMaterialRequirements(
    int finishedProductId, 
    decimal quantityToBuild)
{
    var bomRepo = new BillOfMaterialsRepository(connectionString);
    var requirements = new Dictionary<int, decimal>();

    // Get BOM for the product
    var bom = await bomRepo.GetComponentsForProductAsync(finishedProductId);

    foreach (var component in bom)
    {
        // Calculate net quantity
        decimal netQty = component.Quantity * quantityToBuild;

        // Add scrap allowance
        decimal scrapAllowance = netQty * (component.ScrapFactor / 100m);
        decimal totalRequired = netQty + scrapAllowance;

        requirements[component.ComponentId] = totalRequired;
    }

    return requirements;
}

// Usage
var materials = await CalculateMaterialRequirements(
    finishedProductId: 1, 
    quantityToBuild: 100
);
```

### Inventory Availability Check

```csharp
async Task<bool> CheckInventoryAvailability(
    int productId, 
    decimal requiredQuantity, 
    string location)
{
    var inventoryRepo = new InventoryRepository(connectionString);
    var inventory = await inventoryRepo.GetByProductAndLocationAsync(productId, location);

    return inventory != null && inventory.QuantityOnHand >= requiredQuantity;
}
```

### Product Cost Rollup

```csharp
// Calculate total cost of a finished product based on component costs
async Task<decimal> CalculateProductCost(int finishedProductId)
{
    var bomRepo = new BillOfMaterialsRepository(connectionString);
    var productRepo = new ProductRepository(connectionString);

    var components = await bomRepo.GetComponentsForProductAsync(finishedProductId);
    decimal totalCost = 0;

    foreach (var bomEntry in components)
    {
        var component = await productRepo.GetByIdAsync(bomEntry.ComponentId);
        if (component != null)
        {
            decimal componentCost = component.CostPerUnit * bomEntry.Quantity;

            // Include scrap cost
            decimal scrapCost = componentCost * (bomEntry.ScrapFactor / 100m);

            totalCost += componentCost + scrapCost;
        }
    }

    return totalCost;
}
```

## Database Maintenance

### Backing up Data

```bash
# SQLite database can be backed up by simply copying the file
copy CatoriCity.db CatoriCity_backup_20250101.db
```

### Querying from SQL

```sql
-- Get all finished products with low inventory
SELECT 
    p.product_name,
    p.product_code,
    i.location,
    i.quantity_on_hand
FROM Products p
LEFT JOIN Inventory i ON p.product_id = i.product_id
WHERE p.product_type = 'Finished'
AND (i.quantity_on_hand < 50 OR i.quantity_on_hand IS NULL)
ORDER BY p.product_name;

-- Get BOM explosion (multi-level)
-- Shows all components needed for a finished product
WITH RECURSIVE bom_explosion AS (
    SELECT 
        parent_product_id,
        component_id,
        quantity,
        1 as level
    FROM Bill_of_Materials
    WHERE parent_product_id = 1  -- Replace with your product ID

    UNION ALL

    SELECT 
        b.parent_product_id,
        b.component_id,
        b.quantity * be.quantity,
        be.level + 1
    FROM Bill_of_Materials b
    INNER JOIN bom_explosion be ON b.parent_product_id = be.component_id
)
SELECT 
    level,
    p.product_name,
    p.product_code,
    be.quantity
FROM bom_explosion be
INNER JOIN Products p ON be.component_id = p.product_id
ORDER BY level, p.product_name;
```

## Notes

- All date/time fields in SQLite are stored as TEXT in ISO 8601 format
- REAL type is used for decimal quantities (SQLite doesn't have native DECIMAL)
- Foreign key constraints are enforced with CASCADE DELETE
- Unique indexes prevent duplicate entries
- Check constraints validate data integrity

## Integration with Existing Code

To integrate with your existing database infrastructure:

```csharp
// In your GlobalServices or startup code
public static class ManufacturingServices
{
    private static ProductRepository? _productRepository;
    private static BillOfMaterialsRepository? _bomRepository;
    private static InventoryRepository? _inventoryRepository;

    public static ProductRepository GetProductRepository()
    {
        if (_productRepository == null)
        {
            string connectionString = $"Data Source={GlobalServices.Database}";
            _productRepository = new ProductRepository(connectionString);
        }
        return _productRepository;
    }

    public static BillOfMaterialsRepository GetBomRepository()
    {
        if (_bomRepository == null)
        {
            string connectionString = $"Data Source={GlobalServices.Database}";
            _bomRepository = new BillOfMaterialsRepository(connectionString);
        }
        return _bomRepository;
    }

    public static InventoryRepository GetInventoryRepository()
    {
        if (_inventoryRepository == null)
        {
            string connectionString = $"Data Source={GlobalServices.Database}";
            _inventoryRepository = new InventoryRepository(connectionString);
        }
        return _inventoryRepository;
    }
}
```
