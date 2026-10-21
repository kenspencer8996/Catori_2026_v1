using System.IO;
using System.Windows.Controls;
using CatoriInterfaces.AvatarOutfits;
using CatoriUCLibrary.Objects.Outfits;
namespace CatoriUCLibrary.Views;
public partial class VisualPartAnimatorUC
{
    private readonly Dictionary<string, ItemsControl> clothingHosts = new(StringComparer.Ordinal);
    public void RegisterClothingHost(string partName, ItemsControl host)
    {
        if (!ContainsPart(partName))
        {
            throw new ArgumentException("Cannot clothe an unregistered part: " + partName);
        }
        clothingHosts[partName] = host;
    }
    public void ApplyClothingLayers(IEnumerable<AvatarClothingLayer> layers)
    {
        var prepared = AvatarOutfitParts.Names.ToDictionary(name => name, _ => new List<ClothingLayerViewModel>(), StringComparer.Ordinal);
        foreach (var layer in layers)
        {
            if (!prepared.TryGetValue(layer.PartName, out var images) || !clothingHosts.ContainsKey(layer.PartName))
            {
                throw new ArgumentException("Unknown clothing part: " + layer.PartName);
            }
            System.Windows.Media.ImageSource image;
            try
            {
                image = LoadImage(layer.ImagePath) ?? throw new IOException("Could not load clothing image: " + layer.ImagePath);
            }
            catch (Exception exception) when (exception is FormatException or NotSupportedException)
            {
                throw new InvalidDataException("Invalid clothing image: " + layer.ImagePath, exception);
            }
            if (!double.IsFinite(layer.WidthRatio) || layer.WidthRatio <= 0
                || !double.IsFinite(layer.HeightRatio) || layer.HeightRatio <= 0
                || !double.IsFinite(layer.OffsetXRatio) || !double.IsFinite(layer.OffsetYRatio))
            {
                throw new InvalidDataException("Clothing geometry must have finite offsets and positive dimensions.");
            }
            images.Add(new ClothingLayerViewModel
            {
                Image = image,
                LayerOrder = layer.LayerOrder,
                WidthRatio = layer.WidthRatio,
                HeightRatio = layer.HeightRatio,
                OffsetXRatio = layer.OffsetXRatio,
                OffsetYRatio = layer.OffsetYRatio,
            });
        }
        // Every image is validated before the existing visuals are changed.
        foreach (var host in clothingHosts)
        {
            host.Value.ItemsSource = prepared[host.Key].OrderBy(layer => layer.LayerOrder).ToList();
        }
    }
}
