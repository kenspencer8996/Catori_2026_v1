using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Locations
{
    public class LocationLayoutDatabaseInitializer
    {
        private readonly string _connectionString;

        public LocationLayoutDatabaseInitializer(string connectionString)
        {
            try
            {
                            _connectionString = connectionString;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task InitializeTablesAsync()
        {
            try
            {
                            using var conn = new SqliteConnection(_connectionString);
                            await conn.OpenAsync();
                
                            string scriptPath = Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                "Database",
                                "Scripts",
                                "Location",
                                "CreateLocationLayoutTables.sql");
                
                            if (File.Exists(scriptPath))
                            {
                                string sql = await File.ReadAllTextAsync(scriptPath);
                                using var cmd = new SqliteCommand(sql, conn);
                                await cmd.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                await CreateTablesInlineAsync(conn);
                            }

                            await EnsureLocationLayoutItemItemDataJsonColumnAsync(conn);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task CreateTablesInlineAsync(SqliteConnection conn)
        {
            try
            {
                            const string sql = @"
                                CREATE TABLE IF NOT EXISTS Location (
                                    LocationId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    LocationName TEXT NOT NULL,
                                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    BusinessId INTEGER NULL,
                                    Description TEXT NULL,
                                    BackgroundImagePath TEXT NOT NULL DEFAULT ''
                                );
                
                                CREATE TABLE IF NOT EXISTS LocationLayoutItem (
                                    LocationLayoutItemId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    LocationId INTEGER NOT NULL,
                                    ItemName TEXT NOT NULL,
                                    ItemType TEXT NOT NULL,
                                    MajorItemType TEXT NULL,
                                    X REAL NOT NULL DEFAULT 0,
                                    Y REAL NOT NULL DEFAULT 0,
                                    Z REAL NOT NULL DEFAULT 0,
                                    Width REAL NOT NULL DEFAULT 0,
                                    Height REAL NOT NULL DEFAULT 0,
                                    RotationDegrees REAL NOT NULL DEFAULT 0,
                                    ZIndex INTEGER NOT NULL DEFAULT 0,
                                    IsLocked INTEGER NOT NULL DEFAULT 0,
                                    ItemDataJson TEXT NULL,
                                    MetadataJson TEXT NULL,
                                    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
                                );
                
                                CREATE INDEX IF NOT EXISTS idx_location_layout_item_location_id
                                ON LocationLayoutItem(LocationId);
                
                                CREATE TABLE IF NOT EXISTS LocationLayoutPoint (
                                    LocationLayoutPointId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                    LocationLayoutItemId INTEGER NOT NULL,
                                    LocationId INTEGER NOT NULL,
                                    PointIndex INTEGER NOT NULL,
                                    PointRole TEXT NULL,
                                    X REAL NOT NULL,
                                    Y REAL NOT NULL,
                                    RotationDegrees REAL NULL,
                                    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE,
                                    FOREIGN KEY (LocationLayoutItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE
                                );
                
                                CREATE INDEX IF NOT EXISTS idx_location_layout_point_item_id
                                ON LocationLayoutPoint(LocationLayoutItemId);";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task EnsureLocationLayoutItemItemDataJsonColumnAsync(SqliteConnection conn)
        {
            try
            {
                var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var check = new SqliteCommand("PRAGMA table_info(LocationLayoutItem);", conn))
                using (var reader = await check.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        columns.Add(reader.GetString(1));
                }

                if (columns.Contains("ItemDataJson"))
                    return;

                var sql = columns.Contains("WpfPath")
                    ? "ALTER TABLE LocationLayoutItem RENAME COLUMN WpfPath TO ItemDataJson;"
                    : "ALTER TABLE LocationLayoutItem ADD COLUMN ItemDataJson TEXT NULL;";
                using var alter = new SqliteCommand(sql, conn);
                await alter.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}


