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
using System.Collections.Generic;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    public class OfficeListFilters : ViewModel
    {
        #region Private Fields
                       
        private string _officeName;
        private string _officeCode;
        private bool _active;
        private bool _inactive;
        private bool _isRegionalOffice;
        
        #endregion

        #region Properties
        /// <summary>
        /// Set/Get the Office Name to Search
        /// </summary>		
        public string OfficeName
        {
            get
            {
                return _officeName;
            }
            set
            {
                if (_officeName == value) return;
                _officeName = value;
                OnPropertyChanged("OfficeName");
            }
        }

        /// <summary>
        /// Set/Get the Office Code to Search
        /// </summary>		
        public string OfficeCode
        {
            get
            {
                return _officeCode;
            }
            set
            {
                if (_officeCode == value) return;
                _officeCode = value;
                OnPropertyChanged("OfficeCode");
            }
        }

        /// <summary>
        /// Set/Get the serach filter for Active Offices
        /// </summary>		
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active == value) return;
                _active = value;
                OnPropertyChanged("Active");
            }
        }

        /// <summary>
        /// Set/Get the serach filter for Inactive Offices
        /// </summary>		
        public bool Inactive
        {
            get
            {
                return _inactive;
            }
            set
            {
                if (_inactive == value) return;
                _inactive = value;
                OnPropertyChanged("Inactive");
            }
        }

        /// <summary>
        /// Gets or sets IsRegionalOffice
        /// </summary>		
        public bool IsRegionalOffice
        {
            get
            {
                return _isRegionalOffice;
            }
            set
            {
                if (_isRegionalOffice == value) return;
                _isRegionalOffice = value;
                OnPropertyChanged("IsRegionalOffice");
            }
        }

        #endregion

        #region Constructor
        public OfficeListFilters()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The system sets default values in all filters
        /// </summary>
        public void SetDefaultValues()
        {
            OfficeName = string.Empty;
            OfficeCode = string.Empty;
            Active = false;
            Inactive = false;
            IsRegionalOffice = false;
        }

        #endregion
    }
}
