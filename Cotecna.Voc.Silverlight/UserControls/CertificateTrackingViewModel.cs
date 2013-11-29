using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Web.Services;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;

namespace Cotecna.Voc.Silverlight
{
    public class CertificateTrackingViewModel : ViewModel
    {
        #region events
        /// <summary>
        /// Close the window
        /// </summary>
        public event EventHandler CloseWindow;
        #endregion

        #region fields
        private ObservableCollection<CertificateTracking> _trackingList;
        private ICommand _closeCommand;
        private VocContext _context = new VocContext();
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets TrackingList
        /// </summary>		
        public ObservableCollection<CertificateTracking> TrackingList
        {
            get
            {
                return _trackingList;
            }
            set
            {
                if (_trackingList == value) return;
                _trackingList = value;
                OnPropertyChanged("TrackingList");
            }
        }

        
        /// <summary>
        /// Gets CloseCommand
        /// </summary>
        public ICommand CloseCommand
        {
            get 
            {
                if (_closeCommand == null)
                    _closeCommand = new DelegateCommand(ExecuteCloseCommand);
                return _closeCommand; 
            }
            
        }
        #endregion

        #region constructor
        /// <summary>
        /// Initialize data
        /// </summary>
        /// <param name="certificateId">Id of certificate</param>
        public CertificateTrackingViewModel(int certificateId)
        {
            IsBusy = true;
            EntityQuery<CertificateTracking> query = _context.GetTrackingListByCertificateIdQuery(certificateId);
            _context.Load(query, CompletedGetTrackingListByCertificateIdQuery, null);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Execute Close command
        /// </summary>
        private void ExecuteCloseCommand()
        {
            if (CloseWindow != null)
                CloseWindow(this, EventArgs.Empty);
        }

        /// <summary>
        /// Callback method for GetTrackingListByCertificateIdQuery
        /// </summary>
        /// <param name="operation">Load operation</param>
        private void CompletedGetTrackingListByCertificateIdQuery(LoadOperation<CertificateTracking> operation)
        {
            HandleLoadOperation(operation, () => 
            {
                TrackingList = new ObservableCollection<CertificateTracking>(operation.Entities);
                IsBusy = false;
            });
        }
        #endregion

    }
}
