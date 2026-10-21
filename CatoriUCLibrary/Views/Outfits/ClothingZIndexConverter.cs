using System.Globalization;
using System.Windows.Data;
namespace CatoriUCLibrary.Views;
public sealed class ClothingZIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int index ? index + 1 : 1;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
