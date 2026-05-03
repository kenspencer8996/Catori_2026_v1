using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public class FactoryLayoutItemRepository
    {
        private readonly string _connectionString;

        public FactoryLayoutItemRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<FactoryLayoutItemEntity?> GetByIdAsync(long itemId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM FactoryLayoutItems WHERE factory_layout_item_id = @ItemId", conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapItem(reader) : null;
        }

        public async Task<List<FactoryLayoutItemEntity>> GetByLayoutIdAsync(long factoryLayoutId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM FactoryLayoutItems WHERE factory_layout_id = @FactoryLayoutId ORDER BY z_index, factory_layout_item_id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryLayoutId", factoryLayoutId);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryLayoutItemEntity>();
            while (await reader.ReadAsync())
                list.Add(MapItem(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryLayoutItemEntity item)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO FactoryLayoutItems
                    (factory_layout_id, item_name, item_type, x, y, z, width, height, rotation_degrees, z_index, is_locked, image_path, metadata_json)
                VALUES
                    (@FactoryLayoutId, @ItemName, @ItemType, @X, @Y, @Z, @Width, @Height, @RotationDegrees, @ZIndex, @IsLocked, @ImagePath, @MetadataJson);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            AddItemParameters(cmd, item);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(FactoryLayoutItemEntity item)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE FactoryLayoutItems
                SET factory_layout_id = @FactoryLayoutId,
                    item_name = @ItemName,
                    item_type = @ItemType,
                    x = @X,
                    y = @Y,
                    z = @Z,
                    width = @Width,
                    height = @Height,
                    rotation_degrees = @RotationDegrees,
                    z_index = @ZIndex,
                    is_locked = @IsLocked,
                    image_path = @ImagePath,
                    metadata_json = @MetadataJson
                WHERE factory_layout_item_id = @FactoryLayoutItemId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryLayoutItemId", item.FactoryLayoutItemId);
            AddItemParameters(cmd, item);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long itemId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM FactoryLayoutItems WHERE factory_layout_item_id = @ItemId", conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static void AddItemParameters(SqliteCommand cmd, FactoryLayoutItemEntity item)
        {
            cmd.Parameters.AddWithValue("@FactoryLayoutId", item.FactoryLayoutId);
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

        private static FactoryLayoutItemEntity MapItem(SqliteDataReader reader)
        {
            int idxImagePath = reader.GetOrdinal("image_path");
            int idxMetadata = reader.GetOrdinal("metadata_json");
            return new FactoryLayoutItemEntity
            {
                FactoryLayoutItemId = reader.GetInt64(reader.GetOrdinal("factory_layout_item_id")),
                FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("factory_layout_id")),
                ItemName = reader.GetString(reader.GetOrdinal("item_name")),
                ItemType = Enum.Parse<FactoryLayoutItemType>(reader.GetString(reader.GetOrdinal("item_type"))),
                X = reader.GetDouble(reader.GetOrdinal("x")),
                Y = reader.GetDouble(reader.GetOrdinal("y")),
                Z = reader.GetDouble(reader.GetOrdinal("z")),
                Width = reader.GetDouble(reader.GetOrdinal("width")),
                Height = reader.GetDouble(reader.GetOrdinal("height")),
                RotationDegrees = reader.GetDouble(reader.GetOrdinal("rotation_degrees")),
                ZIndex = reader.GetInt32(reader.GetOrdinal("z_index")),
                IsLocked = reader.GetInt32(reader.GetOrdinal("is_locked")) == 1,
                ImagePath = reader.IsDBNull(idxImagePath) ? null : reader.GetString(idxImagePath),
                MetadataJson = reader.IsDBNull(idxMetadata) ? null : reader.GetString(idxMetadata)
            };
        }
    }
}
