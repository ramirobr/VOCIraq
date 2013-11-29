using Cotecna.Voc.Silverlight.Assets.Resources;
using System;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class YesNoConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            bool boolValue = bool.Parse(value.ToString());

            // Inverting the Parsed bool value if we needed
            if (parameter != null && parameter.ToString().CompareTo("Invert") == 0)
                boolValue = !boolValue;

            return boolValue ? Strings.Yes : Strings.No;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class YesNoNullConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Strings.No;
            bool boolValue = bool.Parse(value.ToString());
            return boolValue ? Strings.Yes : Strings.No;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
