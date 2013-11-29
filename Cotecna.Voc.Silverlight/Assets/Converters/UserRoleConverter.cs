using System;
using System.Windows.Data;
using Cotecna.Voc.Business;

namespace Cotecna.Voc.Silverlight
{
    public class UserRoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return Enum.Parse(typeof(UserRoleEnum), value.ToString(), true);
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
