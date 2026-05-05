using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace CatoriServices.Data
{
    public class ShopItemRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public async Task<List<ShopItemEntity>> GetShopItemsAsync()
        {
            List<ShopItemEntity> items = new List<ShopItemEntity>();
            try
            {
                string sql = "SELECT ShopItemId, Name, Description, Price, ImageName, Storetype, FilePath, Height, Width, RotationDegree FROM ShopItem";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("ShopItemId");
                int idxName = reader.GetOrdinal("Name");
                int idxDesc = reader.GetOrdinal("Description");
                int idxPrice = reader.GetOrdinal("Price");
                int idxImage = reader.GetOrdinal("ImageName");
                int idxStoretype = reader.GetOrdinal("Storetype");
                int idxFilePath = reader.GetOrdinal("FilePath");
                int idxHeight = reader.GetOrdinal("Height");
                int idxWidth = reader.GetOrdinal("Width");
                int idxRotation = reader.GetOrdinal("RotationDegree");
 
                while (reader.Read())
                {
                    var entity = new ShopItemEntity();

                    if (!reader.IsDBNull(idxId)) entity.ShopItemId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxName)) entity.Name = reader.GetString(idxName);
                    if (!reader.IsDBNull(idxDesc)) entity.Description = reader.GetString(idxDesc);
                    if (!reader.IsDBNull(idxPrice)) entity.Price = reader.GetDecimal(idxPrice);
                    if (!reader.IsDBNull(idxImage)) entity.ImageName = reader.GetString(idxImage);
                    if (!reader.IsDBNull(idxStoretype)) entity.StoreType = reader.GetString(idxStoretype);
                    if (!reader.IsDBNull(idxFilePath)) entity.FilePath = reader.GetString(idxFilePath);
                    if (!reader.IsDBNull(idxHeight)) entity.Height = reader.GetDouble(idxHeight);
                    if (!reader.IsDBNull(idxWidth)) entity.Width = reader.GetDouble(idxWidth);
                    if (!reader.IsDBNull(idxRotation)) entity.RotationDegree = reader.GetDouble(idxRotation);

                    items.Add(entity);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return items;
        }

        public async Task<ShopItemEntity> GetShopItemByIdAsync(int id)
        {
            ShopItemEntity item = new ShopItemEntity();
            try
            {
                string sql = $"SELECT ShopItemId, Name, Description, Price, ImageName, Storetype, FilePath, Height, Width, RotationDegree FROM ShopItem WHERE ShopItemId = {id}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("ShopItemId");
                int idxName = reader.GetOrdinal("Name");
                int idxDesc = reader.GetOrdinal("Description");
                int idxPrice = reader.GetOrdinal("Price");
                int idxImage = reader.GetOrdinal("ImageName");
                int idxStoretype = reader.GetOrdinal("Storetype");
                int idxFilePath = reader.GetOrdinal("FilePath");
                int idxHeight = reader.GetOrdinal("Height");
                int idxWidth = reader.GetOrdinal("Width");
                int idxRotation = reader.GetOrdinal("RotationDegree");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxId)) item.ShopItemId = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxName)) item.Name = reader.GetString(idxName);
                    if (!reader.IsDBNull(idxDesc)) item.Description = reader.GetString(idxDesc);
                    if (!reader.IsDBNull(idxPrice)) item.Price = reader.GetDecimal(idxPrice);
                    if (!reader.IsDBNull(idxImage)) item.ImageName = reader.GetString(idxImage);
                    if (!reader.IsDBNull(idxStoretype)) item.StoreType = reader.GetString(idxStoretype);
                    if (!reader.IsDBNull(idxFilePath)) item.FilePath = reader.GetString(idxFilePath);
                    if (!reader.IsDBNull(idxHeight)) item.Height = reader.GetDouble(idxHeight);
                    if (!reader.IsDBNull(idxWidth)) item.Width = reader.GetDouble(idxWidth);
                    if (!reader.IsDBNull(idxRotation)) item.RotationDegree = reader.GetDouble(idxRotation);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return item;
        }

        public async Task<ShopItemEntity> GetShopItemByNameAsync(string name)
        {
            ShopItemEntity item = new ShopItemEntity();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT ShopItemId, Name, Description, Price, ImageName, Storetype, FilePath, Height, Width, RotationDegree FROM ShopItem WHERE Name = @Name";
                var results = await connection.QueryAsync<ShopItemEntity>(sql, new { Name = name });
                if (results != null && results.Any())
                {
                    item = results.First();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return item;
        }

        // Note: matches existing project pattern (Upsert implemented as fire-and-forget)
        public async void Upsert(ShopItemEntity entity)
        {
            try
            {
                // If no id provided, try to find by name
                if (entity.ShopItemId == 0 && !string.IsNullOrEmpty(entity.Name))
                {
                    var foundByName = await GetShopItemByNameAsync(entity.Name).ConfigureAwait(false);
                    if (foundByName != null && foundByName.ShopItemId != 0)
                        entity.ShopItemId = foundByName.ShopItemId;
                }

                var found = await GetShopItemByIdAsync(entity.ShopItemId).ConfigureAwait(false);
                var connection = adoNetHelper.GetConnection();

                if (found != null && found.ShopItemId != 0)
                {
                    // Update
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE ShopItem SET Name = @Name, Description = @Description, Price = @Price, ImageName = @ImageName, Storetype = @Storetype, FilePath = @FilePath, Height = @Height, Width = @Width, RotationDegree = @RotationDegree  WHERE ShopItemId = @Id";
                    command.Parameters.Add(new SqliteParameter("@Name", entity.Name ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Description", entity.Description ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Price", entity.Price));
                    command.Parameters.Add(new SqliteParameter("@ImageName", entity.ImageName ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Storetype", entity.StoreType ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@FilePath", entity.FilePath ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Height", entity.Height));
                    command.Parameters.Add(new SqliteParameter("@Width", entity.Width));
                    command.Parameters.Add(new SqliteParameter("@RotationDegree", entity.RotationDegree));
                    command.Parameters.Add(new SqliteParameter("@Id", entity.ShopItemId));
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Insert
                    using var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO ShopItem (Name, Description, Price, ImageName, Storetype, FilePath, Height, Width, RotationDegree) VALUES (@Name, @Description, @Price, @ImageName, @Storetype, @FilePath, @Height, @Width, @RotationDegree)";
                    command.Parameters.Add(new SqliteParameter("@Name", entity.Name ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Description", entity.Description ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Price", entity.Price));
                    command.Parameters.Add(new SqliteParameter("@ImageName", entity.ImageName ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Storetype", entity.StoreType ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@FilePath", entity.FilePath ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Height", entity.Height));
                    command.Parameters.Add(new SqliteParameter("@Width", entity.Width));
                    command.Parameters.Add(new SqliteParameter("@RotationDegree", entity.RotationDegree));
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
