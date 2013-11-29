using System;
using System.Windows.Data;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Converts a boolean value into Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Returns visibility depending on a boolean value
        /// </summary>
        /// <param name="value">Boolean value</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>a visibility status</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (bool)value;

            if (val)
                return System.Windows.Visibility.Visible;

            return System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Not implemented. It is returning the same value received
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>received value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Converts a visibility type depending on opposite status boolean received
    /// </summary>
    public class BoolToVisibilityInverseConverter : IValueConverter
    {
        /// <summary>
        /// Returns visibility 
        /// </summary>
        /// <param name="value">boolean value</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>visibility status opposite status</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (bool)value;

            if (!val)
                return System.Windows.Visibility.Visible;

            return System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Returns the same received value.
        /// </summary>
        /// <param name="value">Value returned</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>received value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Converts a opposite boolean value respect the boolean value received.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Returns a boolean value
        /// </summary>
        /// <param name="value">boolean value</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>the opposite to received boolean value</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (bool)value;

            return !val;
        }

        /// <summary>
        /// Returns the same recived value 
        /// </summary>
        /// <param name="value">boolean value</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>recived value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Converts a boolean value into Visibility
    /// </summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Returns visibility depending on a boolean value
        /// </summary>
        /// <param name="value">Int value</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>a visibility status</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (int)value;

            if (val == 0 || val == 1)
                return System.Windows.Visibility.Visible;

            return System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Not implemented. It is returning the same value received
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>received value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Converts a visibility type depending on opposite status boolean received
    /// </summary>
    public class ZeroToVisibilityInverseConverter : IValueConverter
    {
        /// <summary>
        /// Returns visibility 
        /// </summary>
        /// <param name="value">boolean value</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>visibility status opposite status</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (int)value;

            if (val > 1)
                return System.Windows.Visibility.Visible;

            return System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Returns the same received value.
        /// </summary>
        /// <param name="value">Value returned</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>received value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
