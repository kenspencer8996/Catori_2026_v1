using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Locations
{
    public class LocationLayoutPointRepository
    {
        private readonly string _connectionString;

        public LocationLayoutPointRepository()
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

        public async Task<List<LocationLayoutPointEntity>> GetByItemIdAsync(long itemId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = "SELECT * FROM LocationLayoutPoint WHERE LocationLayoutItemId = @ItemId ORDER BY PointIndex";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ItemId", itemId);
                using var reader = await cmd.ExecuteReaderAsync();

                var list = new List<LocationLayoutPointEntity>();
                while (await reader.ReadAsync())
                    list.Add(MapPoint(reader));

                return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertAsync(LocationLayoutPointEntity point)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    INSERT INTO LocationLayoutPoint
                        (LocationLayoutItemId, LocationId, PointIndex, PointRole, X, Y, RotationDegrees)
                    VALUES
                        (@LocationLayoutItemId, @LocationId, @PointIndex, @PointRole, @X, @Y, @RotationDegrees);
                    SELECT last_insert_rowid();";

                using var cmd = new SqliteCommand(sql, conn);
                var locationId = await ResolveLocationIdAsync(conn, point);
                AddPointParameters(cmd, point, locationId);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(LocationLayoutPointEntity point)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE LocationLayoutPoint
                    SET LocationLayoutItemId = @LocationLayoutItemId,
                        LocationId = @LocationId,
                        PointIndex = @PointIndex,
                        PointRole = @PointRole,
                        X = @X,
                        Y = @Y,
                        RotationDegrees = @RotationDegrees
                    WHERE LocationLayoutPointId = @LocationLayoutPointId";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@LocationLayoutPointId", point.LocationLayoutPointId);
                var locationId = await ResolveLocationIdAsync(conn, point);
                AddPointParameters(cmd, point, locationId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteByItemIdAsync(long itemId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                using var cmd = new SqliteCommand("DELETE FROM LocationLayoutPoint WHERE LocationLayoutItemId = @ItemId", conn);
                cmd.Parameters.AddWithValue("@ItemId", itemId);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task<long> ResolveLocationIdAsync(SqliteConnection conn, LocationLayoutPointEntity point)
        {
            try
            {
                if (point.LocationId > 0)
                    return point.LocationId;

                using var cmd = new SqliteCommand("SELECT LocationId FROM LocationLayoutItem WHERE LocationLayoutItemId = @ItemId", conn);
                cmd.Parameters.AddWithValue("@ItemId", point.LocationLayoutItemId);

                var result = await cmd.ExecuteScalarAsync();
                return result is null || result == DBNull.Value ? 0 : Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddPointParameters(SqliteCommand cmd, LocationLayoutPointEntity point, long locationId)
        {
            try
            {
                cmd.Parameters.AddWithValue("@LocationLayoutItemId", point.LocationLayoutItemId);
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@PointIndex", point.PointIndex);
                cmd.Parameters.AddWithValue("@PointRole", point.PointRole ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@X", point.X);
                cmd.Parameters.AddWithValue("@Y", point.Y);
                cmd.Parameters.AddWithValue("@RotationDegrees", point.RotationDegrees ?? (object)DBNull.Value);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static LocationLayoutPointEntity MapPoint(SqliteDataReader reader)
        {
            try
            {
                return new LocationLayoutPointEntity
                {
                    LocationLayoutPointId = reader.GetInt64(reader.GetOrdinal("LocationLayoutPointId")),
                    LocationLayoutItemId = reader.GetInt64(reader.GetOrdinal("LocationLayoutItemId")),
                    LocationId = reader.GetInt64(reader.GetOrdinal("LocationId")),
                    PointIndex = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("PointIndex"))),
                    PointRole = GetNullableString(reader, "PointRole"),
                    X = reader.GetDouble(reader.GetOrdinal("X")),
                    Y = reader.GetDouble(reader.GetOrdinal("Y")),
                    RotationDegrees = GetNullableDouble(reader, "RotationDegrees"),
                    SegmentKind = LocationLayoutSegmentKind.Line
                };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

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

        private static double? GetNullableDouble(SqliteDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}