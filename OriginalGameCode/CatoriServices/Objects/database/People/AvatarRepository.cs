using Dapper;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.People;

public sealed class AvatarEntity
{
    public long AvatarId { get; set; }
    public string Name { get; set; }=string.Empty;
    public string Description { get; set; }=string.Empty;
    public string SettingsJson { get; set; }=string.Empty;
}

public sealed class AvatarRepository
{
    private readonly string _connectionString;
    public AvatarRepository(string? databasePath=null)
    {
        _connectionString=new SqliteConnectionStringBuilder{DataSource=databasePath??GlobalServices.Database}.ToString();
        using var connection=new SqliteConnection(_connectionString);connection.Open();
        connection.Execute("CREATE TABLE IF NOT EXISTS Avatar(AvatarId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT NOT NULL COLLATE NOCASE UNIQUE,Description TEXT NULL,SettingsJson TEXT NOT NULL);CREATE TABLE IF NOT EXISTS CurrentAvatar(CurrentAvatarId INTEGER PRIMARY KEY CHECK(CurrentAvatarId=1),AvatarName TEXT NOT NULL)");
    }
    public IReadOnlyList<AvatarEntity> GetAll(){using var c=new SqliteConnection(_connectionString);return c.Query<AvatarEntity>("SELECT AvatarId,Name,Description,SettingsJson FROM Avatar ORDER BY Name COLLATE NOCASE").ToList();}
    public AvatarEntity? GetByName(string name){using var c=new SqliteConnection(_connectionString);return c.QuerySingleOrDefault<AvatarEntity>("SELECT AvatarId,Name,Description,SettingsJson FROM Avatar WHERE Name=@name COLLATE NOCASE",new{name});}
    public void Save(AvatarEntity avatar){using var c=new SqliteConnection(_connectionString);c.Execute("INSERT INTO Avatar(Name,Description,SettingsJson) VALUES(@Name,@Description,@SettingsJson) ON CONFLICT(Name) DO UPDATE SET Description=excluded.Description,SettingsJson=excluded.SettingsJson",avatar);}
    public string? GetCurrentName(){using var c=new SqliteConnection(_connectionString);return c.QuerySingleOrDefault<string>("SELECT AvatarName FROM CurrentAvatar WHERE CurrentAvatarId=1");}
    public void SetCurrentName(string name){using var c=new SqliteConnection(_connectionString);c.Execute("INSERT INTO CurrentAvatar(CurrentAvatarId,AvatarName) VALUES(1,@name) ON CONFLICT(CurrentAvatarId) DO UPDATE SET AvatarName=excluded.AvatarName",new{name});}
}
