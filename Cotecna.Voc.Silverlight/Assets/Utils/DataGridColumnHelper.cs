using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace Cotecna.Voc.Silverlight
{
    public static class DataGridColumnHelper
    {
        #region Header
        public static readonly DependencyProperty HeaderBindingProperty = DependencyProperty.RegisterAttached(
            "HeaderBinding",
            typeof(object),
            typeof(DataGridColumnHelper),
            new PropertyMetadata(null, DataGridColumnHelper.HeaderBinding_PropertyChanged));

        public static object GetHeaderBinding(DependencyObject source)
        {
            return (object)source.GetValue(DataGridColumnHelper.HeaderBindingProperty);
        }

        public static void SetHeaderBinding(DependencyObject target, object value)
        {
            target.SetValue(DataGridColumnHelper.HeaderBindingProperty, value);
        }

        private static void HeaderBinding_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn column = d as DataGridColumn;

            if (column == null) { return; }

            column.Header = e.NewValue;
        }
        #endregion

    }

    public static class MyExtensions
    {
        public static DataGridColumn GetByName(this ObservableCollection<DataGridColumn> col, string name)
        {
            return col.SingleOrDefault(p =>
                (string)p.GetValue(FrameworkElement.NameProperty) == name
            );
        }
    }
}
