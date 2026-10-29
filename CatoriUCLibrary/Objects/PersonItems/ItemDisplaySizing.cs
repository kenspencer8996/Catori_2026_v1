using System.Windows;

namespace CatoriUCLibrary.Objects.PersonItems;

public static class ItemDisplaySizing
{
    public static Size Fit(Size natural, double width, double height)
    {
        if (!double.IsFinite(natural.Width) || !double.IsFinite(natural.Height) || natural.Width <= 0 || natural.Height <= 0)
        {
            natural = new Size(48, 64);
        }
        bool hasWidth = double.IsFinite(width) && width > 0;
        bool hasHeight = double.IsFinite(height) && height > 0;
        double scale = hasWidth && hasHeight ? Math.Min(width / natural.Width, height / natural.Height)
            : hasWidth ? width / natural.Width : hasHeight ? height / natural.Height : 1;
        return new Size(natural.Width * scale, natural.Height * scale);
    }

    public static Size Resolve(PersonItem item, Size current, bool shelf)
    {
        return Fit(current, shelf ? item.ShelfWidth : item.PlacedWidth, shelf ? item.ShelfHeight : item.PlacedHeight);
    }
}
