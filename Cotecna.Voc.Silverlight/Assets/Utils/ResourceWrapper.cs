using System;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Wraps access to the strongly typed resource classes so that you can bind
    /// control properties to resource strings in XAML or access form other projects
    /// </summary>
    public sealed class ResourceWrapper
    {
        readonly Assets.Resources.Strings _wrapper = new Assets.Resources.Strings();

        /// <summary>
        /// Gets the translations for all the labels
        /// </summary>
        public Assets.Resources.Strings Translations
        {
            get { return _wrapper; }
        }
    }   
}
