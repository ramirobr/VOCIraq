using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cotecna.Voc.Silverlight
{
    public class CertificateListFilters: ViewModel
    {
        #region private fields

        private string _certificateNumber;
        private DateTime? _issuanceDateFrom;
        private DateTime? _issuanceDateTo;
        private ObservableCollection<EntryPoint> _entryPointList;
        private int _selectedEntryPointId;
        private ObservableCollection<Office> _officeList;
        private int _selectedOffice;
        private bool _isOfficeListVisible;
        private bool _published;
        private bool _unpublished;
        private bool _myDocuments;        
        private bool _conform;
        private bool _nonConform;
        private bool _cancelled;
        private bool _created;
        private bool _requested;
        private bool _approved;
        private bool _rejected;
        private bool _ongoing;
        private bool _closed;
        private bool _invoiced;
        private bool _ispublishVisible;
        private bool _isUnPublishVisible;
        private bool _isMyDocumentsVisible;
        private bool _isBorderAgent;
        private bool _isNotBorderAgent;
        private bool _nonInvoiced;
        private string _comdivNumber;

        #endregion

        #region properties

        /// <summary>
        /// Gets or Set CertificateNumber
        /// </summary>		
        public string CertificateNumber
        {
            get
            {
                return _certificateNumber;
            }
            set
            {
                if (_certificateNumber == value) return;
                _certificateNumber = value;
                OnPropertyChanged("CertificateNumber");
            }
        }

        
        /// <summary>
        /// Gets or Set IssuanceDateFrom
        /// </summary>		
        public DateTime? IssuanceDateFrom
        {
            get
            {
                return _issuanceDateFrom;
            }
            set
            {
                if (_issuanceDateFrom == value) return;
                _issuanceDateFrom = value;
                OnPropertyChanged("IssuanceDateFrom");
            }
        }

        /// <summary>
        /// Gets or sets IssuanceDateTo
        /// </summary>		
        public DateTime? IssuanceDateTo
        {
            get
            {
                return _issuanceDateTo;
            }
            set
            {
                if (_issuanceDateTo == value) return;
                _issuanceDateTo = value;
                OnPropertyChanged("IssuanceDateTo");
            }
        }
        
        /// <summary>
        /// Gets or sets EntryPointList
        /// </summary>		
        public ObservableCollection<EntryPoint> EntryPointList
        {
            get
            {
                return _entryPointList;
            }
            set
            {
                if (_entryPointList == value) return;
                _entryPointList = value;
                OnPropertyChanged("EntryPointList");
            }
        }

        
        /// <summary>
        /// Gets or sets SelectedEntryPointId
        /// </summary>		
        public int SelectedEntryPointId
        {
            get
            {
                return _selectedEntryPointId;
            }
            set
            {
                if (_selectedEntryPointId == value) return;
                _selectedEntryPointId = value;
                OnPropertyChanged("SelectedEntryPointId");
            }
        }
        
        /// <summary>
        /// Gets or sets OfficeList
        /// </summary>		
        public ObservableCollection<Office> OfficeList
        {
            get
            {
                return _officeList;
            }
            set
            {
                if (_officeList == value) return;
                _officeList = value;
                OnPropertyChanged("OfficeList");
            }
        }

        
        /// <summary>
        /// Gets or sets SelectedOffice
        /// </summary>		
        public int SelectedOffice
        {
            get
            {
                return _selectedOffice;
            }
            set
            {
                if (_selectedOffice == value) return;
                _selectedOffice = value;
                OnPropertyChanged("SelectedOffice");
            }
        }

        
        /// <summary>
        /// Gets or sets IsOfficeListVisible
        /// </summary>		
        public bool IsOfficeListVisible
        {
            get
            {
                return _isOfficeListVisible;
            }
            set
            {
                if (_isOfficeListVisible == value) return;
                _isOfficeListVisible = value;
                OnPropertyChanged("IsOfficeListVisible");
            }
        }

        /// <summary>
        /// Gets or sets Published
        /// </summary>		
        public bool Published
        {
            get
            {
                return _published;
            }
            set
            {
                if (_published == value) return;
                _published = value;
                OnPropertyChanged("Published");
            }
        }

        
        /// <summary>
        /// Gets o sets Unpublished
        /// </summary>		
        public bool Unpublished
        {
            get
            {
                return _unpublished;
            }
            set
            {
                if (_unpublished == value) return;
                _unpublished = value;
                OnPropertyChanged("Unpublished");
            }
        }

        
        /// <summary>
        /// Gets or sets MyDocuments
        /// </summary>		
        public bool MyDocuments
        {
            get
            {
                return _myDocuments;
            }
            set
            {
                if (_myDocuments == value) return;
                _myDocuments = value;
                OnPropertyChanged("MyDocuments");
            }
        }

        

        /// <summary>
        /// Gets or sets Conform
        /// </summary>		
        public bool Conform
        {
            get
            {
                return _conform;
            }
            set
            {
                if (_conform == value) return;
                _conform = value;
                OnPropertyChanged("Conform");
            }
        }

        
        /// <summary>
        /// Gets or sets NonConform
        /// </summary>		
        public bool NonConform
        {
            get
            {
                return _nonConform;
            }
            set
            {
                if (_nonConform == value) return;
                _nonConform = value;
                OnPropertyChanged("NonConform");
            }
        }

        /// <summary>
        /// Gets or sets Cancelled
        /// </summary>		
        public bool Cancelled
        {
            get
            {
                return _cancelled;
            }
            set
            {
                if (_cancelled == value) return;
                _cancelled = value;
                OnPropertyChanged("Cancelled");
            }
        }

        /// <summary>
        /// Gets or sets Created
        /// </summary>		
        public bool Created
        {
            get
            {
                return _created;
            }
            set
            {
                if (_created == value) return;
                _created = value;
                OnPropertyChanged("Created");
            }
        }

        
        /// <summary>
        /// Gets or sets Requested
        /// </summary>		
        public bool Requested
        {
            get
            {
                return _requested;
            }
            set
            {
                if (_requested == value) return;
                _requested = value;
                OnPropertyChanged("Requested");
            }
        }

        /// <summary>
        /// Gets or sets Approved
        /// </summary>		
        public bool Approved
        {
            get
            {
                return _approved;
            }
            set
            {
                if (_approved == value) return;
                _approved = value;
                OnPropertyChanged("Approved");
            }
        }
        
        /// <summary>
        /// Gets or sets Rejected
        /// </summary>		
        public bool Rejected
        {
            get
            {
                return _rejected;
            }
            set
            {
                if (_rejected == value) return;
                _rejected = value;
                OnPropertyChanged("Rejected");
            }
        }
        
        /// <summary>
        /// Gets or sets Ongoing
        /// </summary>		
        public bool Ongoing
        {
            get
            {
                return _ongoing;
            }
            set
            {
                if (_ongoing == value) return;
                _ongoing = value;
                OnPropertyChanged("Ongoing");
            }
        }

        /// <summary>
        /// Gets or sets Closed
        /// </summary>		
        public bool Closed
        {
            get
            {
                return _closed;
            }
            set
            {
                if (_closed == value) return;
                _closed = value;
                OnPropertyChanged("Closed");
            }
        }


        /// <summary>
        /// Gets or sets Invoiced
        /// </summary>		
        public bool Invoiced
        {
            get
            {
                return _invoiced;
            }
            set
            {
                if (_invoiced == value) return;
                _invoiced = value;
                OnPropertyChanged("Invoiced");
            }
        }

        /// <summary>
        /// Gets or sets IsPublishVisible
        /// </summary>		
        public bool IsPublishVisible
        {
            get
            {
                return _ispublishVisible;
            }
            set
            {
                if (_ispublishVisible == value) return;
                _ispublishVisible = value;
                OnPropertyChanged("IsPublishVisible");
            }
        }

        
        /// <summary>
        /// Gets or sets 
        /// </summary>		
        public bool IsUnPublishVisible
        {
            get
            {
                return _isUnPublishVisible;
            }
            set
            {
                if (_isUnPublishVisible == value) return;
                _isUnPublishVisible = value;
                OnPropertyChanged("IsUnPublishVisible");
            }
        }

        /// <summary>
        /// Gets or sets IsMyDocumentsVisible
        /// </summary>		
        public bool IsMyDocumentsVisible
        {
            get
            {
                return _isMyDocumentsVisible;
            }
            set
            {
                if (_isMyDocumentsVisible == value) return;
                _isMyDocumentsVisible = value;
                OnPropertyChanged("IsMyDocumentsVisible");
            }
        }

        /// <summary>
        /// Gets or sets IsBorderAgent
        /// </summary>		
        public bool IsBorderAgent
        {
            get
            {
                return _isBorderAgent;
            }
            set
            {
                if (_isBorderAgent == value) return;
                _isBorderAgent = value;
                OnPropertyChanged("IsBorderAgent");
            }
        }

        /// <summary>
        /// Gets or sets IsNotBorderAgent
        /// </summary>		
        public bool IsNotBorderAgent
        {
            get
            {
                return _isNotBorderAgent;
            }
            set
            {
                if (_isNotBorderAgent == value) return;
                _isNotBorderAgent = value;
                OnPropertyChanged("IsNotBorderAgent");
            }
        }

        /// <summary>
        /// Gets or sets NonInvoiced
        /// </summary>		
        public bool NonInvoiced
        {
            get
            {
                return _nonInvoiced;
            }
            set
            {
                if (_nonInvoiced == value) return;
                _nonInvoiced = value;
                OnPropertyChanged("NonInvoiced");
            }
        }

        /// <summary>
        /// Gets or sets ComdivNumber
        /// </summary>		
        public string ComdivNumber
        {
            get
            {
                return _comdivNumber;
            }
            set
            {
                if (_comdivNumber == value) return;
                _comdivNumber = value;
                OnPropertyChanged("ComdivNumber");
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Get an instance CerticateListFilters and initialize the filters
        /// </summary>
        public CertificateListFilters()
        {
            EntryPointList = new ObservableCollection<EntryPoint>();
            OfficeList = new ObservableCollection<Office>();

            InitialzeData();
            SetDefaultValues();
            SetDefaultView();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Initialze the data on the filters
        /// </summary>
        private void InitialzeData()
        {
            //get entry points list
            foreach (var entry in StaticReferences.GetEntryPoints())
            {
                EntryPointList.Add(entry);
            }
            //get officess list
            foreach (var office in StaticReferences.GetOffices())
            {
                OfficeList.Add(office);
            }
            
            EntryPointList.Insert(0, new EntryPoint { EntryPointId = 0, Name = Strings.All });
            OfficeList.Insert(0, new Office { OfficeId = 0, OfficeName = Strings.All });
        }
        #endregion

        #region public methods
        /// <summary>
        /// The system sets default values in all filters
        /// </summary>
        public void SetDefaultValues()
        {
            SelectedEntryPointId = 0;
            SelectedOffice = 0;
            ComdivNumber = string.Empty;
            CertificateNumber = string.Empty;
            IssuanceDateFrom = null;
            IssuanceDateTo = null;
            Published = false;
            Unpublished = false;
            MyDocuments = false;
            Conform = false;
            NonConform = false;
            Cancelled = false;
            Created = false;
            Requested = false;
            Approved = false;
            Rejected = false;
            Ongoing = false;
            Closed = false;
            IsOfficeListVisible = false;
            IsPublishVisible = false;
            IsUnPublishVisible = false;
            IsMyDocumentsVisible = false;
            IsBorderAgent = false;
            IsNotBorderAgent = false;
            Invoiced = false;
            NonInvoiced = false;
        }

        /// <summary>
        /// The system sets the default view regarding the role of the user
        /// </summary>
        public void SetDefaultView()
        {
            //Coordinator's default view
            if (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator))
            {
                IsOfficeListVisible = App.CurrentUser.IsRoUser;
                if (IsOfficeListVisible)
                {
                    OfficeList = null;
                    var localOffices = StaticReferences.GetOffices().Where(x => x.RegionalOfficeId == App.CurrentUser.OfficeId).ToObservableCollection();
                    var roOffice = StaticReferences.GetOffices().FirstOrDefault(x => x.OfficeId == App.CurrentUser.OfficeId);
                    localOffices.Add(roOffice);
                    OfficeList = new ObservableCollection<Office>(localOffices.OrderBy(x => x.OfficeName));
                    OfficeList.Insert(0, new Office { OfficeId = 0, OfficeName = Strings.All });
                }
                SelectedOffice = App.CurrentUser.OfficeId;
                Unpublished = true;
                IsPublishVisible = true;
                IsUnPublishVisible = true;
                IsMyDocumentsVisible = true;
                IsNotBorderAgent = true;
            }
            //Default view of issuer and local admin
            else if (App.CurrentUser.IsInRole(UserRoleEnum.Issuer) || App.CurrentUser.IsInRole(UserRoleEnum.Admin))
            {
                IsOfficeListVisible = App.CurrentUser.IsRoUser;
                if (IsOfficeListVisible)
                {
                    OfficeList = null;
                    var localOffices = StaticReferences.GetOffices().Where(x => x.RegionalOfficeId == App.CurrentUser.OfficeId).ToObservableCollection();
                    var roOffice = StaticReferences.GetOffices().FirstOrDefault(x => x.OfficeId == App.CurrentUser.OfficeId);
                    localOffices.Add(roOffice);
                    OfficeList = new ObservableCollection<Office>(localOffices.OrderBy(x => x.OfficeName));
                    OfficeList.Insert(0, new Office { OfficeId = 0, OfficeName = Strings.All });
                }
                SelectedOffice = App.CurrentUser.OfficeId;
                Requested = true;
                IsPublishVisible = true;
                IsUnPublishVisible = true;
                IsMyDocumentsVisible = true;
                IsNotBorderAgent = true;
            }
            //Default view of super admin
            else if (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                Published = true;
                IsOfficeListVisible = true;
                IsPublishVisible = true;
                IsUnPublishVisible = true;
                IsMyDocumentsVisible = true;
                IsNotBorderAgent = true;
            }
            //Default view of border agent 
            else if (App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent) || App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin))
            {
                Published = true;
                Approved = true;
                Ongoing = true;
                Conform = true;
                IsBorderAgent = true;
                SelectedEntryPointId = App.CurrentUser.EntryPointId.GetValueOrDefault();
            }
        }


        #endregion

    }
}
