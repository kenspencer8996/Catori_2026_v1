using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Locations
{
    public class LocationRepository
    {
        private readonly string _connectionString;

        public LocationRepository()
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

        public async Task<LocationEntity?> GetByIdAsync(int locationId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                var locationIdColumn = await GetLocationIdColumnAsync(conn);
                using var cmd = new SqliteCommand($"SELECT * FROM Location WHERE {locationIdColumn} = @LocationId", conn);
                cmd.Parameters.AddWithValue("@LocationId", locationId);

                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapLocation(reader) : null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<LocationLayoutEntity?> GetLayoutByIdAsync(long LocationId)
            => await GetActiveLayoutByLocationIdAsync(LocationId);

        public async Task<LocationLayoutEntity?> GetActiveLayoutByLocationIdAsync(long locationId)
        {
            try
            {
                var location = await GetByIdAsync((int)locationId);
                return location is null ? null : MapLocationAsLayout(location);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<LocationLayoutEntity>> GetLayoutsByLocationIdAsync(long locationId)
        {
            try
            {
                var layout = await GetActiveLayoutByLocationIdAsync(locationId);
                return layout is null ? new List<LocationLayoutEntity>() : new List<LocationLayoutEntity> { layout };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public Task<int> InsertLayoutAsync(LocationLayoutEntity layout)
        {
            try
            {
                return Task.FromResult(layout.LocationId);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public Task<bool> UpdateLayoutAsync(LocationLayoutEntity layout)
        {
            try
            {
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public Task<bool> DeleteLayoutAsync(long LocationId)
        {
            try
            {
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<LocationEntity?> GetByNameAsync(string locationName)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                var locationNameColumn = await GetLocationNameColumnAsync(conn);
                using var cmd = new SqliteCommand($"SELECT * FROM Location WHERE {locationNameColumn} = @LocationName ORDER BY {locationNameColumn} LIMIT 1", conn);
                cmd.Parameters.AddWithValue("@LocationName", locationName);

                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapLocation(reader) : null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<LocationEntity>> GetAllAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                var locationNameColumn = await GetLocationNameColumnAsync(conn);
                using var cmd = new SqliteCommand($"SELECT * FROM Location ORDER BY {locationNameColumn}", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                var list = new List<LocationEntity>();
                while (await reader.ReadAsync())
                    list.Add(MapLocation(reader));

                return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<LocationEntity>> GetByBusinessIdAsync(int businessId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                var locationNameColumn = await GetLocationNameColumnAsync(conn);
                using var cmd = new SqliteCommand($"SELECT * FROM Location WHERE BusinessId = @BusinessId ORDER BY {locationNameColumn}", conn);
                cmd.Parameters.AddWithValue("@BusinessId", businessId);
                using var reader = await cmd.ExecuteReaderAsync();

                var list = new List<LocationEntity>();
                while (await reader.ReadAsync())
                    list.Add(MapLocation(reader));

                return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertAsync(LocationEntity location)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    INSERT INTO Location
                        (LocationName, CreatedAt, BusinessId, Description, BackgroundImagePath)
                    VALUES
                        (@LocationName, @CreatedAt, @BusinessId, @Description, @BackgroundImagePath);
                    SELECT last_insert_rowid();";

                using var cmd = new SqliteCommand(sql, conn);
                AddParameters(cmd, location);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task UpdateAsync(LocationEntity location)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = @"
                    UPDATE Location
                    SET LocationName = @LocationName,
                        BusinessId = @BusinessId,
                        Description = @Description,
                        BackgroundImagePath = @BackgroundImagePath
                    WHERE LocationId = @LocationId";

                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@LocationId", location.LocationId);
                AddParameters(cmd, location);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task DeleteAsync(int locationId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                using var cmd = new SqliteCommand("DELETE FROM Location WHERE LocationId = @LocationId", conn);
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddParameters(SqliteCommand cmd, LocationEntity location)
        {
            try
            {
                cmd.Parameters.AddWithValue("@LocationName", location.LocationName);
                cmd.Parameters.AddWithValue("@CreatedAt", (location.CreatedAt == default ? DateTime.Now : location.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@BusinessId", location.BusinessId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", location.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BackgroundImagePath", location.BackgroundImagePath ?? "");
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static LocationEntity MapLocation(SqliteDataReader reader)
        {
            try
            {
                return new LocationEntity
                {
                    LocationId = GetRequiredInt(reader, "LocationId", "FactoryId"),
                    BusinessId = GetNullableInt(reader, "BusinessId"),
                    LocationName = GetRequiredString(reader, "LocationName", "FactoryName"),
                    Description = GetNullableString(reader, "Description"),
                    BackgroundImagePath = GetNullableString(reader, "BackgroundImagePath") ?? "",
                    CreatedAt = GetDateTimeOrDefault(reader, "CreatedAt", DateTime.Now)
                };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static LocationLayoutEntity MapLocationAsLayout(LocationEntity location)
        {
            try
            {
                return new LocationLayoutEntity
                {
                    LocationId = location.LocationId,
                    LayoutName = location.LocationName + " Layout",
                    CanvasWidth = 1920,
                    CanvasHeight = 1080,
                    IsActive = true,
                    CreatedAt = location.CreatedAt
                };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static bool HasColumn(SqliteDataReader reader, string columnName)
        {
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static Task<string> GetLocationIdColumnAsync(SqliteConnection conn)
            => GetFirstExistingColumnAsync(conn, "Location", "LocationId", "FactoryId");

        private static Task<string> GetLocationNameColumnAsync(SqliteConnection conn)
            => GetFirstExistingColumnAsync(conn, "Location", "LocationName", "FactoryName");

        private static async Task<string> GetFirstExistingColumnAsync(SqliteConnection conn, string tableName, params string[] columnNames)
        {
            try
            {
                foreach (var columnName in columnNames)
                {
                    if (await HasTableColumnAsync(conn, tableName, columnName))
                        return columnName;
                }

                return columnNames[0];
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task<bool> HasTableColumnAsync(SqliteConnection conn, string tableName, string columnName)
        {
            try
            {
                using var cmd = new SqliteCommand($"PRAGMA table_info({tableName})", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static string GetRequiredString(SqliteDataReader reader, string columnName, string legacyColumnName)
        {
            try
            {
                var resolvedColumnName = HasColumn(reader, columnName) ? columnName : legacyColumnName;
                var ordinal = reader.GetOrdinal(resolvedColumnName);
                return reader.IsDBNull(ordinal) ? "" : reader.GetString(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static int GetRequiredInt(SqliteDataReader reader, string columnName, string legacyColumnName)
        {
            try
            {
                var resolvedColumnName = HasColumn(reader, columnName) ? columnName : legacyColumnName;
                var ordinal = reader.GetOrdinal(resolvedColumnName);
                return reader.GetInt32(ordinal);
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
                if (!HasColumn(reader, columnName))
                    return null;

                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static int? GetNullableInt(SqliteDataReader reader, string columnName)
        {
            try
            {
                if (!HasColumn(reader, columnName))
                    return null;

                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static DateTime GetDateTimeOrDefault(SqliteDataReader reader, string columnName, DateTime defaultValue)
        {
            try
            {
                var value = GetNullableString(reader, columnName);
                return DateTime.TryParse(value, out var parsed) ? parsed : defaultValue;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}