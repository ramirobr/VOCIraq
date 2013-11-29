using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System;
using System.Collections.ObjectModel;

namespace Cotecna.Voc.Silverlight
{
    public class SecurityPaperFilters: ViewModel
    {
        #region private fields
        private string _number;
        private ObservableCollection<EntryPoint> _entryPointList;
        private DateTime? _issuanceDateFrom;
        private DateTime? _issuanceDateTo;
        private int _selectedEntryPointId;
        private bool _misPrinted;
        private bool _cancelled;
        private bool _notIssued;
        private bool _issued;
        #endregion

        #region properties

        /// <summary>
        /// Gets or sets Number
        /// </summary>		
        public string Number
        {
            get
            {
                return _number;
            }
            set
            {
                if (_number == value) return;
                _number = value;
                OnPropertyChanged("Number");
            }
        }

        /// <summary>
        /// List of entry points
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
        /// Gets or sets IssuanceDate
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
        /// Gets or sets IssuanceDateFrom
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
        /// Gets or sets Issued
        /// </summary>		
        public bool Issued
        {
            get
            {
                return _issued;
            }
            set
            {
                if (_issued == value) return;
                _issued = value;
                OnPropertyChanged("Issued");
            }
        }

        /// <summary>
        /// Gets or sets NotIssued
        /// </summary>		
        public bool NotIssued
        {
            get
            {
                return _notIssued;
            }
            set
            {
                if (_notIssued == value) return;
                _notIssued = value;
                OnPropertyChanged("NotIssued");
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
        /// Gets or sets MisPrinted
        /// </summary>		
        public bool MisPrinted
        {
            get
            {
                return _misPrinted;
            }
            set
            {
                if (_misPrinted == value) return;
                _misPrinted = value;
                OnPropertyChanged("MisPrinted");
            }
        }
        #endregion

        #region contructor
        /// <summary>
        /// Get an instance of SecurityPaperFilters and initialize all
        /// </summary>
        public SecurityPaperFilters()
        {
            EntryPointList = new ObservableCollection<EntryPoint>(StaticReferences.GetEntryPoints());
            EntryPointList.Insert(0, new EntryPoint { EntryPointId = 0, Name = Strings.All });
            if (App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin))
                SelectedEntryPointId = App.CurrentUser.EntryPointId.GetValueOrDefault();
        }
        #endregion

        #region public methods
        /// <summary>
        /// Reset all filters
        /// </summary>
        public void ResetFilters()
        {
            if (App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin))
                SelectedEntryPointId = App.CurrentUser.EntryPointId.GetValueOrDefault();
            else
                SelectedEntryPointId = 0;

            IssuanceDateFrom = null;
            IssuanceDateTo = null;
            Issued = false;
            NotIssued = false;
            Cancelled = false;
            MisPrinted = false;

            Number = string.Empty;
        }
        #endregion
    }
}
