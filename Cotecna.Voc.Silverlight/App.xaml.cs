using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using Cotecna.Voc.Business;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Application class
    /// </summary>
    public partial class App : Application
    {
        public static bool IsForTest { get; set; }
        public static VocUser CurrentUser { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;
            this.InstallStateChanged += this.App_InstallStateChanged;
            InitializeComponent();
        }

        /// <summary>
        /// On application Startup 
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                //Show screen maximized
                Application.Current.MainWindow.WindowState = WindowState.Maximized;

                // Check for updates.
                Application.Current.CheckAndDownloadUpdateCompleted += Application_CheckAndDownloadUpdateCompleted;
                Application.Current.CheckAndDownloadUpdateAsync();
            }
            else
            {
                if (e.InitParams != null)
                {
                   if( e.InitParams.Keys.Contains("WebTest"))
                   {
                       bool isforTest;
                       bool.TryParse(e.InitParams["WebTest"],out isforTest);
                       IsForTest = isforTest;
                   }
                    
                }
            }
            //USE MainPage 
            this.RootVisual = new MainPage();

        }

        /// <summary>
        /// Check and download the last application
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Check error or more information</param>
        void Application_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                MessageBox.Show("A new version has been installed. " +
                "Please start again the application.");

                App.Current.MainWindow.Close();
            }
            else if (e.Error != null)
            {
                if (e.Error is PlatformNotSupportedException)
                {
                    MessageBox.Show("An application update is available, " +
                    "but it requires a new version of Silverlight. " +
                    "Visit http://silverlight.net to upgrade.");
                }
                else
                {
                    MessageBox.Show("An error has occurred while the system was starting up. "+
                        "Please contact your System Administrator");
                }
            }
        }

        /// <summary>
        /// Will inform the user when the installation was a success or a failure 
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        void App_InstallStateChanged(object sender, EventArgs e)
        {
            try
            {
                Install install = ((MainPage)this.RootVisual).MainContentControl.Content as Install;
            if (install != null && install.DataContext != null)
            {
                InstallViewModel installViewModel = (InstallViewModel)install.DataContext;
                switch (this.InstallState)
                {
                    case InstallState.InstallFailed:
                        installViewModel.DisplayInstallationFailed();
                        break;
                    case InstallState.Installed:
                        installViewModel.DisplayInstallationSucceded();
                        break;
                }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// On exit application
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Application_Exit(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// On unhandled exception
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Check errors</param>
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        /// <summary>
        /// Handle errors for In browser
        /// </summary>
        /// <param name="e"></param>
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
