using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Robots
{
    public class RobotSequenceRepository : IRobotSequenceRepository
    {
        private readonly string _connectionString;

        public RobotSequenceRepository()
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

        public async Task<List<RobotSequenceEntity>> GetAllAsync()
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = SelectSequenceSql + @"
                                ORDER BY rs.LocationId, rs.SequenceName, rp.PoseIndex, rps.SegmentIndex";
                
                            return await ReadSequencesAsync(conn, sql, null);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<RobotSequenceEntity?> GetByIdAsync(long robotSequenceId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = SelectSequenceSql + @"
                                WHERE rs.RobotSequenceId = @RobotSequenceId
                                ORDER BY rp.PoseIndex, rps.SegmentIndex";
                
                            var sequences = await ReadSequencesAsync(conn, sql, cmd =>
                                cmd.Parameters.AddWithValue("@RobotSequenceId", robotSequenceId));
                            return sequences.FirstOrDefault();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<RobotSequenceEntity?> GetByNameAsync(string sequenceName)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = SelectSequenceSql + @"
                                WHERE rs.SequenceName = @SequenceName
                                ORDER BY rp.PoseIndex, rps.SegmentIndex";
                
                            var sequences = await ReadSequencesAsync(conn, sql, cmd =>
                                cmd.Parameters.AddWithValue("@SequenceName", sequenceName));
                            return sequences.FirstOrDefault();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<RobotSequenceEntity>> GetByLocationIdAsync(long locationId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = SelectSequenceSql + @"
                                WHERE rs.LocationId = @LocationId
                                ORDER BY rs.SequenceName, rp.PoseIndex, rps.SegmentIndex";
                
                            return await ReadSequencesAsync(conn, sql, cmd =>
                                cmd.Parameters.AddWithValue("@LocationId", locationId));
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<long> SaveAsync(RobotSequenceEntity sequence)
        {
            try
            {
                            if (sequence.RobotSequenceId <= 0)
                            {
                                var existing = await GetByNameAsync(sequence.SequenceName);
                                if (existing != null && existing.LocationId == sequence.LocationId)
                                    sequence.RobotSequenceId = existing.RobotSequenceId;
                            }
                
                            if (sequence.RobotSequenceId <= 0)
                                sequence.RobotSequenceId = await InsertAsync(sequence);
                            else
                                await UpdateAsync(sequence);
                
                            await ReplacePosesAsync(sequence.RobotSequenceId, sequence.Poses);
                            return sequence.RobotSequenceId;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(RobotSequenceEntity sequence)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                UPDATE RobotSequence
                                SET LocationId = @LocationId,
                                    MachineInstanceId = @MachineInstanceId,
                                    SequenceName = @SequenceName,
                                    RobotX = @RobotX,
                                    RobotY = @RobotY,
                                    RobotWidth = @RobotWidth,
                                    RobotHeight = @RobotHeight,
                                    UpdatedAt = @UpdatedAt
                                WHERE RobotSequenceId = @RobotSequenceId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddParameters(cmd, sequence);
                            cmd.Parameters.AddWithValue("@RobotSequenceId", sequence.RobotSequenceId);
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task<long> InsertAsync(RobotSequenceEntity sequence)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                INSERT INTO RobotSequence
                                    (LocationId, MachineInstanceId, SequenceName, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt)
                                VALUES
                                    (@LocationId, @MachineInstanceId, @SequenceName, @RobotX, @RobotY, @RobotWidth, @RobotHeight, @CreatedAt, @UpdatedAt);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddParameters(cmd, sequence);
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task ReplacePosesAsync(long robotSequenceId, IList<RobotPoseEntity> poses)
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
                                NormalizePoseSegments(poses[i]);
                                poses[i].RobotPoseId = await InsertPoseAsync(conn, (SqliteTransaction)tx, poses[i]);
                                await InsertPoseSegmentsAsync(conn, (SqliteTransaction)tx, poses[i]);
                            }
                
                            await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task<long> InsertPoseAsync(SqliteConnection conn, SqliteTransaction transaction, RobotPoseEntity pose)
        {
            try
            {
                            const string sql = @"
                                INSERT INTO RobotPose
                                    (RobotSequenceId, LocationId, PoseIndex, PoseName, Joint1, Joint2, Joint3, JointEnd, DurationMilliseconds)
                                VALUES
                                    (@RobotSequenceId, @LocationId, @PoseIndex, @PoseName, @Joint1, @Joint2, @Joint3, @JointEnd, @DurationMilliseconds);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn, transaction);
                            AddPoseParameters(cmd, pose);
                            var result = await cmd.ExecuteScalarAsync();
                            return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task InsertPoseSegmentsAsync(SqliteConnection conn, SqliteTransaction transaction, RobotPoseEntity pose)
        {
            try
            {
                            const string sql = @"
                                INSERT INTO RobotPoseSegment
                                    (RobotPoseId, SegmentIndex, Angle, CreatedAt, UpdatedAt)
                                VALUES
                                    (@RobotPoseId, @SegmentIndex, @Angle, @CreatedAt, @UpdatedAt);
                                SELECT last_insert_rowid();";
                
                            for (int i = 0; i < pose.Segments.Count; i++)
                            {
                                var segment = pose.Segments[i];
                                segment.RobotPoseId = pose.RobotPoseId;
                                segment.SegmentIndex = i;
                
                                using var cmd = new SqliteCommand(sql, conn, transaction);
                                cmd.Parameters.AddWithValue("@RobotPoseId", pose.RobotPoseId);
                                cmd.Parameters.AddWithValue("@SegmentIndex", segment.SegmentIndex);
                                cmd.Parameters.AddWithValue("@Angle", segment.Angle);
                                cmd.Parameters.AddWithValue("@CreatedAt", segment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                var result = await cmd.ExecuteScalarAsync();
                                segment.RobotPoseSegmentId = Convert.ToInt64(result);
                            }
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private const string SelectSequenceSql = @"
            SELECT
                rs.RobotSequenceId AS SequenceRobotSequenceId,
                rs.LocationId AS SequenceLocationId,
                rs.MachineInstanceId AS SequenceMachineInstanceId,
                rs.SequenceName AS SequenceName,
                rs.RobotX AS SequenceRobotX,
                rs.RobotY AS SequenceRobotY,
                rs.RobotWidth AS SequenceRobotWidth,
                rs.RobotHeight AS SequenceRobotHeight,
                rs.CreatedAt AS SequenceCreatedAt,
                rs.UpdatedAt AS SequenceUpdatedAt,
                rp.RobotPoseId AS PoseRobotPoseId,
                rp.RobotSequenceId AS PoseRobotSequenceId,
                rp.LocationId AS PoseLocationId,
                rp.PoseIndex AS PoseIndex,
                rp.PoseName AS PoseName,
                rp.Joint1 AS PoseJoint1,
                rp.Joint2 AS PoseJoint2,
                rp.Joint3 AS PoseJoint3,
                rp.JointEnd AS PoseJointEnd,
                rp.DurationMilliseconds AS PoseDurationMilliseconds,
                rps.RobotPoseSegmentId AS PoseSegmentRobotPoseSegmentId,
                rps.RobotPoseId AS PoseSegmentRobotPoseId,
                rps.SegmentIndex AS PoseSegmentIndex,
                rps.Angle AS PoseSegmentAngle
            FROM RobotSequence rs
            LEFT JOIN RobotPose rp ON rp.RobotSequenceId = rs.RobotSequenceId
            LEFT JOIN RobotPoseSegment rps ON rps.RobotPoseId = rp.RobotPoseId";

        private static async Task<List<RobotSequenceEntity>> ReadSequencesAsync(
            SqliteConnection conn,
            string sql,
            Action<SqliteCommand>? configure)
        {
            try
            {
                            using var cmd = new SqliteCommand(sql, conn);
                            configure?.Invoke(cmd);
                
                            using var reader = await cmd.ExecuteReaderAsync();
                            var byId = new Dictionary<long, RobotSequenceEntity>();
                            var posesById = new Dictionary<long, RobotPoseEntity>();
                
                            while (await reader.ReadAsync())
                            {
                                var sequenceId = reader.GetInt64(reader.GetOrdinal("SequenceRobotSequenceId"));
                                if (!byId.TryGetValue(sequenceId, out var sequence))
                                {
                                    sequence = MapSequence(reader);
                                    byId.Add(sequenceId, sequence);
                                }
                
                                var poseIdOrdinal = reader.GetOrdinal("PoseRobotPoseId");
                                if (reader.IsDBNull(poseIdOrdinal))
                                    continue;
                
                                var poseId = reader.GetInt64(poseIdOrdinal);
                                if (!posesById.TryGetValue(poseId, out var pose))
                                {
                                    pose = MapPose(reader);
                                    posesById.Add(poseId, pose);
                                    sequence.Poses.Add(pose);
                                }
                
                                var poseSegmentIdOrdinal = reader.GetOrdinal("PoseSegmentRobotPoseSegmentId");
                                if (!reader.IsDBNull(poseSegmentIdOrdinal))
                                    pose.Segments.Add(MapPoseSegment(reader));
                            }
                
                            foreach (var pose in posesById.Values)
                                ApplySegmentAngles(pose);
                
                            return byId.Values.ToList();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static RobotSequenceEntity MapSequence(SqliteDataReader reader)
        {
            try
            {
                            return new RobotSequenceEntity
                            {
                                RobotSequenceId = reader.GetInt64(reader.GetOrdinal("SequenceRobotSequenceId")),
                                LocationId = reader.GetInt64(reader.GetOrdinal("SequenceLocationId")),
                                MachineInstanceId = reader.GetInt64(reader.GetOrdinal("SequenceMachineInstanceId")),
                                SequenceName = reader.GetString(reader.GetOrdinal("SequenceName")),
                                RobotX = reader.GetDouble(reader.GetOrdinal("SequenceRobotX")),
                                RobotY = reader.GetDouble(reader.GetOrdinal("SequenceRobotY")),
                                RobotWidth = reader.GetDouble(reader.GetOrdinal("SequenceRobotWidth")),
                                RobotHeight = reader.GetDouble(reader.GetOrdinal("SequenceRobotHeight")),
                                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("SequenceCreatedAt"))),
                                UpdatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("SequenceUpdatedAt")))
                            };
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
                                RobotPoseId = reader.GetInt64(reader.GetOrdinal("PoseRobotPoseId")),
                                RobotSequenceId = reader.GetInt64(reader.GetOrdinal("PoseRobotSequenceId")),
                                LocationId = reader.GetInt64(reader.GetOrdinal("PoseLocationId")),
                                PoseIndex = reader.GetInt32(reader.GetOrdinal("PoseIndex")),
                                PoseName = reader.GetString(reader.GetOrdinal("PoseName")),
                                Joint1 = reader.GetDouble(reader.GetOrdinal("PoseJoint1")),
                                Joint2 = reader.GetDouble(reader.GetOrdinal("PoseJoint2")),
                                Joint3 = reader.GetDouble(reader.GetOrdinal("PoseJoint3")),
                                JointEnd = reader.GetDouble(reader.GetOrdinal("PoseJointEnd")),
                                DurationMilliseconds = reader.GetInt32(reader.GetOrdinal("PoseDurationMilliseconds"))
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static RobotPoseSegmentEntity MapPoseSegment(SqliteDataReader reader)
        {
            try
            {
                            return new RobotPoseSegmentEntity
                            {
                                RobotPoseSegmentId = reader.GetInt64(reader.GetOrdinal("PoseSegmentRobotPoseSegmentId")),
                                RobotPoseId = reader.GetInt64(reader.GetOrdinal("PoseSegmentRobotPoseId")),
                                SegmentIndex = reader.GetInt32(reader.GetOrdinal("PoseSegmentIndex")),
                                Angle = reader.GetDouble(reader.GetOrdinal("PoseSegmentAngle"))
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddParameters(SqliteCommand cmd, RobotSequenceEntity sequence)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@LocationId", sequence.LocationId);
                            cmd.Parameters.AddWithValue("@MachineInstanceId", sequence.MachineInstanceId);
                            cmd.Parameters.AddWithValue("@SequenceName", sequence.SequenceName);
                            cmd.Parameters.AddWithValue("@RobotX", sequence.RobotX);
                            cmd.Parameters.AddWithValue("@RobotY", sequence.RobotY);
                            cmd.Parameters.AddWithValue("@RobotWidth", sequence.RobotWidth);
                            cmd.Parameters.AddWithValue("@RobotHeight", sequence.RobotHeight);
                            cmd.Parameters.AddWithValue("@CreatedAt", sequence.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddPoseParameters(SqliteCommand cmd, RobotPoseEntity pose)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@RobotSequenceId", pose.RobotSequenceId);
                            cmd.Parameters.AddWithValue("@LocationId", pose.LocationId);
                            cmd.Parameters.AddWithValue("@PoseIndex", pose.PoseIndex);
                            cmd.Parameters.AddWithValue("@PoseName", pose.PoseName);
                            cmd.Parameters.AddWithValue("@Joint1", pose.Joint1);
                            cmd.Parameters.AddWithValue("@Joint2", pose.Joint2);
                            cmd.Parameters.AddWithValue("@Joint3", pose.Joint3);
                            cmd.Parameters.AddWithValue("@JointEnd", pose.JointEnd);
                            cmd.Parameters.AddWithValue("@DurationMilliseconds", pose.DurationMilliseconds);
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
                            MachineCatalogRepository.EnsureSchema(conn);
                
                            using (var cmd = new SqliteCommand(@"
                                CREATE TABLE IF NOT EXISTS RobotSequence (
                                    RobotSequenceId INTEGER PRIMARY KEY AUTOINCREMENT,
                                    LocationId INTEGER NOT NULL,
                                    MachineInstanceId INTEGER NOT NULL DEFAULT 0,
                                    SequenceName TEXT NOT NULL,
                                    RobotX REAL NOT NULL DEFAULT 300,
                                    RobotY REAL NOT NULL DEFAULT 200,
                                    RobotWidth REAL NOT NULL DEFAULT 100,
                                    RobotHeight REAL NOT NULL DEFAULT 100,
                                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                                );
                                CREATE INDEX IF NOT EXISTS idx_robot_sequence_location_id ON RobotSequence(LocationId);
                                CREATE INDEX IF NOT EXISTS idx_robot_sequence_machine_instance ON RobotSequence(MachineInstanceId);
                                CREATE UNIQUE INDEX IF NOT EXISTS idx_robot_sequence_location_name ON RobotSequence(LocationId, SequenceName);", conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                
                            EnsureColumn(conn, "RobotSequence", "MachineInstanceId", "INTEGER NOT NULL DEFAULT 0");
                            EnsureRobotPoseSequenceColumn(conn);
                            EnsureRobotPoseEndColumn(conn);
                            EnsureRobotPoseSegmentTable(conn);
                            BackfillRobotSequence(conn);
                            BackfillRobotPoseSegments(conn);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void EnsureRobotPoseSequenceColumn(SqliteConnection conn)
        {
            try
            {
                            EnsureColumn(conn, "RobotPose", "RobotSequenceId", "INTEGER NOT NULL DEFAULT 0");
                
                            using var indexCmd = new SqliteCommand("CREATE INDEX IF NOT EXISTS idx_robot_pose_sequence ON RobotPose(RobotSequenceId, PoseIndex)", conn);
                            indexCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void EnsureRobotPoseEndColumn(SqliteConnection conn)
        {
            try
            {
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

        private static void EnsureRobotPoseSegmentTable(SqliteConnection conn)
        {
            try
            {
                            using var cmd = new SqliteCommand(@"
                                CREATE TABLE IF NOT EXISTS RobotPoseSegment (
                                    RobotPoseSegmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                                    RobotPoseId INTEGER NOT NULL,
                                    SegmentIndex INTEGER NOT NULL,
                                    Angle REAL NOT NULL DEFAULT 0,
                                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    FOREIGN KEY (RobotPoseId) REFERENCES RobotPose(RobotPoseId) ON DELETE CASCADE
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS idx_robot_pose_segment_pose_index ON RobotPoseSegment(RobotPoseId, SegmentIndex);", conn);
                            cmd.ExecuteNonQuery();
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
                            using var columnsCmd = new SqliteCommand($"PRAGMA table_info('{tableName}')", conn);
                            using var reader = columnsCmd.ExecuteReader();
                            var hasColumn = false;
                            while (reader.Read())
                            {
                                if (string.Equals(reader.GetString(reader.GetOrdinal("name")), columnName, StringComparison.OrdinalIgnoreCase))
                                    hasColumn = true;
                            }
                            reader.Close();
                
                            if (!hasColumn)
                            {
                                using var alterCmd = new SqliteCommand($"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition}", conn);
                                alterCmd.ExecuteNonQuery();
                            }
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void BackfillRobotSequence(SqliteConnection conn)
        {
            try
            {
                            if (!TableExists(conn, "MachineLayoutDesigner") || !TableExists(conn, "RobotPose"))
                                return;
                
                            using var cmd = new SqliteCommand(@"
                                INSERT OR IGNORE INTO RobotSequence
                                    (LocationId, SequenceName, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt)
                                SELECT LocationId, SequenceName, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt
                                FROM MachineLayoutDesigner
                                WHERE SequenceName IS NOT NULL AND TRIM(SequenceName) <> '';
                
                                UPDATE RobotPose
                                SET RobotSequenceId = (
                                    SELECT rs.RobotSequenceId
                                    FROM RobotSequence rs
                                    WHERE rs.LocationId = RobotPose.LocationId
                                    ORDER BY rs.UpdatedAt DESC
                                    LIMIT 1
                                )
                                WHERE RobotSequenceId = 0;", conn);
                            cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void BackfillRobotPoseSegments(SqliteConnection conn)
        {
            try
            {
                            using var cmd = new SqliteCommand(@"
                                INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
                                SELECT RobotPoseId, 0, Joint1 FROM RobotPose;
                
                                INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
                                SELECT RobotPoseId, 1, Joint2 FROM RobotPose;
                
                                INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
                                SELECT RobotPoseId, 2, Joint3 FROM RobotPose;
                
                                INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
                                SELECT RobotPoseId, 3, JointEnd FROM RobotPose;", conn);
                            cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void NormalizePoseSegments(RobotPoseEntity pose)
        {
            try
            {
                            if (pose.Segments.Count == 0)
                            {
                                pose.Segments.Add(new RobotPoseSegmentEntity { SegmentIndex = 0, Angle = pose.Joint1 });
                                pose.Segments.Add(new RobotPoseSegmentEntity { SegmentIndex = 1, Angle = pose.Joint2 });
                                pose.Segments.Add(new RobotPoseSegmentEntity { SegmentIndex = 2, Angle = pose.Joint3 });
                                pose.Segments.Add(new RobotPoseSegmentEntity { SegmentIndex = 3, Angle = pose.JointEnd });
                                return;
                            }
                
                            ApplySegmentAngles(pose);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void ApplySegmentAngles(RobotPoseEntity pose)
        {
            try
            {
                            foreach (var segment in pose.Segments.OrderBy(s => s.SegmentIndex))
                            {
                                switch (segment.SegmentIndex)
                                {
                                    case 0:
                                        pose.Joint1 = segment.Angle;
                                        break;
                                    case 1:
                                        pose.Joint2 = segment.Angle;
                                        break;
                                    case 2:
                                        pose.Joint3 = segment.Angle;
                                        break;
                                    case 3:
                                        pose.JointEnd = segment.Angle;
                                        break;
                                }
                            }
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

        private static bool TableExists(SqliteConnection conn, string tableName)
        {
            try
            {
                            using var cmd = new SqliteCommand(@"
                                SELECT 1
                                FROM sqlite_master
                                WHERE type = 'table' AND name = @TableName
                                LIMIT 1", conn);
                            cmd.Parameters.AddWithValue("@TableName", tableName);
                            return cmd.ExecuteScalar() != null;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
    }
}
