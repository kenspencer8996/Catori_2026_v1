namespace CatoriServices.Tests;

internal static class TestDatabaseScripts
{
    public static async Task<SqliteTestDatabase> CreateFromScriptAsync(params string[] scriptPathParts)
    {
        var db = new SqliteTestDatabase();
        var scriptPath = GetRepositoryPath(scriptPathParts);

        await db.ExecuteScriptAsync(await File.ReadAllTextAsync(scriptPath));
        return db;
    }

    public static string GetRepositoryPath(params string[] pathParts)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        return Path.Combine(new[] { root }.Concat(pathParts).ToArray());
    }
}
