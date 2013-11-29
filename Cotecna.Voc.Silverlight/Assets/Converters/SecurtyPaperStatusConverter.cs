using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class SecurtyPaperStatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            SecurityPaperStatusEnum currentAction;
            Enum.TryParse(value.ToString(), false, out currentAction);
            string strValue = string.Empty;
            switch (currentAction)
            {
                case SecurityPaperStatusEnum.NotIssued:
                    strValue = Strings.NotIssued;
                    break;
                case SecurityPaperStatusEnum.Issued:
                    strValue = Strings.Issued;
                    break;
                case SecurityPaperStatusEnum.Cancelled:
                    strValue = Strings.Cancelled;
                    break;
                case SecurityPaperStatusEnum.MissPrinted:
                    strValue = Strings.MisPrinted;
                    break;
                default:
                    break;
            }
            return strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
