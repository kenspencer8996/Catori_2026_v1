using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Robots
{
    public class RobotRepository
    {
        private readonly string _connectionString = "Data Source=" + GlobalServices.Database + " ;";

        private SqliteConnection GetConnection() => new(_connectionString);

        public async Task<RobotEntity?> GetByLocationIdAsync(long locationId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            const string sql = @"SELECT RobotId, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt, LocationId
                                 FROM Robot WHERE LocationId = @LocationId ORDER BY UpdatedAt DESC LIMIT 1";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@LocationId", locationId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            var robot = new RobotEntity
            {
                RobotId = reader.GetInt64(0),
                RobotX = reader.GetDouble(1),
                RobotY = reader.GetDouble(2),
                RobotWidth = reader.GetDouble(3),
                RobotHeight = reader.GetDouble(4),
                CreatedAt = DateTime.Parse(reader.GetString(5)),
                UpdatedAt = DateTime.Parse(reader.GetString(6)),
                LocationId = reader.GetInt64(7)
            };
            await reader.DisposeAsync();
            robot.Poses = await GetPosesAsync(conn, robot.RobotId);
            return robot;
        }

        public async Task<long> SaveAsync(RobotEntity robot)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = (SqliteTransaction)await conn.BeginTransactionAsync();

            if (robot.RobotId <= 0)
            {
                using var find = new SqliteCommand("SELECT RobotId FROM Robot WHERE LocationId = @LocationId ORDER BY UpdatedAt DESC LIMIT 1", conn, tx);
                find.Parameters.AddWithValue("@LocationId", robot.LocationId);
                var existing = await find.ExecuteScalarAsync();
                robot.RobotId = existing == null ? 0 : Convert.ToInt64(existing);
            }

            if (robot.RobotId <= 0)
            {
                const string insertSql = @"INSERT INTO Robot (RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt, LocationId)
                                           VALUES (@RobotX, @RobotY, @RobotWidth, @RobotHeight, @CreatedAt, @UpdatedAt, @LocationId)
                                           RETURNING RobotId";
                using var insert = new SqliteCommand(insertSql, conn, tx);
                AddParameters(insert, robot);
                robot.RobotId = Convert.ToInt64(await insert.ExecuteScalarAsync());
            }
            else
            {
                const string updateSql = @"UPDATE Robot SET RobotX=@RobotX, RobotY=@RobotY, RobotWidth=@RobotWidth,
                                           RobotHeight=@RobotHeight, UpdatedAt=@UpdatedAt, LocationId=@LocationId WHERE RobotId=@RobotId";
                using var update = new SqliteCommand(updateSql, conn, tx);
                AddParameters(update, robot);
                update.Parameters.AddWithValue("@RobotId", robot.RobotId);
                await update.ExecuteNonQueryAsync();
            }

            using (var delete = new SqliteCommand("DELETE FROM RobotPose WHERE RobotId = @RobotId", conn, tx))
            {
                delete.Parameters.AddWithValue("@RobotId", robot.RobotId);
                await delete.ExecuteNonQueryAsync();
            }

            foreach (var pose in robot.Poses)
            {
                pose.RobotId = robot.RobotId;
                const string poseSql = @"INSERT INTO RobotPose (RobotId, PoseName, Pose)
                                         VALUES (@RobotId, @PoseName, @Pose) RETURNING RobotPoseId";
                using var poseCommand = new SqliteCommand(poseSql, conn, tx);
                poseCommand.Parameters.AddWithValue("@RobotId", pose.RobotId);
                poseCommand.Parameters.AddWithValue("@PoseName", pose.PoseName);
                poseCommand.Parameters.AddWithValue("@Pose", pose.Pose);
                pose.RobotPoseId = Convert.ToInt64(await poseCommand.ExecuteScalarAsync());
            }

            await tx.CommitAsync();
            return robot.RobotId;
        }

        private static async Task<List<RobotPoseEntity>> GetPosesAsync(SqliteConnection conn, long robotId)
        {
            using var cmd = new SqliteCommand("SELECT RobotPoseId, RobotId, PoseName, Pose FROM RobotPose WHERE RobotId = @RobotId ORDER BY RobotPoseId", conn);
            cmd.Parameters.AddWithValue("@RobotId", robotId);
            using var reader = await cmd.ExecuteReaderAsync();
            var poses = new List<RobotPoseEntity>();
            while (await reader.ReadAsync())
                poses.Add(new RobotPoseEntity { RobotPoseId = reader.GetInt64(0), RobotId = reader.GetInt64(1), PoseName = reader.GetString(2), Pose = reader.GetString(3) });
            return poses;
        }

        private static void AddParameters(SqliteCommand cmd, RobotEntity robot)
        {
            cmd.Parameters.AddWithValue("@RobotX", robot.RobotX);
            cmd.Parameters.AddWithValue("@RobotY", robot.RobotY);
            cmd.Parameters.AddWithValue("@RobotWidth", robot.RobotWidth);
            cmd.Parameters.AddWithValue("@RobotHeight", robot.RobotHeight);
            cmd.Parameters.AddWithValue("@CreatedAt", robot.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@LocationId", robot.LocationId);
        }
    }
}
