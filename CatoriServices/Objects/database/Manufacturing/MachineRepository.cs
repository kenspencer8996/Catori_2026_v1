using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Manufacturing
{
    public class MachineRepository
    {
        private readonly string _connectionString;

        public MachineRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<MachineEntity?> GetByIdAsync(int machineId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "SELECT * FROM Machine WHERE MachineId = @MachineId";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MachineId", machineId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapMachine(reader) : null;
        }

        public async Task<List<MachineEntity>> GetAllAsync()
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

        public async Task<List<MachineEntity>> GetByMachineTypeIdAsync(int machineTypeId)
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

        public async Task<int> InsertAsync(MachineEntity machine)
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

        public async Task<bool> UpdateAsync(MachineEntity machine)
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

        public async Task<bool> DeleteAsync(int machineId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "DELETE FROM Machine WHERE MachineId = @MachineId";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MachineId", machineId);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static void AddParameters(SqliteCommand cmd, MachineEntity machine)
        {
            cmd.Parameters.AddWithValue("@MachineTypeId", machine.MachineTypeId);
            cmd.Parameters.AddWithValue("@Name", machine.Name);
            cmd.Parameters.AddWithValue("@ImagePath", machine.ImagePath ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ControlTypeName", machine.ControlTypeName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", machine.Description ?? (object)DBNull.Value);
        }

        private static MachineEntity MapMachine(SqliteDataReader reader)
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

        private static string? GetNullableString(SqliteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}


