using CatoriServices.Objects;

namespace CatoriServices.Tests;

[CollectionDefinition(Name)]
public sealed class GlobalTestStateCollection : ICollectionFixture<GlobalTestState>
{
    public const string Name = "GlobalServices database tests";
}

public sealed class GlobalTestState : IDisposable
{
    private readonly string _originalDatabase = GlobalServices.Database;
    private readonly string _originalLogFilePath = cLogger.LogFilePath;
    private readonly string _testLogFilePath = Path.Combine(Path.GetTempPath(), "CatoriServicesTests", "test.log");

    public void UseDatabase(string databasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_testLogFilePath)!);
        GlobalServices.Database = databasePath;
        cLogger.LogFilePath = _testLogFilePath;
    }

    public void Dispose()
    {
        GlobalServices.Database = _originalDatabase;
        cLogger.LogFilePath = _originalLogFilePath;
    }
}

