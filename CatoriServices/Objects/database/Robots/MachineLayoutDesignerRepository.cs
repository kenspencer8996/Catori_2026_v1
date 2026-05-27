using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Robots
{
    public class MachineLayoutDesignerRepository
    {
        private readonly string _connectionString;
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public MachineLayoutDesignerRepository()
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

        public async Task<MachineLayoutDesignerEntity?> GetByNameAsync(string sequenceName)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM MachineLayoutDesigner WHERE SequenceName = @SequenceName";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@SequenceName", sequenceName);
                
                            using var reader = await cmd.ExecuteReaderAsync();
                            return await reader.ReadAsync() ? MapMachineLayoutDesigner(reader) : null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<MachineLayoutDesignerEntity?> GetByLocationIdAsync(long locationId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM MachineLayoutDesigner WHERE LocationId = @LocationId ORDER BY UpdatedAt DESC LIMIT 1";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationId", locationId);
                
                            using var reader = await cmd.ExecuteReaderAsync();
                            return await reader.ReadAsync() ? MapMachineLayoutDesigner(reader) : null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<MachineLayoutDesignerEntity>> GetAllAsync()
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM MachineLayoutDesigner ORDER BY SequenceName";
                            using var cmd = new SqliteCommand(sql, conn);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<MachineLayoutDesignerEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapMachineLayoutDesigner(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<long> SaveAsync(MachineLayoutDesignerEntity designer)
        {
            try
            {
                            var existingId = designer.MachineLayoutDesignerId;
                            if (existingId <= 0)
                                existingId = await GetIdByNameAsync(designer.SequenceName);
                
                            if (existingId <= 0)
                                return await InsertAsync(designer);
                
                            designer.MachineLayoutDesignerId = existingId;
                            await UpdateAsync(designer);
                            return designer.MachineLayoutDesignerId;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<long> InsertAsync(MachineLayoutDesignerEntity designer)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                INSERT INTO MachineLayoutDesigner
                                    (LocationId, SequenceName, SelectionX, SelectionY, SelectionWidth, SelectionHeight,
                                     RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt)
                                VALUES
                                    (@LocationId, @SequenceName, @SelectionX, @SelectionY, @SelectionWidth, @SelectionHeight,
                                     @RobotX, @RobotY, @RobotWidth, @RobotHeight, @CreatedAt, @UpdatedAt);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddParameters(cmd, designer);
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(MachineLayoutDesignerEntity designer)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                UPDATE MachineLayoutDesigner
                                SET LocationId = @LocationId,
                                    SequenceName = @SequenceName,
                                    SelectionX = @SelectionX,
                                    SelectionY = @SelectionY,
                                    SelectionWidth = @SelectionWidth,
                                    SelectionHeight = @SelectionHeight,
                                    RobotX = @RobotX,
                                    RobotY = @RobotY,
                                    RobotWidth = @RobotWidth,
                                    RobotHeight = @RobotHeight,
                                    UpdatedAt = @UpdatedAt
                                WHERE MachineLayoutDesignerId = @MachineLayoutDesignerId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@MachineLayoutDesignerId", designer.MachineLayoutDesignerId);
                            AddParameters(cmd, designer);
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task<long> GetIdByNameAsync(string sequenceName)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT MachineLayoutDesignerId FROM MachineLayoutDesigner WHERE SequenceName = @SequenceName";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@SequenceName", sequenceName);
                            var result = await cmd.ExecuteScalarAsync();
                            return result == null || result == DBNull.Value ? 0 : Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddParameters(SqliteCommand cmd, MachineLayoutDesignerEntity designer)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@LocationId", designer.LocationId);
                            cmd.Parameters.AddWithValue("@SequenceName", designer.SequenceName);
                            cmd.Parameters.AddWithValue("@SelectionX", designer.SelectionX);
                            cmd.Parameters.AddWithValue("@SelectionY", designer.SelectionY);
                            cmd.Parameters.AddWithValue("@SelectionWidth", designer.SelectionWidth);
                            cmd.Parameters.AddWithValue("@SelectionHeight", designer.SelectionHeight);
                            cmd.Parameters.AddWithValue("@RobotX", designer.RobotX);
                            cmd.Parameters.AddWithValue("@RobotY", designer.RobotY);
                            cmd.Parameters.AddWithValue("@RobotWidth", designer.RobotWidth);
                            cmd.Parameters.AddWithValue("@RobotHeight", designer.RobotHeight);
                            cmd.Parameters.AddWithValue("@CreatedAt", designer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static MachineLayoutDesignerEntity MapMachineLayoutDesigner(SqliteDataReader reader)
        {
            try
            {
                            return new MachineLayoutDesignerEntity
                            {
                                MachineLayoutDesignerId = reader.GetInt64(reader.GetOrdinal("MachineLayoutDesignerId")),
                                LocationId = reader.GetInt64(reader.GetOrdinal("LocationId")),
                                SequenceName = reader.GetString(reader.GetOrdinal("SequenceName")),
                                SelectionX = reader.GetDouble(reader.GetOrdinal("SelectionX")),
                                SelectionY = reader.GetDouble(reader.GetOrdinal("SelectionY")),
                                SelectionWidth = reader.GetDouble(reader.GetOrdinal("SelectionWidth")),
                                SelectionHeight = reader.GetDouble(reader.GetOrdinal("SelectionHeight")),
                                RobotX = reader.GetDouble(reader.GetOrdinal("RobotX")),
                                RobotY = reader.GetDouble(reader.GetOrdinal("RobotY")),
                                RobotWidth = reader.GetDouble(reader.GetOrdinal("RobotWidth")),
                                RobotHeight = reader.GetDouble(reader.GetOrdinal("RobotHeight")),
                                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt"))),
                                UpdatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("UpdatedAt")))
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}


