using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Robots
{
    public class MachineCatalogRepository
    {
        private readonly string _connectionString;

        public MachineCatalogRepository()
        {
            try
            {
                            _connectionString = "Data Source=" + GlobalServices.Database + " ;";
                
                            using var conn = GetConnection();
                            conn.Open();
                            EnsureSchema(conn);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection(_connectionString);

        public async Task<List<MachineDefinitionEntity>> GetAllDefinitionsAsync()
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            return await ReadDefinitionsAsync(conn, SelectDefinitionSql + " ORDER BY md.MachineName", null);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<MachineDefinitionEntity?> GetDefinitionByIdAsync(long machineDefinitionId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            var definitions = await ReadDefinitionsAsync(conn, SelectDefinitionSql + @"
                                WHERE md.MachineDefinitionId = @MachineDefinitionId", cmd => cmd.Parameters.AddWithValue("@MachineDefinitionId", machineDefinitionId));
                            return definitions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public async Task<MachineDefinitionEntity?> GetDefinitionByNameAsync(string machineName)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            var definitions = await ReadDefinitionsAsync(conn, SelectDefinitionSql + @"
                                WHERE md.MachineName = @MachineName", cmd => cmd.Parameters.AddWithValue("@MachineName", machineName));
                            return definitions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }
        public async Task<long> SaveDefinitionAsync(MachineDefinitionEntity definition)
        {
            try
            {
                            if (definition.MachineDefinitionId <= 0)
                                definition.MachineDefinitionId = await InsertDefinitionAsync(definition);
                            else
                                await UpdateDefinitionAsync(definition);
                
                            return definition.MachineDefinitionId;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<List<MachineInstanceEntity>> GetAllInstancesAsync()
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            return await ReadInstancesAsync(conn, SelectInstanceSql + " ORDER BY mi.InstanceName, mis.SegmentIndex", null);
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<MachineInstanceEntity?> GetInstanceByIdAsync(long machineInstanceId)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            var instances = await ReadInstancesAsync(conn, SelectInstanceSql + @"
                                WHERE mi.MachineInstanceId = @MachineInstanceId
                                ORDER BY mis.SegmentIndex", cmd => cmd.Parameters.AddWithValue("@MachineInstanceId", machineInstanceId));
                            return instances.FirstOrDefault();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public async Task<long> SaveInstanceAsync(MachineInstanceEntity instance)
        {
            try
            {
                            if (instance.MachineInstanceId <= 0)
                                instance.MachineInstanceId = await InsertInstanceAsync(instance);
                            else
                                await UpdateInstanceAsync(instance);
                
                            await ReplaceInstanceSegmentsAsync(instance.MachineInstanceId, instance.Segments);
                            return instance.MachineInstanceId;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task<long> InsertDefinitionAsync(MachineDefinitionEntity definition)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                INSERT INTO MachineDefinition
                                    (MachineType, MachineName, Description, DefaultWidth, DefaultHeight, CreatedAt, UpdatedAt)
                                VALUES
                                    (@MachineType, @MachineName, @Description, @DefaultWidth, @DefaultHeight, @CreatedAt, @UpdatedAt);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddDefinitionParameters(cmd, definition);
                            return Convert.ToInt64(await cmd.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task<bool> UpdateDefinitionAsync(MachineDefinitionEntity definition)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                UPDATE MachineDefinition
                                SET MachineType = @MachineType,
                                    MachineName = @MachineName,
                                    Description = @Description,
                                    DefaultWidth = @DefaultWidth,
                                    DefaultHeight = @DefaultHeight,
                                    UpdatedAt = @UpdatedAt
                                WHERE MachineDefinitionId = @MachineDefinitionId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddDefinitionParameters(cmd, definition);
                            cmd.Parameters.AddWithValue("@MachineDefinitionId", definition.MachineDefinitionId);
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task<long> InsertInstanceAsync(MachineInstanceEntity instance)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                INSERT INTO MachineInstance
                                    (MachineDefinitionId, InstanceName, DisplayName, DefaultScale, DefaultWidth, DefaultHeight, CreatedAt, UpdatedAt)
                                VALUES
                                    (@MachineDefinitionId, @InstanceName, @DisplayName, @DefaultScale, @DefaultWidth, @DefaultHeight, @CreatedAt, @UpdatedAt);
                                SELECT last_insert_rowid();";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddInstanceParameters(cmd, instance);
                            return Convert.ToInt64(await cmd.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task<bool> UpdateInstanceAsync(MachineInstanceEntity instance)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                
                            const string sql = @"
                                UPDATE MachineInstance
                                SET MachineDefinitionId = @MachineDefinitionId,
                                    InstanceName = @InstanceName,
                                    DisplayName = @DisplayName,
                                    DefaultScale = @DefaultScale,
                                    DefaultWidth = @DefaultWidth,
                                    DefaultHeight = @DefaultHeight,
                                    UpdatedAt = @UpdatedAt
                                WHERE MachineInstanceId = @MachineInstanceId";
                
                            using var cmd = new SqliteCommand(sql, conn);
                            AddInstanceParameters(cmd, instance);
                            cmd.Parameters.AddWithValue("@MachineInstanceId", instance.MachineInstanceId);
                            return await cmd.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private async Task ReplaceInstanceSegmentsAsync(long machineInstanceId, IList<MachineInstanceSegmentEntity> segments)
        {
            try
            {
                            using var conn = GetConnection();
                            await conn.OpenAsync();
                            using var tx = await conn.BeginTransactionAsync();
                
                            using (var deleteCmd = new SqliteCommand("DELETE FROM MachineInstanceSegment WHERE MachineInstanceId = @MachineInstanceId", conn, (SqliteTransaction)tx))
                            {
                                deleteCmd.Parameters.AddWithValue("@MachineInstanceId", machineInstanceId);
                                await deleteCmd.ExecuteNonQueryAsync();
                            }
                
                            for (int i = 0; i < segments.Count; i++)
                            {
                                var segment = segments[i];
                                segment.MachineInstanceId = machineInstanceId;
                                segment.SegmentIndex = i;
                
                                using var cmd = new SqliteCommand(@"
                                    INSERT INTO MachineInstanceSegment
                                        (MachineInstanceId, SegmentIndex, SegmentName, Length, Width, InitialAngle, MinAngle, MaxAngle, Overlap, Color, ImageName, CreatedAt, UpdatedAt)
                                    VALUES
                                        (@MachineInstanceId, @SegmentIndex, @SegmentName, @Length, @Width, @InitialAngle, @MinAngle, @MaxAngle, @Overlap, @Color, @ImageName, @CreatedAt, @UpdatedAt);
                                    SELECT last_insert_rowid();", conn, (SqliteTransaction)tx);
                                AddInstanceSegmentParameters(cmd, segment);
                                segment.MachineInstanceSegmentId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                            }
                
                            await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        public static void EnsureSchema(SqliteConnection conn)
        {
            try
            {
                            using var cmd = new SqliteCommand(@"
                                CREATE TABLE IF NOT EXISTS MachineDefinition (
                                    MachineDefinitionId INTEGER PRIMARY KEY AUTOINCREMENT,
                                    MachineType TEXT NOT NULL,
                                    MachineName TEXT NOT NULL,
                                    Description TEXT NOT NULL DEFAULT '',
                                    DefaultWidth REAL NOT NULL DEFAULT 100,
                                    DefaultHeight REAL NOT NULL DEFAULT 100,
                                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_definition_type_name ON MachineDefinition(MachineType, MachineName);
                
                                CREATE TABLE IF NOT EXISTS MachineInstance (
                                    MachineInstanceId INTEGER PRIMARY KEY AUTOINCREMENT,
                                    MachineDefinitionId INTEGER NOT NULL,
                                    InstanceName TEXT NOT NULL,
                                    DisplayName TEXT NOT NULL DEFAULT '',
                                    DefaultScale REAL NOT NULL DEFAULT 1,
                                    DefaultWidth REAL NOT NULL DEFAULT 100,
                                    DefaultHeight REAL NOT NULL DEFAULT 100,
                                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    FOREIGN KEY (MachineDefinitionId) REFERENCES MachineDefinition(MachineDefinitionId)
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_instance_definition_name ON MachineInstance(MachineDefinitionId, InstanceName);
                
                                CREATE TABLE IF NOT EXISTS MachineInstanceSegment (
                                    MachineInstanceSegmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                                    MachineInstanceId INTEGER NOT NULL,
                                    SegmentIndex INTEGER NOT NULL,
                                    SegmentName TEXT NOT NULL DEFAULT '',
                                    Length REAL NOT NULL DEFAULT 124,
                                    Width REAL NOT NULL DEFAULT 40,
                                    InitialAngle REAL NOT NULL DEFAULT 0,
                                    MinAngle REAL NOT NULL DEFAULT -180,
                                    MaxAngle REAL NOT NULL DEFAULT 180,
                                    Overlap REAL NOT NULL DEFAULT 16,
                                    Color TEXT NOT NULL DEFAULT '',
                                    ImageName TEXT NOT NULL DEFAULT '',
                                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                                    FOREIGN KEY (MachineInstanceId) REFERENCES MachineInstance(MachineInstanceId) ON DELETE CASCADE
                                );
                                CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_instance_segment_index ON MachineInstanceSegment(MachineInstanceId, SegmentIndex);", conn);
                            cmd.ExecuteNonQuery();
                
                            EnsureColumn(conn, "MachineInstanceSegment", "SegmentName", "TEXT NOT NULL DEFAULT ''");
                            EnsureColumn(conn, "MachineInstanceSegment", "Length", "REAL NOT NULL DEFAULT 124");
                            EnsureColumn(conn, "MachineInstanceSegment", "Width", "REAL NOT NULL DEFAULT 40");
                            EnsureColumn(conn, "MachineInstanceSegment", "InitialAngle", "REAL NOT NULL DEFAULT 0");
                            EnsureColumn(conn, "MachineInstanceSegment", "MinAngle", "REAL NOT NULL DEFAULT -180");
                            EnsureColumn(conn, "MachineInstanceSegment", "MaxAngle", "REAL NOT NULL DEFAULT 180");
                            EnsureColumn(conn, "MachineInstanceSegment", "Overlap", "REAL NOT NULL DEFAULT 16");
                            EnsureColumn(conn, "MachineInstanceSegment", "ImageName", "TEXT NOT NULL DEFAULT ''");
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private const string SelectDefinitionSql = @"
            SELECT
                md.MachineDefinitionId AS DefinitionMachineDefinitionId,
                md.MachineType AS DefinitionMachineType,
                md.MachineName AS DefinitionMachineName,
                md.Description AS DefinitionDescription,
                md.DefaultWidth AS DefinitionDefaultWidth,
                md.DefaultHeight AS DefinitionDefaultHeight,
                md.CreatedAt AS DefinitionCreatedAt,
                md.UpdatedAt AS DefinitionUpdatedAt
            FROM MachineDefinition md";

        private const string SelectInstanceSql = @"
            SELECT
                mi.MachineInstanceId AS InstanceMachineInstanceId,
                mi.MachineDefinitionId AS InstanceMachineDefinitionId,
                mi.InstanceName AS InstanceName,
                mi.DisplayName AS InstanceDisplayName,
                mi.DefaultScale AS InstanceDefaultScale,
                mi.DefaultWidth AS InstanceDefaultWidth,
                mi.DefaultHeight AS InstanceDefaultHeight,
                mi.CreatedAt AS InstanceCreatedAt,
                mi.UpdatedAt AS InstanceUpdatedAt,
                mis.MachineInstanceSegmentId AS SegmentMachineInstanceSegmentId,
                mis.MachineInstanceId AS SegmentMachineInstanceId,
                mis.SegmentIndex AS SegmentIndex,
                mis.SegmentName AS SegmentName,
                mis.Length AS SegmentLength,
                mis.Width AS SegmentWidth,
                mis.InitialAngle AS SegmentInitialAngle,
                mis.MinAngle AS SegmentMinAngle,
                mis.MaxAngle AS SegmentMaxAngle,
                mis.Overlap AS SegmentOverlap,
                mis.Color AS SegmentColor,
                mis.ImageName AS SegmentImageName
            FROM MachineInstance mi
            LEFT JOIN MachineInstanceSegment mis ON mis.MachineInstanceId = mi.MachineInstanceId";

        private static async Task<List<MachineDefinitionEntity>> ReadDefinitionsAsync(SqliteConnection conn, string sql, Action<SqliteCommand>? configure)
        {
            try
            {
                            using var cmd = new SqliteCommand(sql, conn);
                            configure?.Invoke(cmd);
                            using var reader = await cmd.ExecuteReaderAsync();
                            var byId = new Dictionary<long, MachineDefinitionEntity>();
                
                            while (await reader.ReadAsync())
                            {
                                var definitionId = reader.GetInt64(reader.GetOrdinal("DefinitionMachineDefinitionId"));
                                if (!byId.TryGetValue(definitionId, out var definition))
                                {
                                    definition = new MachineDefinitionEntity
                                    {
                                        MachineDefinitionId = definitionId,
                                        MachineType = reader.GetString(reader.GetOrdinal("DefinitionMachineType")),
                                        MachineName = reader.GetString(reader.GetOrdinal("DefinitionMachineName")),
                                        Description = reader.GetString(reader.GetOrdinal("DefinitionDescription")),
                                        DefaultWidth = reader.GetDouble(reader.GetOrdinal("DefinitionDefaultWidth")),
                                        DefaultHeight = reader.GetDouble(reader.GetOrdinal("DefinitionDefaultHeight")),
                                        CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("DefinitionCreatedAt"))),
                                        UpdatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("DefinitionUpdatedAt")))
                                    };
                                    byId.Add(definitionId, definition);
                                }
                
                            }
                
                            return byId.Values.ToList();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static async Task<List<MachineInstanceEntity>> ReadInstancesAsync(SqliteConnection conn, string sql, Action<SqliteCommand>? configure)
        {
            try
            {
                            using var cmd = new SqliteCommand(sql, conn);
                            configure?.Invoke(cmd);
                            using var reader = await cmd.ExecuteReaderAsync();
                            var byId = new Dictionary<long, MachineInstanceEntity>();
                
                            while (await reader.ReadAsync())
                            {
                                var instanceId = reader.GetInt64(reader.GetOrdinal("InstanceMachineInstanceId"));
                                if (!byId.TryGetValue(instanceId, out var instance))
                                {
                                    instance = new MachineInstanceEntity
                                    {
                                        MachineInstanceId = instanceId,
                                        MachineDefinitionId = reader.GetInt64(reader.GetOrdinal("InstanceMachineDefinitionId")),
                                        InstanceName = reader.GetString(reader.GetOrdinal("InstanceName")),
                                        DisplayName = reader.GetString(reader.GetOrdinal("InstanceDisplayName")),
                                        DefaultScale = reader.GetDouble(reader.GetOrdinal("InstanceDefaultScale")),
                                        DefaultWidth = reader.GetDouble(reader.GetOrdinal("InstanceDefaultWidth")),
                                        DefaultHeight = reader.GetDouble(reader.GetOrdinal("InstanceDefaultHeight")),
                                        CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("InstanceCreatedAt"))),
                                        UpdatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("InstanceUpdatedAt")))
                                    };
                                    byId.Add(instanceId, instance);
                                }
                
                                var segmentIdOrdinal = reader.GetOrdinal("SegmentMachineInstanceSegmentId");
                                if (!reader.IsDBNull(segmentIdOrdinal))
                                    instance.Segments.Add(MapInstanceSegment(reader));
                            }
                
                            return byId.Values.ToList();
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static MachineInstanceSegmentEntity MapInstanceSegment(SqliteDataReader reader)
        {
            try
            {
                            return new MachineInstanceSegmentEntity
                            {
                                MachineInstanceSegmentId = reader.GetInt64(reader.GetOrdinal("SegmentMachineInstanceSegmentId")),
                                MachineInstanceId = reader.GetInt64(reader.GetOrdinal("SegmentMachineInstanceId")),
                                SegmentIndex = reader.GetInt32(reader.GetOrdinal("SegmentIndex")),
                                SegmentName = reader.GetString(reader.GetOrdinal("SegmentName")),
                                Length = reader.GetDouble(reader.GetOrdinal("SegmentLength")),
                                Width = reader.GetDouble(reader.GetOrdinal("SegmentWidth")),
                                InitialAngle = reader.GetDouble(reader.GetOrdinal("SegmentInitialAngle")),
                                MinAngle = reader.GetDouble(reader.GetOrdinal("SegmentMinAngle")),
                                MaxAngle = reader.GetDouble(reader.GetOrdinal("SegmentMaxAngle")),
                                Overlap = reader.GetDouble(reader.GetOrdinal("SegmentOverlap")),
                                Color = reader.GetString(reader.GetOrdinal("SegmentColor")),
                                ImageName = reader.GetString(reader.GetOrdinal("SegmentImageName"))
                            };
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddDefinitionParameters(SqliteCommand cmd, MachineDefinitionEntity definition)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@MachineType", definition.MachineType);
                            cmd.Parameters.AddWithValue("@MachineName", definition.MachineName);
                            cmd.Parameters.AddWithValue("@Description", definition.Description);
                            cmd.Parameters.AddWithValue("@DefaultWidth", definition.DefaultWidth);
                            cmd.Parameters.AddWithValue("@DefaultHeight", definition.DefaultHeight);
                            cmd.Parameters.AddWithValue("@CreatedAt", definition.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddInstanceParameters(SqliteCommand cmd, MachineInstanceEntity instance)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@MachineDefinitionId", instance.MachineDefinitionId);
                            cmd.Parameters.AddWithValue("@InstanceName", instance.InstanceName);
                            cmd.Parameters.AddWithValue("@DisplayName", instance.DisplayName);
                            cmd.Parameters.AddWithValue("@DefaultScale", instance.DefaultScale);
                            cmd.Parameters.AddWithValue("@DefaultWidth", instance.DefaultWidth);
                            cmd.Parameters.AddWithValue("@DefaultHeight", instance.DefaultHeight);
                            cmd.Parameters.AddWithValue("@CreatedAt", instance.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                cLogger.Log(ex.ToString());
                throw;
            }
        }

        private static void AddInstanceSegmentParameters(SqliteCommand cmd, MachineInstanceSegmentEntity segment)
        {
            try
            {
                            cmd.Parameters.AddWithValue("@MachineInstanceId", segment.MachineInstanceId);
                            cmd.Parameters.AddWithValue("@SegmentIndex", segment.SegmentIndex);
                            cmd.Parameters.AddWithValue("@SegmentName", segment.SegmentName);
                            cmd.Parameters.AddWithValue("@Length", segment.Length);
                            cmd.Parameters.AddWithValue("@Width", segment.Width);
                            cmd.Parameters.AddWithValue("@InitialAngle", segment.InitialAngle);
                            cmd.Parameters.AddWithValue("@MinAngle", segment.MinAngle);
                            cmd.Parameters.AddWithValue("@MaxAngle", segment.MaxAngle);
                            cmd.Parameters.AddWithValue("@Overlap", segment.Overlap);
                            cmd.Parameters.AddWithValue("@Color", segment.Color);
                            cmd.Parameters.AddWithValue("@ImageName", segment.ImageName);
                            cmd.Parameters.AddWithValue("@CreatedAt", segment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
    }
}
