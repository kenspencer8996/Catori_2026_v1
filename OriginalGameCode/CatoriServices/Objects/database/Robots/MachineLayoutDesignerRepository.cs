using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Robots
{
    public class MachineLayoutDesignerRepository
    {
        private readonly string _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        private SqliteConnection GetConnection()
        {
            return new(_connectionString);
        }

        public async Task<MachineLayoutDesignerEntity?> GetByLocationIdAsync(long locationId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand("SELECT MachineLayoutDesignerId, LocationId, SelectionX, SelectionY, SelectionWidth, SelectionHeight, CreatedAt, UpdatedAt FROM MachineLayoutDesigner WHERE LocationId=@LocationId ORDER BY UpdatedAt DESC LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@LocationId", locationId);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Map(reader) : null;
        }

        public async Task<long> SaveAsync(MachineLayoutDesignerEntity designer)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            if (designer.MachineLayoutDesignerId <= 0)
            {
                using var find = new SqliteCommand("SELECT MachineLayoutDesignerId FROM MachineLayoutDesigner WHERE LocationId=@LocationId ORDER BY UpdatedAt DESC LIMIT 1", conn);
                find.Parameters.AddWithValue("@LocationId", designer.LocationId);
                var existing = await find.ExecuteScalarAsync();
                designer.MachineLayoutDesignerId = existing == null ? 0 : Convert.ToInt64(existing);
            }
            if (designer.MachineLayoutDesignerId <= 0)
            {
                using var insert = new SqliteCommand(@"INSERT INTO MachineLayoutDesigner (LocationId, SelectionX, SelectionY, SelectionWidth, SelectionHeight, CreatedAt, UpdatedAt)
                                                      VALUES (@LocationId,@SelectionX,@SelectionY,@SelectionWidth,@SelectionHeight,@CreatedAt,@UpdatedAt)
                                                      RETURNING MachineLayoutDesignerId", conn);
                AddParameters(insert, designer);
                designer.MachineLayoutDesignerId = Convert.ToInt64(await insert.ExecuteScalarAsync());
            }
            else
            {
                using var update = new SqliteCommand(@"UPDATE MachineLayoutDesigner SET LocationId=@LocationId, SelectionX=@SelectionX, SelectionY=@SelectionY,
                                                      SelectionWidth=@SelectionWidth, SelectionHeight=@SelectionHeight, UpdatedAt=@UpdatedAt
                                                      WHERE MachineLayoutDesignerId=@MachineLayoutDesignerId", conn);
                AddParameters(update, designer);
                update.Parameters.AddWithValue("@MachineLayoutDesignerId", designer.MachineLayoutDesignerId);
                await update.ExecuteNonQueryAsync();
            }
            return designer.MachineLayoutDesignerId;
        }

        private static void AddParameters(SqliteCommand cmd, MachineLayoutDesignerEntity designer)
        {
            cmd.Parameters.AddWithValue("@LocationId", designer.LocationId);
            cmd.Parameters.AddWithValue("@SelectionX", designer.SelectionX);
            cmd.Parameters.AddWithValue("@SelectionY", designer.SelectionY);
            cmd.Parameters.AddWithValue("@SelectionWidth", designer.SelectionWidth);
            cmd.Parameters.AddWithValue("@SelectionHeight", designer.SelectionHeight);
            cmd.Parameters.AddWithValue("@CreatedAt", designer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private static MachineLayoutDesignerEntity Map(SqliteDataReader reader)
        {
            return new()
            {
                MachineLayoutDesignerId = reader.GetInt64(0),
                LocationId = reader.GetInt64(1),
                SelectionX = reader.GetDouble(2),
                SelectionY = reader.GetDouble(3),
                SelectionWidth = reader.GetDouble(4),
                SelectionHeight = reader.GetDouble(5),
                CreatedAt = DateTime.Parse(reader.GetString(6)),
                UpdatedAt = DateTime.Parse(reader.GetString(7))
            };
        }
    }
}
