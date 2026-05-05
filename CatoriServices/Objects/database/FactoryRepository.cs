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

            using var cmd = new SqliteCommand("SELECT * FROM Factory WHERE FactoryId = @FactoryId", conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapFactory(reader) : null;
        }

        public async Task<List<FactoryEntity>> GetAllAsync()
        {
            List<FactoryEntity> list = new List<FactoryEntity>();
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                using var cmd = new SqliteCommand("SELECT * FROM Factory ORDER BY FactoryName", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                list = new List<FactoryEntity>();
                while (await reader.ReadAsync())
                    list.Add(MapFactory(reader));

            }
            catch (Exception ex)
            {

                throw;
            }
            return list;
        }

        public async Task<List<FactoryEntity>> GetByBusinessIdAsync(int businessId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM Factory ORDER BY FactoryName", conn);

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryEntity>();
            while (await reader.ReadAsync())
                list.Add(MapFactory(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryEntity factory)
        {
            object result;
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO Factory (FactoryName, BackgroundImagePath, CreatedAt)
                VALUES (@FactoryName, @BackgroundImagePath, @CreatedAt);
                SELECT last_insert_rowid();";

            try
            {
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FactoryName", factory.FactoryName);
                cmd.Parameters.AddWithValue("@BackgroundImagePath", factory.BackgroundImagePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedAt", (factory.CreatedAt == default ? DateTime.Now : factory.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));

                result = await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            return Convert.ToInt32(result);
        }

        public async Task UpdateAsync(FactoryEntity factory)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE Factory
                SET FactoryName = @FactoryName,
                    BackgroundImagePath = @BackgroundImagePath
                WHERE FactoryId = @FactoryId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryId", factory.FactoryId);
            cmd.Parameters.AddWithValue("@FactoryName", factory.FactoryName);
            cmd.Parameters.AddWithValue("@BackgroundImagePath", factory.BackgroundImagePath ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM Factory WHERE FactoryId = @FactoryId", conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);
            await cmd.ExecuteNonQueryAsync();
        }

        private static FactoryEntity MapFactory(SqliteDataReader reader)
        {
            int idxBackground = reader.GetOrdinal("BackgroundImagePath");

            return new FactoryEntity
            {
                FactoryId = reader.GetInt64(reader.GetOrdinal("FactoryId")),
                FactoryName = reader.GetString(reader.GetOrdinal("FactoryName")),
                BackgroundImagePath = reader.IsDBNull(idxBackground) ? null : reader.GetString(idxBackground),
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
            };
        }
    }
}
