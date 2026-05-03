using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public class FactoryPartRouteRepository
    {
        private readonly string _connectionString;

        public FactoryPartRouteRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<FactoryPartRouteEntity?> GetByIdAsync(long routeId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM FactoryPartRoutes WHERE factory_part_route_id = @RouteId", conn);
            cmd.Parameters.AddWithValue("@RouteId", routeId);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapRoute(reader) : null;
        }

        public async Task<List<FactoryPartRouteEntity>> GetByLayoutIdAsync(long factoryLayoutId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM FactoryPartRoutes WHERE factory_layout_id = @FactoryLayoutId ORDER BY route_name", conn);
            cmd.Parameters.AddWithValue("@FactoryLayoutId", factoryLayoutId);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryPartRouteEntity>();
            while (await reader.ReadAsync())
                list.Add(MapRoute(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryPartRouteEntity route)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO FactoryPartRoutes
                    (factory_layout_id, route_name, product_id, from_item_id, to_item_id, is_active, created_at)
                VALUES
                    (@FactoryLayoutId, @RouteName, @ProductId, @FromItemId, @ToItemId, @IsActive, @CreatedAt);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            AddRouteParameters(cmd, route);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(FactoryPartRouteEntity route)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE FactoryPartRoutes
                SET factory_layout_id = @FactoryLayoutId,
                    route_name = @RouteName,
                    product_id = @ProductId,
                    from_item_id = @FromItemId,
                    to_item_id = @ToItemId,
                    is_active = @IsActive
                WHERE factory_part_route_id = @FactoryPartRouteId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryPartRouteId", route.FactoryPartRouteId);
            AddRouteParameters(cmd, route);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long routeId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM FactoryPartRoutes WHERE factory_part_route_id = @RouteId", conn);
            cmd.Parameters.AddWithValue("@RouteId", routeId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<FactoryPartRoutePointEntity>> GetPointsAsync(long routeId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM FactoryPartRoutePoints WHERE factory_part_route_id = @RouteId ORDER BY point_index", conn);
            cmd.Parameters.AddWithValue("@RouteId", routeId);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryPartRoutePointEntity>();
            while (await reader.ReadAsync())
                list.Add(MapRoutePoint(reader));

            return list;
        }

        public async Task<int> InsertPointAsync(FactoryPartRoutePointEntity point)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO FactoryPartRoutePoints (factory_part_route_id, point_index, x, y, z, seconds_from_start)
                VALUES (@FactoryPartRouteId, @PointIndex, @X, @Y, @Z, @SecondsFromStart);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            AddPointParameters(cmd, point);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> ReplacePointsAsync(long routeId, IEnumerable<FactoryPartRoutePointEntity> points)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = await conn.BeginTransactionAsync();

            using (var deleteCmd = new SqliteCommand("DELETE FROM FactoryPartRoutePoints WHERE factory_part_route_id = @RouteId", conn, (SqliteTransaction)tx))
            {
                deleteCmd.Parameters.AddWithValue("@RouteId", routeId);
                await deleteCmd.ExecuteNonQueryAsync();
            }

            foreach (var point in points)
            {
                point.FactoryPartRouteId = routeId;
                string sql = @"
                    INSERT INTO FactoryPartRoutePoints (factory_part_route_id, point_index, x, y, z, seconds_from_start)
                    VALUES (@FactoryPartRouteId, @PointIndex, @X, @Y, @Z, @SecondsFromStart);";

                using var insertCmd = new SqliteCommand(sql, conn, (SqliteTransaction)tx);
                AddPointParameters(insertCmd, point);
                await insertCmd.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
            return true;
        }

        private static void AddRouteParameters(SqliteCommand cmd, FactoryPartRouteEntity route)
        {
            cmd.Parameters.AddWithValue("@FactoryLayoutId", route.FactoryLayoutId);
            cmd.Parameters.AddWithValue("@RouteName", route.RouteName);
            cmd.Parameters.AddWithValue("@ProductId", route.ProductId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FromItemId", route.FromItemId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ToItemId", route.ToItemId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", route.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@CreatedAt", (route.CreatedAt == default ? DateTime.Now : route.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private static void AddPointParameters(SqliteCommand cmd, FactoryPartRoutePointEntity point)
        {
            cmd.Parameters.AddWithValue("@FactoryPartRouteId", point.FactoryPartRouteId);
            cmd.Parameters.AddWithValue("@PointIndex", point.PointIndex);
            cmd.Parameters.AddWithValue("@X", point.X);
            cmd.Parameters.AddWithValue("@Y", point.Y);
            cmd.Parameters.AddWithValue("@Z", point.Z);
            cmd.Parameters.AddWithValue("@SecondsFromStart", point.SecondsFromStart);
        }

        private static FactoryPartRouteEntity MapRoute(SqliteDataReader reader)
        {
            int idxProduct = reader.GetOrdinal("product_id");
            int idxFrom = reader.GetOrdinal("from_item_id");
            int idxTo = reader.GetOrdinal("to_item_id");

            return new FactoryPartRouteEntity
            {
                FactoryPartRouteId = reader.GetInt64(reader.GetOrdinal("factory_part_route_id")),
                FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("factory_layout_id")),
                RouteName = reader.GetString(reader.GetOrdinal("route_name")),
                ProductId = reader.IsDBNull(idxProduct) ? null : reader.GetInt32(idxProduct),
                FromItemId = reader.IsDBNull(idxFrom) ? null : reader.GetInt64(idxFrom),
                ToItemId = reader.IsDBNull(idxTo) ? null : reader.GetInt64(idxTo),
                IsActive = reader.GetInt32(reader.GetOrdinal("is_active")) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("created_at")))
            };
        }

        private static FactoryPartRoutePointEntity MapRoutePoint(SqliteDataReader reader)
        {
            return new FactoryPartRoutePointEntity
            {
                FactoryPartRoutePointId = reader.GetInt64(reader.GetOrdinal("factory_part_route_point_id")),
                FactoryPartRouteId = reader.GetInt64(reader.GetOrdinal("factory_part_route_id")),
                PointIndex = reader.GetInt32(reader.GetOrdinal("point_index")),
                X = reader.GetDouble(reader.GetOrdinal("x")),
                Y = reader.GetDouble(reader.GetOrdinal("y")),
                Z = reader.GetDouble(reader.GetOrdinal("z")),
                SecondsFromStart = reader.GetDouble(reader.GetOrdinal("seconds_from_start"))
            };
        }
    }
}
