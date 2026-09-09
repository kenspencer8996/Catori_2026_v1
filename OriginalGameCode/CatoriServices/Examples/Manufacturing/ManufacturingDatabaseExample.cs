using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
using System;
using System.Threading.Tasks;
namespace CatoriServices.Examples.Manufacturing
{
    /// <summary>
    /// Example usage of the Manufacturing database tables
    /// </summary>
    public class ManufacturingDatabaseExample
    {
        private readonly string _connectionString;

        public ManufacturingDatabaseExample(string connectionString)
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
        /// Complete example demonstrating all CRUD operations
        /// </summary>
        public async Task RunExampleAsync()
        {
            try
            {
                            Console.WriteLine("=== Manufacturing Database Example ===\n");
                
                            // Step 1: Initialize database
                            Console.WriteLine("Step 1: Initializing database tables...");
                            var initializer = new ManufacturingDatabaseInitializer(_connectionString);
                            await initializer.InitializeTablesAsync();
                            Console.WriteLine("âœ“ Tables created successfully\n");
                
                            // Step 2: Create repositories
                            var productRepo = new ProductRepository();
                            var bomRepo = new BillOfMaterialsRepository();
                            var inventoryRepo = new LocationInventoryRepository();
                
                            // Step 3: Create products
                            Console.WriteLine("Step 2: Creating products...");
                
                            var widget = new ProductEntity
                            {
                                ProductName = "Premium Widget",
                                ProductCode = "PW-2025",
                                ProductType = ProductType.Finished,
                                UnitOfMeasure = "pcs",
                                CostPerUnit = 75.00m,
                                CreatedAt = DateTime.Now
                            };
                            int widgetId = await productRepo.InsertAsync(widget);
                            Console.WriteLine($"âœ“ Created finished product: {widget.ProductName} (ID: {widgetId})");
                
                            var frameComponent = new ProductEntity
                            {
                                ProductName = "Metal Frame",
                                ProductCode = "MF-100",
                                ProductType = ProductType.Component,
                                UnitOfMeasure = "pcs",
                                CostPerUnit = 15.00m,
                                CreatedAt = DateTime.Now
                            };
                            int frameId = await productRepo.InsertAsync(frameComponent);
                            Console.WriteLine($"âœ“ Created component: {frameComponent.ProductName} (ID: {frameId})");
                
                            var motorComponent = new ProductEntity
                            {
                                ProductName = "Electric Motor",
                                ProductCode = "EM-200",
                                ProductType = ProductType.Component,
                                UnitOfMeasure = "pcs",
                                CostPerUnit = 25.00m,
                                CreatedAt = DateTime.Now
                            };
                            int motorId = await productRepo.InsertAsync(motorComponent);
                            Console.WriteLine($"âœ“ Created component: {motorComponent.ProductName} (ID: {motorId})\n");
                
                            // Step 4: Create Bill of Materials
                            Console.WriteLine("Step 3: Creating Bill of Materials...");
                
                            var bomFrame = new BillOfMaterialsEntity
                            {
                                ParentProductId = widgetId,
                                ComponentId = frameId,
                                Quantity = 1.0m,
                                ScrapFactor = 2.0m,
                                EffectiveDate = DateTime.Now,
                                ExpiryDate = null
                            };
                            await bomRepo.InsertAsync(bomFrame);
                            Console.WriteLine($"âœ“ Added BOM: Widget needs 1x Frame (2% scrap)");
                
                            var bomMotor = new BillOfMaterialsEntity
                            {
                                ParentProductId = widgetId,
                                ComponentId = motorId,
                                Quantity = 2.0m,
                                ScrapFactor = 5.0m,
                                EffectiveDate = DateTime.Now,
                                ExpiryDate = null
                            };
                            await bomRepo.InsertAsync(bomMotor);
                            Console.WriteLine($"âœ“ Added BOM: Widget needs 2x Motor (5% scrap)\n");
                
                            // Step 5: Add inventory
                            Console.WriteLine("Step 4: Adding inventory...");
                
                            await inventoryRepo.InsertAsync(new InventoryEntity
                            {
                                ProductId = widgetId,
                                Location = "Main Warehouse",
                                QuantityOnHand = 50.0m,
                                LastUpdated = DateTime.Now
                            });
                            Console.WriteLine($"âœ“ Added inventory: 50 Widgets at Main Warehouse");
                
                            await inventoryRepo.InsertAsync(new InventoryEntity
                            {
                                ProductId = frameId,
                                Location = "Main Warehouse",
                                QuantityOnHand = 200.0m,
                                LastUpdated = DateTime.Now
                            });
                            Console.WriteLine($"âœ“ Added inventory: 200 Frames at Main Warehouse");
                
                            await inventoryRepo.InsertAsync(new InventoryEntity
                            {
                                ProductId = motorId,
                                Location = "Main Warehouse",
                                QuantityOnHand = 500.0m,
                                LastUpdated = DateTime.Now
                            });
                            Console.WriteLine($"âœ“ Added inventory: 500 Motors at Main Warehouse\n");
                
                            // Step 6: Perform queries
                            Console.WriteLine("Step 5: Running queries...");
                
                            // Get all components for the widget
                            var components = await bomRepo.GetComponentsForProductAsync(widgetId);
                            Console.WriteLine($"\nâœ“ Components needed for {widget.ProductName}:");
                            foreach (var comp in components)
                            {
                                var compProduct = await productRepo.GetByIdAsync(comp.ComponentId);
                                decimal withScrap = comp.Quantity * (1 + comp.ScrapFactor / 100m);
                                Console.WriteLine($"  - {compProduct?.ProductName}: {comp.Quantity} pcs " +
                                                $"(+{comp.ScrapFactor}% scrap = {withScrap} total)");
                            }
                
                            // Calculate material requirements for production
                            Console.WriteLine($"\nâœ“ Material requirements to build 100 widgets:");
                            foreach (var comp in components)
                            {
                                var compProduct = await productRepo.GetByIdAsync(comp.ComponentId);
                                decimal netQty = comp.Quantity * 100;
                                decimal scrapQty = netQty * (comp.ScrapFactor / 100m);
                                decimal totalQty = netQty + scrapQty;
                
                                var currentStock = await inventoryRepo.GetTotalQuantityForProductAsync(comp.ComponentId);
                                string availability = currentStock >= totalQty ? "âœ“ Available" : "âœ— Insufficient";
                
                                Console.WriteLine($"  - {compProduct?.ProductName}: {totalQty} needed " +
                                                $"({currentStock} in stock) {availability}");
                            }
                
                            // Get inventory summary
                            Console.WriteLine($"\nâœ“ Current inventory levels:");
                            var allInventory = await inventoryRepo.GetAllAsync();
                            foreach (var inv in allInventory)
                            {
                                var prod = await productRepo.GetByIdAsync(inv.ProductId);
                                Console.WriteLine($"  - {prod?.ProductName}: {inv.QuantityOnHand} {prod?.UnitOfMeasure} " +
                                                $"at {inv.Location}");
                            }
                
                            // Calculate total cost
                            Console.WriteLine($"\nâœ“ Cost breakdown for {widget.ProductName}:");
                            decimal totalCost = 0;
                            foreach (var comp in components)
                            {
                                var compProduct = await productRepo.GetByIdAsync(comp.ComponentId);
                                if (compProduct != null)
                                {
                                    decimal baseCost = compProduct.CostPerUnit * comp.Quantity;
                                    decimal scrapCost = baseCost * (comp.ScrapFactor / 100m);
                                    decimal componentTotalCost = baseCost + scrapCost;
                                    totalCost += componentTotalCost;
                
                                    Console.WriteLine($"  - {compProduct.ProductName}: " +
                                                    $"${compProduct.CostPerUnit} Ã— {comp.Quantity} " +
                                                    $"+ ${scrapCost:F2} scrap = ${componentTotalCost:F2}");
                                }
                            }
                            Console.WriteLine($"  Total material cost: ${totalCost:F2}");
                            Console.WriteLine($"  Selling price: ${widget.CostPerUnit}");
                            Console.WriteLine($"  Gross margin: ${widget.CostPerUnit - totalCost:F2} " +
                                            $"({((widget.CostPerUnit - totalCost) / widget.CostPerUnit * 100):F1}%)\n");
                
                            Console.WriteLine("=== Example completed successfully! ===");
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Example of inventory adjustment
        /// </summary>
        public async Task InventoryAdjustmentExampleAsync()
        {
            try
            {
                            var inventoryRepo = new LocationInventoryRepository();
                            var productRepo = new ProductRepository();
                
                            Console.WriteLine("\n=== Inventory Adjustment Example ===\n");
                
                            // Find a product
                            var products = await productRepo.GetByTypeAsync(ProductType.Component);
                            if (products.Count > 0)
                            {
                                var product = products[0];
                                string location = "Main Warehouse";
                
                                // Get current inventory
                                var inventory = await inventoryRepo.GetByProductAndLocationAsync(product.ProductId, location);
                                if (inventory != null)
                                {
                                    Console.WriteLine($"Current stock: {inventory.QuantityOnHand} {product.UnitOfMeasure}");
                
                                    // Simulate production consumption
                                    decimal consumed = 50.0m;
                                    await inventoryRepo.AdjustQuantityAsync(product.ProductId, location, -consumed);
                                    Console.WriteLine($"Consumed: {consumed} {product.UnitOfMeasure}");
                
                                    // Get updated inventory
                                    inventory = await inventoryRepo.GetByProductAndLocationAsync(product.ProductId, location);
                                    Console.WriteLine($"Updated stock: {inventory?.QuantityOnHand} {product.UnitOfMeasure}\n");
                                }
                            }
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}

