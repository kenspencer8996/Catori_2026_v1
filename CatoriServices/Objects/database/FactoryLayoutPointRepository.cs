using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public class FactoryLayoutPointRepository
    {
        private readonly string _connectionString;

        public FactoryLayoutPointRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<List<FactoryLayoutPointEntity>> GetByItemIdAsync(long itemId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "SELECT * FROM FactoryLayoutObjectPoint WHERE LayoutObjectId = @ItemId ORDER BY PointIndex";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryLayoutPointEntity>();
            while (await reader.ReadAsync())
                list.Add(MapPoint(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryLayoutPointEntity point)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO FactoryLayoutObjectPoint
                    (LayoutObjectId, PointIndex, X, Y, PointRole)
                VALUES
                    (@LayoutObjectId, @PointIndex, @X, @Y, @PointRole);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            AddPointParameters(cmd, point);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(FactoryLayoutPointEntity point)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                UPDATE FactoryLayoutObjectPoint
                SET LayoutObjectId = @LayoutObjectId,
                    PointIndex = @PointIndex,
                    X = @X,
                    Y = @Y,
                    PointRole = @PointRole
                WHERE LayoutObjectPointId = @LayoutObjectPointId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@LayoutObjectPointId", point.FactoryLayoutPointId);
            AddPointParameters(cmd, point);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteByItemIdAsync(long itemId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM FactoryLayoutObjectPoint WHERE LayoutObjectId = @ItemId", conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        private static void AddPointParameters(SqliteCommand cmd, FactoryLayoutPointEntity point)
        {
            cmd.Parameters.AddWithValue("@LayoutObjectId", point.FactoryLayoutItemId);
            cmd.Parameters.AddWithValue("@PointIndex", point.PointIndex);
            cmd.Parameters.AddWithValue("@X", point.X);
            cmd.Parameters.AddWithValue("@Y", point.Y);
            cmd.Parameters.AddWithValue("@PointRole", point.PointRole ?? (object)DBNull.Value);
        }

        private static FactoryLayoutPointEntity MapPoint(SqliteDataReader reader)
        {
            var roleOrdinal = reader.GetOrdinal("PointRole");

            return new FactoryLayoutPointEntity
            {
                FactoryLayoutPointId = reader.GetInt64(reader.GetOrdinal("LayoutObjectPointId")),
                FactoryLayoutItemId = reader.GetInt64(reader.GetOrdinal("LayoutObjectId")),
                PointIndex = reader.GetInt32(reader.GetOrdinal("PointIndex")),
                PointRole = reader.IsDBNull(roleOrdinal) ? null : reader.GetString(roleOrdinal),
                X = reader.GetDouble(reader.GetOrdinal("X")),
                Y = reader.GetDouble(reader.GetOrdinal("Y")),
                Z = 0,
                SegmentKind = FactoryLayoutSegmentKind.Line
            };
        }
    }
}
