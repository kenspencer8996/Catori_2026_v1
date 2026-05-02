using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatoriServices.Objects.database
{
    public class FactoryInventoryRepository
    {
        private readonly string _connectionString;

        public FactoryInventoryRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<InventoryEntity?> GetByIdAsync(int inventoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Inventory WHERE inventory_id = @InventoryId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@InventoryId", inventoryId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return MapInventory(reader);
        }

        public async Task<InventoryEntity?> GetByProductAndLocationAsync(int productId, string location)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Inventory WHERE product_id = @ProductId AND location = @Location";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Location", location);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return MapInventory(reader);
        }

        public async Task<List<InventoryEntity>> GetByProductAsync(int productId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Inventory WHERE product_id = @ProductId ORDER BY location";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProductId", productId);

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<InventoryEntity>();
            while (await reader.ReadAsync())
                list.Add(MapInventory(reader));

            return list;
        }

        public async Task<List<InventoryEntity>> GetByLocationAsync(string location)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Inventory WHERE location = @Location ORDER BY product_id";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Location", location);

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<InventoryEntity>();
            while (await reader.ReadAsync())
                list.Add(MapInventory(reader));

            return list;
        }

        public async Task<decimal> GetTotalQuantityForProductAsync(int productId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT COALESCE(SUM(quantity_on_hand), 0) FROM Inventory WHERE product_id = @ProductId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProductId", productId);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToDecimal(result);
        }

        public async Task<List<InventoryEntity>> GetAllAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Inventory ORDER BY location, product_id";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<InventoryEntity>();
            while (await reader.ReadAsync())
                list.Add(MapInventory(reader));

            return list;
        }

        public async Task<int> InsertAsync(InventoryEntity inventory)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO Inventory (product_id, location, quantity_on_hand, last_updated)
                VALUES (@ProductId, @Location, @QuantityOnHand, @LastUpdated);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProductId", inventory.ProductId);
            cmd.Parameters.AddWithValue("@Location", inventory.Location);
            cmd.Parameters.AddWithValue("@QuantityOnHand", inventory.QuantityOnHand);
            cmd.Parameters.AddWithValue("@LastUpdated", inventory.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"));

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(InventoryEntity inventory)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE Inventory 
                SET product_id = @ProductId,
                    location = @Location,
                    quantity_on_hand = @QuantityOnHand,
                    last_updated = @LastUpdated
                WHERE inventory_id = @InventoryId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@InventoryId", inventory.InventoryId);
            cmd.Parameters.AddWithValue("@ProductId", inventory.ProductId);
            cmd.Parameters.AddWithValue("@Location", inventory.Location);
            cmd.Parameters.AddWithValue("@QuantityOnHand", inventory.QuantityOnHand);
            cmd.Parameters.AddWithValue("@LastUpdated", inventory.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"));

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> AdjustQuantityAsync(int productId, string location, decimal quantityChange)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE Inventory 
                SET quantity_on_hand = quantity_on_hand + @QuantityChange,
                    last_updated = @LastUpdated
                WHERE product_id = @ProductId AND location = @Location";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Location", location);
            cmd.Parameters.AddWithValue("@QuantityChange", quantityChange);
            cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int inventoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "DELETE FROM Inventory WHERE inventory_id = @InventoryId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@InventoryId", inventoryId);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private InventoryEntity MapInventory(SqliteDataReader reader)
        {
            return new InventoryEntity
            {
                InventoryId = reader.GetInt32(reader.GetOrdinal("inventory_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                Location = reader.GetString(reader.GetOrdinal("location")),
                QuantityOnHand = reader.GetDecimal(reader.GetOrdinal("quantity_on_hand")),
                LastUpdated = DateTime.Parse(reader.GetString(reader.GetOrdinal("last_updated")))
            };
        }
    }
}
