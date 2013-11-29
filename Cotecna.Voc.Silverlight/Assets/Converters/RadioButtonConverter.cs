using System;
using System.Windows.Data;
using Cotecna.Voc.Business;

namespace Cotecna.Voc.Silverlight
{
    public class RadioButtonCertificateStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            if((bool)value)  
                return Enum.Parse(typeof(CertificateStatusEnum),parameter.ToString(), true);
            return null;
        }
    }

    public class RadioButtonNullableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            if (parameter.ToString() == "True")
                return value;
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString() == "True")
                return value;
            return !(bool)value;
        }
    } 

}
