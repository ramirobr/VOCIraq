using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;

namespace Cotecna.Voc.FileAccess
{
    public class ConcurrencyUserThread
    {
        private System.Timers.Timer _timer;

        private object lockObject = new object();

        private static ConcurrencyUserThread _thread = null;

        FileAccessService _fileAccess;
        private ExceptionManager exceptionManager;

        /// <summary>
        /// Singleton pattern
        /// </summary>
        public static ConcurrencyUserThread Instance
        {
            get
            {
                if (_thread == null)
                {
                    _thread = new ConcurrencyUserThread();
                }
                return _thread;
            }
        }



        /// <summary>
        /// Constructor to instanciate timer object
        /// </summary>
        protected ConcurrencyUserThread()
        {
            lock (lockObject)
            {
                double timeThread = 0;
                string timeThreadText = Properties.Settings.Default.TimePeriod;
                double.TryParse(timeThreadText, out timeThread);
                timeThread = timeThread == 0 ? 2 : timeThread;
                _fileAccess = new FileAccessService();

                if (_timer == null)
                {
                    _timer = new System.Timers.Timer();
                    _timer.Interval = TimeSpan.FromSeconds(timeThread).TotalMilliseconds;
                    _timer.Elapsed += TimerElapsed;
                    _timer.Start();
                }
            }
        }


        /// <summary>
        /// Timer event to be executed constantly according in a given time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TimerElapsed(object sender, EventArgs e)
        {
            try
            {

                if (_fileAccess != null)
                {
                    _fileAccess.GetPhysicalFile();
                }
            }
            catch (Exception ex)
            {
                // Use the EAB to handle the exceptions according to the configuration
                OnError(ex);
            }
        }


        /// <summary>
        /// Used to handle exceptions and register them in the log file
        /// </summary>    
        public ExceptionManager ExceptionManager
        {
            get
            {
                if (exceptionManager == null)
                {
                    IUnityContainer container = new UnityContainer();
                    container.AddNewExtension<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.EnterpriseLibraryCoreExtension>();
                    return container.Resolve<ExceptionManager>();
                }
                else
                    return exceptionManager;
            }
            set
            {
                exceptionManager = value;
            }
        }


        protected void OnError(Exception errorInfo)
        {
            this.ExceptionManager.HandleException(errorInfo, "AllExceptionsPolicy");
        }



        public void StartContinueProcess()
        {
        }
    }
}