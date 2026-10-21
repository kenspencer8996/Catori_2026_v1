using System.ComponentModel;
using CatoriInterfaces.AvatarOutfits;
using CatoriUCLibrary.Objects.Outfits;
namespace CatoriUCLibrary.Views;
public sealed class OutfitEditorViewModel : INotifyPropertyChanged
{
    public AvatarOutfit Outfit { get; }
    public string Name
    {
        get => Outfit.Name;
        set
        {
            if (Outfit.Name == value)
            {
                return;
            }
            Outfit.Name = value;
            Notify(nameof(Name));
            Edited?.Invoke(this, EventArgs.Empty);
        }
    }
    public string ThumbnailImage => Outfit.ThumbnailImage;
    public string GarmentSummary => Outfit.Garments.Count == 0 ? "Drag clothing components onto this hanger" : string.Join(" + ", Outfit.Garments.Select(item => item.Name));
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? Edited;
    public OutfitEditorViewModel(AvatarOutfit outfit)
    {
        Outfit = AvatarWardrobeStore.Snapshot(outfit);
    }
    public void AddGarment(AvatarGarment garment)
    {
        ArgumentNullException.ThrowIfNull(garment);
        if (string.IsNullOrWhiteSpace(garment.Slot) || garment.Layers == null || garment.Layers.Count == 0)
        {
            throw new ArgumentException("The clothing component has no slot or images.", nameof(garment));
        }
        Outfit.Garments.RemoveAll(item => string.Equals(item.Slot, garment.Slot, StringComparison.OrdinalIgnoreCase));
        Outfit.Garments.Add(AvatarWardrobeStore.SnapshotGarment(garment));
        Outfit.ThumbnailImage = garment.ThumbnailImage;
        Notify(nameof(ThumbnailImage));
        Notify(nameof(GarmentSummary));
        Edited?.Invoke(this, EventArgs.Empty);
    }
    private void Notify(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
