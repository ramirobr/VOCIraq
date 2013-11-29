using System;
using System.Windows.Data;
using Cotecna.Voc.Business;

namespace Cotecna.Voc.Silverlight
{
    public class ResultEnumConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;
            ResultEnum result;
            if (Enum.TryParse<ResultEnum>(value.ToString(), out result))
                return result.ToString();
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
