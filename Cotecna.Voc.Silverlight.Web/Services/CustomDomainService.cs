using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Web;
using Cotecna.Voc.Business;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;

namespace Cotecna.Voc.Silverlight.Web.Services
{
    public class CustomDomainService:DomainService
    {
        private ExceptionManager exceptionManager;
        private LogWriter logWriter;

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

        /// <summary>
        /// Used to log entries in the log file
        /// </summary>
        public LogWriter LogWriter
        {
            get
            {
                if (logWriter == null)
                {
                    IUnityContainer container = new UnityContainer();
                    container.AddNewExtension<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.EnterpriseLibraryCoreExtension>();
                    return container.Resolve<LogWriter>();
                }
                else
                    return logWriter;
            }
            set
            {
                logWriter = value;
            }
        }

        /// <summary>
        /// Writes a message in the Log file
        /// </summary>
        /// <param name="message"></param>
        protected void WriteLog(string message)
        {
            //In Web.config there is a listener "Monitor Rolling Flat File Trace Listener"
            LogWriter.Write(message, "Monitor");
        }



        protected override void OnError(DomainServiceErrorInfo errorInfo)
        {
            //Do log and error handling
            //Handle the exception with Enterprise Library (configured in Web.Config)
            //Search for this in the Web.config file <exceptionPolicies> tag

            this.ExceptionManager.HandleException(errorInfo.Error, "AllExceptionsPolicy");

            base.OnError(errorInfo);
        }

        


    }
}