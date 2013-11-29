using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// View model for install screen
    /// </summary>
    public class InstallViewModel : ViewModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallViewModel"/> class.
        /// </summary>
        public InstallViewModel()
        {
            if (Application.Current.InstallState == InstallState.Installed)
            {
                Message = Strings.ApplicationAlreadyInstalled;
                IsNotInstalled = false;
            }
            else
            {
                IsNotInstalled = true;
                this.ErrorMessage = "";
            }
        }

        private bool _isNotInstalled;
                
        /// <summary>
        /// Gets or sets a value indicating whether application is installed
        /// </summary>
        public bool IsNotInstalled
        {
            get { return _isNotInstalled; }
            set { _isNotInstalled = value;
            OnPropertyChanged(() => this.IsNotInstalled);
            }
        }

        private ICommand _installCommand;
        /// <summary>
        /// Gets install command
        /// </summary>
        public ICommand Install
        {
            get
            {
                if (_installCommand == null)
                    _installCommand = new DelegateCommand(Installation);
                return _installCommand;
            }
        }

        /// <summary>
        /// Installation process
        /// </summary>
        public void Installation()
        {
            // Make sure that the application is not already installed.
            if (Application.Current.InstallState != InstallState.Installed)
            {
                // Attempt to install it.
                bool installAccepted = Application.Current.Install();
                if (!installAccepted)
                {
                    this.ErrorMessage = "You declined the install. Click Install to try again.";
                }
                else
                {
                    IsNotInstalled = false;
                    Message = Strings.ApplicationInstalling;
                }
            }
        }
                
        /// <summary>
        /// Installation failed error
        /// </summary>
        public void DisplayInstallationFailed()
        {
            this.ErrorMessage = "The application failed to install on your machine";
        }

        /// <summary>
        /// Installation succeded
        /// </summary>
        public void DisplayInstallationSucceded()
        {
            Message = Strings.ApplicationAlreadyInstalled;
            IsNotInstalled = false;
        }
    }
}
