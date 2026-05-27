using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CatoriServices.Objects.database.Manufacturing
{
    public class ProductRepository
    {
        private string _connectionString;

        public ProductRepository()
        {
            try
            {
                            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<ProductEntity?> GetByIdAsync(int productId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            string sql = "SELECT * FROM Products WHERE product_id = @ProductId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ProductId", productId);
                
                            using var reader = await cmd.ExecuteReaderAsync();
                            if (!await reader.ReadAsync()) return null;
                
                            return MapProduct(reader);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<ProductEntity?> GetByCodeAsync(string productCode)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            string sql = "SELECT * FROM Products WHERE product_code = @ProductCode";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ProductCode", productCode);
                
                            using var reader = await cmd.ExecuteReaderAsync();
                            if (!await reader.ReadAsync()) return null;
                
                            return MapProduct(reader);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<ProductEntity>> GetAllAsync()
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            string sql = "SELECT * FROM Products ORDER BY product_name";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<ProductEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapProduct(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<ProductEntity>> GetByTypeAsync(ProductType productType)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            string sql = "SELECT * FROM Products WHERE product_type = @ProductType ORDER BY product_name";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ProductType", productType.ToString());
                
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<ProductEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapProduct(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertAsync(ProductEntity product)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            string sql = @"
                                INSERT INTO Products (product_name, product_code, product_type, unit_of_measure, cost_per_unit, created_at)
                                VALUES (@ProductName, @ProductCode, @ProductType, @UnitOfMeasure, @CostPerUnit, @CreatedAt);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                            cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                            cmd.Parameters.AddWithValue("@ProductType", product.ProductType.ToString());
                            cmd.Parameters.AddWithValue("@UnitOfMeasure", product.UnitOfMeasure);
                            cmd.Parameters.AddWithValue("@CostPerUnit", product.CostPerUnit);
                            cmd.Parameters.AddWithValue("@CreatedAt", product.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ProductEntity product)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            string sql = @"
                                UPDATE Products 
                                SET product_name = @ProductName,
                                    product_code = @ProductCode,
                                    product_type = @ProductType,
                                    unit_of_measure = @UnitOfMeasure,
                                    cost_per_unit = @CostPerUnit
                                WHERE product_id = @ProductId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                            cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                            cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                            cmd.Parameters.AddWithValue("@ProductType", product.ProductType.ToString());
                            cmd.Parameters.AddWithValue("@UnitOfMeasure", product.UnitOfMeasure);
                            cmd.Parameters.AddWithValue("@CostPerUnit", product.CostPerUnit);
                
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            string sql = "DELETE FROM Products WHERE product_id = @ProductId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@ProductId", productId);
                
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private ProductEntity MapProduct(SqliteDataReader reader)
        {
            try
            {
                            return new ProductEntity
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                                ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                ProductCode = reader.GetString(reader.GetOrdinal("product_code")),
                                ProductType = Enum.Parse<ProductType>(reader.GetString(reader.GetOrdinal("product_type"))),
                                UnitOfMeasure = reader.GetString(reader.GetOrdinal("unit_of_measure")),
                                CostPerUnit = reader.GetDecimal(reader.GetOrdinal("cost_per_unit")),
                                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("created_at")))
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}


