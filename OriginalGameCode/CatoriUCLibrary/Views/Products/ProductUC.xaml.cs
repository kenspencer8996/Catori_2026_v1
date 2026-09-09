using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.Products;

public partial class ProductUC : UserControl, CatoriInterfaces.IImagePathSource
{
    public string? ImagePath { get; }

    public ProductUC(string? imagePath)
    {
        ImagePath = imagePath;
        InitializeComponent();

        Uri? imageUri = ResolveImageUri(imagePath);
        if (imageUri is null)
        {
            System.Diagnostics.Debug.WriteLine($"Image not found: {imagePath}");
            return;
        }
        try
        {
            ProductImage.Source = LoadImage(imageUri);
            System.Diagnostics.Debug.WriteLine($"ProductUC {imagePath} Width / Height: {ProductImage.Width} / {ProductImage.Height}");
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Product image could not be loaded: {imagePath}. {exception.Message}");
        }
        System.Diagnostics.Debug.WriteLine($"ProductUC hostGrid Width / Height: {hostGrid.Width} / {hostGrid.Height}");

    }

    private static Uri? ResolveImageUri(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return null;

        if (Uri.TryCreate(imagePath, UriKind.Absolute, out Uri? absoluteUri)
            && (!absoluteUri.IsFile || File.Exists(absoluteUri.LocalPath)))
            return absoluteUri;

        string applicationPath = Path.GetFullPath(imagePath, AppContext.BaseDirectory);
        return File.Exists(applicationPath)
            ? new Uri(applicationPath, UriKind.Absolute)
            : null;
    }

    private static BitmapSource LoadImage(Uri imageUri)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;

        if (imageUri.IsFile)
        {
            using var stream = new FileStream(
                imageUri.LocalPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);
            image.StreamSource = stream;
            image.EndInit();
        }
        else
        {
            image.UriSource = imageUri;
            image.EndInit();
        }

        image.Freeze();
        return image;
    }
}
