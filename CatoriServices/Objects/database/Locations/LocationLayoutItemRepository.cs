using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Locations
{
    public class LocationLayoutItemRepository
    {
        private readonly string _connectionString;

        public LocationLayoutItemRepository()
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

        public async Task<LocationLayoutItemEntity?> GetByIdAsync(long itemId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            using var cmd = new SqliteCommand("SELECT * FROM LocationLayoutItem WHERE LocationLayoutItemId = @ItemId", conn);
                            cmd.Parameters.AddWithValue("@ItemId", itemId);
                            using var reader = await cmd.ExecuteReaderAsync();
                            return await reader.ReadAsync() ? MapItem(reader) : null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<LocationLayoutItemEntity>> GetLayoutItemsByIdAsync(long locationId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM LocationLayoutItem WHERE LocationId = @LocationId ORDER BY ZIndex, LocationLayoutItemId";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationId", locationId);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<LocationLayoutItemEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapItem(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertAsync(LocationLayoutItemEntity item)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                INSERT INTO LocationLayoutItem
                                    (LocationId, ItemName, ItemType, X, Y, Z, Width, Height, RotationDegrees,
                                     ZIndex, IsLocked, ImagePath, MetadataJson)
                                VALUES
                                    (@LocationId, @ItemName, @ItemType, @X, @Y, @Z, @Width, @Height, @RotationDegrees,
                                     @ZIndex, @IsLocked, @ImagePath, @MetadataJson);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddItemParameters(cmd, item);
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(LocationLayoutItemEntity item)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                UPDATE LocationLayoutItem
                                SET LocationId = @LocationId,
                                    ItemName = @ItemName,
                                    ItemType = @ItemType,
                                    X = @X,
                                    Y = @Y,
                                    Z = @Z,
                                    Width = @Width,
                                    Height = @Height,
                                    RotationDegrees = @RotationDegrees,
                                    ZIndex = @ZIndex,
                                    IsLocked = @IsLocked,
                                    ImagePath = @ImagePath,
                                    MetadataJson = @MetadataJson
                                WHERE LocationLayoutItemId = @LocationLayoutItemId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationLayoutItemId", item.LocationLayoutItemId);
                            AddItemParameters(cmd, item);
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long itemId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            using var cmd = new SqliteCommand("DELETE FROM LocationLayoutItem WHERE LocationLayoutItemId = @ItemId", conn);
                            cmd.Parameters.AddWithValue("@ItemId", itemId);
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddItemParameters(SqliteCommand cmd, LocationLayoutItemEntity item)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@LocationId", item.LocationId);
                            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                            cmd.Parameters.AddWithValue("@ItemType", item.ItemType.ToString());
                            cmd.Parameters.AddWithValue("@X", item.X);
                            cmd.Parameters.AddWithValue("@Y", item.Y);
                            cmd.Parameters.AddWithValue("@Z", item.Z);
                            cmd.Parameters.AddWithValue("@Width", item.Width);
                            cmd.Parameters.AddWithValue("@Height", item.Height);
                            cmd.Parameters.AddWithValue("@RotationDegrees", item.RotationDegrees);
                            cmd.Parameters.AddWithValue("@ZIndex", item.ZIndex);
                            cmd.Parameters.AddWithValue("@IsLocked", item.IsLocked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@ImagePath", item.ImagePath ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@MetadataJson", item.MetadataJson ?? (object)DBNull.Value);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static LocationLayoutItemEntity MapItem(SqliteDataReader reader)
        {
            try
            {
                            return new LocationLayoutItemEntity
                            {
                                LocationLayoutItemId = reader.GetInt64(reader.GetOrdinal("LocationLayoutItemId")),
                                LocationId = reader.GetInt64(reader.GetOrdinal("LocationId")),
                                ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                                ItemType = ParseItemType(reader.GetString(reader.GetOrdinal("ItemType"))),
                                X = reader.GetDouble(reader.GetOrdinal("X")),
                                Y = reader.GetDouble(reader.GetOrdinal("Y")),
                                Z = reader.GetDouble(reader.GetOrdinal("Z")),
                                Width = reader.GetDouble(reader.GetOrdinal("Width")),
                                Height = reader.GetDouble(reader.GetOrdinal("Height")),
                                RotationDegrees = reader.GetDouble(reader.GetOrdinal("RotationDegrees")),
                                ZIndex = reader.GetInt32(reader.GetOrdinal("ZIndex")),
                                IsLocked = reader.GetInt32(reader.GetOrdinal("IsLocked")) == 1,
                                ImagePath = GetNullableString(reader, "ImagePath"),
                                MetadataJson = GetNullableString(reader, "MetadataJson")
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static LocationLayoutItemType ParseItemType(string objectType)
            => Enum.TryParse<LocationLayoutItemType>(objectType, true, out var itemType)
                ? itemType
                : LocationLayoutItemType.Decoration;

        private static string? GetNullableString(SqliteDataReader reader, string columnName)
        {
            try
            {
                            var ordinal = reader.GetOrdinal(columnName);
                            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}


