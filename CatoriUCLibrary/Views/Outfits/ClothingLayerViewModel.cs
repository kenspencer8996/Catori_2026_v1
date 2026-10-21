using System.Windows.Media;
namespace CatoriUCLibrary.Views;
public sealed class ClothingLayerViewModel
{
    public required ImageSource Image { get; init; }
    public int LayerOrder { get; init; }
    public double WidthRatio { get; init; } = 1;
    public double HeightRatio { get; init; } = 1;
    public double OffsetXRatio { get; init; }
    public double OffsetYRatio { get; init; }
}
