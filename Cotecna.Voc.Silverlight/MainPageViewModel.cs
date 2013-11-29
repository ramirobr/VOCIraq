using System;
using System.ComponentModel;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// View model for main page screen
    /// </summary>
    public class MainPageViewModel:ViewModel
    {
        #region private fields and properties

        private string _loggedUser = String.Empty;

        /// <summary>
        /// Gets or sets the name of the logged user.
        /// </summary>
        public string LoggedUser
        {
            get { return _loggedUser; }
            set { _loggedUser = value; OnPropertyChanged("LoggedUser"); }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {

            if (!DesignerProperties.IsInDesignTool)
            {
                //CallServiceGetUserRights();
            }
        }
        #endregion

        #region Private methods

        /// <summary>
        /// change view when user select a different option in top menu
        /// </summary>
        /// <param name="view">View to be displayed</param>
        internal void ChangeView(VocViews view)
        {
            if (GoToView != null)
                GoToView(this, new ChangeViewEventArgs(view));
        }

        #endregion

        #region Events and commands

        /// <summary>
        /// Event fired to display the corresponding view
        /// </summary>
        internal event EventHandler<ChangeViewEventArgs> GoToView;

        #endregion
    }
}
