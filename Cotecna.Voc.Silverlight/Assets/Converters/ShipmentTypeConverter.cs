using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class ShipmentTypeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ShipmentTypeEnum enumValue;
            Enum.TryParse(value.ToString(), out enumValue);
            string result = string.Empty;
            switch (enumValue)
            {
                case ShipmentTypeEnum.Air:
                    result = Strings.Air;
                    break;
                case ShipmentTypeEnum.Maritime:
                    result = Strings.Maritime;
                    break;
                case ShipmentTypeEnum.Land:
                    result = Strings.Land;
                    break;
                case ShipmentTypeEnum.SeaLand:
                    result = Strings.SeaLand;
                    break;
                default:
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
