using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Behaviors
{
    public class EmptyCollectionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection collection)
            {
                // Если параметр "words", проверяем коллекцию слов
                if (parameter as string == "words")
                    return collection.Count == 0;

                // Если параметр "rules", проверяем коллекцию правил
                if (parameter as string == "rules")
                    return collection.Count == 0;
            }
            return value == null; // Если коллекция null, показываем сообщение
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
