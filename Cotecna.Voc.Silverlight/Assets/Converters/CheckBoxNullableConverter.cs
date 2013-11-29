using System;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class CheckBoxNullableConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            bool result = bool.Parse(value.ToString());
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
