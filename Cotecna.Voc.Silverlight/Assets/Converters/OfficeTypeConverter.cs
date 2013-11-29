using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class OfficeTypeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            OfficeTypeEnum officeType;
            Enum.TryParse(value.ToString(), out officeType);
            string officeTypeText = string.Empty;
            switch (officeType)
            {
                case OfficeTypeEnum.LocalOffice:
                    officeTypeText = Strings.LocalOffice;
                    break;
                case OfficeTypeEnum.RegionalOffice:
                    officeTypeText = Strings.RegionalOffice;
                    break;
                default:
                    break;
            }
            return officeTypeText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
