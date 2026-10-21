using System;
using System.IO;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace SceneGeometryControlPropertiesMigrator
{
    internal static class Program
    {
        private const string SourceDb = @"C:\Databases\Catori.Studio.Dev.db";
        private const string BackupDb = SourceDb + ".backup";

        [STAThread]
        static void Main()
        {
            Console.WriteLine("SceneGeometry ControlProperties Migrator");
            if (!File.Exists(SourceDb))
            {
                Console.WriteLine($"Database not found: {SourceDb}");
                return;
            }

            // create backup
            Console.WriteLine($"Creating backup: {BackupDb}");
            File.Copy(SourceDb, BackupDb, overwrite: true);

            string connectionString = $"Data Source={SourceDb};Mode=ReadWrite;";
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            // verify column exists
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA table_info(\"SceneGeometry\");";
                var readerdb = cmd.ExecuteReader();
                bool hasMetadataJson = false;
                while (readerdb.Read())
                {
                    string name = readerdb.GetString(1);
                    if (string.Equals(name, "MetadataJson", StringComparison.OrdinalIgnoreCase))
                        hasMetadataJson = true;
                }
                if (!hasMetadataJson)
                {
                    Console.WriteLine("SceneGeometry.MetadataJson column not found.");
                    return;
                }
            }

            using var select = conn.CreateCommand();
            select.CommandText = "SELECT SceneGeometryId, MetadataJson FROM SceneGeometry;";
            using var readerAll = select.ExecuteReader();
            int count = 0;
            while (readerAll.Read())
            {
                count++;
            }
            readerAll.Close();
            Console.WriteLine($"Found {count} rows in SceneGeometry.");

            using var transaction = conn.BeginTransaction();
            using var select2 = conn.CreateCommand();
            select2.Transaction = transaction;
            select2.CommandText = "SELECT SceneGeometryId, MetadataJson FROM SceneGeometry;";

            using var update = conn.CreateCommand();
            update.Transaction = transaction;
            update.CommandText = "UPDATE SceneGeometry SET MetadataJson = @MetadataJson WHERE SceneGeometryId = @Id;";
            var paramMetadata = update.CreateParameter();
            paramMetadata.ParameterName = "@MetadataJson";
            update.Parameters.Add(paramMetadata);
            var paramId = update.CreateParameter();
            paramId.ParameterName = "@Id";
            update.Parameters.Add(paramId);

            using var reader = select2.ExecuteReader();
            int processed = 0;
            while (reader.Read())
            {
                var id = reader.GetInt64(0);
                var metadata = reader.IsDBNull(1) ? null : reader.GetString(1);

                // Convert legacy metadata into ControlProperties JSON
                var props = ConvertMetadataToControlProperties(metadata);
                var json = JsonSerializer.Serialize(props);

                paramMetadata.Value = json;
                paramId.Value = id;
                update.ExecuteNonQuery();
                processed++;
                if (processed % 100 == 0) Console.WriteLine($"Processed {processed} rows...");
            }

            transaction.Commit();
            Console.WriteLine($"Migration complete. Processed {processed} rows.");
        }

        private static object ConvertMetadataToControlProperties(string? metadataJson)
        {
            // If metadata already looks like ControlProperties (simple heuristic), return as-is
            if (!string.IsNullOrWhiteSpace(metadataJson) && metadataJson.TrimStart().StartsWith('{'))
            {
                try
                {
                    using var doc = JsonDocument.Parse(metadataJson);
                    if (doc.RootElement.TryGetProperty("Properties", out _))
                    {
                        // assume it's already a ControlProperties-like JSON
                        return JsonSerializer.Deserialize<object>(metadataJson)!;
                    }
                }
                catch { /* not json */ }
            }

            // Otherwise create new ControlProperties mapping legacy metadata fields into Properties dictionary
            var dict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.Ordinal);
            dict["SchemaVersion"] = "1";
            dict["ControlType"] = "SceneGeometry";
            if (!string.IsNullOrWhiteSpace(metadataJson))
            {
                // store legacy metadata under LegacyMetadata key
                dict["LegacyMetadata"] = metadataJson;
            }

            return new { Properties = dict };
        }
    }
}
