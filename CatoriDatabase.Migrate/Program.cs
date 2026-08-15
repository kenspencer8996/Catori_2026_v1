using CatoriServices.Objects.database.Production;

if(args.Length!=1||string.IsNullOrWhiteSpace(args[0]))
{
    Console.Error.WriteLine("Usage: CatoriDatabase.Migrate <database-path>");
    return 2;
}
string databasePath=Path.GetFullPath(args[0]);
if(!File.Exists(databasePath))
{
    Console.Error.WriteLine($"Database not found: {databasePath}");
    return 3;
}
string backupPath=$"{databasePath}.backup-{DateTime.Now:yyyyMMdd-HHmmss}";
File.Copy(databasePath,backupPath,false);
try
{
    RuntimeProductionSchemaMigrator.Migrate(databasePath);
    Console.WriteLine($"Migrated: {databasePath}");
    Console.WriteLine($"Backup:   {backupPath}");
    return 0;
}
catch(Exception ex)
{
    Console.Error.WriteLine(ex);
    Console.Error.WriteLine($"Migration failed. Original backup: {backupPath}");
    return 1;
}
