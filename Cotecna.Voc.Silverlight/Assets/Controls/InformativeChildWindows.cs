using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Save confirmation when changes were not saved
    /// </summary>
    public class SaveConfirmation : InformativeChildWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveConfirmation"/> class.
        /// </summary>
        public SaveConfirmation()
        {
            Message = Strings.SaveConfirmationMessage;
            Title = Strings.UnsavedChanges;
            Image = StyleImages.Warning;
            ButtonSelection = ButtonSelections.YesNo;
        }
    }


    /// <summary>
    /// Delete confirmation untli proceeding with the deletion
    /// </summary>
    public class DeleteConfirmation : InformativeChildWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteConfirmation"/> class.
        /// </summary>
        public DeleteConfirmation()
        {
            //Message = Strings.SaveConfirmationMessage;
            Title = Strings.DeleteConfirmation;
            Image = StyleImages.Warning;
            ButtonSelection = ButtonSelections.YesNo;
        }
    }

    /// <summary>
    /// Delete confirmation event args
    /// </summary>
    internal class DeleteConfirmationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets message
        /// </summary>
        internal string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteConfirmationEventArgs"/> class.
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        internal DeleteConfirmationEventArgs(string message)
        {
            Message = message;
        }

    }



}
