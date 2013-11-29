using System;
using System.Linq;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class OfficeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;
            int OfficeId = int.Parse(value.ToString());
            var Office = StaticReferences.GetOffices().FirstOrDefault(ep => ep.OfficeId == OfficeId);
            string OfficeName = Office == null ? string.Empty : Office.OfficeName;
            return OfficeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
