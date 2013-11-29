
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Input;
using Cotecna.Silverlight.Controls.Extension;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;
namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Class ViewModel for the list of users
    /// </summary>
    public class UserListViewModel : ViewModel
    {
        #region Private Fields
        private Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel _gridViewModel;
        private AuthenticationDomainContext _context = new AuthenticationDomainContext();
        private ObservableCollection<UserProfile> _userList;
        private UserListFilters _userListFilters;
        private bool _isNewVisible;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the filters to search Users
        /// </summary>	
        public UserListFilters UserListFilters
        {
            get
            {
                return _userListFilters;
            }
            set
            {
                if (_userListFilters != value)
                {
                    _userListFilters = value;
                    OnPropertyChanged("UserListFilters");
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of Users
        /// </summary>		
        public ObservableCollection<UserProfile> UserList
        {
            get
            {
                return _userList;
            }
            set
            {
                _userList = value;
                OnPropertyChanged(() => UserList);
            }
        }

        
        /// <summary>
        /// Gets or sets IsNewVisible
        /// </summary>		
        public bool IsNewVisible
        {
            get
            {
                return _isNewVisible;
            }
            set
            {
                if (_isNewVisible == value) return;
                _isNewVisible = value;
                OnPropertyChanged("IsNewVisible");
            }
        }

        /// <summary>
        /// Gets data context of paginator control
        /// </summary>		
        public Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel GridViewModel
        {
            get
            {
                if (_gridViewModel == null)
                {
                    _gridViewModel = new Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel
                    {
                        PageSize = (uint)App.CurrentUser.PageSize,
                    };
                    _gridViewModel.PageIndexChanged += GridViewModelPageIndexChanged;
                    _gridViewModel.LaunchExcelExportation += _gridViewModel_LaunchExcelExportation;
                }
                return _gridViewModel;
            }
        }

        #endregion

        #region Commands

        private ICommand _searchCommand;
        /// <summary>
        /// Get search command
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new DelegateCommand(ExecuteSearchCommand);
                return _searchCommand;
            }
        }

        private ICommand _resetCommand;

        /// <summary>
        /// Gets reset command
        /// </summary>
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                    _resetCommand = new DelegateCommand(ExecuteResetCommand);
                return _resetCommand;
            }
        }

        private ICommand _editCommand;

        /// <summary>
        /// Gets Edit command
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                    _editCommand = new DelegateCommand<UserProfile>(ExecuteEditCommand);
                return _editCommand;
            }
        }


        private ICommand _newCommand;

        /// <summary>
        /// Gets New command
        /// </summary>
        public ICommand NewCommand
        {
            get
            {
                if (_newCommand == null)
                    _newCommand = new DelegateCommand(ExecuteNewCommand);
                return _newCommand;
            }
        }


        #endregion

        #region Event Handlers
        /// <summary>
        /// Event used to notify to display the edit screen
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<UserViewModel>> UserEditionRequested;

        /// <summary>
        /// Show or hide export to excel button
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<Visibility>> ShowHideExportButton;
        #endregion

        #region Constructor
        public UserListViewModel()
        {
            UserListFilters = new UserListFilters();
            UserList = new ObservableCollection<UserProfile>();
            IsBusy = true;
            //Get the list of users
            GetUsersPaginated(0);
            IsNewVisible = App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Show or hide export button
        /// </summary>
        /// <param name="visibility">visible or collapsed</param>
        private void ExecuteShowHideExportButton(Visibility visibility)
        {
            if (ShowHideExportButton != null)
                ShowHideExportButton(this, new ContextEditionEventArgs<Visibility>(visibility));
        }

        /// <summary>
        /// Method to export user list to excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _gridViewModel_LaunchExcelExportation(object sender, EventArgs e)
        {
            ExportUsersToExcel();
        }


        /// <summary>
        /// Execute the click event of the button new
        /// </summary>
        private void ExecuteNewCommand()
        {
            UserProfile newUser = new UserProfile { IsInternalUser = true,IsActive=true };
            if (App.CurrentUser.IsInRole(UserRoleEnum.Admin))
                newUser.OfficeId = App.CurrentUser.OfficeId;

            if (UserEditionRequested != null)
            {
                UserViewModel UserViewModel = new UserViewModel();
                UserViewModel.OnSaveCompleted += Model_OnSaveCompleted;

                UserViewModel.Initialize(newUser);
                UserEditionRequested(this, new ContextEditionEventArgs<UserViewModel>(UserViewModel));
            }
        }

        /// <summary>
        /// Edit the selected user
        /// </summary>
        /// <param name="item">user item</param>
        private void ExecuteEditCommand(UserProfile user)
        {
            if (UserEditionRequested != null)
            {
                UserViewModel UserViewModel = new UserViewModel();
                UserViewModel.OnSaveCompleted += Model_OnSaveCompleted;

                UserViewModel.Initialize(user.UserId, string.Concat(user.FirstName," " ,user.LastName));
                UserEditionRequested(this, new ContextEditionEventArgs<UserViewModel>(UserViewModel));
            }
        }

        /// <summary>
        /// On save done
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Model_OnSaveCompleted(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Refresh Screen
        /// </summary>
        private void Refresh()
        {
            IsBusy = true;
            //Get the list of users
            GetUsersPaginated((int)_gridViewModel.PageIndex - 1);
        }


        /// <summary>
        /// Execute search command
        /// </summary>
        private void ExecuteSearchCommand()
        {
            IsBusy = true;
            //Get the list of users
            GetUsersPaginated(0);
        }

        /// <summary>
        /// Execute reset command
        /// </summary>
        private void ExecuteResetCommand()
        {
            UserListFilters.SetDefaultValues();
            UserListFilters.SetDefaultView();
            IsBusy = true;
            //Get the list of users
            GetUsersPaginated(0);
        }

        /// <summary>
        /// Control of pagination
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void GridViewModelPageIndexChanged(object sender, PageIndexChangedArgs e)
        {
            IsBusy = true;
            GetUsersPaginated((int)e.PageNumber());
        }

        /// <summary>
        /// Get the list of users paginated
        /// </summary>
        /// <param name="selectedPage">Page number in 0 base</param>
        private void GetUsersPaginated(int selectedPage)
        {
            //calculate the current index
            int currentIndex = selectedPage * App.CurrentUser.PageSize;
            //set the page on the paginator
            if (GridViewModel.PageIndex > 1 && selectedPage == 0)
                GridViewModel.PageIndex = 1;
            //begin the pagination

            //Clean the context
            _context.UserInRoles.Clear();

            //get the paginated list of users
            EntityQuery<UserProfile> query = _context.GetUsersQuery(
                                            _userListFilters.FirstName,
                                            _userListFilters.LastName,
                                            _userListFilters.OfficeIdSelected,
                                            _userListFilters.EntryPointIdSelected,
                                            _userListFilters.RoleIdSelected,
                                            App.CurrentUser.Roles)
                                            .OrderBy(x => x.UserId)
                                            .Skip(currentIndex)
                                            .Take(App.CurrentUser.PageSize);
            query.IncludeTotalCount = true;
            _context.Load(query, LoadBehavior.RefreshCurrent, LoadUsersCompleted, null);

           

        }

        /// <summary>
        /// Completed method for GetCertificatesQuery
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void LoadUsersCompleted(LoadOperation<UserProfile> operation)
        {
            GridViewModel.TotalRecords = (uint)operation.TotalEntityCount;
            UserList = new ObservableCollection<UserProfile>(operation.Entities);
            GlobalAccessor.Instance.MessageStatus = Strings.Ready;
            IsBusy = false;
            ExecuteShowHideExportButton(operation.TotalEntityCount > 0 ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// Get the list of all users 
        /// </summary>
        private void ExportUsersToExcel()
        {
            IsBusy = true;
            //get the paginated list of certificates
            _context.ExportUsersToExcel(    _userListFilters.FirstName,
                                            _userListFilters.LastName,
                                            _userListFilters.OfficeIdSelected,
                                            _userListFilters.EntryPointIdSelected,
                                            _userListFilters.RoleIdSelected,
                                            App.CurrentUser.Roles,
                                            ExportUsersToExcelCompleted, null);
        }

        /// <summary>
        /// Completed method for ExportUsersToExcel
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void ExportUsersToExcelCompleted(InvokeOperation<string> operation)
        {
            HandleInvokeOperation(operation, delegate
            {  
                string path= operation.Value.ToString();
                IsBusy = false;
                DownloadUserExcelFile(path);
            });
        }

        /// <summary>
        /// Download the corresponding template
        /// </summary>
        /// <param name="item">document item parameter</param>
        private void DownloadUserExcelFile(string path)
        {
            //if item null, downoad new word template

            var serviceUri = System.Windows.Application.Current.Host.Source.AbsoluteUri;
            string currentUri = serviceUri.Replace(serviceUri.Split('/').Last(), "");
            currentUri = currentUri.Replace("ClientBin/", "");
            string fullFilePath = string.Format("{0}DownloadFile.aspx?ExportDocument={1}", currentUri, path);

            Uri url = new Uri(fullFilePath);
            if (App.Current.IsRunningOutOfBrowser)
            {
                MyHyperlinkButton button = new MyHyperlinkButton();
                button.NavigateUri = url;
                button.TargetName = "_blank";
                button.ClickMe();
            }
            else
                System.Windows.Browser.HtmlPage.Window.Navigate(url, "_newWindow", "toolbar=0,menubar=0,resizable=1,scrollbars=1,top=0,left=0");


        }

        #endregion

        #region Public Methods

        #endregion

    }
}
