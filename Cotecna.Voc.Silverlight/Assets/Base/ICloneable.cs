

namespace Cotecna.Voc.Silverlight
{
    public interface ICloneable<T>
    {
        /// <summary>
        /// updates all properties from <c>source</c>
        /// </summary>
        /// <param name="source">Source data</param>
        void Clone(T source);
    }
}
