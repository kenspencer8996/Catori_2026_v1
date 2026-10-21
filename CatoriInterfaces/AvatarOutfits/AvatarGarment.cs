namespace CatoriInterfaces.AvatarOutfits;
public sealed class AvatarGarment
{
    public string GarmentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public string ThumbnailImage { get; set; } = string.Empty;
    public List<AvatarClothingLayer> Layers { get; set; } = [];
}
