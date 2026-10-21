using System.IO;
using System.Text.Json;
using CatoriInterfaces.AvatarOutfits;
namespace CatoriUCLibrary.Objects.Outfits;
public sealed class AvatarGarmentCatalog
{
    public List<string> Errors { get; } = [];
    public IReadOnlyList<AvatarGarment> Load(string componentsFolder)
    {
        Directory.CreateDirectory(componentsFolder);
        Errors.Clear();
        var result = new List<AvatarGarment>();
        foreach (string manifest in Directory.EnumerateFiles(componentsFolder, "garment.json", SearchOption.AllDirectories))
        {
            try
            {
                var garment = JsonSerializer.Deserialize<AvatarGarment>(File.ReadAllText(manifest), AvatarWardrobeStore.JsonOptions)
                    ?? throw new InvalidDataException("The garment manifest is empty.");
                if (string.IsNullOrWhiteSpace(garment.GarmentId) || string.IsNullOrWhiteSpace(garment.Name)
                    || string.IsNullOrWhiteSpace(garment.Slot) || garment.Layers == null || garment.Layers.Count == 0)
                {
                    throw new InvalidDataException("A garment needs GarmentId, Name, Slot and Layers.");
                }
                if (result.Any(item => item.GarmentId == garment.GarmentId))
                {
                    throw new InvalidDataException("Duplicate GarmentId: " + garment.GarmentId);
                }
                string folder = Path.GetDirectoryName(manifest)!;
                garment.ThumbnailImage = ResolvePackageImage(folder, garment.ThumbnailImage);
                foreach (var layer in garment.Layers)
                {
                    if (layer == null)
                    {
                        throw new InvalidDataException("The garment contains a null layer.");
                    }
                    if (!AvatarOutfitParts.Names.Contains(layer.PartName, StringComparer.Ordinal))
                    {
                        throw new InvalidDataException("Unknown clothing part: " + layer.PartName);
                    }
                    layer.ImagePath = ResolvePackageImage(folder, layer.ImagePath);
                }
                result.Add(garment);
            }
            catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException or ArgumentException)
            {
                Errors.Add(Path.GetFileName(Path.GetDirectoryName(manifest)) + ": " + exception.Message);
            }
        }
        return result.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }
    private static string ResolvePackageImage(string folder, string path)
    {
        if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path))
        {
            throw new InvalidDataException("Package image paths must be relative to garment.json.");
        }
        string fullPath = Path.GetFullPath(Path.Combine(folder, path));
        string prefix = Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
        {
            throw new InvalidDataException("Package image is missing or outside the package: " + path);
        }
        return fullPath;
    }
}
