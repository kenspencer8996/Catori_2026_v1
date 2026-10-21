using CatoriUCLibrary.Views.Person;
using CatoriInterfaces.AvatarOutfits;
using CatoriUCLibrary.Objects.Outfits;
namespace CatoriUCLibrary.Views;
public partial class PersonUC
{
    public AvatarOutfit? CurrentOutfit { get; private set; }
    public event EventHandler? OutfitApplied;
    public void ApplyOutfit(AvatarOutfit outfit)
    {
        ArgumentNullException.ThrowIfNull(outfit);
        Dispatcher.VerifyAccess();
        var snapshot = AvatarWardrobeStore.Snapshot(outfit);
        if (snapshot.Garments == null || snapshot.Garments.Any(garment => garment == null || garment.Layers == null || garment.Layers.Any(layer => layer == null)))
        {
            throw new ArgumentException("The outfit contains invalid garment data.", nameof(outfit));
        }
        var store = new AvatarWardrobeStore();
        var layers = snapshot.Garments.SelectMany(garment => garment.Layers).ToList();
        foreach (var layer in layers)
        {
            layer.ImagePath = store.ResolveImage(layer.ImagePath);
        }
        PartAnimator.ApplyClothingLayers(layers);
        CurrentOutfit = snapshot;
        OutfitApplied?.Invoke(this, EventArgs.Empty);
    }
    private void RefreshClothingHosts()
    {
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.Body), BodyClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.LeftUpperArm), LeftUpperArmClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.RightUpperArm), RightUpperArmClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.LeftForearm), LeftForearmClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.RightForearm), RightForearmClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.LeftThigh), LeftThighClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.RightThigh), RightThighClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.LeftLowerLeg), LeftLowerLegClothing);
        PartAnimator.RegisterClothingHost(nameof(PersonPartType.RightLowerLeg), RightLowerLegClothing);
        if (CurrentOutfit != null)
        {
            ApplyOutfit(CurrentOutfit);
        }
    }
}
