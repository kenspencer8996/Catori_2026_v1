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

            using var cmd = new SqliteCommand("SELECT * FROM FactoryLayoutObject WHERE LayoutObjectId = @ItemId", conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? await MapItemAsync(reader) : null;
        }

        public async Task<List<FactoryLayoutItemEntity>> GetByLayoutIdAsync(long factoryLayoutId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "SELECT * FROM FactoryLayoutObject WHERE FactoryId = @FactoryId ORDER BY ZIndex, LayoutObjectId";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryLayoutId);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryLayoutItemEntity>();
            while (await reader.ReadAsync())
                list.Add(await MapItemAsync(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryLayoutItemEntity item)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO FactoryLayoutObject
                    (FactoryId, ObjectName, ObjectType, ImagePath, ZIndex, IsInteractive, IsVisible, Notes)
                VALUES
                    (@FactoryId, @ObjectName, @ObjectType, @ImagePath, @ZIndex, @IsInteractive, @IsVisible, @Notes);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            AddItemParameters(cmd, item);
            var result = await cmd.ExecuteScalarAsync();
            var itemId = Convert.ToInt32(result);

            if (item.X != 0 || item.Y != 0)
                await UpsertAnchorPointAsync(conn, itemId, item.X, item.Y);

            return itemId;
        }

        public async Task<bool> UpdateAsync(FactoryLayoutItemEntity item)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                UPDATE FactoryLayoutObject
                SET FactoryId = @FactoryId,
                    ObjectName = @ObjectName,
                    ObjectType = @ObjectType,
                    ImagePath = @ImagePath,
                    ZIndex = @ZIndex,
                    IsInteractive = @IsInteractive,
                    IsVisible = @IsVisible,
                    Notes = @Notes
                WHERE LayoutObjectId = @LayoutObjectId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@LayoutObjectId", item.FactoryLayoutItemId);
            AddItemParameters(cmd, item);
            var rows = await cmd.ExecuteNonQueryAsync();

            await UpsertAnchorPointAsync(conn, item.FactoryLayoutItemId, item.X, item.Y);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long itemId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM FactoryLayoutObject WHERE LayoutObjectId = @ItemId", conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static void AddItemParameters(SqliteCommand cmd, FactoryLayoutItemEntity item)
        {
            cmd.Parameters.AddWithValue("@FactoryId", item.FactoryLayoutId);
            cmd.Parameters.AddWithValue("@ObjectName", item.ItemName);
            cmd.Parameters.AddWithValue("@ObjectType", item.ItemType.ToString());
            cmd.Parameters.AddWithValue("@ImagePath", item.ImagePath ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ZIndex", item.ZIndex);
            cmd.Parameters.AddWithValue("@IsInteractive", item.IsLocked ? 0 : 1);
            cmd.Parameters.AddWithValue("@IsVisible", 1);
            cmd.Parameters.AddWithValue("@Notes", item.MetadataJson ?? (object)DBNull.Value);
        }

        private static async Task<FactoryLayoutItemEntity> MapItemAsync(SqliteDataReader reader)
        {
            var itemId = reader.GetInt64(reader.GetOrdinal("LayoutObjectId"));
            var item = new FactoryLayoutItemEntity
            {
                FactoryLayoutItemId = itemId,
                FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("FactoryId")),
                ItemName = reader.GetString(reader.GetOrdinal("ObjectName")),
                ItemType = ParseItemType(reader.GetString(reader.GetOrdinal("ObjectType"))),
                ZIndex = reader.GetInt32(reader.GetOrdinal("ZIndex")),
                IsLocked = reader.GetInt32(reader.GetOrdinal("IsInteractive")) == 0,
                ImagePath = GetNullableString(reader, "ImagePath"),
                MetadataJson = GetNullableString(reader, "Notes")
            };

            var point = await GetAnchorPointAsync(itemId);
            if (point is not null)
            {
                item.X = point.Value.X;
                item.Y = point.Value.Y;
            }

            return item;
        }

        private static FactoryLayoutItemType ParseItemType(string objectType)
            => Enum.TryParse<FactoryLayoutItemType>(objectType, true, out var itemType)
                ? itemType
                : FactoryLayoutItemType.Decoration;

        private static string? GetNullableString(SqliteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static async Task<(double X, double Y)?> GetAnchorPointAsync(long itemId)
        {
            using var conn = new SqliteConnection("Data Source=" + GlobalServices.Database + " ;");
            await conn.OpenAsync();

            const string sql = @"
                SELECT X, Y
                FROM FactoryLayoutObjectPoint
                WHERE LayoutObjectId = @ItemId
                ORDER BY CASE WHEN PointRole = 'Anchor' THEN 0 ELSE 1 END, PointIndex
                LIMIT 1";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync()
                ? (reader.GetDouble(reader.GetOrdinal("X")), reader.GetDouble(reader.GetOrdinal("Y")))
                : null;
        }

        private static async Task UpsertAnchorPointAsync(SqliteConnection conn, long itemId, double x, double y)
        {
            const string deleteSql = "DELETE FROM FactoryLayoutObjectPoint WHERE LayoutObjectId = @ItemId AND PointIndex = 0 AND (PointRole IS NULL OR PointRole = 'Anchor')";
            using (var deleteCmd = new SqliteCommand(deleteSql, conn))
            {
                deleteCmd.Parameters.AddWithValue("@ItemId", itemId);
                await deleteCmd.ExecuteNonQueryAsync();
            }

            const string insertSql = @"
                INSERT INTO FactoryLayoutObjectPoint (LayoutObjectId, PointIndex, X, Y, PointRole)
                VALUES (@ItemId, 0, @X, @Y, 'Anchor')";
            using var insertCmd = new SqliteCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("@ItemId", itemId);
            insertCmd.Parameters.AddWithValue("@X", x);
            insertCmd.Parameters.AddWithValue("@Y", y);
            await insertCmd.ExecuteNonQueryAsync();
        }
    }
}
