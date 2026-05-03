using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public class FactoryRepository : IFactoryRepository
    {
        private readonly string _connectionString;

        public FactoryRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<FactoryEntity?> GetByIdAsync(int factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM Factories WHERE factory_id = @FactoryId", conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapFactory(reader) : null;
        }

        public async Task<List<FactoryEntity>> GetAllAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM Factories ORDER BY factory_name", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryEntity>();
            while (await reader.ReadAsync())
                list.Add(MapFactory(reader));

            return list;
        }

        public async Task<List<FactoryEntity>> GetByBusinessIdAsync(int businessId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM Factories WHERE business_id = @BusinessId ORDER BY factory_name", conn);
            cmd.Parameters.AddWithValue("@BusinessId", businessId);

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryEntity>();
            while (await reader.ReadAsync())
                list.Add(MapFactory(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryEntity factory)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO Factories (business_id, factory_name, background_image_path, created_at)
                VALUES (@BusinessId, @FactoryName, @BackgroundImagePath, @CreatedAt);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BusinessId", factory.BusinessId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FactoryName", factory.FactoryName);
            cmd.Parameters.AddWithValue("@BackgroundImagePath", factory.BackgroundImagePath ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", (factory.CreatedAt == default ? DateTime.Now : factory.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task UpdateAsync(FactoryEntity factory)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE Factories
                SET business_id = @BusinessId,
                    factory_name = @FactoryName,
                    background_image_path = @BackgroundImagePath
                WHERE factory_id = @FactoryId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryId", factory.FactoryId);
            cmd.Parameters.AddWithValue("@BusinessId", factory.BusinessId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FactoryName", factory.FactoryName);
            cmd.Parameters.AddWithValue("@BackgroundImagePath", factory.BackgroundImagePath ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM Factories WHERE factory_id = @FactoryId", conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);
            await cmd.ExecuteNonQueryAsync();
        }

        private static FactoryEntity MapFactory(SqliteDataReader reader)
        {
            int idxBusinessId = reader.GetOrdinal("business_id");
            int idxBackground = reader.GetOrdinal("background_image_path");

            return new FactoryEntity
            {
                FactoryId = reader.GetInt64(reader.GetOrdinal("factory_id")),
                BusinessId = reader.IsDBNull(idxBusinessId) ? null : reader.GetInt64(idxBusinessId),
                FactoryName = reader.GetString(reader.GetOrdinal("factory_name")),
                BackgroundImagePath = reader.IsDBNull(idxBackground) ? null : reader.GetString(idxBackground),
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("created_at")))
            };
        }
    }
}
