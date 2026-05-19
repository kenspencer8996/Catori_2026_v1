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
            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
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

                throw;
            }
        }

        public async Task ReplaceForLocationAsync(long locationId, IList<RobotPoseEntity> poses)
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

        public async Task<long> InsertAsync(RobotPoseEntity pose)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            return await InsertAsync(conn, null, pose);
        }

        private static async Task<long> InsertAsync(SqliteConnection conn, SqliteTransaction? transaction, RobotPoseEntity pose)
        {
            const string sql = @"
                INSERT INTO RobotPose
                    (LocationId, PoseIndex, PoseName, Joint1, Joint2, Joint3, JointHand, DurationMilliseconds)
                VALUES
                    (@LocationId, @PoseIndex, @PoseName, @Joint1, @Joint2, @Joint3, @JointHand, @DurationMilliseconds);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(sql, conn);
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.Parameters.AddWithValue("@LocationId", pose.LocationId);
            cmd.Parameters.AddWithValue("@PoseIndex", pose.PoseIndex);
            cmd.Parameters.AddWithValue("@PoseName", pose.PoseName);
            cmd.Parameters.AddWithValue("@Joint1", pose.Joint1);
            cmd.Parameters.AddWithValue("@Joint2", pose.Joint2);
            cmd.Parameters.AddWithValue("@Joint3", pose.Joint3);
            cmd.Parameters.AddWithValue("@JointHand", pose.JointHand);
            cmd.Parameters.AddWithValue("@DurationMilliseconds", pose.DurationMilliseconds);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        private static RobotPoseEntity MapPose(SqliteDataReader reader)
        {
            return new RobotPoseEntity
            {
                RobotPoseId = reader.GetInt64(reader.GetOrdinal("RobotPoseId")),
                LocationId = reader.GetInt64(reader.GetOrdinal("LocationId")),
                PoseIndex = reader.GetInt32(reader.GetOrdinal("PoseIndex")),
                PoseName = reader.GetString(reader.GetOrdinal("PoseName")),
                Joint1 = reader.GetDouble(reader.GetOrdinal("Joint1")),
                Joint2 = reader.GetDouble(reader.GetOrdinal("Joint2")),
                Joint3 = reader.GetDouble(reader.GetOrdinal("Joint3")),
                JointHand = reader.GetDouble(reader.GetOrdinal("JointHand")),
                DurationMilliseconds = reader.GetInt32(reader.GetOrdinal("DurationMilliseconds"))
            };
        }
    }
}


