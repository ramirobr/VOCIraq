using System;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class DateConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;
            DateTime date = (DateTime)value;
            string formattedDate = string.Empty;
            if(parameter == null)
                formattedDate = date.ToString("dd/MM/yyyy");
            else if(parameter.ToString().Equals("time"))
                formattedDate = date.ToString("dd/MM/yyyy HH:mm");
            else
                formattedDate = date.ToString("dd/MM/yyyy");
            return formattedDate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
