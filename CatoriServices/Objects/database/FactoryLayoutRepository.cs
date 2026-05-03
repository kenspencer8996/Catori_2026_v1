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
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM FactoryLayouts WHERE factory_layout_id = @FactoryLayoutId", conn);
            cmd.Parameters.AddWithValue("@FactoryLayoutId", factoryLayoutId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapLayout(reader) : null;
        }

        public async Task<FactoryLayoutEntity?> GetActiveByFactoryIdAsync(long factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = "SELECT * FROM FactoryLayouts WHERE factory_id = @FactoryId AND is_active = 1 ORDER BY factory_layout_id DESC LIMIT 1";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapLayout(reader) : null;
        }

        public async Task<List<FactoryLayoutEntity>> GetByFactoryIdAsync(long factoryId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("SELECT * FROM FactoryLayouts WHERE factory_id = @FactoryId ORDER BY layout_name", conn);
            cmd.Parameters.AddWithValue("@FactoryId", factoryId);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<FactoryLayoutEntity>();
            while (await reader.ReadAsync())
                list.Add(MapLayout(reader));

            return list;
        }

        public async Task<int> InsertAsync(FactoryLayoutEntity layout)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO FactoryLayouts (factory_id, layout_name, canvas_width, canvas_height, is_active, notes, created_at)
                VALUES (@FactoryId, @LayoutName, @CanvasWidth, @CanvasHeight, @IsActive, @Notes, @CreatedAt);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            AddLayoutParameters(cmd, layout);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(FactoryLayoutEntity layout)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
                UPDATE FactoryLayouts
                SET factory_id = @FactoryId,
                    layout_name = @LayoutName,
                    canvas_width = @CanvasWidth,
                    canvas_height = @CanvasHeight,
                    is_active = @IsActive,
                    notes = @Notes
                WHERE factory_layout_id = @FactoryLayoutId";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@FactoryLayoutId", layout.FactoryLayoutId);
            AddLayoutParameters(cmd, layout);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(long factoryLayoutId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            using var cmd = new SqliteCommand("DELETE FROM FactoryLayouts WHERE factory_layout_id = @FactoryLayoutId", conn);
            cmd.Parameters.AddWithValue("@FactoryLayoutId", factoryLayoutId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static void AddLayoutParameters(SqliteCommand cmd, FactoryLayoutEntity layout)
        {
            cmd.Parameters.AddWithValue("@FactoryId", layout.FactoryId);
            cmd.Parameters.AddWithValue("@LayoutName", layout.LayoutName);
            cmd.Parameters.AddWithValue("@CanvasWidth", layout.CanvasWidth);
            cmd.Parameters.AddWithValue("@CanvasHeight", layout.CanvasHeight);
            cmd.Parameters.AddWithValue("@IsActive", layout.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@Notes", layout.Notes ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", (layout.CreatedAt == default ? DateTime.Now : layout.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private static FactoryLayoutEntity MapLayout(SqliteDataReader reader)
        {
            int idxNotes = reader.GetOrdinal("notes");
            return new FactoryLayoutEntity
            {
                FactoryLayoutId = reader.GetInt64(reader.GetOrdinal("factory_layout_id")),
                FactoryId = reader.GetInt64(reader.GetOrdinal("factory_id")),
                LayoutName = reader.GetString(reader.GetOrdinal("layout_name")),
                CanvasWidth = reader.GetDouble(reader.GetOrdinal("canvas_width")),
                CanvasHeight = reader.GetDouble(reader.GetOrdinal("canvas_height")),
                IsActive = reader.GetInt32(reader.GetOrdinal("is_active")) == 1,
                Notes = reader.IsDBNull(idxNotes) ? null : reader.GetString(idxNotes),
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("created_at")))
            };
        }
    }
}
