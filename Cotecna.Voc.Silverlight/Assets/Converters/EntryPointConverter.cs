using System;
using System.Linq;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class EntryPointConverter : IValueConverter 
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;
            int entryPointId = int.Parse(value.ToString());
            var entryPoint = StaticReferences.GetEntryPoints().FirstOrDefault(ep => ep.EntryPointId == entryPointId);
            string entryPointName = entryPoint==null?string.Empty: entryPoint.Name;
            return entryPointName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
