using Microsoft.Data.Sqlite;

namespace CatoriServices.Objects.database.Production;

public sealed record ProductionPathConfiguration(
    long LayoutItemId,string PathName,string PathData,string PartName,string? PartImagePath,string? PickupRobotName);

public sealed record ProductionRobotConfiguration(
    long RobotLayoutItemId,string RobotName,string? PickupPathName,string? DropPathName,
    string? OutputPartName,string? OutputPartImagePath,bool ReleasePartAtDrop);

public sealed class ProductionLayoutRepository
{
    private readonly string _connectionString;
    public ProductionLayoutRepository(string? databasePath=null)
    {
        string path=string.IsNullOrWhiteSpace(databasePath)?GlobalServices.Database:databasePath;
        RuntimeProductionSchemaMigrator.Migrate(path);
        _connectionString=new SqliteConnectionStringBuilder { DataSource=path,ForeignKeys=true }.ToString();
    }

    public IReadOnlyList<ProductionPathConfiguration> GetPaths(long locationId)
    {
        using var connection=Open();using var command=connection.CreateCommand();
        command.CommandText=@"SELECT path.LocationLayoutItemId,path.ItemName,path.ItemDataJson,
part.part_name,part.image_path,robot.ItemName
FROM LocationLayoutItem path
LEFT JOIN LocationProductionAssetPart assignment ON assignment.location_layout_item_id=path.LocationLayoutItemId
LEFT JOIN Parts part ON part.part_id=assignment.part_id
LEFT JOIN RobotPathAssignment robotAssignment ON robotAssignment.PickupPathItemId=path.LocationLayoutItemId
LEFT JOIN LocationLayoutItem robot ON robot.LocationLayoutItemId=robotAssignment.RobotLayoutItemId
WHERE path.LocationId=@Location AND lower(path.ItemType) IN ('path','pline','polyline','line','conveyor')
AND path.ItemDataJson IS NOT NULL AND trim(path.ItemDataJson)<>''
ORDER BY path.LocationLayoutItemId;";
        command.Parameters.AddWithValue("@Location",locationId);
        using var reader=command.ExecuteReader();var result=new List<ProductionPathConfiguration>();
        while(reader.Read())result.Add(new ProductionPathConfiguration(reader.GetInt64(0),reader.GetString(1),reader.GetString(2),
            reader.IsDBNull(3)?$"{reader.GetString(1)}Part":reader.GetString(3),reader.IsDBNull(4)?null:reader.GetString(4),
            reader.IsDBNull(5)?null:reader.GetString(5)));
        return result;
    }

    public IReadOnlyList<ProductionRobotConfiguration> GetRobots(long locationId)
    {
        using var connection=Open();using var command=connection.CreateCommand();
        command.CommandText=@"SELECT robot.LocationLayoutItemId,robot.ItemName,pickup.ItemName,dropPath.ItemName,
output.part_name,output.image_path,assignment.ReleasePartAtDrop
FROM RobotPathAssignment assignment
JOIN LocationLayoutItem robot ON robot.LocationLayoutItemId=assignment.RobotLayoutItemId
LEFT JOIN LocationLayoutItem pickup ON pickup.LocationLayoutItemId=assignment.PickupPathItemId
LEFT JOIN LocationLayoutItem dropPath ON dropPath.LocationLayoutItemId=assignment.DropPathItemId
LEFT JOIN Parts output ON output.part_id=assignment.OutputPartId
WHERE robot.LocationId=@Location ORDER BY robot.LocationLayoutItemId;";
        command.Parameters.AddWithValue("@Location",locationId);
        using var reader=command.ExecuteReader();var result=new List<ProductionRobotConfiguration>();
        while(reader.Read())result.Add(new ProductionRobotConfiguration(reader.GetInt64(0),reader.GetString(1),
            reader.IsDBNull(2)?null:reader.GetString(2),reader.IsDBNull(3)?null:reader.GetString(3),
            reader.IsDBNull(4)?null:reader.GetString(4),reader.IsDBNull(5)?null:reader.GetString(5),reader.GetInt32(6)!=0));
        return result;
    }

    private SqliteConnection Open(){var connection=new SqliteConnection(_connectionString);connection.Open();return connection;}
}
