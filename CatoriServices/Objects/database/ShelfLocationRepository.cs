using CatoriServices.Objects.Entities;
using CityAppServices.Objects.database;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatoriServices.Objects.database
{
    public class ShelfLocationRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public async Task<List<ShelfLocationEntity>> GetShelfLocationsAsync()
        {
            List<ShelfLocationEntity> locations = new List<ShelfLocationEntity>();
            try
            {
                string sql = "SELECT ShelfLocationID, StoreType, Aisle, Shelf, PositionX, PositionY, PositionZ, Width, Height, ShopItemId FROM ShelfLocation";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("ShelfLocationID");
                int idxStoreType = reader.GetOrdinal("StoreType");
                int idxAisle = reader.GetOrdinal("Aisle");
                int idxShelf = reader.GetOrdinal("Shelf");
                int idxPosX = reader.GetOrdinal("PositionX");
                int idxPosY = reader.GetOrdinal("PositionY");
                int idxPosZ = reader.GetOrdinal("PositionZ");
                int idxWidth = reader.GetOrdinal("Width");
                int idxHeight = reader.GetOrdinal("Height");
                int idxShopItemId = reader.GetOrdinal("ShopItemId");

                while (reader.Read())
                {
                    var entity = new ShelfLocationEntity();

                    if (!reader.IsDBNull(idxId)) entity.ShelfLocationID = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxStoreType)) entity.StoreType = reader.GetString(idxStoreType);
                    if (!reader.IsDBNull(idxAisle)) entity.Aisle = reader.GetString(idxAisle);
                    if (!reader.IsDBNull(idxShelf)) entity.Shelf = reader.GetString(idxShelf);
                    if (!reader.IsDBNull(idxPosX)) entity.PositionX = reader.GetDouble(idxPosX);
                    if (!reader.IsDBNull(idxPosY)) entity.PositionY = reader.GetDouble(idxPosY);
                    if (!reader.IsDBNull(idxPosZ)) entity.PositionZ = reader.GetDouble(idxPosZ);
                    if (!reader.IsDBNull(idxWidth)) entity.Width = reader.GetDouble(idxWidth);
                    if (!reader.IsDBNull(idxHeight)) entity.Height = reader.GetDouble(idxHeight);
                    if (!reader.IsDBNull(idxShopItemId)) entity.ShopItemId = reader.GetInt32(idxShopItemId);

                    locations.Add(entity);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return locations;
        }

        public async Task<ShelfLocationEntity> GetShelfLocationByIdAsync(int id)
        {
            ShelfLocationEntity location = new ShelfLocationEntity();
            try
            {
                string sql = $"SELECT ShelfLocationID, StoreType, Aisle, Shelf, PositionX, PositionY, PositionZ, Width, Height, ShopItemId FROM ShelfLocation WHERE ShelfLocationID = {id}";
                IDataReader reader = sqlhelper.GetReader(sql);

                int idxId = reader.GetOrdinal("ShelfLocationID");
                int idxStoreType = reader.GetOrdinal("StoreType");
                int idxAisle = reader.GetOrdinal("Aisle");
                int idxShelf = reader.GetOrdinal("Shelf");
                int idxPosX = reader.GetOrdinal("PositionX");
                int idxPosY = reader.GetOrdinal("PositionY");
                int idxPosZ = reader.GetOrdinal("PositionZ");
                int idxWidth = reader.GetOrdinal("Width");
                int idxHeight = reader.GetOrdinal("Height");
                int idxShopItemId = reader.GetOrdinal("ShopItemId");

                while (reader.Read())
                {
                    if (!reader.IsDBNull(idxId)) location.ShelfLocationID = reader.GetInt32(idxId);
                    if (!reader.IsDBNull(idxStoreType)) location.StoreType = reader.GetString(idxStoreType);
                    if (!reader.IsDBNull(idxAisle)) location.Aisle = reader.GetString(idxAisle);
                    if (!reader.IsDBNull(idxShelf)) location.Shelf = reader.GetString(idxShelf);
                    if (!reader.IsDBNull(idxPosX)) location.PositionX = reader.GetDouble(idxPosX);
                    if (!reader.IsDBNull(idxPosY)) location.PositionY = reader.GetDouble(idxPosY);
                    if (!reader.IsDBNull(idxPosZ)) location.PositionZ = reader.GetDouble(idxPosZ);
                    if (!reader.IsDBNull(idxWidth)) location.Width = reader.GetDouble(idxWidth);
                    if (!reader.IsDBNull(idxHeight)) location.Height = reader.GetDouble(idxHeight);
                    if (!reader.IsDBNull(idxShopItemId)) location.ShopItemId = reader.GetInt32(idxShopItemId);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return location;
        }

        public async Task<List<ShelfLocationEntity>> GetShelfLocationsByShopItemIdAsync(int shopItemId)
        {
            List<ShelfLocationEntity> locations = new List<ShelfLocationEntity>();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT ShelfLocationID, StoreType, Aisle, Shelf, PositionX, PositionY, PositionZ, Width, Height, ShopItemId FROM ShelfLocation WHERE ShopItemId = @ShopItemId";
                var results = await connection.QueryAsync<ShelfLocationEntity>(sql, new { ShopItemId = shopItemId });
                if (results != null && results.Any())
                {
                    locations = results.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return locations;
        }

        public async Task<List<ShelfLocationEntity>> GetShelfLocationsByStoreTypeAsync(string storeType)
        {
            List<ShelfLocationEntity> locations = new List<ShelfLocationEntity>();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT ShelfLocationID, StoreType, Aisle, Shelf, PositionX, PositionY, PositionZ, Width, Height, ShopItemId FROM ShelfLocation WHERE StoreType = @StoreType";
                var results = await connection.QueryAsync<ShelfLocationEntity>(sql, new { StoreType = storeType });
                if (results != null && results.Any())
                {
                    locations = results.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return locations;
        }

        // Matches existing project pattern (Upsert implemented as fire-and-forget)
        public async void Upsert(ShelfLocationEntity entity)
        {
            try
            {
                var found = await GetShelfLocationByIdAsync(entity.ShelfLocationID).ConfigureAwait(false);
                var connection = adoNetHelper.GetConnection();

                if (found != null && found.ShelfLocationID != 0)
                {
                    // Update
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE ShelfLocation SET StoreType = @StoreType, Aisle = @Aisle, Shelf = @Shelf, PositionX = @PositionX, PositionY = @PositionY, PositionZ = @PositionZ, Width = @Width, Height = @Height, ShopItemId = @ShopItemId WHERE ShelfLocationID = @Id";
                    command.Parameters.Add(new SqliteParameter("@StoreType", entity.StoreType ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Aisle", entity.Aisle ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Shelf", entity.Shelf ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@PositionX", entity.PositionX));
                    command.Parameters.Add(new SqliteParameter("@PositionY", entity.PositionY));
                    command.Parameters.Add(new SqliteParameter("@PositionZ", entity.PositionZ));
                    command.Parameters.Add(new SqliteParameter("@Width", entity.Width));
                    command.Parameters.Add(new SqliteParameter("@Height", entity.Height));
                    command.Parameters.Add(new SqliteParameter("@ShopItemId", entity.ShopItemId));
                    command.Parameters.Add(new SqliteParameter("@Id", entity.ShelfLocationID));
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Insert
                    using var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO ShelfLocation (StoreType, Aisle, Shelf, PositionX, PositionY, PositionZ, Width, Height, ShopItemId) VALUES (@StoreType, @Aisle, @Shelf, @PositionX, @PositionY, @PositionZ, @Width, @Height, @ShopItemId)";
                    command.Parameters.Add(new SqliteParameter("@StoreType", entity.StoreType ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Aisle", entity.Aisle ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@Shelf", entity.Shelf ?? string.Empty));
                    command.Parameters.Add(new SqliteParameter("@PositionX", entity.PositionX));
                    command.Parameters.Add(new SqliteParameter("@PositionY", entity.PositionY));
                    command.Parameters.Add(new SqliteParameter("@PositionZ", entity.PositionZ));
                    command.Parameters.Add(new SqliteParameter("@Width", entity.Width));
                    command.Parameters.Add(new SqliteParameter("@Height", entity.Height));
                    command.Parameters.Add(new SqliteParameter("@ShopItemId", entity.ShopItemId));
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(int shelfLocationId)
        {
            string sql = "DELETE FROM ShelfLocation WHERE ShelfLocationID = @Id";
            var connection = adoNetHelper.GetConnection();
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SqliteParameter("@Id", shelfLocationId));
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
