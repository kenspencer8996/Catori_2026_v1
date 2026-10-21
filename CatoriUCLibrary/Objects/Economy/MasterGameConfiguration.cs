using System.IO;
using System.Text.Json;

namespace CatoriUCLibrary.Objects.Economy;

public sealed class MasterGameConfiguration
{
    public string GameId { get; set; } = "CatoriApp2027";
    public decimal DefaultBalance { get; set; } = 1000m;
    public static MasterGameConfiguration Load(string path)
    {
        var config = File.Exists(path)
            ? JsonSerializer.Deserialize<MasterGameConfiguration>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : new MasterGameConfiguration();
        if (config == null || string.IsNullOrWhiteSpace(config.GameId) || config.DefaultBalance < 0
            || config.DefaultBalance != decimal.Round(config.DefaultBalance, 2))
        {
            throw new InvalidDataException("Master configuration requires a GameId and non-negative DefaultBalance with at most two decimal places.");
        }
        return config;
    }
}
