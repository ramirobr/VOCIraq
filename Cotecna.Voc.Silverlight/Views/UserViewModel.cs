using System;
using System.Linq;
using System.Collections.Generic;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// User view model what manage user information
    /// </summary>
    public class UserViewModel : ViewModel
    {
        #region Private Fields
        private AuthenticationDomainContext _context=new AuthenticationDomainContext();
        string _label;
        private List<Office> _offices;
        private List<EntryPoint> _entryPoints;
        private List<DictionaryEnum> _roles;
        private bool _isOfficeVisible;
        private bool _isEntryPointVisible;
        private int? _roleIdSelected;
        private UserProfile _user;
        private bool _isSaveVisible;
        private bool _isFilePathVisible;
        private bool _isSignatureVisible;
        private string _userHeaderText;
        private string _userInstructionsText;
        private bool _isEnabled;
        #endregion

        #region Properties
        /// <summary>
        /// Gets text to be displayed as header of tab item
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { 
                _label = value;
                OnPropertyChanged("Label");
                }
        }

        /// <summary>
        /// Set/Get the user office
        /// </summary>
        public int? OfficeIdSelected
        {
            get
            {
                return _user==null?0:(_user.OfficeId??0);
            }
            set
            {
                _user.OfficeId = (value == 0) ? null : value;
                OnPropertyChanged("OfficeIdSelected");
            }
        }


        /// <summary>
        /// Set/Get the user EntryPoint 
        /// </summary>
        public int? EntryPointIdSelected
        {
            get
            {
                return _user == null ? 0 : (_user.EntryPointId ?? 0);
            }
            set
            {
                _user.EntryPointId = (value == 0) ? null : value;
                OnPropertyChanged("EntryPointIdSelected");
            }
        }



        /// <summary>
        /// Set/Get the user Role
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
                    SetVisibility(true);
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
        /// Set/Get the office visibility
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
        /// Set/Get the signature visibility
        /// </summary>
        public bool IsSignatureVisible
        {
            get
            {
                return _isSignatureVisible;
            }
            set
            {
                if (_isSignatureVisible != value)
                {
                    _isSignatureVisible = value;
                    OnPropertyChanged("IsSignatureVisible");
                }
            }
        }


        /// <summary>
        /// Set/Get the entry point visibility
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

        /// <summary>
        /// Set/Get the save button visibility
        /// </summary>
        public bool IsSaveVisible
        {
            get
            {
                return _isSaveVisible;
            }
            set
            {
                if (_isSaveVisible != value)
                {
                    _isSaveVisible = value;
                    OnPropertyChanged("IsSaveVisible");
                }
            }
        }

        

        public bool IsFilePathVisible
        {
            get
            {
                return _isFilePathVisible;
            }
            set
            {
                if (_isFilePathVisible != value)
                {
                    _isFilePathVisible = value;
                    OnPropertyChanged("IsFilePathVisible");
                }
            }
        }


        /// <summary>
        /// Gets or sets user to Edit
        /// </summary>
        public UserProfile User
        {
            get
            {
                return _user;
            }
            set
            {
                if (_user!=null)
                _user.PropertyChanged -= User_PropertyChanged;
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged("User");
                }
                _user.PropertyChanged += User_PropertyChanged;
            }
        }

        /// <summary>
        /// Get/Set user Name
        /// </summary>
        public string UserName
        {
            get
            {
                return _user == null ? null : _user.UserName;
            }
            set
            {
                if (_user.UserName != value)
                {
                    _user.UserName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        /// <summary>
        /// Get/Set user FilePath
        /// </summary>
        public string FilePath
        {
            get
            {
                return _user == null ? null : _user.FilePath;
            }
            set
            {
                if (_user.FilePath != value)
                {
                    _user.FilePath = value;
                    OnPropertyChanged("FilePath");
                }
            }
        }

        /// <summary>
        /// Get/Set user Signature
        /// </summary>
        public byte[] SignatureFile
        {
            get
            {
                return _user == null ? null : _user.SignatureFile;
            }
            set
            {
                if (_user.SignatureFile != value)
                {
                    _user.SignatureFile = value;
                    OnPropertyChanged("SignatureFile");
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>		
        public string Email
        {
            get
            {
                return _user == null ? null : _user.Email;
            }
            set
            {
                if (_user.Email != value)
                {
                    _user.Email = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        
        /// <summary>
        /// Gets / Sets the text of the header for User screen
        /// </summary>		
        public string UserHeaderText
        {
            get
            {
                return _userHeaderText;
            }
            set
            {
                if (_userHeaderText == value) return;
                _userHeaderText = value;
                OnPropertyChanged("UserHeaderText");
            }
        }

        
        /// <summary>
        /// Gets / Sets the text of the instructions section for User screen
        /// </summary>		
        public string UserInstructionsText
        {
            get
            {
                return _userInstructionsText;
            }
            set
            {
                if (_userInstructionsText == value) return;
                _userInstructionsText = value;
                OnPropertyChanged("UserInstructionsText");
            }
        }

        
        /// <summary>
        /// Gets / Sets IsEnabled to enable or disable the fields in the screen according to the logged user role
        /// </summary>		
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Stores the content of the mandatory field when getting the focus
        /// </summary>
        public string ContentFocused { get; set; }

        private bool _isRoCheckVisible;
        /// <summary>
        /// Get or set IsRoCheckVisible
        /// </summary>		
        public bool IsRoCheckVisible
        {
            get
            {
                return _isRoCheckVisible;
            }
            set
            {
                if (_isRoCheckVisible == value) return;
                _isRoCheckVisible = value;
                OnPropertyChanged("IsRoCheckVisible");
            }
        }

        #endregion

        #region Commands
        private ICommand _saveCommand;
        /// <summary>
        /// Gets save command
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new DelegateCommand(ExecuteSave);
                return _saveCommand;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event used to notify that save is completed
        /// </summary>
        internal event EventHandler OnSaveCompleted;


        #endregion

        #region Constructor
        public UserViewModel()
        {
            Offices = new List<Office>();
            EntryPoints = new List<EntryPoint>();
            Roles = new List<DictionaryEnum>();

            InitialzeData();
            SetDefaultView();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Insert/Update User 
        /// </summary>
        private void ExecuteSave()        
        {
            RemoveAllErrors();

            if (ValidateData())
                return;
            IsBusy = true;
            _context.UserAleradyExist(_user.UserId,UserName, OfficeIdSelected, EntryPointIdSelected, RoleIdSelected, UserAlreadyExistCompleted, null);            
        }

        /// <summary>
        /// Execute when a user was been validated
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void UserAlreadyExistCompleted(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                if ((bool)operation.Value == true)
                {
                    AlertDisplay(Strings.UserAlreadyExists);
                    return;
                }

                if (_user.EntityState == EntityState.Detached)
                {
                    _context.UserProfiles.Add(_user);
                    _user.UserInRoles.Add(new UserInRole { RoleId = _roleIdSelected.Value });
                }
                else
                    if (!_user.UserInRoles.Any(x => x.RoleId == _roleIdSelected))
                    {
                        foreach (var role in _user.UserInRoles)
                        {
                            _user.UserInRoles.Remove(role);
                            _context.UserInRoles.Remove(role);
                        }
                        _context.UserInRoles.Add(new UserInRole { UserId = _user.UserId, RoleId = _roleIdSelected.Value });
                    }
                _context.SubmitChanges(SaveCompleted, null);

            });
        }

        /// <summary>
        /// Execute when a user was saved
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void SaveCompleted(SubmitOperation operation)
        {
            HandleSubmitOperation(operation, () =>
            {
                GlobalAccessor.Instance.MessageStatus = Strings.UserUpdateStatusMessage;
                Initialize(_user.UserId, string.Concat(User.FirstName, " ", User.LastName));
                if (_user.UserName.ToLower() == App.CurrentUser.Name.ToLower())
                {
                    AlertDisplay(Strings.CurrentUserUpdateInformation);
                }
                if (OnSaveCompleted != null)
                    OnSaveCompleted(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// This method is executed when any property of user has been changed.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetVisibility(_user.HasChanges || _user.EntityState == EntityState.Detached);
        }


        /// <summary>
        /// Set Visibility of controls
        /// </summary>
        private void SetVisibility(bool isDirty)
        {
            IsSaveVisible = isDirty && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);

            IsOfficeVisible = (_roleIdSelected != (int)UserRoleEnum.BorderAgent && _roleIdSelected != (int)UserRoleEnum.LOAdmin && _roleIdSelected != (int)UserRoleEnum.Supervisor) && App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin);
            IsFilePathVisible = IsEntryPointVisible = _roleIdSelected == (int)UserRoleEnum.BorderAgent || _roleIdSelected == (int)UserRoleEnum.LOAdmin;
            IsSignatureVisible = _roleIdSelected != (int)UserRoleEnum.BorderAgent && _roleIdSelected != (int)UserRoleEnum.LOAdmin && _roleIdSelected != (int)UserRoleEnum.Supervisor;

            IsRoCheckVisible = _roleIdSelected != (int)UserRoleEnum.Supervisor &&
                                _roleIdSelected != (int)UserRoleEnum.SuperAdmin &&
                                _roleIdSelected != (int)UserRoleEnum.BorderAgent &&
                                _roleIdSelected != (int)UserRoleEnum.LOAdmin &&
                                _user.OfficeId != null &&
                                _offices.First(x => x.OfficeId == _user.OfficeId).OfficeType.GetValueOrDefault() 
                                == OfficeTypeEnum.RegionalOffice;

           if (isDirty)
           {
                //Reset field acooding visibility of them
                if (!IsOfficeVisible && App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin))
                    OfficeIdSelected = 0;
                if (!IsEntryPointVisible)
                    EntryPointIdSelected = 0;
                if (!IsFilePathVisible)
                    FilePath = string.Empty;
                if (!IsSignatureVisible)
                    SignatureFile = null;
            }
            
        }

        /// <summary>
        /// Initialze the data on the user screen
        /// </summary>
        private void InitialzeData()
        {
            //get entry points list
            EntryPoints = StaticReferences.GetEntryPoints().ToList();
            //get officess list
            Offices = StaticReferences.GetOffices().ToList();
            //retrieve the list of roles
            Enum.GetValues(typeof(UserRoleEnum)).Cast<UserRoleEnum>().OrderBy(x => x.ToString()).ToList().ForEach(data =>
            {
                Roles.Add(new DictionaryEnum
                {
                    DictionaryEnumId = (int)data,
                    DictionaryEnumName = data.ToString()
                });
            });

            if (!EntryPoints.Any(x => x.EntryPointId == 0))
                EntryPoints.Insert(0, new EntryPoint { EntryPointId = 0, Name = String.Empty });
            if (!Offices.Any(x => x.OfficeId == 0))
                Offices.Insert(0, new Office { OfficeId = 0, OfficeName = String.Empty });

            if (!App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                UserHeaderText = Strings.EditUser;
                UserInstructionsText = Strings.EditUserInstructions;
                IsEnabled = true;
            }
            else
            {
                UserHeaderText = Strings.ViewUser;
                UserInstructionsText = Strings.ViewUserInstructions;
                IsEnabled = false;
            }
        }



        /// <summary>
        /// Execute when a user was loaded
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void LoadUserCompleted(LoadOperation<UserProfile> operation)
        {
            HandleLoadOperation(operation, () =>
            {
                SetUser(operation.Entities.FirstOrDefault());
                IsBusy = false;
            });
        }

        /// <summary>
        /// Set User Entity
        /// </summary>
        /// <param name="user"></param>
        private void SetUser(UserProfile user)
        {
            
            User = user;
            _roleIdSelected = User.UserInRoles.Select(x=> x.RoleId).FirstOrDefault();
            OnPropertyChanged("RoleIdSelected");
            OnPropertyChanged("OfficeIdSelected");
            OnPropertyChanged("EntryPointIdSelected");
            OnPropertyChanged("UserName");            
            OnPropertyChanged("SignatureFile");
            OnPropertyChanged("FilePath");
            OnPropertyChanged("Email");
            SetVisibility(false);
        
        }

        /// <summary>
        /// The system sets the default view regarding the role of the user
        /// </summary>
        public void SetDefaultView()
        {
            IsSignatureVisible = false;
            IsOfficeVisible = false;
            IsEntryPointVisible = false;
            IsFilePathVisible = false;
            IsRoCheckVisible = false;
            //Default view of super admin
            if (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Client));
            }
            //Default view of local admin
            else if (App.CurrentUser.IsInRole(UserRoleEnum.Admin))
            {                
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Client));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.BorderAgent));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.LOAdmin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.SuperAdmin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Supervisor));
            }
            //Default view of LO admin
            if (App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin))
            {
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Client));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.SuperAdmin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Admin));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Coordinator));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Issuer));
                Roles.Remove(Roles.FirstOrDefault(x => x.DictionaryEnumId == (int)UserRoleEnum.Supervisor));
            }

        }

        /// <summary>
        /// Validate Mandatory fields
        /// </summary>
        /// <returns></returns>
        private bool ValidateData()
        {
            ValidateUserName();

            ValidateEmail();

            if (_roleIdSelected==null || _roleIdSelected==0)
                AddError("RoleIdSelected", Strings.RoleIsMandatory);
            else
                RemoveError("RoleIdSelected", Strings.RoleIsMandatory);


            if ((_roleIdSelected == (int)UserRoleEnum.Coordinator || _roleIdSelected == (int)UserRoleEnum.Issuer || 
                _roleIdSelected == (int)UserRoleEnum.Admin || _roleIdSelected == (int)UserRoleEnum.SuperAdmin) 
                && (OfficeIdSelected==null || OfficeIdSelected==0))
                AddError("OfficeIdSelected", Strings.OfficeIsMandatoryForRoles);
            else
                RemoveError("OfficeIdSelected", Strings.OfficeIsMandatoryForRoles);

            if ((_roleIdSelected == (int)UserRoleEnum.BorderAgent || _roleIdSelected == (int)UserRoleEnum.LOAdmin)
                && (EntryPointIdSelected == null || EntryPointIdSelected == 0))
                AddError("EntryPointIdSelected", Strings.EntryPointIsMandatoryForRoles);
            else
                RemoveError("EntryPointIdSelected", Strings.EntryPointIsMandatoryForRoles);

            if ((_roleIdSelected == (int)UserRoleEnum.BorderAgent || _roleIdSelected == (int)UserRoleEnum.LOAdmin)
                && string.IsNullOrEmpty(FilePath))
                AddError("FilePath", Strings.FilePathIsMandatoryForRoles);
            else
                RemoveError("FilePath", Strings.FilePathIsMandatoryForRoles);


            return HasErrors;

        }

        /// <summary>
        /// Verifies if the UserName and Email mandatoryfields are empty
        /// </summary>
        /// <returns></returns>
        private bool MandatoryFieldsEmpty()
        {
            if (string.IsNullOrEmpty(User.UserName) && string.IsNullOrEmpty(User.Email))
            {
                return true;
            }
            
            return false;
        }


        #endregion

        #region Public Methods
        /// <summary>
        /// Set the Signature to user
        /// </summary>
        /// <param name="signatureArray"></param>
        public void UploadSignature(byte[] signatureArray)
        {
            SignatureFile = signatureArray;        
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewModel"/> class. Constructor of ViewModel Screen with one argument
        /// </summary>
        /// <param name="userId">Identity of User to edit</param>
        /// <param name="label">Label to display in tab control</param>
        public void Initialize(int userId, string label)
        {
            Label = label;
            IsBusy = true;
            _context.Load(_context.GetUserByUserIdQuery(userId), LoadBehavior.RefreshCurrent, LoadUserCompleted, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewModel"/> class. Constructor of ViewModel Screen with one argument
        /// </summary>
        /// <param name="user">User to edit</param>
        public void Initialize(UserProfile user)
        {
            Label = "New User";
            IsBusy = true;
            SetUser(user);
            IsBusy = false;
        }

        /// <summary>
        /// Seach the information of the user in the active directory
        /// </summary>
        /// <param name="userName"></param>
        public void SearchUserInformationActiveDirectory(string userName)
        {
            UserName = userName;
            if (!MandatoryFieldsEmpty() && UserName.CompareTo(ContentFocused)!= 0)
            {
                RemoveAllErrors();
                if (!ValidateUserName())
                {
                    string[] infoAD = userName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    IsBusy = true;
                    _context.GetUserInformationActiveDirectory(infoAD[1], infoAD[0], GetUserInformationActiveDirectoryComleted, null);
                }
            }
        }

        /// <summary>
        /// Searches the user information by email in the active directory
        /// </summary>
        /// <param name="email"></param>
        public void SearchUserInformationByEmail(string email)
        {
            Email = email;

            if (!MandatoryFieldsEmpty() && Email.CompareTo(ContentFocused)!=0)
            {
                RemoveAllErrors();
                if (!ValidateEmail())
                {
                    _context.GetUserInformationByEmail(Email, GetUserInformationByEmailCompleted, null);
                    IsBusy = true;
                }
            }
            
        }

        /// <summary>
        /// Validate the field user name
        /// </summary>
        /// <returns></returns>
        private bool ValidateUserName()
        {
            //validate if the information is filled
            if (string.IsNullOrEmpty(UserName))
                AddError("UserName", Strings.UserNameIsMandatory);
            else
                RemoveError("UserName", Strings.UserNameIsMandatory);

            //validate the format of the user name
            if (!string.IsNullOrEmpty(UserName) && !Regex.IsMatch(UserName, @"(\w[a-zA-Z]+)(\\)(\w[a-zA-Z]+)", RegexOptions.IgnoreCase))
                AddError("UserName", Strings.UserNameFormatNotValid);
            else
                RemoveError("UserName", Strings.UserNameFormatNotValid);

            return HasErrors;
        }

        /// <summary>
        /// Validates the email field
        /// </summary>
        /// <returns></returns>
        private bool ValidateEmail()
        {
            // Email is mandatory for all users
            if (string.IsNullOrEmpty(Email))
                AddError("Email", Strings.EmailMandatory);
            else
                RemoveError("Email", Strings.EmailMandatory);

            if (!string.IsNullOrEmpty(Email) && !Regex.IsMatch(Email.ToLower(),
                    @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase))
                AddError("Email", Strings.EmailNotValid);
            else
                RemoveError("Email", Strings.EmailNotValid);

            return HasErrors;
        }

        /// <summary>
        /// Callback method for GetUserInformationActiveDirectory
        /// </summary>
        /// <param name="operation">Operation</param>
        private void GetUserInformationActiveDirectoryComleted(InvokeOperation<UserProfile> operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                //set the information
                if (operation.Value != null)
                {
                    Email = operation.Value.Email;
                    User.FirstName = operation.Value.FirstName;
                    User.LastName = operation.Value.LastName;
                    OnPropertyChanged("User");
                    OnPropertyChanged("Email");
                }
                else
                {
                    // When the user is not found we don't need to blank any field, just display an alert message.
                    AlertDisplay(Strings.UserNotFound);
                }
                IsBusy = false;
            });
        }

        /// <summary>
        /// Callback method for GetUserInformationByEmail
        /// </summary>
        /// <param name="operation"></param>
        private void GetUserInformationByEmailCompleted(InvokeOperation<UserProfile> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                //set the information
                if (operation.Value != null)
                {
                    User.FirstName = operation.Value.FirstName;
                    User.LastName = operation.Value.LastName;
                    User.UserName = operation.Value.UserName;
                    OnPropertyChanged("User");
                    OnPropertyChanged("UserName");
                }
                else
                {
                    // When the user is not found we don't need to blank any field, just display an alert message.
                    AlertDisplay(Strings.UserNotFoundByEmail);
                }
                IsBusy = false;
            });
        }

        #endregion

    }
}
