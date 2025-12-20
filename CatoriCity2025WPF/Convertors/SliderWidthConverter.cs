using System.Globalization;

namespace CatoriCity2025WPF.Convertors
{
    public class SliderWidthConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double sliderValue = (double)value;
            return sliderValue * 3; // Adjust multiplier based on slider width
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

