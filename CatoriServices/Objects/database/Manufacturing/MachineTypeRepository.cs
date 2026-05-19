using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Manufacturing
{
    public class MachineTypeRepository
    {
        private readonly string _connectionString;

        public MachineTypeRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<MachineTypeEntity?> GetByIdAsync(int machineTypeId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "SELECT * FROM MachineType WHERE MachineTypeId = @MachineTypeId";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MachineTypeId", machineTypeId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapMachineType(reader) : null;
        }

        public async Task<MachineTypeEntity?> GetByNameAsync(string name)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "SELECT * FROM MachineType WHERE Name = @Name";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", name);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapMachineType(reader) : null;
        }

        public async Task<List<MachineTypeEntity>> GetAllAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "SELECT * FROM MachineType ORDER BY Name";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<MachineTypeEntity>();
            while (await reader.ReadAsync())
                list.Add(MapMachineType(reader));

            return list;
        }

        public async Task<int> InsertAsync(MachineTypeEntity machineType)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO MachineType (Name)
                VALUES (@Name);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", machineType.Name);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(MachineTypeEntity machineType)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = @"
                UPDATE MachineType
                SET Name = @Name
                WHERE MachineTypeId = @MachineTypeId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MachineTypeId", machineType.MachineTypeId);
            cmd.Parameters.AddWithValue("@Name", machineType.Name);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int machineTypeId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            const string sql = "DELETE FROM MachineType WHERE MachineTypeId = @MachineTypeId";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MachineTypeId", machineTypeId);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static MachineTypeEntity MapMachineType(SqliteDataReader reader)
        {
            return new MachineTypeEntity
            {
                MachineTypeId = reader.GetInt32(reader.GetOrdinal("MachineTypeId")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };
        }
    }
}


