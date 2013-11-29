using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cotecna.Voc.Silverlight
{
    public static class QuereableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            ObservableCollection<T> returnObject = new ObservableCollection<T>(collection);
            return returnObject;
        }
    }
}
