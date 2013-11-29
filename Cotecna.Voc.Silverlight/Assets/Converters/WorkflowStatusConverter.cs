using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    public class WorkflowStatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            WorkflowStatusEnum statusEnum;
            Enum.TryParse(value.ToString(), out statusEnum);
            string strValue = string.Empty;
            switch (statusEnum)
            {
                case WorkflowStatusEnum.Created:
                    strValue = Strings.Created;
                    break;
                case WorkflowStatusEnum.Requested:
                    strValue = Strings.Requested;
                    break;
                case WorkflowStatusEnum.Approved:
                    strValue = Strings.Approved;
                    break;
                case WorkflowStatusEnum.Rejected:
                    strValue = Strings.Rejected;
                    break;
                case WorkflowStatusEnum.Ongoing:
                    strValue = Strings.Ongoing;
                    break;
                case WorkflowStatusEnum.Closed:
                    strValue = Strings.Closed;
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
