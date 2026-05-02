using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatoriServices.Objects.database
{
    public class BillOfMaterialsRepository
    {
        private string _connectionString;

        public BillOfMaterialsRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<BillOfMaterialsEntity?> GetByIdAsync(int bomId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Bill_of_Materials WHERE bom_id = @BomId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BomId", bomId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return MapBillOfMaterials(reader);
        }

        public async Task<List<BillOfMaterialsEntity>> GetComponentsForProductAsync(int parentProductId, DateTime? asOfDate = null)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                SELECT * FROM Bill_of_Materials 
                WHERE parent_product_id = @ParentProductId
                AND effective_date <= @AsOfDate
                AND (expiry_date IS NULL OR expiry_date >= @AsOfDate)
                ORDER BY component_id";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ParentProductId", parentProductId);
            cmd.Parameters.AddWithValue("@AsOfDate", (asOfDate ?? DateTime.Now).ToString("yyyy-MM-dd"));

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<BillOfMaterialsEntity>();
            while (await reader.ReadAsync())
                list.Add(MapBillOfMaterials(reader));

            return list;
        }

        public async Task<List<BillOfMaterialsEntity>> GetUsageForComponentAsync(int componentId, DateTime? asOfDate = null)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                SELECT * FROM Bill_of_Materials 
                WHERE component_id = @ComponentId
                AND effective_date <= @AsOfDate
                AND (expiry_date IS NULL OR expiry_date >= @AsOfDate)
                ORDER BY parent_product_id";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ComponentId", componentId);
            cmd.Parameters.AddWithValue("@AsOfDate", (asOfDate ?? DateTime.Now).ToString("yyyy-MM-dd"));

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<BillOfMaterialsEntity>();
            while (await reader.ReadAsync())
                list.Add(MapBillOfMaterials(reader));

            return list;
        }

        public async Task<List<BillOfMaterialsEntity>> GetAllAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Bill_of_Materials ORDER BY parent_product_id, component_id";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<BillOfMaterialsEntity>();
            while (await reader.ReadAsync())
                list.Add(MapBillOfMaterials(reader));

            return list;
        }

        public async Task<int> InsertAsync(BillOfMaterialsEntity bom)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO Bill_of_Materials (parent_product_id, component_id, quantity, scrap_factor, effective_date, expiry_date)
                VALUES (@ParentProductId, @ComponentId, @Quantity, @ScrapFactor, @EffectiveDate, @ExpiryDate);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ParentProductId", bom.ParentProductId);
            cmd.Parameters.AddWithValue("@ComponentId", bom.ComponentId);
            cmd.Parameters.AddWithValue("@Quantity", bom.Quantity);
            cmd.Parameters.AddWithValue("@ScrapFactor", bom.ScrapFactor);
            cmd.Parameters.AddWithValue("@EffectiveDate", bom.EffectiveDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@ExpiryDate", bom.ExpiryDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(BillOfMaterialsEntity bom)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE Bill_of_Materials 
                SET parent_product_id = @ParentProductId,
                    component_id = @ComponentId,
                    quantity = @Quantity,
                    scrap_factor = @ScrapFactor,
                    effective_date = @EffectiveDate,
                    expiry_date = @ExpiryDate
                WHERE bom_id = @BomId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BomId", bom.BomId);
            cmd.Parameters.AddWithValue("@ParentProductId", bom.ParentProductId);
            cmd.Parameters.AddWithValue("@ComponentId", bom.ComponentId);
            cmd.Parameters.AddWithValue("@Quantity", bom.Quantity);
            cmd.Parameters.AddWithValue("@ScrapFactor", bom.ScrapFactor);
            cmd.Parameters.AddWithValue("@EffectiveDate", bom.EffectiveDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@ExpiryDate", bom.ExpiryDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int bomId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "DELETE FROM Bill_of_Materials WHERE bom_id = @BomId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BomId", bomId);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private BillOfMaterialsEntity MapBillOfMaterials(SqliteDataReader reader)
        {
            return new BillOfMaterialsEntity
            {
                BomId = reader.GetInt32(reader.GetOrdinal("bom_id")),
                ParentProductId = reader.GetInt32(reader.GetOrdinal("parent_product_id")),
                ComponentId = reader.GetInt32(reader.GetOrdinal("component_id")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("quantity")),
                ScrapFactor = reader.GetDecimal(reader.GetOrdinal("scrap_factor")),
                EffectiveDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("effective_date"))),
                ExpiryDate = reader.IsDBNull(reader.GetOrdinal("expiry_date")) 
                    ? null 
                    : DateTime.Parse(reader.GetString(reader.GetOrdinal("expiry_date")))
            };
        }
    }
}
