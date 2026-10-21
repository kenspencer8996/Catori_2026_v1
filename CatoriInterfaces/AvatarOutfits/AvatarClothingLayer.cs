namespace CatoriInterfaces.AvatarOutfits;
public sealed class AvatarClothingLayer
{
    public string PartName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public int LayerOrder { get; set; }
    public double WidthRatio { get; set; } = 1;
    public double HeightRatio { get; set; } = 1;
    public double OffsetXRatio { get; set; }
    public double OffsetYRatio { get; set; }
}
