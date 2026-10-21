namespace CatoriInterfaces.AvatarOutfits;
public sealed class AvatarOutfit
{
    public string OutfitId { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "New outfit";
    public string ThumbnailImage { get; set; } = string.Empty;
    public List<AvatarGarment> Garments { get; set; } = [];
}
