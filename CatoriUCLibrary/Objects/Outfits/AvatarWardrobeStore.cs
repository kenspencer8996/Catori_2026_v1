using System.IO;
using System.Text.Json;
using CatoriInterfaces.AvatarOutfits;
namespace CatoriUCLibrary.Objects.Outfits;
public sealed class AvatarWardrobeStore
{
    public string RootFolder { get; }
    public string ComponentsFolder => Path.Combine(RootFolder, "Components");
    public string OutfitsFile => Path.Combine(RootFolder, "outfits.json");
    public static JsonSerializerOptions JsonOptions { get; } = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
    public AvatarWardrobeStore(string? rootFolder = null)
    {
        RootFolder = Path.GetFullPath(rootFolder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CatoriApp", "Wardrobe"));
    }
    public IReadOnlyList<AvatarOutfit> Load()
    {
        if (!File.Exists(OutfitsFile))
        {
            return [];
        }
        var outfits = JsonSerializer.Deserialize<List<AvatarOutfit>>(File.ReadAllText(OutfitsFile), JsonOptions)
            ?? throw new InvalidDataException("The wardrobe JSON contains no outfit list.");
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var outfit in outfits)
        {
            if (outfit == null || string.IsNullOrWhiteSpace(outfit.OutfitId) || !ids.Add(outfit.OutfitId))
            {
                throw new InvalidDataException("Outfits need unique, nonempty OutfitId values.");
            }
            RewritePaths(outfit, ResolveImage);
        }
        return outfits;
    }
    public void Save(IEnumerable<AvatarOutfit> outfits)
    {
        Directory.CreateDirectory(RootFolder);
        var copies = outfits.Select(Snapshot).ToList();
        foreach (var outfit in copies)
        {
            RewritePaths(outfit, MakePortable);
        }
        string temporary = OutfitsFile + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(copies, JsonOptions));
        if (File.Exists(OutfitsFile))
        {
            File.Copy(OutfitsFile, OutfitsFile + ".bak", true);
        }
        File.Move(temporary, OutfitsFile, true);
    }
    public string ResolveImage(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.StartsWith("pack:", StringComparison.OrdinalIgnoreCase) || Path.IsPathRooted(path))
        {
            return path;
        }
        return Path.GetFullPath(Path.Combine(RootFolder, path));
    }
    public static AvatarOutfit Snapshot(AvatarOutfit outfit)
    {
        return JsonSerializer.Deserialize<AvatarOutfit>(JsonSerializer.Serialize(outfit, JsonOptions), JsonOptions)
            ?? throw new InvalidDataException("The outfit is empty.");
    }
    public static AvatarGarment SnapshotGarment(AvatarGarment garment)
    {
        return JsonSerializer.Deserialize<AvatarGarment>(JsonSerializer.Serialize(garment, JsonOptions), JsonOptions)
            ?? throw new InvalidDataException("The garment is empty.");
    }
    private string MakePortable(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path))
        {
            return path;
        }
        string relative = Path.GetRelativePath(RootFolder, path);
        return relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) ? path : relative;
    }
    private static void RewritePaths(AvatarOutfit outfit, Func<string, string> transform)
    {
        outfit.ThumbnailImage = transform(outfit.ThumbnailImage);
        foreach (var garment in outfit.Garments ?? throw new InvalidDataException("An outfit is missing its garments."))
        {
            if (garment == null)
            {
                throw new InvalidDataException("The outfit contains a null garment.");
            }
            garment.ThumbnailImage = transform(garment.ThumbnailImage);
            foreach (var layer in garment.Layers ?? throw new InvalidDataException("A garment is missing its layers."))
            {
                if (layer == null)
                {
                    throw new InvalidDataException("A garment contains a null layer.");
                }
                layer.ImagePath = transform(layer.ImagePath);
            }
        }
    }
}
