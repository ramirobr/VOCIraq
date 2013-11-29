using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    public class UserListFilters : ViewModel
    {
        #region Private Fields
        
        private List<Office> _offices;
        private List<EntryPoint> _entryPoints;
        private List<DictionaryEnum> _roles;
        private string _firstName;
        private string _lastName;
        private int? _officeIdSelected;
        private int? _entryPointIdSelected;
        private int? _roleIdSelected;
        private bool _isOfficeVisible;
        private bool _isEntryPointVisible;

        #endregion

        #region Properties
        /// <summary>
        /// Set/Get the user firstName to Search
        /// </summary>
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }


        /// <summary>
        /// Set/Get the user lastName to Search
        /// </summary>
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged("LastName");
                }
            }
        }

        
        /// <summary>
        /// Set/Get the user office to Search
        /// </summary>
        public int? OfficeIdSelected
        {
            get
            {
                return _officeIdSelected;
            }
            set
            {
                if (_officeIdSelected != value)
                {
                    _officeIdSelected = value;
                    OnPropertyChanged("OfficeIdSelected");
                }
            }
        }


        /// <summary>
        /// Set/Get the user EntryPoint to Search
        /// </summary>
        public int? EntryPointIdSelected
        {
            get
            {
                return _entryPointIdSelected;
            }
            set
            {
                if (_entryPointIdSelected != value)
                {
                    _entryPointIdSelected = value;
                    OnPropertyChanged("EntryPointIdSelected");
                }
            }
        }
        
        /// <summary>
        /// Set/Get the user Role to Search
        /// </summary>
        public int? RoleIdSelected
        {
            get
            {
                return _roleIdSelected;
            }
            set
            {
                if (_roleIdSelected != value)
                {
                    _roleIdSelected = value;
                    OnPropertyChanged("RoleIdSelected");
                }
            }
        }



        /// <summary>
        /// Set/Get the office List
        /// </summary>
        public List<Office> Offices
        {
            get
            {
                return _offices;
            }
            set
            {
                if (_offices != value)
                {
                    _offices = value;
                    OnPropertyChanged("Offices");
                }
            }
        }

        
        /// <summary>
        /// Set/Get the entryPoint List
        /// </summary>
        public List<EntryPoint> EntryPoints
        {
            get
            {
                return _entryPoints;
            }
            set
            {
                if (_entryPoints != value)
                {
                    _entryPoints = value;
                    OnPropertyChanged("EntryPoints");
                }
            }
        }

        
        /// <summary>
        /// Set/Get the roles List
        /// </summary>
        public List<DictionaryEnum> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                if (_roles != value)
                {
                    _roles = value;
                    OnPropertyChanged("Roles");
                }
            }
        }


        /// <summary>
        /// Set/Get the office filter and columns visibility
        /// </summary>
        public bool IsOfficeVisible
        {
            get
            {
                return _isOfficeVisible;
            }
            set
            {
                if (_isOfficeVisible != value)
                {
                    _isOfficeVisible = value;
                    OnPropertyChanged("IsOfficeVisible");
                }
            }
        }


        /// <summary>
        /// Set/Get the entry point filter and columns visibility
        /// </summary>

        public bool IsEntryPointVisible
        {
            get
            {
                return _isEntryPointVisible;
            }
            set
            {
                if (_isEntryPointVisible != value)
                {
                    _isEntryPointVisible = value;
                    OnPropertyChanged("IsEntryPointVisible");
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Get an instance CerticateListFilters and initialize the filters
        /// </summary>
        public UserListFilters()
        {
            Offices = new List<Office>();
            EntryPoints = new List<EntryPoint>();
            Roles = new List<DictionaryEnum>();
           
            InitialzeData();
            SetDefaultValues();
            SetDefaultView();
        }
        #endregion

        #region Private Methods
            /// <summary>
        /// Initialze the data on the filters
        /// </summary>
        private void InitialzeData()
        {
            //get entry points list
            EntryPoints = StaticReferences.GetEntryPoints().ToList();
            //get officess list
            Offices = StaticReferences.GetOffices().ToList();
            //retrieve the list of roles
            Enum.GetValues(typeof(UserRoleEnum)).Cast<UserRoleEnum>().OrderBy(x=> x.ToString()).ToList().ForEach(data =>
            {
                Roles.Add(new DictionaryEnum
                {
                    DictionaryEnumId = (int)data,
                    DictionaryEnumName = data.ToString()
                });
            });
            if(!EntryPoints.Any(x=> x.EntryPointId == 0))
            EntryPoints.Insert(0, new EntryPoint { EntryPointId = 0, Name = Strings.All });
            if (!Offices.Any(x => x.OfficeId == 0))
            Offices.Insert(0, new Office { OfficeId = 0, OfficeName = Strings.All });
            if (!Roles.Any(x => x.DictionaryEnumId == 0))
            Roles.Insert(0, new DictionaryEnum { DictionaryEnumId = 0, DictionaryEnumName = Strings.All });

        }       
        #endregion

        #region Public Methods
         /// <summary>
        /// The system sets default values in all filters
        /// </summary>
        public void SetDefaultValues()
        {
            EntryPointIdSelected = 0;
            OfficeIdSelected = App.CurrentUser.IsInRole(UserRoleEnum.Admin)?App.CurrentUser.OfficeId:0;
            RoleIdSelected = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        /// <summary>
        /// The system sets the default view regarding the role of the user
        /// </summary>
        public void SetDefaultView()
        {
            //Default view of super admin
            if (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                IsOfficeVisible = true;
                IsEntryPointVisible = true;
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Client));
            }
            //Default view of local admin
            else if (App.CurrentUser.IsInRole(UserRoleEnum.Admin))
            {
                IsOfficeVisible = false;
                IsEntryPointVisible = false;
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Client));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.BorderAgent));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.LOAdmin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.SuperAdmin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Supervisor));
            }
            //Default view of LO admin
            else if (App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin))
            {
                IsOfficeVisible = false;
                IsEntryPointVisible = true;
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Client));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.SuperAdmin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Admin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Coordinator));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Issuer));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Supervisor));

            }

        }
        #endregion
    }
}
