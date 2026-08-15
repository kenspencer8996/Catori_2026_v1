using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Production;

public static class RuntimeProductionSchemaMigrator
{
    public static void Migrate(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        using var connection=new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource=databasePath,
            ForeignKeys=true
        }.ToString());
        connection.Open();
        using var transaction=connection.BeginTransaction();
        try
        {
            Execute(connection,transaction,"CREATE TABLE IF NOT EXISTS SchemaVersion(Name TEXT PRIMARY KEY,Version INTEGER NOT NULL,AppliedAt TEXT NOT NULL DEFAULT(datetime('now')));");
            EnsurePartsAndProducts(connection,transaction);
            EnsureCanonicalBom(connection,transaction);
            EnsureProductionLayoutTables(connection,transaction);
            Execute(connection,transaction,"INSERT INTO SchemaVersion(Name,Version,AppliedAt) VALUES('ProductionRuntime',1,datetime('now')) ON CONFLICT(Name) DO UPDATE SET Version=excluded.Version,AppliedAt=excluded.AppliedAt;");
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void EnsurePartsAndProducts(SqliteConnection connection,SqliteTransaction transaction)
    {
        Execute(connection,transaction,@"CREATE TABLE IF NOT EXISTS Parts(
part_id INTEGER PRIMARY KEY AUTOINCREMENT,
part_name TEXT NOT NULL UNIQUE,
description TEXT NULL,
image_path TEXT NULL,
unit_of_measure TEXT NOT NULL DEFAULT 'each',
cost_per_unit REAL NOT NULL DEFAULT 0,
created_at TEXT NOT NULL DEFAULT(datetime('now')));" );
        Execute(connection,transaction,@"CREATE TABLE IF NOT EXISTS Products(
product_id INTEGER PRIMARY KEY AUTOINCREMENT,
product_name TEXT NOT NULL,
product_code TEXT NOT NULL UNIQUE,
product_type TEXT NOT NULL,
unit_of_measure TEXT NOT NULL DEFAULT 'each',
cost_per_unit REAL NOT NULL DEFAULT 0,
created_at TEXT NOT NULL DEFAULT(datetime('now')),
part_id INTEGER NULL);" );
        if(!ColumnExists(connection,transaction,"Products","part_id"))
            Execute(connection,transaction,"ALTER TABLE Products ADD COLUMN part_id INTEGER NULL;");
        Execute(connection,transaction,@"INSERT OR IGNORE INTO Parts(part_name,description,image_path,unit_of_measure,cost_per_unit,created_at)
SELECT product_name,NULL,NULL,COALESCE(unit_of_measure,'each'),COALESCE(cost_per_unit,0),COALESCE(created_at,datetime('now'))
FROM Products WHERE part_id IS NULL;");
        Execute(connection,transaction,@"UPDATE Products SET part_id=(SELECT part_id FROM Parts WHERE part_name=Products.product_name LIMIT 1)
WHERE part_id IS NULL;");
    }

    private static void EnsureCanonicalBom(SqliteConnection connection,SqliteTransaction transaction)
    {
        if(!TableExists(connection,transaction,"Bill_of_Materials"))
        {
            CreateCanonicalBom(connection,transaction);
            return;
        }
        if(ColumnExists(connection,transaction,"Bill_of_Materials","parent_part_id"))return;
        if(!ColumnExists(connection,transaction,"Bill_of_Materials","parent_product_id"))return;

        if(!TableExists(connection,transaction,"Bill_of_Materials_Legacy"))
            Execute(connection,transaction,"ALTER TABLE Bill_of_Materials RENAME TO Bill_of_Materials_Legacy;");
        else
            Execute(connection,transaction,"DROP TABLE Bill_of_Materials;");
        CreateCanonicalBom(connection,transaction);
        Execute(connection,transaction,@"INSERT INTO Bill_of_Materials
(bom_id,parent_part_id,child_part_id,quantity,scrap_factor,effective_date,expiry_date)
SELECT legacy.bom_id,parent.part_id,child.part_id,legacy.quantity,legacy.scrap_factor,legacy.effective_date,legacy.expiry_date
FROM Bill_of_Materials_Legacy legacy
JOIN Products parent ON parent.product_id=legacy.parent_product_id
JOIN Products child ON child.product_id=legacy.component_id
WHERE parent.part_id IS NOT NULL AND child.part_id IS NOT NULL;");
    }

    private static void CreateCanonicalBom(SqliteConnection connection,SqliteTransaction transaction)
        =>Execute(connection,transaction,@"CREATE TABLE IF NOT EXISTS Bill_of_Materials(
bom_id INTEGER PRIMARY KEY AUTOINCREMENT,
parent_part_id INTEGER NOT NULL,
child_part_id INTEGER NOT NULL,
quantity REAL NOT NULL,
scrap_factor REAL NOT NULL DEFAULT 0,
effective_date TEXT NOT NULL DEFAULT(date('now')),
expiry_date TEXT NULL,
FOREIGN KEY(parent_part_id) REFERENCES Parts(part_id) ON DELETE CASCADE,
FOREIGN KEY(child_part_id) REFERENCES Parts(part_id) ON DELETE CASCADE);" );

    private static void EnsureProductionLayoutTables(SqliteConnection connection,SqliteTransaction transaction)
    {
        Execute(connection,transaction,@"CREATE TABLE IF NOT EXISTS LocationProductionAsset(
location_layout_item_id INTEGER PRIMARY KEY,
asset_type TEXT NOT NULL,
part_capacity INTEGER NOT NULL DEFAULT 1 CHECK(part_capacity>=0),
FOREIGN KEY(location_layout_item_id) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE);
CREATE TABLE IF NOT EXISTS LocationProductionAssetPart(
location_production_asset_part_id INTEGER PRIMARY KEY AUTOINCREMENT,
location_layout_item_id INTEGER NOT NULL UNIQUE,
part_id INTEGER NOT NULL,
assigned_at TEXT NOT NULL DEFAULT(datetime('now')),
FOREIGN KEY(location_layout_item_id) REFERENCES LocationProductionAsset(location_layout_item_id) ON DELETE CASCADE,
FOREIGN KEY(part_id) REFERENCES Parts(part_id));
CREATE TABLE IF NOT EXISTS LocationLayoutRelationship(
RelationshipId INTEGER PRIMARY KEY AUTOINCREMENT,
LocationId INTEGER NOT NULL,
SourceLayoutItemId INTEGER NOT NULL,
TargetLayoutItemId INTEGER NOT NULL,
RelationshipType TEXT NOT NULL,
Sequence INTEGER NOT NULL DEFAULT 0,
MetadataJson TEXT NULL,
IsEnabled INTEGER NOT NULL DEFAULT 1,
FOREIGN KEY(SourceLayoutItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE,
FOREIGN KEY(TargetLayoutItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE,
UNIQUE(SourceLayoutItemId,TargetLayoutItemId));
CREATE TABLE IF NOT EXISTS RobotPathAssignment(
RobotLayoutItemId INTEGER PRIMARY KEY,
PickupPathItemId INTEGER NULL,
DropPathItemId INTEGER NULL,
OutputPartId INTEGER NULL,
ReleasePartAtDrop INTEGER NOT NULL DEFAULT 1,
FOREIGN KEY(RobotLayoutItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE,
FOREIGN KEY(PickupPathItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE SET NULL,
FOREIGN KEY(DropPathItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE SET NULL,
FOREIGN KEY(OutputPartId) REFERENCES Parts(part_id) ON DELETE SET NULL);" );
    }

    private static bool TableExists(SqliteConnection connection,SqliteTransaction transaction,string table)
        =>Scalar(connection,transaction,"SELECT 1 FROM sqlite_schema WHERE type='table' AND name=@Name;",("@Name",table))!=null;

    private static bool ColumnExists(SqliteConnection connection,SqliteTransaction transaction,string table,string column)
    {
        using var command=connection.CreateCommand();command.Transaction=transaction;
        command.CommandText=$"PRAGMA table_info(\"{table.Replace("\"","\"\"")}\");";
        using var reader=command.ExecuteReader();
        while(reader.Read())if(string.Equals(reader.GetString(1),column,StringComparison.OrdinalIgnoreCase))return true;
        return false;
    }

    private static object? Scalar(SqliteConnection connection,SqliteTransaction transaction,string sql,params (string Name,object Value)[] values)
    {
        using var command=connection.CreateCommand();command.Transaction=transaction;command.CommandText=sql;
        foreach(var value in values)command.Parameters.AddWithValue(value.Name,value.Value);
        return command.ExecuteScalar();
    }

    private static void Execute(SqliteConnection connection,SqliteTransaction transaction,string sql)
    {
        using var command=connection.CreateCommand();command.Transaction=transaction;command.CommandText=sql;command.ExecuteNonQuery();
    }
}
