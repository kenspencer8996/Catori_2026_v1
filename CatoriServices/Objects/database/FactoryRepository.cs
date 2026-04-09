using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{

    public class FactoryRepository : IFactoryRepository
    {
        private readonly string _connectionString;

        public FactoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<FactoryEntity?> GetByIdAsync(int factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Factory WHERE FactoryId = @FactoryId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read()) return null;

            return Map(reader);
        }

        public async Task<List<FactoryEntity>> GetAllAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Factory";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryEntity>();
            while (await reader.ReadAsync())
                list.Add(Map(reader));

            return list;
        }

        public async Task<List<FactoryEntity>> GetByBusinessIdAsync(int businessId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM Factory WHERE BusinessId = @BusinessId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BusinessId", businessId);

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryEntity>();
            while (await reader.ReadAsync())
                list.Add(Map(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryEntity factory)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
            INSERT INTO Factory (BusinessId, InteriorName, FactoryName, Description)
            VALUES (@BusinessId, @InteriorName, @FactoryName, @Description);
            SELECT last_insert_rowid();
        ";

            using var cmd = new SqliteCommand(sql, conn);

            cmd.Parameters.AddWithValue("@BusinessId", factory.BusinessId);
            cmd.Parameters.AddWithValue("@InteriorName", factory.InteriorName);
            cmd.Parameters.AddWithValue("@FactoryName", (object?)factory.FactoryName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", (object?)factory.Description ?? DBNull.Value);

            object result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task UpdateAsync(FactoryEntity factory)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
            UPDATE Factory
            SET BusinessId = @BusinessId,
                InteriorName = @InteriorName,
                FactoryName = @FactoryName,
                Description = @Description
            WHERE FactoryId = @FactoryId
        ";

            using var cmd = new SqliteCommand(sql, conn);

            cmd.Parameters.AddWithValue("@FactoryId", factory.FactoryId);
            cmd.Parameters.AddWithValue("@BusinessId", factory.BusinessId);
            cmd.Parameters.AddWithValue("@InteriorName", factory.InteriorName);
            cmd.Parameters.AddWithValue("@FactoryName", (object?)factory.FactoryName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", (object?)factory.Description ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "DELETE FROM Factory WHERE FactoryId = @FactoryId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);

            await cmd.ExecuteNonQueryAsync();
        }

        private FactoryEntity Map(SqliteDataReader reader)
        {
            return new FactoryEntity
            {
                FactoryId = Convert.ToInt32(reader["FactoryId"]),
                BusinessId = Convert.ToInt32(reader["BusinessId"]),
                InteriorName = reader["InteriorName"].ToString()!,
                FactoryName = reader["FactoryName"] as string,
                Description = reader["Description"] as string
            };
        }
    }
}
