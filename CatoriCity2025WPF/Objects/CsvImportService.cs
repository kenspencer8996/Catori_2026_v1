using System.Globalization;
using System.IO;
using CatoriCity2025WPF.Objects;

public static class CsvImportService
{
    // ---------------------------
    // Products
    // ---------------------------
    public static List<ProductViewModel> LoadProducts(string filePath)
    {
        var list = new List<ProductViewModel>();
        var lines = File.ReadAllLines(filePath);

        if (lines.Length <= 1) return list;

        foreach (var line in lines.Skip(1))
        {
            var cols = line.Split(',');

            if (cols.Length < 5) continue;

            string prodtypestring = cols[2].Trim();
            ProductType productType = ProductType.Component;
            switch (prodtypestring)
            {
                case "Finished":
                    productType = ProductType.Finished;
                    break;
                case "Component":
                    productType = ProductType.Component;
                    break;
                default:
                    break;
            }
        
            list.Add(new ProductViewModel
            {
                ProductName = cols[0].Trim(),
                ProductCode = cols[1].Trim(),
                ProductType = productType,
                UnitOfMeasure = cols[3].Trim(),
                CostPerUnit = ParseDecimal(cols[4])
            });
        }

        return list;
    }

    // ---------------------------
    // BOM
    // ---------------------------
    public static List<BomItemViewModel> LoadBomItems(string filePath)
    {
        var list = new List<BomItemViewModel>();
        var lines = File.ReadAllLines(filePath);

        if (lines.Length <= 1) return list;

        foreach (var line in lines.Skip(1))
        {
            var cols = line.Split(',');

            if (cols.Length < 4) continue;

            list.Add(new BomItemViewModel
            {
                // You will resolve IDs later using ProductCode mapping
                ComponentCode = cols[1].Trim(),
                Quantity = ParseDecimal(cols[2]),
                ScrapFactor = ParseDecimal(cols[3]),
                EffectiveDate = DateTime.Today
            });
        }

        return list;
    }

    // ---------------------------
    // Inventory
    // ---------------------------
    public static List<InventoryItemViewModel> LoadInventory(string filePath)
    {
        var list = new List<InventoryItemViewModel>();
        var lines = File.ReadAllLines(filePath);

        if (lines.Length <= 1) return list;

        foreach (var line in lines.Skip(1))
        {
            var cols = line.Split(',');

            if (cols.Length < 3) continue;

            list.Add(new InventoryItemViewModel
            {
                ProductName = cols[0].Trim(),
                Location = cols[1].Trim(),
                QuantityOnHand = ParseDecimal(cols[2]),
                LastUpdated = DateTime.Now
            });
        }

        return list;
    }

    // ---------------------------
    // Components
    // ---------------------------
    public static List<ComponentViewModel> LoadComponents(string filePath)
    {
        var list = new List<ComponentViewModel>();
        var lines = File.ReadAllLines(filePath);

        if (lines.Length <= 1) return list;

        foreach (var line in lines.Skip(1))
        {
            var cols = line.Split(',');

            if (cols.Length < 2) continue;

            list.Add(new ComponentViewModel
            {
                ComponentName = cols[0].Trim(),
                Quantity = ParseDecimal(cols[1])
            });
        }

        return list;
    }

    // ---------------------------
    // Helper
    // ---------------------------
    private static decimal ParseDecimal(string input)
    {
        if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            return value;

        return 0;
    }
}