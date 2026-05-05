using CatoriServices.Objects.Entities;
using CityAppServices;
using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database
{
    public class FactoryLayoutRepository
    {
        private readonly string _connectionString;

        public FactoryLayoutRepository()
        {
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<FactoryLayoutEntity?> GetByIdAsync(long factoryLayoutId)
            => await GetActiveByFactoryIdAsync(factoryLayoutId);

        public async Task<FactoryLayoutEntity?> GetActiveByFactoryIdAsync(long factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM Factory WHERE FactoryId = @FactoryId", conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapFactoryAsLayout(reader) : null;
        }

        public async Task<List<FactoryLayoutEntity>> GetByFactoryIdAsync(long factoryId)
        {
            var layout = await GetActiveByFactoryIdAsync(factoryId);
            return layout is null ? new List<FactoryLayoutEntity>() : new List<FactoryLayoutEntity> { layout };
        }

        public Task<int> InsertAsync(FactoryLayoutEntity layout)
        {
            // The live schema has one layout surface per Factory row.
            return Task.FromResult(Convert.ToInt32(layout.FactoryId));
        }

        public Task<bool> UpdateAsync(FactoryLayoutEntity layout)
        {
            // Canvas/name metadata is stored on Factory in the live schema.
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(long factoryLayoutId)
        {
            // Deleting a layout would mean deleting the Factory row in the live schema.
            return Task.FromResult(false);
        }

        private static FactoryLayoutEntity MapFactoryAsLayout(SqliteDataReader reader)
        {
            var factoryId = reader.GetInt64(reader.GetOrdinal("FactoryId"));
            return new FactoryLayoutEntity
            {
                FactoryLayoutId = factoryId,
                FactoryId = factoryId,
                LayoutName = reader.GetString(reader.GetOrdinal("FactoryName")) + " Layout",
                CanvasWidth = 1920,
                CanvasHeight = 1080,
                IsActive = true,
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
            };
        }
    }
}
