using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Manufacturing
{
    public class MachineRepository
    {
        private readonly string _connectionString;

        public MachineRepository()
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

        public async Task<MachineEntity?> GetByIdAsync(int machineId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM Machine WHERE MachineId = @MachineId";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@MachineId", machineId);
                
                            using var reader = await cmd.ExecuteReaderAsync();
                            return await reader.ReadAsync() ? MapMachine(reader) : null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<MachineEntity>> GetAllAsync()
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM Machine ORDER BY Name";
                            using var cmd = new SqliteCommand(sql, conn);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<MachineEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapMachine(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<MachineEntity>> GetByMachineTypeIdAsync(int machineTypeId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM Machine WHERE MachineTypeId = @MachineTypeId ORDER BY Name";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@MachineTypeId", machineTypeId);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<MachineEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapMachine(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertAsync(MachineEntity machine)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                INSERT INTO Machine
                                    (MachineTypeId, Name, ImagePath, ControlTypeName, Description)
                                VALUES
                                    (@MachineTypeId, @Name, @ImagePath, @ControlTypeName, @Description);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddParameters(cmd, machine);
                
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(MachineEntity machine)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                UPDATE Machine
                                SET MachineTypeId = @MachineTypeId,
                                    Name = @Name,
                                    ImagePath = @ImagePath,
                                    ControlTypeName = @ControlTypeName,
                                    Description = @Description
                                WHERE MachineId = @MachineId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@MachineId", machine.MachineId);
                            AddParameters(cmd, machine);
                
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int machineId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "DELETE FROM Machine WHERE MachineId = @MachineId";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@MachineId", machineId);
                
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddParameters(SqliteCommand cmd, MachineEntity machine)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@MachineTypeId", machine.MachineTypeId);
                            cmd.Parameters.AddWithValue("@Name", machine.Name);
                            cmd.Parameters.AddWithValue("@ImagePath", machine.ImagePath ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ControlTypeName", machine.ControlTypeName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Description", machine.Description ?? (object)DBNull.Value);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static MachineEntity MapMachine(SqliteDataReader reader)
        {
            try
            {
                            return new MachineEntity
                            {
                                MachineId = reader.GetInt32(reader.GetOrdinal("MachineId")),
                                MachineTypeId = reader.GetInt32(reader.GetOrdinal("MachineTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImagePath = GetNullableString(reader, "ImagePath"),
                                ControlTypeName = GetNullableString(reader, "ControlTypeName"),
                                Description = GetNullableString(reader, "Description")
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static string? GetNullableString(SqliteDataReader reader, string columnName)
        {
            try
            {
                            var ordinal = reader.GetOrdinal(columnName);
                            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}


