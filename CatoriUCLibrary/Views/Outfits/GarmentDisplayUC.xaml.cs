using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CatoriInterfaces.AvatarOutfits;
using CatoriUCLibrary.Objects.Outfits;

namespace CatoriUCLibrary.Views;

public partial class GarmentDisplayUC : UserControl
{
    private Point? _dragStart;
    private AvatarOutfit? _outfit;
    public static readonly DependencyProperty OutfitJsonProperty = DependencyProperty.Register(
        nameof(OutfitJson), typeof(string), typeof(GarmentDisplayUC), new PropertyMetadata("", OutfitChanged));
    public string OutfitJson
    {
        get => (string)GetValue(OutfitJsonProperty);
        set => SetValue(OutfitJsonProperty, value);
    }
    public GarmentDisplayUC()
    {
        InitializeComponent();
    }
    private static void OutfitChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((GarmentDisplayUC)sender).LoadOutfit();
    }
    private void LoadOutfit()
    {
        _outfit = string.IsNullOrWhiteSpace(OutfitJson) ? null
            : JsonSerializer.Deserialize<AvatarOutfit>(OutfitJson, AvatarWardrobeStore.JsonOptions);
        GarmentDisplayName.Text = _outfit?.Name ?? "";
        GarmentThumbnail.Source = null;
        if (_outfit == null)
        {
            return;
        }
        _outfit.ThumbnailImage = Resolve(_outfit.ThumbnailImage);
        foreach (AvatarGarment garment in _outfit.Garments)
        {
            garment.ThumbnailImage = Resolve(garment.ThumbnailImage);
            foreach (AvatarClothingLayer layer in garment.Layers)
            {
                layer.ImagePath = Resolve(layer.ImagePath);
            }
        }
        if (File.Exists(_outfit.ThumbnailImage))
        {
            using var stream = File.OpenRead(_outfit.ThumbnailImage);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            GarmentThumbnail.Source = image;
        }
    }
    private static string Resolve(string path)
    {
        return string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path)
            ? path : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
    }
    public DataObject CreateDragData()
    {
        if (_outfit == null || _outfit.Garments.Count == 0)
        {
            throw new InvalidOperationException("This display has no garment.");
        }
        return new DataObject(AvatarOutfitDragFormats.Outfit, AvatarWardrobeStore.Snapshot(_outfit));
    }
    private void Garment_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dragStart = e.GetPosition(this);
    }
    private void Garment_MouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStart is not Point start || e.LeftButton != MouseButtonState.Pressed)
        {
            _dragStart = null;
            return;
        }
        Point current = e.GetPosition(this);
        if (Math.Abs(current.X - start.X) < SystemParameters.MinimumHorizontalDragDistance
            && Math.Abs(current.Y - start.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }
        _dragStart = null;
        if (_outfit?.Garments.Count > 0)
        {
            DragDrop.DoDragDrop(this, CreateDragData(), DragDropEffects.Copy);
        }
    }
}
