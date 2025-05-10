using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Behaviors
{
    public class MatchingResultToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return Colors.Transparent;

            var chiWord = values[0] as string;
            var results = values[1] as Dictionary<string, bool>;

            if (chiWord == null || results == null)
                return Colors.Transparent;

            if (results.TryGetValue(chiWord, out bool isCorrect))
                return isCorrect ? Colors.Transparent : Colors.Red;

            return Colors.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

