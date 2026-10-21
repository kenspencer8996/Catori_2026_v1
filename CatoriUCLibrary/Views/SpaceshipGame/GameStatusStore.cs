using System.IO;

using System.Text.Json;


namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;


public static class GameStatusStore
{

    private static readonly object SyncRoot = new();

    private static readonly JsonSerializerOptions JsonOptions = new() {
 WriteIndented = true }
;

    private static readonly string FolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "CatoriLearning", "catoriPlay2");


    public static string FilePath {
 get;
 }
 = Path.Combine(FolderPath, "game-status.json");

    public static GameStatus Current {
 get;
 private set;
 }
 = new();


    public static void Load()
    {

        lock (SyncRoot)
        {

            try
            {

                Current = File.Exists(FilePath)
                    ? JsonSerializer.Deserialize<GameStatus>(File.ReadAllText(FilePath), JsonOptions) ?? new GameStatus()
                    : new GameStatus();

            }

            catch (JsonException)
            {

                Current = new GameStatus();

            }


            Current.Games ??= new GameSections();

            Current.Games.AstronautChase ??= new AstronautChaseStatus();

            Current.Games.AstronautRadarChase ??= new AstronautChaseStatus();

            Current.Games.BoxMatch1 ??= new BoxMatch1Status();

            Current.Games.AstronautChase.RequiredHits = Math.Max(8, Current.Games.AstronautChase.RequiredHits);

            Current.Games.AstronautRadarChase.RequiredHits = Math.Max(8, Current.Games.AstronautRadarChase.RequiredHits);

            Save();

        }

    }


    public static void Save()
    {

        lock (SyncRoot)
        {

            Directory.CreateDirectory(FolderPath);

            var temporaryPath = FilePath + ".tmp";

            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(Current, JsonOptions));

            File.Move(temporaryPath, FilePath, true);

        }

    }

}









