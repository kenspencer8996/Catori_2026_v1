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

            string sql = "SELECT * FROM FactoryLayoutPoints WHERE factory_layout_item_id = @ItemId ORDER BY point_index";
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

            string sql = @"
                INSERT INTO FactoryLayoutPoints
                    (factory_layout_item_id, point_index, point_role, x, y, z, segment_kind, control1_x, control1_y, control2_x, control2_y, rotation_degrees)
                VALUES
                    (@FactoryLayoutItemId, @PointIndex, @PointRole, @X, @Y, @Z, @SegmentKind, @Control1X, @Control1Y, @Control2X, @Control2Y, @RotationDegrees);
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

            string sql = @"
                UPDATE FactoryLayoutPoints
                SET factory_layout_item_id = @FactoryLayoutItemId,
                    point_index = @PointIndex,
                    point_role = @PointRole,
                    x = @X,
                    y = @Y,
                    z = @Z,
                    segment_kind = @SegmentKind,
                    control1_x = @Control1X,
                    control1_y = @Control1Y,
                    control2_x = @Control2X,
                    control2_y = @Control2Y,
                    rotation_degrees = @RotationDegrees
                WHERE factory_layout_point_id = @FactoryLayoutPointId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryLayoutPointId", point.FactoryLayoutPointId);
            AddPointParameters(cmd, point);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteByItemIdAsync(long itemId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM FactoryLayoutPoints WHERE factory_layout_item_id = @ItemId", conn);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        private static void AddPointParameters(SqliteCommand cmd, FactoryLayoutPointEntity point)
        {
            cmd.Parameters.AddWithValue("@FactoryLayoutItemId", point.FactoryLayoutItemId);
            cmd.Parameters.AddWithValue("@PointIndex", point.PointIndex);
            cmd.Parameters.AddWithValue("@PointRole", point.PointRole ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@X", point.X);
            cmd.Parameters.AddWithValue("@Y", point.Y);
            cmd.Parameters.AddWithValue("@Z", point.Z);
            cmd.Parameters.AddWithValue("@SegmentKind", point.SegmentKind.ToString());
            cmd.Parameters.AddWithValue("@Control1X", point.Control1X ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Control1Y", point.Control1Y ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Control2X", point.Control2X ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Control2Y", point.Control2Y ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RotationDegrees", point.RotationDegrees ?? (object)DBNull.Value);
        }

        private static FactoryLayoutPointEntity MapPoint(SqliteDataReader reader)
        {
            int idxRole = reader.GetOrdinal("point_role");
            int idxControl1X = reader.GetOrdinal("control1_x");
            int idxControl1Y = reader.GetOrdinal("control1_y");
            int idxControl2X = reader.GetOrdinal("control2_x");
            int idxControl2Y = reader.GetOrdinal("control2_y");
            int idxRotation = reader.GetOrdinal("rotation_degrees");

            return new FactoryLayoutPointEntity
            {
                FactoryLayoutPointId = reader.GetInt64(reader.GetOrdinal("factory_layout_point_id")),
                FactoryLayoutItemId = reader.GetInt64(reader.GetOrdinal("factory_layout_item_id")),
                PointIndex = reader.GetInt32(reader.GetOrdinal("point_index")),
                PointRole = reader.IsDBNull(idxRole) ? null : reader.GetString(idxRole),
                X = reader.GetDouble(reader.GetOrdinal("x")),
                Y = reader.GetDouble(reader.GetOrdinal("y")),
                Z = reader.GetDouble(reader.GetOrdinal("z")),
                SegmentKind = Enum.Parse<FactoryLayoutSegmentKind>(reader.GetString(reader.GetOrdinal("segment_kind"))),
                Control1X = reader.IsDBNull(idxControl1X) ? null : reader.GetDouble(idxControl1X),
                Control1Y = reader.IsDBNull(idxControl1Y) ? null : reader.GetDouble(idxControl1Y),
                Control2X = reader.IsDBNull(idxControl2X) ? null : reader.GetDouble(idxControl2X),
                Control2Y = reader.IsDBNull(idxControl2Y) ? null : reader.GetDouble(idxControl2Y),
                RotationDegrees = reader.IsDBNull(idxRotation) ? null : reader.GetDouble(idxRotation)
            };
        }
    }
}
