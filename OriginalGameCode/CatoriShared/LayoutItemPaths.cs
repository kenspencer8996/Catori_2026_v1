using System.Text.Json;
using System.Text.Json.Serialization;

namespace CatoriApp.Game.Objects.AnimationOnPath;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PathSelectionMode
{
    First,
    Random,
    Sequential
}

public sealed class LayoutItemPathSet
{
    public int SchemaVersion { get; set; } = 1;
    public PathSelectionMode SelectionMode { get; set; } = PathSelectionMode.First;
    public List<LayoutPath> Paths { get; set; } = [];
}

public sealed class LayoutPath
{
    public string Name { get; set; } = "Path 1";
    public string WpfPath { get; set; } = string.Empty;
    public string? PartName { get; set; }
    public int Order { get; set; }
}

public static class LayoutItemPathSetSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    public static LayoutItemPathSet Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim() is "[]" or "null")
            return CreateDefault();

        string trimmed = value.Trim();
        if (trimmed.StartsWith('{'))
        {
            try
            {
                var set = JsonSerializer.Deserialize<LayoutItemPathSet>(trimmed, Options);
                if (set?.Paths is { Count: > 0 })
                {
                    Normalize(set);
                    return set;
                }
            }
            catch (JsonException)
            {
                // Legacy ItemDataJson values are raw WPF geometry strings.
            }
        }

        return new LayoutItemPathSet
        {
            Paths = [new LayoutPath { Name = "Path 1", WpfPath = value, Order = 0 }]
        };
    }

    public static string Serialize(LayoutItemPathSet set)
    {
        ArgumentNullException.ThrowIfNull(set);
        Normalize(set);
        return JsonSerializer.Serialize(set, Options);
    }

    public static string GetFirstPath(string? value)
    {
        return Parse(value).Paths.OrderBy(path => path.Order).FirstOrDefault()?.WpfPath ?? string.Empty;
    }

    public static bool IsPathSet(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
                && value.TrimStart().StartsWith('{')
                && value.Contains("\"paths\"", StringComparison.OrdinalIgnoreCase);
    }

    private static LayoutItemPathSet CreateDefault()
    {
        return new() { Paths = [new LayoutPath { Name = "Path 1", Order = 0 }] };
    }

    private static void Normalize(LayoutItemPathSet set)
    {
        if (set.Paths.Count == 0)
            set.Paths.Add(new LayoutPath { Name = "Path 1" });
        int index = 0;
        foreach (var path in set.Paths.OrderBy(path => path.Order).ToList())
        {
            path.Name = string.IsNullOrWhiteSpace(path.Name) ? $"Path {index + 1}" : path.Name.Trim();
            path.WpfPath ??= string.Empty;
            path.Order = index++;
        }
        set.Paths = set.Paths.OrderBy(path => path.Order).ToList();
    }
}
