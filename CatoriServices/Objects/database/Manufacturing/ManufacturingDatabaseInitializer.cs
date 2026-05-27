using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Manufacturing
{
    /// <summary>
    /// Helper class to initialize manufacturing database tables
    /// </summary>
    public class ManufacturingDatabaseInitializer
    {
        private readonly string _connectionString;

        public ManufacturingDatabaseInitializer(string connectionString)
        {
            try
            {
                            _connectionString = connectionString;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Creates all manufacturing tables if they don't exist
        /// </summary>
        public async Task InitializeTablesAsync()
        {
            try
            {
                            using var conn = new SqliteConnection(_connectionString);
                            await conn.OpenAsync();
                
                            // Read and execute the SQL script
                            string scriptPath = Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                "Database",
                                "Scripts",
                                "Manufacturing",
                                "CreateManufacturingTables.sql");
                
                            if (File.Exists(scriptPath))
                            {
                                string sql = await File.ReadAllTextAsync(scriptPath);
                                using var cmd = new SqliteCommand(sql, conn);
                                await cmd.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                // Fallback: create tables inline if script file not found
                                await CreateTablesInlineAsync(conn);
                            }
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task CreateTablesInlineAsync(SqliteConnection conn)
        {
            try
            {
                            string createProductsTable = @"
                                CREATE TABLE IF NOT EXISTS Products (
                                    product_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    product_name TEXT NOT NULL,
                                    product_code TEXT NOT NULL UNIQUE,
                                    product_type TEXT NOT NULL CHECK(product_type IN ('Finished', 'Component')),
                                    unit_of_measure TEXT NOT NULL,
                                    cost_per_unit REAL NOT NULL DEFAULT 0.00,
                                    created_at TEXT NOT NULL DEFAULT (datetime('now')),
                                    CONSTRAINT chk_cost_positive CHECK(cost_per_unit >= 0)
                                );
                                CREATE INDEX IF NOT EXISTS idx_products_code ON Products(product_code);
                                CREATE INDEX IF NOT EXISTS idx_products_type ON Products(product_type);";
                
                            string createBomTable = @"
                                CREATE TABLE IF NOT EXISTS Bill_of_Materials (
                                    bom_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    parent_product_id INTEGER NOT NULL,
                                    component_id INTEGER NOT NULL,
                                    quantity REAL NOT NULL,
                                    scrap_factor REAL NOT NULL DEFAULT 0.00,
                                    effective_date TEXT NOT NULL DEFAULT (date('now')),
                                    expiry_date TEXT,
                                    FOREIGN KEY (parent_product_id) REFERENCES Products(product_id) ON DELETE CASCADE,
                                    FOREIGN KEY (component_id) REFERENCES Products(product_id) ON DELETE CASCADE,
                                    CONSTRAINT chk_quantity_positive CHECK(quantity > 0),
                                    CONSTRAINT chk_scrap_factor_range CHECK(scrap_factor >= 0 AND scrap_factor <= 100),
                                    CONSTRAINT chk_different_products CHECK(parent_product_id != component_id),
                                    CONSTRAINT chk_date_order CHECK(expiry_date IS NULL OR expiry_date >= effective_date)
                                );
                                CREATE INDEX IF NOT EXISTS idx_bom_parent ON Bill_of_Materials(parent_product_id);
                                CREATE INDEX IF NOT EXISTS idx_bom_component ON Bill_of_Materials(component_id);
                                CREATE INDEX IF NOT EXISTS idx_bom_dates ON Bill_of_Materials(effective_date, expiry_date);
                                CREATE UNIQUE INDEX IF NOT EXISTS idx_bom_unique ON Bill_of_Materials(parent_product_id, component_id, effective_date);";
                
                            string createInventoryTable = @"
                                CREATE TABLE IF NOT EXISTS Inventory (
                                    inventory_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    product_id INTEGER NOT NULL,
                                    location TEXT NOT NULL,
                                    quantity_on_hand REAL NOT NULL DEFAULT 0.00,
                                    last_updated TEXT NOT NULL DEFAULT (datetime('now')),
                                    FOREIGN KEY (product_id) REFERENCES Products(product_id) ON DELETE CASCADE,
                                    CONSTRAINT chk_quantity_non_negative CHECK(quantity_on_hand >= 0)
                                );
                                CREATE INDEX IF NOT EXISTS idx_inventory_product ON Inventory(product_id);
                                CREATE INDEX IF NOT EXISTS idx_inventory_location ON Inventory(location);
                                CREATE UNIQUE INDEX IF NOT EXISTS idx_inventory_unique ON Inventory(product_id, location);";
                
                            using (var cmd = new SqliteCommand(createProductsTable, conn))
                                await cmd.ExecuteNonQueryAsync();
                
                            using (var cmd = new SqliteCommand(createBomTable, conn))
                                await cmd.ExecuteNonQueryAsync();
                
                            using (var cmd = new SqliteCommand(createInventoryTable, conn))
                                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Inserts sample data for testing purposes
        /// </summary>
        public async Task InsertSampleDataAsync()
        {
            try
            {
                            var productRepo = new ProductRepository();
                            var bomRepo = new BillOfMaterialsRepository();
                            var inventoryRepo = new LocationInventoryRepository();
                
                            // Create sample products
                            var finishedProduct = new ProductEntity
                            {
                                ProductName = "Finished Widget",
                                ProductCode = "FW-001",
                                ProductType = ProductType.Finished,
                                UnitOfMeasure = "pcs",
                                CostPerUnit = 25.00m,
                                CreatedAt = DateTime.Now
                            };
                            int finishedId = await productRepo.InsertAsync(finishedProduct);
                
                            var componentA = new ProductEntity
                            {
                                ProductName = "Component A",
                                ProductCode = "CA-001",
                                ProductType = ProductType.Component,
                                UnitOfMeasure = "pcs",
                                CostPerUnit = 5.00m,
                                CreatedAt = DateTime.Now
                            };
                            int componentAId = await productRepo.InsertAsync(componentA);
                
                            var componentB = new ProductEntity
                            {
                                ProductName = "Component B",
                                ProductCode = "CB-001",
                                ProductType = ProductType.Component,
                                UnitOfMeasure = "pcs",
                                CostPerUnit = 3.50m,
                                CreatedAt = DateTime.Now
                            };
                            int componentBId = await productRepo.InsertAsync(componentB);
                
                            // Create BOM entries
                            await bomRepo.InsertAsync(new BillOfMaterialsEntity
                            {
                                ParentProductId = finishedId,
                                ComponentId = componentAId,
                                Quantity = 2.0m,
                                ScrapFactor = 5.0m,
                                EffectiveDate = DateTime.Now,
                                ExpiryDate = null
                            });
                
                            await bomRepo.InsertAsync(new BillOfMaterialsEntity
                            {
                                ParentProductId = finishedId,
                                ComponentId = componentBId,
                                Quantity = 1.0m,
                                ScrapFactor = 2.5m,
                                EffectiveDate = DateTime.Now,
                                ExpiryDate = null
                            });
                
                            // Create inventory entries
                            await inventoryRepo.InsertAsync(new InventoryEntity
                            {
                                ProductId = finishedId,
                                Location = "Warehouse A",
                                QuantityOnHand = 100.0m,
                                LastUpdated = DateTime.Now
                            });
                
                            await inventoryRepo.InsertAsync(new InventoryEntity
                            {
                                ProductId = componentAId,
                                Location = "Warehouse A",
                                QuantityOnHand = 500.0m,
                                LastUpdated = DateTime.Now
                            });
                
                            await inventoryRepo.InsertAsync(new InventoryEntity
                            {
                                ProductId = componentBId,
                                Location = "Warehouse B",
                                QuantityOnHand = 300.0m,
                                LastUpdated = DateTime.Now
                            });
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}



