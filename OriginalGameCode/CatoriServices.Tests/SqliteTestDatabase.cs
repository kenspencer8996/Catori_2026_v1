using Microsoft.Data.Sqlite;

namespace CatoriServices.Tests;

internal sealed class SqliteTestDatabase : IDisposable
{
    public string DatabasePath { get; }

    public SqliteTestDatabase()
    {
        DatabasePath = Path.Combine(Path.GetTempPath(), "CatoriServicesTests", Guid.NewGuid() + ".db");
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
    }

    public async Task ExecuteScriptAsync(string script)
    {
        await using var conn = new SqliteConnection("Data Source=" + DatabasePath);
        await conn.OpenAsync();
        await using var cmd = new SqliteCommand(script, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(DatabasePath))
                File.Delete(DatabasePath);
        }
        catch
        {
            // Best-effort cleanup. A failed delete should not hide the test result.
        }
    }
}
