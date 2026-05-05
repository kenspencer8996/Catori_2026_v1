using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public class FactoryLayoutDatabaseInitializer
    {
        private readonly string _connectionString;

        public FactoryLayoutDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InitializeTablesAsync()
        {
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            string scriptPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Database",
                "Scripts",
                "CreateFactoryLayoutTables.sql");

            string sql = File.Exists(scriptPath)
                ? await File.ReadAllTextAsync(scriptPath)
                : GetInlineCreateScript();

            using var cmd = new SqliteCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        private static string GetInlineCreateScript()
        {
            return @"
                CREATE TABLE IF NOT EXISTS Factory (
                    FactoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                    FactoryName TEXT NOT NULL,
                    BackgroundImagePath TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );
                CREATE TABLE IF NOT EXISTS FactoryLayoutObject (
                    LayoutObjectId INTEGER PRIMARY KEY AUTOINCREMENT,
                    FactoryId INTEGER NOT NULL,
                    ObjectName TEXT NOT NULL,
                    ObjectType TEXT NOT NULL,
                    ImagePath TEXT NULL,
                    ZIndex INTEGER NOT NULL DEFAULT 0,
                    IsInteractive INTEGER NOT NULL DEFAULT 1,
                    IsVisible INTEGER NOT NULL DEFAULT 1,
                    Notes TEXT NULL,
                    FOREIGN KEY (FactoryId) REFERENCES Factory(FactoryId) ON DELETE CASCADE
                );
                CREATE TABLE IF NOT EXISTS FactoryLayoutObjectPoint (
                    LayoutObjectPointId INTEGER PRIMARY KEY AUTOINCREMENT,
                    LayoutObjectId INTEGER NOT NULL,
                    PointIndex INTEGER NOT NULL,
                    X REAL NOT NULL,
                    Y REAL NOT NULL,
                    PointRole TEXT NULL,
                    FOREIGN KEY (LayoutObjectId) REFERENCES FactoryLayoutObject(LayoutObjectId) ON DELETE CASCADE
                );";
        }
    }
}
