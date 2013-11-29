using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Converter to show an Online status into a Color
    /// </summary>
    public class OnlineToColorConverter : IValueConverter
    {
        Brush _onlineBrush = new SolidColorBrush(Colors.Green);
        /// <summary>
        /// Gets/sets the online status color
        /// </summary>
        public Brush OnlineBrush { 
            get
            {
                return _onlineBrush;
            }
            set
            {
                _onlineBrush = value;
            }
        }

        Brush _offlineBrush = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Gets/Sets the offline status color
        /// </summary>
        public Brush OfflineBrush
        {
            get
            {
                return _offlineBrush;
            }
            set
            {
                _offlineBrush = value;
            }
        }

        

        #region IValueConverter Members

        /// <summary>
        /// Transforms a bool value into a color
        /// </summary>
        /// <param name="value">bolean value received</param>
        /// <param name="targetType">Not used</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>online/offline color</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null)
                return null;

            bool isOnline = (bool)value;

            return isOnline ? OnlineBrush : OfflineBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
