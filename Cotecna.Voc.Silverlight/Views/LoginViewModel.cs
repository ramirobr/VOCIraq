using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;
using System;
using System.Linq;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cotecna.Voc.Silverlight
{
    public class LoginViewModel: ViewModel
    {
        public event EventHandler ChangeScreen;

        private string _domainUserName;
        private AuthenticationDomainContext _authenticationContext = new AuthenticationDomainContext();

        /// <summary>
        /// Get or set DomainUserName
        /// </summary>		
        public string DomainUserName
        {
            get
            {
                return _domainUserName;
            }
            set
            {
                if (_domainUserName == value) return;
                _domainUserName = value;
                OnPropertyChanged("DomainUserName");
            }
        }

        public void AuthenticateInSystem()
        {
            IsBusy = true;
            _authenticationContext.GetWindowsUser(GetWindowsUserCompleted, null);
        }

        private void GetWindowsUserCompleted(InvokeOperation<string> operation)
        {
            DomainUserName = operation.Value;
            _authenticationContext.PerformAuthentication(DomainUserName, PerformAuthenticationCompleted, null);
        }

        private void PerformAuthenticationCompleted(InvokeOperation<VocUser> operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                IsBusy = false;
                VocUser user = operation.Value;
                if (user != null)
                {
                    if (user.IsCoordinatorOrSuperior || user.IsBorderAgentOrSuperior)
                    {
                        //Get the list of entry points
                        StaticReferences.GetEntryPoints(() =>
                        {
                            App.CurrentUser = operation.Value;
                            if (ChangeScreen != null)
                                ChangeScreen(this, new EventArgs());
                        });
                    }
                    else
                    {
                        MessageBox.Show(Strings.NonAuthorizedMessage);
                    }
                }
                else
                {
                    MessageBox.Show(Strings.NonAuthorizedMessage);
                }
            });   
        }
    }
}
