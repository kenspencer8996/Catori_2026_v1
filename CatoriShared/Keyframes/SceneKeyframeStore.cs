using System.IO;
using System.Text.Json;
using Path = System.IO.Path;
namespace CatoriShared.Keyframes;
public static class SceneKeyframeStore
{
    public const string DefinitionDirectory = @"C:\KeyframeDefinitions";
    public static string FileName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || sceneName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("Scene name must be a valid file name.", nameof(sceneName));
        }
        return Path.Combine(DefinitionDirectory, sceneName + "KeyframeDefinition.json");
    }
    public static string FileName(string viewName, string sceneName)
    {
        if (string.IsNullOrWhiteSpace(viewName) || viewName is "." or ".."
            || viewName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("View name must be a valid folder name.", nameof(viewName));
        }
        return Path.Combine(DefinitionDirectory, viewName, Path.GetFileName(FileName(sceneName)));
    }
    public static Keyframes Load(string viewName, string sceneName)
    {
        return LoadFile(FileName(viewName, sceneName));
    }
    public static Keyframes Load(string sceneName)
    {
        return LoadFile(FileName(sceneName));
    }
    private static Keyframes LoadFile(string file)
    {
        return File.Exists(file)
            ? JsonSerializer.Deserialize<Keyframes>(File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("Empty scene keyframe definition: " + file)
            : new Keyframes();
    }
    public static void Save(string viewName, string sceneName, Keyframes keyframes)
    {
        string file = FileName(viewName, sceneName);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, JsonSerializer.Serialize(keyframes, new JsonSerializerOptions { WriteIndented = true }));
    }
    public static void Save(string sceneName, Keyframes keyframes)
    {
        string file = FileName(sceneName);
        Directory.CreateDirectory(DefinitionDirectory);
        File.WriteAllText(file, JsonSerializer.Serialize(keyframes, new JsonSerializerOptions { WriteIndented = true }));
    }
    public static KeyframeProcessor CreateProcessor(string viewName, string sceneName, string pathName, string pathData)
    {
        var frames = new Keyframes { Frames = Load(viewName, sceneName).Frames.Where(frame => string.Equals(frame.PathName, pathName, StringComparison.OrdinalIgnoreCase)).ToList() };
        return new KeyframeProcessor(frames, PathLength(CatoriApp.Game.Objects.AnimationOnPath.LayoutItemPathSetSerializer.GetFirstPath(pathData)));
    }
    public static KeyframeProcessor CreateProcessor(string sceneName, string pathName, string pathData)
    {
        var frames = new Keyframes { Frames = Load(sceneName).Frames.Where(frame => string.Equals(frame.PathName, pathName, StringComparison.OrdinalIgnoreCase)).ToList() };
        return new KeyframeProcessor(frames, PathLength(CatoriApp.Game.Objects.AnimationOnPath.LayoutItemPathSetSerializer.GetFirstPath(pathData)));
    }
    public static double PathLength(string pathData)
    {
        var geometry = Geometry.Parse(pathData).GetFlattenedPathGeometry();
        double length = 0;
        foreach (var figure in geometry.Figures)
        {
            Point previous = figure.StartPoint;
            foreach (var segment in figure.Segments)
            {
                IEnumerable<Point> points = segment is PolyLineSegment poly ? poly.Points : segment is LineSegment line ? new[] { line.Point } : Array.Empty<Point>();
                foreach (Point point in points)
                {
                    length += (point - previous).Length;
                    previous = point;
                }
            }
            if (figure.IsClosed)
            {
                length += (figure.StartPoint - previous).Length;
            }
        }
        return length;
    }
}

