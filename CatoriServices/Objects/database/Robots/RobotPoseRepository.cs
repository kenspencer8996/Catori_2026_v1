using CatoriServices.Objects.Entities;
using Microsoft.Data.Sqlite;
namespace CatoriServices.Objects.database.Robots
{
    public class RobotPoseRepository
    {
        private readonly string _connectionString;
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();

        public RobotPoseRepository()
        {
            try
            {
                            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
                            EnsureSchema();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<List<RobotPoseEntity>> GetByLocationIdAsync(long locationId)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                const string sql = "SELECT * FROM RobotPose WHERE LocationId = @LocationId ORDER BY PoseIndex";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                using var reader = await cmd.ExecuteReaderAsync();

                var list = new List<RobotPoseEntity>();
                while (await reader.ReadAsync())
                    list.Add(MapPose(reader));

                return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());

                throw;
            }
        }

        public async Task<List<RobotPoseEntity>> GetByRobotSequenceIdAsync(long robotSequenceId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = "SELECT * FROM RobotPose WHERE RobotSequenceId = @RobotSequenceId ORDER BY PoseIndex";
                            using var cmd = new SqliteCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@RobotSequenceId", robotSequenceId);
                            using var reader = await cmd.ExecuteReaderAsync();
                
                            var list = new List<RobotPoseEntity>();
                            while (await reader.ReadAsync())
                                list.Add(MapPose(reader));
                
                            return list;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task ReplaceForLocationAsync(long locationId, IList<RobotPoseEntity> poses)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            using var tx = await conn.BeginTransactionAsync();
                
                            using (var deleteCmd = new SqliteCommand("DELETE FROM RobotPose WHERE LocationId = @LocationId", conn))
                            {
                                deleteCmd.Transaction = (SqliteTransaction)tx;
                                deleteCmd.Parameters.AddWithValue("@LocationId", locationId);
                                await deleteCmd.ExecuteNonQueryAsync();
                            }
                
                            for (int i = 0; i < poses.Count; i++)
                            {
                                poses[i].LocationId = locationId;
                                poses[i].PoseIndex = i;
                                poses[i].RobotPoseId = await InsertAsync(conn, (SqliteTransaction)tx, poses[i]);
                            }
                
                            await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task ReplaceForRobotSequenceAsync(long robotSequenceId, IList<RobotPoseEntity> poses)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            using var tx = await conn.BeginTransactionAsync();
                
                            using (var deleteCmd = new SqliteCommand("DELETE FROM RobotPose WHERE RobotSequenceId = @RobotSequenceId", conn))
                            {
                                deleteCmd.Transaction = (SqliteTransaction)tx;
                                deleteCmd.Parameters.AddWithValue("@RobotSequenceId", robotSequenceId);
                                await deleteCmd.ExecuteNonQueryAsync();
                            }
                
                            for (int i = 0; i < poses.Count; i++)
                            {
                                poses[i].RobotSequenceId = robotSequenceId;
                                poses[i].PoseIndex = i;
                                poses[i].RobotPoseId = await InsertAsync(conn, (SqliteTransaction)tx, poses[i]);
                            }
                
                            await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<long> InsertAsync(RobotPoseEntity pose)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            return await InsertAsync(conn, null, pose);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task<long> InsertAsync(SqliteConnection conn, SqliteTransaction? transaction, RobotPoseEntity pose)
        {
            try
            {
                            const string sql = @"
                                INSERT INTO RobotPose
                                    (RobotSequenceId, LocationId, PoseIndex, PoseName, Joint1, Joint2, Joint3, JointEnd, DurationMilliseconds)
                                VALUES
                                    (@RobotSequenceId, @LocationId, @PoseIndex, @PoseName, @Joint1, @Joint2, @Joint3, @JointEnd, @DurationMilliseconds);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            if (transaction != null)
                                cmd.Transaction = transaction;
                
                            cmd.Parameters.AddWithValue("@RobotSequenceId", pose.RobotSequenceId);
                            cmd.Parameters.AddWithValue("@LocationId", pose.LocationId);
                            cmd.Parameters.AddWithValue("@PoseIndex", pose.PoseIndex);
                            cmd.Parameters.AddWithValue("@PoseName", pose.PoseName);
                            cmd.Parameters.AddWithValue("@Joint1", pose.Joint1);
                            cmd.Parameters.AddWithValue("@Joint2", pose.Joint2);
                            cmd.Parameters.AddWithValue("@Joint3", pose.Joint3);
                            cmd.Parameters.AddWithValue("@JointEnd", pose.JointEnd);
                            cmd.Parameters.AddWithValue("@DurationMilliseconds", pose.DurationMilliseconds);
                
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static RobotPoseEntity MapPose(SqliteDataReader reader)
        {
            try
            {
                            return new RobotPoseEntity
                            {
                                RobotPoseId = reader.GetInt64(reader.GetOrdinal("RobotPoseId")),
                                RobotSequenceId = GetInt64OrDefault(reader, "RobotSequenceId"),
                                LocationId = reader.GetInt64(reader.GetOrdinal("LocationId")),
                                PoseIndex = reader.GetInt32(reader.GetOrdinal("PoseIndex")),
                                PoseName = reader.GetString(reader.GetOrdinal("PoseName")),
                                Joint1 = reader.GetDouble(reader.GetOrdinal("Joint1")),
                                Joint2 = reader.GetDouble(reader.GetOrdinal("Joint2")),
                                Joint3 = reader.GetDouble(reader.GetOrdinal("Joint3")),
                                JointEnd = reader.GetDouble(reader.GetOrdinal("JointEnd")),
                                DurationMilliseconds = reader.GetInt32(reader.GetOrdinal("DurationMilliseconds"))
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private void EnsureSchema()
        {
            try
            {
                            using var conn = GetConnection();
                            conn.Open();
                            EnsureColumn(conn, "RobotPose", "JointEnd", "REAL NOT NULL DEFAULT 0");
                
                            if (ColumnExists(conn, "RobotPose", "JointHand"))
                            {
                                using var cmd = new SqliteCommand(@"
                                    UPDATE RobotPose
                                    SET JointEnd = JointHand
                                    WHERE JointEnd = 0 AND JointHand <> 0", conn);
                                cmd.ExecuteNonQuery();
                            }
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void EnsureColumn(SqliteConnection conn, string tableName, string columnName, string definition)
        {
            try
            {
                            if (ColumnExists(conn, tableName, columnName))
                                return;
                
                            using var alterCmd = new SqliteCommand($"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition}", conn);
                            alterCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static bool ColumnExists(SqliteConnection conn, string tableName, string columnName)
        {
            try
            {
                            using var columnsCmd = new SqliteCommand($"PRAGMA table_info('{tableName}')", conn);
                            using var reader = columnsCmd.ExecuteReader();
                            while (reader.Read())
                            {
                                if (string.Equals(reader.GetString(reader.GetOrdinal("name")), columnName, StringComparison.OrdinalIgnoreCase))
                                    return true;
                            }
                
                            return false;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static long GetInt64OrDefault(SqliteDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt64(ordinal);
            }
            catch (IndexOutOfRangeException ex)
            {
                cLogger.Log(ex.ToString());
                return 0;
            }
        }
    }
}


