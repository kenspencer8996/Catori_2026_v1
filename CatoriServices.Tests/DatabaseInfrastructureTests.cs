using System.Data;
using System.Reflection;
using CatoriServices.Objects.database;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class DatabaseInfrastructureTests
{
    private readonly GlobalTestState _globalState;

    public DatabaseInfrastructureTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public void SQLiteConnection_placeholder_can_be_constructed()
    {
        var type = typeof(LocationRepository).Assembly.GetType("CatoriServices.Objects.database.Core.SQLiteConnection");

        Assert.NotNull(type);
        Assert.NotNull(Activator.CreateInstance(type!, nonPublic: true));
    }

    [Fact]
    public async Task Internal_DatabaseHelper_and_AdoNetHelper_return_open_sqlite_connections()
    {
        using var db = new SqliteTestDatabase();
        _globalState.UseDatabase(db.DatabasePath);

        using var databaseHelperConnection = InvokeInternalConnection("CatoriServices.Objects.database.Core.DatabaseHelper");
        using var adoNetHelperConnection = InvokeInternalConnection("CatoriServices.Objects.database.Core.AdoNetHelper");

        Assert.Equal(ConnectionState.Open, databaseHelperConnection.State);
        Assert.Equal(ConnectionState.Open, adoNetHelperConnection.State);
    }

    [Fact]
    public async Task Internal_SqlHelper_reads_query_results_and_closes_connection_with_reader()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Probe (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL);
            INSERT INTO Probe (Id, Name) VALUES (1, 'ok');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var helperType = typeof(LocationRepository).Assembly.GetType("CatoriServices.Objects.database.Core.SqlHelper");
        Assert.NotNull(helperType);
        var helper = Activator.CreateInstance(helperType!, nonPublic: true);
        var method = helperType!.GetMethod("GetReader", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        using var reader = Assert.IsAssignableFrom<IDataReader>(method!.Invoke(helper, new object[] { "SELECT Name FROM Probe WHERE Id = 1" }));

        Assert.True(reader.Read());
        Assert.Equal("ok", reader.GetString(0));
    }

    private static SqliteConnection InvokeInternalConnection(string typeName)
    {
        var type = typeof(LocationRepository).Assembly.GetType(typeName);
        Assert.NotNull(type);
        var helper = Activator.CreateInstance(type!, nonPublic: true);
        var method = type!.GetMethod("GetConnection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(method);

        return Assert.IsType<SqliteConnection>(method!.Invoke(helper, Array.Empty<object>()));
    }
}

