using System;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Event arguments used by events which need to notify screens some user control needs to open and 
    /// a datacontext needs to be assigned. This datacontext is included here.
    /// </summary>
    /// <typeparam name="T">View model type</typeparam>
    public class ContextEditionEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets view model property
        /// </summary>
        public T Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextEditionEventArgs"/> class.
        /// </summary>
        /// <param name="viewmodel">View model to be managed</param>
        public ContextEditionEventArgs(T viewmodel)
        {
            Context = viewmodel;
        }
    }
}

