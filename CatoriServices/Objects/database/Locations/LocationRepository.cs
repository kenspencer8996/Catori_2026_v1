using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Locations
{
    public class LocationRepository
    {
        private readonly string _connectionString;

        public LocationRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
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

                throw;
            }
        }

        public async Task<LocationLayoutEntity?> GetLayoutByIdAsync(long LocationId)
            => await GetActiveLayoutByLocationIdAsync(LocationId);

        public async Task<LocationLayoutEntity?> GetActiveLayoutByLocationIdAsync(long locationId)
        {
            var location = await GetByIdAsync((int)locationId);
            return location is null ? null : MapLocationAsLayout(location);
        }

        public async Task<List<LocationLayoutEntity>> GetLayoutsByLocationIdAsync(long locationId)
        {
            var layout = await GetActiveLayoutByLocationIdAsync(locationId);
            return layout is null ? new List<LocationLayoutEntity>() : new List<LocationLayoutEntity> { layout };
        }

        public Task<int> InsertLayoutAsync(LocationLayoutEntity layout)
        {
            // LocationId is intentionally the LocationId in the live schema.
            return Task.FromResult(layout.LocationId);
        }

        public Task<bool> UpdateLayoutAsync(LocationLayoutEntity layout)
        {
            // Canvas/name metadata is stored on Location in the live schema.
            return Task.FromResult(true);
        }

        public Task<bool> DeleteLayoutAsync(long LocationId)
        {
            // Deleting a layout would mean deleting the Location row in the live schema.
            return Task.FromResult(false);
        }

        public async Task<LocationEntity?> GetByNameAsync(string locationName)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            var locationNameColumn = await GetLocationNameColumnAsync(conn);
            var orderBy = await GetLocationOrderByAsync(conn, locationNameColumn);
            using var cmd = new SqliteCommand($"SELECT * FROM Location WHERE {locationNameColumn} = @LocationName ORDER BY {orderBy} LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@LocationName", locationName);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapLocation(reader) : null;
        }

        public async Task<List<LocationEntity>> GetAllAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            var locationNameColumn = await GetLocationNameColumnAsync(conn);
            var orderBy = await GetLocationOrderByAsync(conn, locationNameColumn);
            using var cmd = new SqliteCommand($"SELECT * FROM Location ORDER BY {orderBy}", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<LocationEntity>();
            while (await reader.ReadAsync())
                list.Add(MapLocation(reader));

            return list;
        }

        public async Task<List<LocationEntity>> GetByBusinessIdAsync(int businessId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            var locationNameColumn = await GetLocationNameColumnAsync(conn);
            var orderBy = await GetLocationOrderByAsync(conn, locationNameColumn);
            using var cmd = new SqliteCommand($"SELECT * FROM Location WHERE BusinessId = @BusinessId ORDER BY {orderBy}", conn);
            cmd.Parameters.AddWithValue("@BusinessId", businessId);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<LocationEntity>();
            while (await reader.ReadAsync())
                list.Add(MapLocation(reader));

            return list;
        }

        public async Task<int> InsertAsync(LocationEntity location)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO Location
                    (BusinessId, LocationName, Description, BackgroundImagePath, InteriorType, WorldMapImagePath,
                     HotspotLeft, HotspotTop, HotspotWidth, HotspotHeight, DesignWidth, DesignHeight,
                     DefaultRobotX, DefaultRobotY, IsActive, SortOrder, CreatedAt, UpdatedDate)
                VALUES
                    (@BusinessId, @LocationName, @Description, @BackgroundImagePath, @InteriorType, @WorldMapImagePath,
                     @HotspotLeft, @HotspotTop, @HotspotWidth, @HotspotHeight, @DesignWidth, @DesignHeight,
                     @DefaultRobotX, @DefaultRobotY, @IsActive, @SortOrder, @CreatedAt, @UpdatedDate);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            AddParameters(cmd, location);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task UpdateAsync(LocationEntity location)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                UPDATE Location
                SET BusinessId = @BusinessId,
                    LocationName = @LocationName,
                    Description = @Description,
                    BackgroundImagePath = @BackgroundImagePath,
                    InteriorType = @InteriorType,
                    WorldMapImagePath = @WorldMapImagePath,
                    HotspotLeft = @HotspotLeft,
                    HotspotTop = @HotspotTop,
                    HotspotWidth = @HotspotWidth,
                    HotspotHeight = @HotspotHeight,
                    DesignWidth = @DesignWidth,
                    DesignHeight = @DesignHeight,
                    DefaultRobotX = @DefaultRobotX,
                    DefaultRobotY = @DefaultRobotY,
                    IsActive = @IsActive,
                    SortOrder = @SortOrder,
                    UpdatedDate = @UpdatedDate
                WHERE LocationId = @LocationId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@LocationId", location.LocationId);
            location.UpdatedDate = DateTime.Now;
            AddParameters(cmd, location);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int locationId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM Location WHERE LocationId = @LocationId", conn);
            cmd.Parameters.AddWithValue("@LocationId", locationId);
            await cmd.ExecuteNonQueryAsync();
        }

        private static void AddParameters(SqliteCommand cmd, LocationEntity location)
        {
            cmd.Parameters.AddWithValue("@BusinessId", location.BusinessId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LocationName", location.LocationName);
            cmd.Parameters.AddWithValue("@Description", location.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BackgroundImagePath", location.BackgroundImagePath ?? "");
            cmd.Parameters.AddWithValue("@InteriorType", string.IsNullOrWhiteSpace(location.InteriorType) ? nameof(LocationEntity) : location.InteriorType);
            cmd.Parameters.AddWithValue("@WorldMapImagePath", location.WorldMapImagePath ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HotspotLeft", location.HotspotLeft);
            cmd.Parameters.AddWithValue("@HotspotTop", location.HotspotTop);
            cmd.Parameters.AddWithValue("@HotspotWidth", location.HotspotWidth);
            cmd.Parameters.AddWithValue("@HotspotHeight", location.HotspotHeight);
            cmd.Parameters.AddWithValue("@DesignWidth", location.DesignWidth);
            cmd.Parameters.AddWithValue("@DesignHeight", location.DesignHeight);
            cmd.Parameters.AddWithValue("@DefaultRobotX", location.DefaultRobotX ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DefaultRobotY", location.DefaultRobotY ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", location.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@SortOrder", location.SortOrder);
            cmd.Parameters.AddWithValue("@CreatedAt", (location.CreatedAt == default ? DateTime.Now : location.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@UpdatedDate", location.UpdatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
        }

        private static LocationEntity MapLocation(SqliteDataReader reader)
        {
            return new LocationEntity
            {
                LocationId = GetRequiredInt(reader, "LocationId", "FactoryId"),
                BusinessId = GetNullableInt(reader, "BusinessId"),
                LocationName = GetRequiredString(reader, "LocationName", "FactoryName"),
                Description = GetNullableString(reader, "Description"),
                BackgroundImagePath = GetNullableString(reader, "BackgroundImagePath") ?? "",
                InteriorType = GetNullableString(reader, "InteriorType") ?? nameof(LocationEntity),
                WorldMapImagePath = GetNullableString(reader, "WorldMapImagePath"),
                HotspotLeft = GetDoubleOrDefault(reader, "HotspotLeft", 0),
                HotspotTop = GetDoubleOrDefault(reader, "HotspotTop", 0),
                HotspotWidth = GetDoubleOrDefault(reader, "HotspotWidth", 100),
                HotspotHeight = GetDoubleOrDefault(reader, "HotspotHeight", 100),
                DesignWidth = GetDoubleOrDefault(reader, "DesignWidth", 1920),
                DesignHeight = GetDoubleOrDefault(reader, "DesignHeight", 1080),
                DefaultRobotX = GetNullableDouble(reader, "DefaultRobotX"),
                DefaultRobotY = GetNullableDouble(reader, "DefaultRobotY"),
                IsActive = GetIntOrDefault(reader, "IsActive", 1) == 1,
                SortOrder = GetIntOrDefault(reader, "SortOrder", 0),
                CreatedAt = GetDateTimeOrDefault(reader, "CreatedAt", DateTime.Now),
                UpdatedDate = GetNullableDateTime(reader, "UpdatedDate")
            };
        }

        private static LocationLayoutEntity MapLocationAsLayout(LocationEntity location)
        {
            return new LocationLayoutEntity
            {
                LocationId = location.LocationId,
                LayoutName = location.LocationName + " Layout",
                CanvasWidth = location.DesignWidth,
                CanvasHeight = location.DesignHeight,
                IsActive = location.IsActive,
                CreatedAt = location.CreatedAt
            };
        }

        private static bool HasColumn(SqliteDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static Task<string> GetLocationIdColumnAsync(SqliteConnection conn)
            => GetFirstExistingColumnAsync(conn, "Location", "LocationId", "FactoryId");

        private static Task<string> GetLocationNameColumnAsync(SqliteConnection conn)
            => GetFirstExistingColumnAsync(conn, "Location", "LocationName", "FactoryName");

        private static async Task<string> GetLocationOrderByAsync(SqliteConnection conn, string locationNameColumn)
        {
            return await HasTableColumnAsync(conn, "Location", "SortOrder")
                ? $"SortOrder, {locationNameColumn}"
                : locationNameColumn;
        }

        private static async Task<string> GetFirstExistingColumnAsync(SqliteConnection conn, string tableName, params string[] columnNames)
        {
            foreach (var columnName in columnNames)
            {
                if (await HasTableColumnAsync(conn, tableName, columnName))
                    return columnName;
            }

            return columnNames[0];
        }

        private static async Task<bool> HasTableColumnAsync(SqliteConnection conn, string tableName, string columnName)
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

        private static string GetRequiredString(SqliteDataReader reader, string columnName, string legacyColumnName)
        {
            var resolvedColumnName = HasColumn(reader, columnName) ? columnName : legacyColumnName;
            var ordinal = reader.GetOrdinal(resolvedColumnName);
            return reader.IsDBNull(ordinal) ? "" : reader.GetString(ordinal);
        }

        private static int GetRequiredInt(SqliteDataReader reader, string columnName, string legacyColumnName)
        {
            var resolvedColumnName = HasColumn(reader, columnName) ? columnName : legacyColumnName;
            var ordinal = reader.GetOrdinal(resolvedColumnName);
            return reader.GetInt32(ordinal);
        }

        private static string? GetNullableString(SqliteDataReader reader, string columnName)
        {
            if (!HasColumn(reader, columnName))
                return null;

            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static int? GetNullableInt(SqliteDataReader reader, string columnName)
        {
            if (!HasColumn(reader, columnName))
                return null;

            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        private static int GetIntOrDefault(SqliteDataReader reader, string columnName, int defaultValue)
        {
            if (!HasColumn(reader, columnName))
                return defaultValue;

            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
        }

        private static double GetDoubleOrDefault(SqliteDataReader reader, string columnName, double defaultValue)
        {
            if (!HasColumn(reader, columnName))
                return defaultValue;

            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDouble(ordinal);
        }

        private static double? GetNullableDouble(SqliteDataReader reader, string columnName)
        {
            if (!HasColumn(reader, columnName))
                return null;

            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);
        }

        private static DateTime GetDateTimeOrDefault(SqliteDataReader reader, string columnName, DateTime defaultValue)
        {
            var value = GetNullableString(reader, columnName);
            return DateTime.TryParse(value, out var parsed) ? parsed : defaultValue;
        }

        private static DateTime? GetNullableDateTime(SqliteDataReader reader, string columnName)
        {
            var value = GetNullableString(reader, columnName);
            return DateTime.TryParse(value, out var parsed) ? parsed : null;
        }
    }
}


