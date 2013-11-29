using System;
using System.Windows.Data;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    public class DocumentCategoryConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            string documentType = string.Empty;
            switch ((Cotecna.Voc.Business.DocumentTypeEnum)value)
            {
                case Cotecna.Voc.Business.DocumentTypeEnum.Certificate:
                    documentType = Strings.Certificate;
                    break;

                case Cotecna.Voc.Business.DocumentTypeEnum.SupportingDocument:
                    documentType = Strings.SupportingDocument;
                    break;

                default:
                    break;
            }

            return documentType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
