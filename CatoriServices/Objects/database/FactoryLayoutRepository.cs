using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;
using System.Data;

public class FactoryLayoutRepository
{
    private readonly string _connectionString;

    public FactoryLayoutRepository()
    {
        _connectionString = "Data Source=" + GlobalServices.Database + " ;";
    }

    public void Insert(FactoryLayoutEntity layout)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string sql = @"
            INSERT INTO FactoryLayout (FactoryLayoutId, FactoryId, Name, Description)
            VALUES (@FactoryLayoutId, @FactoryId, @Name, @Description);";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FactoryLayoutId", layout.FactoryLayoutId);
        cmd.Parameters.AddWithValue("@FactoryId", layout.FactoryId);
        cmd.Parameters.AddWithValue("@Name", layout.Name);
        cmd.Parameters.AddWithValue("@Description", layout.Description);

        cmd.ExecuteNonQuery();
    }

    public void Update(FactoryLayoutEntity layout)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string sql = @"
            UPDATE FactoryLayout
            SET FactoryId = @FactoryId,
                Name = @Name,
                Description = @Description
            WHERE FactoryLayoutId = @FactoryLayoutId;";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FactoryLayoutId", layout.FactoryLayoutId);
        cmd.Parameters.AddWithValue("@FactoryId", layout.FactoryId);
        cmd.Parameters.AddWithValue("@Name", layout.Name);
        cmd.Parameters.AddWithValue("@Description", layout.Description);

        cmd.ExecuteNonQuery();
    }

    public void Delete(long id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string sql = "DELETE FROM FactoryLayout WHERE FactoryLayoutId = @id;";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();
    }

    public FactoryLayoutEntity? GetById(long id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string sql = "SELECT * FROM FactoryLayout WHERE FactoryLayoutId = @id;";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return new FactoryLayoutEntity
        {
            FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("FactoryLayoutId")),
            FactoryId = reader.GetInt64(reader.GetOrdinal("FactoryId")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.GetString(reader.GetOrdinal("Description"))
        };
    }
    public FactoryLayoutEntity? GetByFactoryInteriorName(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string sql = "SELECT * FROM FactoryLayout WHERE Name = @name;";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", name);

        using SqliteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return new FactoryLayoutEntity
        {
            FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("FactoryLayoutId")),
            FactoryId = reader.GetInt64(reader.GetOrdinal("FactoryId")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.GetString(reader.GetOrdinal("Description"))
        };
    }
    public List<FactoryLayoutEntity> GetAll()
    {
        var list = new List<FactoryLayoutEntity>();

        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string sql = "SELECT * FROM FactoryLayout;";

        using var cmd = new SqliteCommand(sql, conn);
        using SqliteDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new FactoryLayoutEntity
            {
                FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("FactoryLayoutId")),
                FactoryId = reader.GetInt64(reader.GetOrdinal("FactoryId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description"))
            });
        }

        return list;
    }
}
