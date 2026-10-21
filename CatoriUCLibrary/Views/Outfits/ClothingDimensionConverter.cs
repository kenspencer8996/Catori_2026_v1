using System.Globalization;
using System.Windows.Data;

namespace CatoriUCLibrary.Views;

public sealed class ClothingDimensionConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Length == 2 && values[0] is double dimension && values[1] is double ratio
            && double.IsFinite(dimension) && double.IsFinite(ratio) ? dimension * ratio : 0d;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
