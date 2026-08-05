using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace CatoriServices.Objects.database.Robots
{
    public class RobotPoseRepository
    {
        private readonly string _connectionString = "Data Source=" + GlobalServices.Database + " ;";
        private SqliteConnection GetConnection() => new(_connectionString);

        public async Task<List<RobotPoseEntity>> GetByLocationLayoutItemIdAsync(long locationLayoutItemId)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqliteCommand(@"SELECT ItemDataJson
                                                FROM LocationLayoutItem
                                                WHERE LocationLayoutItemId = @LocationLayoutItemId", conn);
            cmd.Parameters.AddWithValue("@LocationLayoutItemId", locationLayoutItemId);
            var json = await cmd.ExecuteScalarAsync() as string;
            var poses = new List<RobotPoseEntity>();
            if (string.IsNullOrWhiteSpace(json))
                return poses;

            try
            {
                var storedPoses = JsonSerializer.Deserialize<List<StoredRobotPose>>(json) ?? new();
                for (var index = 0; index < storedPoses.Count; index++)
                {
                    poses.Add(new RobotPoseEntity
                    {
                        RobotPoseId = index + 1,
                        LocationLayoutItemId = locationLayoutItemId,
                        PoseName = storedPoses[index].PoseName,
                        Pose = storedPoses[index].Angles.GetRawText()
                    });
                }
            }
            catch (JsonException) { }
            return poses;
        }

        public async Task ReplaceAsync(long locationLayoutItemId, IEnumerable<RobotPoseEntity> poses)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            var storedPoses = new List<StoredRobotPose>();
            var poseIndex = 1L;
            foreach (var pose in poses)
            {
                pose.LocationLayoutItemId = locationLayoutItemId;
                pose.RobotPoseId = poseIndex++;
                using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(pose.Pose) ? "[]" : pose.Pose);
                storedPoses.Add(new StoredRobotPose
                {
                    PoseName = pose.PoseName,
                    Angles = document.RootElement.Clone()
                });
            }

            using var update = new SqliteCommand(@"UPDATE LocationLayoutItem
                                                   SET ItemDataJson = @ItemDataJson
                                                   WHERE LocationLayoutItemId = @LocationLayoutItemId", conn);
            update.Parameters.AddWithValue("@LocationLayoutItemId", locationLayoutItemId);
            update.Parameters.AddWithValue("@ItemDataJson", JsonSerializer.Serialize(storedPoses));
            await update.ExecuteNonQueryAsync();
        }

        private sealed class StoredRobotPose
        {
            public string PoseName { get; set; } = string.Empty;
            public JsonElement Angles { get; set; }
        }
    }
}
