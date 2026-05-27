using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;

public class ComponentRepository
{
    private readonly string _connectionString;

    public ComponentRepository()
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

    public async Task<ComponentEntity?> GetByIdAsync(int componentId)
    {
        try
        {
                    using var conn = GetConnection();
                    await conn.OpenAsync();
            
                    string sql = "SELECT * FROM Components WHERE component_id = @ComponentId";
            
                    using var cmd = new SqliteCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ComponentId", componentId);
            
                    using var reader = await cmd.ExecuteReaderAsync();
            
                    if (!await reader.ReadAsync())
                        return null;
            
                    return MapComponent(reader);
        }
        catch (Exception ex)
        {
            cLogger.Log(ex.ToString());
            throw;
        }
    }

    public async Task<List<ComponentEntity>> GetAllAsync()
    {
        try
        {
                    using var conn = GetConnection();
                    await conn.OpenAsync();
            
                    string sql = "SELECT * FROM Components ORDER BY component_name";
            
                    using var cmd = new SqliteCommand(sql, conn);
                    using var reader = await cmd.ExecuteReaderAsync();
            
                    var list = new List<ComponentEntity>();
            
                    while (await reader.ReadAsync())
                        list.Add(MapComponent(reader));
            
                    return list;
        }
        catch (Exception ex)
        {
            cLogger.Log(ex.ToString());
            throw;
        }
    }

    public async Task<int> InsertAsync(ComponentEntity component)
    {
        try
        {
                    using var conn = GetConnection();
                    await conn.OpenAsync();
            
                    string sql = @"
                        INSERT INTO Components (component_name, quantity)
                        VALUES (@ComponentName, @Quantity);
                        SELECT last_insert_rowid();";
            
                    using var cmd = new SqliteCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ComponentName", component.ComponentName);
                    cmd.Parameters.AddWithValue("@Quantity", component.Quantity);
            
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            cLogger.Log(ex.ToString());
            throw;
        }
    }

    public async Task<bool> UpdateAsync(ComponentEntity component)
    {
        try
        {
                    using var conn = GetConnection();
                    await conn.OpenAsync();
            
                    string sql = @"
                        UPDATE Components
                        SET component_name = @ComponentName,
                            quantity = @Quantity
                        WHERE component_id = @ComponentId";
            
                    using var cmd = new SqliteCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ComponentId", component.ComponentId);
                    cmd.Parameters.AddWithValue("@ComponentName", component.ComponentName);
                    cmd.Parameters.AddWithValue("@Quantity", component.Quantity);
            
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            cLogger.Log(ex.ToString());
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int componentId)
    {
        try
        {
                    using var conn = GetConnection();
                    await conn.OpenAsync();
            
                    string sql = "DELETE FROM Components WHERE component_id = @ComponentId";
            
                    using var cmd = new SqliteCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ComponentId", componentId);
            
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            cLogger.Log(ex.ToString());
            throw;
        }
    }

    private ComponentEntity MapComponent(SqliteDataReader reader)
    {
        try
        {
                    return new ComponentEntity
                    {
                        ComponentId = reader.GetInt32(reader.GetOrdinal("component_id")),
                        ComponentName = reader.GetString(reader.GetOrdinal("component_name")),
                        Quantity = reader.GetDecimal(reader.GetOrdinal("quantity"))
                    };
        }
        catch (Exception ex)
        {
            cLogger.Log(ex.ToString());
            throw;
        }
    }
}
