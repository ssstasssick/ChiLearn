using System.Globalization;

namespace ChiLearn.Models
{
    public class ProgressToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                return progress switch
                {
                    < 0.33 => Color.FromRgb(255, 50, 50),   // Красный
                    < 0.66 => Color.FromRgb(255, 200, 0),    // Жёлтый
                    _ => Color.FromRgb(50, 200, 50)          // Зелёный
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}