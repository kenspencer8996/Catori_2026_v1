using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Locations
{
    public class LocationLayoutMachineRepository
    {
        private readonly string _connectionString;

        public LocationLayoutMachineRepository()
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

        public async Task<LocationLayoutMachineEntity?> GetByIdAsync(int locationLayoutMachineId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM LocationLayoutMachine WHERE LocationLayoutMachineId = @LocationLayoutMachineId";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationLayoutMachineId", locationLayoutMachineId);
                
                            using var reader = await cmd.ExecuteReaderAsync();
                            return await reader.ReadAsync() ? MapLocationLayoutMachine(reader) : null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<LocationLayoutMachineEntity>> GetByLocationIdAsync(int locationId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM LocationLayoutMachine WHERE LocationId = @LocationId ORDER BY ZIndex, LocationLayoutMachineId";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationId", locationId);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<LocationLayoutMachineEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapLocationLayoutMachine(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<LocationLayoutMachineEntity>> GetByMachineIdAsync(int machineId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM LocationLayoutMachine WHERE MachineId = @MachineId ORDER BY LocationId, ZIndex";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@MachineId", machineId);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<LocationLayoutMachineEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapLocationLayoutMachine(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertAsync(LocationLayoutMachineEntity locationLayoutMachine)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                INSERT INTO LocationLayoutMachine
                                    (LocationId, MachineId, X, Y, Width, Height, Rotation, ZIndex, IsEnabled)
                                VALUES
                                    (@LocationId, @MachineId, @X, @Y, @Width, @Height, @Rotation, @ZIndex, @IsEnabled);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddParameters(cmd, locationLayoutMachine);
                
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(LocationLayoutMachineEntity locationLayoutMachine)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                UPDATE LocationLayoutMachine
                                SET LocationId = @LocationId,
                                    MachineId = @MachineId,
                                    X = @X,
                                    Y = @Y,
                                    Width = @Width,
                                    Height = @Height,
                                    Rotation = @Rotation,
                                    ZIndex = @ZIndex,
                                    IsEnabled = @IsEnabled
                                WHERE LocationLayoutMachineId = @LocationLayoutMachineId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationLayoutMachineId", locationLayoutMachine.LocationLayoutMachineId);
                            AddParameters(cmd, locationLayoutMachine);
                
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int locationLayoutMachineId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "DELETE FROM LocationLayoutMachine WHERE LocationLayoutMachineId = @LocationLayoutMachineId";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationLayoutMachineId", locationLayoutMachineId);
                
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<int> DeleteByLocationIdAsync(int locationId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "DELETE FROM LocationLayoutMachine WHERE LocationId = @LocationId";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@LocationId", locationId);
                
                            return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddParameters(SqliteCommand cmd, LocationLayoutMachineEntity locationLayoutMachine)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@LocationId", locationLayoutMachine.LocationId);
                            cmd.Parameters.AddWithValue("@MachineId", locationLayoutMachine.MachineId);
                            cmd.Parameters.AddWithValue("@X", locationLayoutMachine.X);
                            cmd.Parameters.AddWithValue("@Y", locationLayoutMachine.Y);
                            cmd.Parameters.AddWithValue("@Width", locationLayoutMachine.Width ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Height", locationLayoutMachine.Height ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Rotation", locationLayoutMachine.Rotation);
                            cmd.Parameters.AddWithValue("@ZIndex", locationLayoutMachine.ZIndex);
                            cmd.Parameters.AddWithValue("@IsEnabled", locationLayoutMachine.IsEnabled ? 1 : 0);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static LocationLayoutMachineEntity MapLocationLayoutMachine(SqliteDataReader reader)
        {
            try
            {
                            return new LocationLayoutMachineEntity
                            {
                                LocationLayoutMachineId = reader.GetInt32(reader.GetOrdinal("LocationLayoutMachineId")),
                                LocationId = reader.GetInt32(reader.GetOrdinal("LocationId")),
                                MachineId = reader.GetInt32(reader.GetOrdinal("MachineId")),
                                X = reader.GetDouble(reader.GetOrdinal("X")),
                                Y = reader.GetDouble(reader.GetOrdinal("Y")),
                                Width = GetNullableDouble(reader, "Width"),
                                Height = GetNullableDouble(reader, "Height"),
                                Rotation = GetDoubleOrDefault(reader, "Rotation", 0),
                                ZIndex = GetIntOrDefault(reader, "ZIndex", 100),
                                IsEnabled = GetIntOrDefault(reader, "IsEnabled", 1) == 1
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static double? GetNullableDouble(SqliteDataReader reader, string columnName)
        {
            try
            {
                            var ordinal = reader.GetOrdinal(columnName);
                            return reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static double GetDoubleOrDefault(SqliteDataReader reader, string columnName, double defaultValue)
        {
            try
            {
                            var ordinal = reader.GetOrdinal(columnName);
                            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDouble(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static int GetIntOrDefault(SqliteDataReader reader, string columnName, int defaultValue)
        {
            try
            {
                            var ordinal = reader.GetOrdinal(columnName);
                            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}


