SceneGeometry ControlProperties Migrator

This console app will:
- Backup the source database file
- Read SceneGeometry.SceneGeometryId and SceneGeometry.MetadataJson
- Convert MetadataJson into ControlProperties JSON and write back to MetadataJson

Usage:
- Adjust SourceDb path in Program.cs if needed
- Build and run the project

Notes:
- The conversion is conservative: if MetadataJson already looks like ControlProperties JSON (object with a "Properties" property), it is left as-is.
- Legacy metadata is preserved in the resulting ControlProperties under the "LegacyMetadata" key.
- Always check backup before proceeding with further migrations.
