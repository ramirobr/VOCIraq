
namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Defines a method that a class implements in order to check whether a business object has been modified in a child window
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public interface IEditableDataComparer<in T>
    {

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        bool Equals(T other);
    }
}
