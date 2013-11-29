using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Input;
using Cotecna.Silverlight.Controls.Extension;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;

namespace Cotecna.Voc.Silverlight
{
    public class SecurityPaperListViewModel: ViewModel
    {

        #region events
        /// <summary>
        /// Show or hide export button
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<Visibility>> ShowHideExportButton;
        #endregion

        #region private fields
        private ICommand _resetCommand;
        private ICommand _searchCommand;
        private ICommand _misPrintedCommand;
        private ICommand _cancelCommand;
        private ICommand _newCommand;
        private ICommand _deleteCommand;
        private ObservableCollection<SecurityPaper> _securityPaperList;
        private Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel _gridViewModel;
        private SecurityPaperFilters _securityPaperFilters;
        private bool _checkAll;
        private bool _isMisPrintedVisible;
        private bool _isCancelVisible;
        private bool _isNewVisible;
        private bool _isDeleteVisible;

        private VocContext _proxy = new VocContext();
        #endregion

        #region properties

        /// <summary>
        /// Get and set the filters
        /// </summary>		
        public SecurityPaperFilters SecurityPaperFilters
        {
            get
            {
                return _securityPaperFilters;
            }
            set
            {
                if (_securityPaperFilters == value) return;
                _securityPaperFilters = value;
                OnPropertyChanged("SecurityPaperFilters");
            }
        }


        /// <summary>
        /// Search command
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
        
        /// <summary>
        /// Reset command
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

        /// <summary>
        /// List of security papers
        /// </summary>		
        public ObservableCollection<SecurityPaper> SecurityPaperList
        {
            get
            {
                return _securityPaperList;
            }
            set
            {
                _securityPaperList = value;
                OnPropertyChanged(() => SecurityPaperList);
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
                        PageSize = (uint)App.CurrentUser.PageSize
                    };
                    _gridViewModel.PageIndexChanged += GridViewModelPageIndexChanged;
                    _gridViewModel.LaunchExcelExportation += GridViewModelLaunchExcelExportation;
                }
                return _gridViewModel;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether check all is clicked
        /// </summary>
        public bool CheckAll
        {
            get
            {
                return _checkAll;
            }
            set
            {
                    _checkAll = value;
                    OnPropertyChanged(() => CheckAll);
                    CheckAllAction();
                }
            }

        /// <summary>
        /// Gets MisPrintedCommand
        /// </summary>
        public ICommand MisPrintedCommand
        {
            get 
            {
                if (_misPrintedCommand == null)
                    _misPrintedCommand = new DelegateCommand(ExecuteMisPrintedCommand);
                return _misPrintedCommand; 
            }
        }

        /// <summary>
        /// Gets CancelCommand
        /// </summary>
        public ICommand CancelCommand
        {
            get 
            {
                if (_cancelCommand == null)
                    _cancelCommand = new DelegateCommand(ExecuteCancelCommand);
                return _cancelCommand; 
            }
        }

        /// <summary>
        /// Gets NewCommand
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

        /// <summary>
        /// Gets DeleteCommand
        /// </summary>
        public ICommand DeleteCommand
        {
            get 
            {
                if (_deleteCommand == null)
                    _deleteCommand = new DelegateCommand(ExecuteDeleteCommand);
                return _deleteCommand; 
            }
        }

        private ICommand _uncancelCommand;

        public ICommand UncancelCommand
        {
            get 
            {
                if (_uncancelCommand == null)
                    _uncancelCommand = new DelegateCommand(ExecuteUncancellCommand);
                return _uncancelCommand; 
            }
        }
        

        /// <summary>
        /// Gets or sets IsMisPrintedVisible
        /// </summary>		
        public bool IsMisPrintedVisible
        {
            get
            {
                return _isMisPrintedVisible;
            }
            set
            {
                if (_isMisPrintedVisible == value) return;
                _isMisPrintedVisible = value;
                OnPropertyChanged("IsMisPrintedVisible");
            }
        }

        private bool _isEditVisible;
        /// <summary>
        /// Gets or sets IsEditVisible
        /// </summary>
        public bool IsEditVisible
        {
            get 
            { 
                return _isEditVisible; 
            }
            set 
            {
                if (_isEditVisible == value) return;
                _isEditVisible = value;
                OnPropertyChanged("IsEditVisible");
            }
        }

        private ICommand _editCommand;
        /// <summary>
        /// Gets EditCommand
        /// </summary>
        public ICommand EditCommand
        {
            get 
            {
                if (_editCommand == null)
                    _editCommand = new DelegateCommand(ExecuteEditCommand);
                return _editCommand; 
            }
        }
        
        

        /// <summary>
        /// Gets or sets IsCancelVisible
        /// </summary>		        
        public bool IsCancelVisible
        {
            get
            {
                return _isCancelVisible;
            }
            set
            {
                if (_isCancelVisible == value) return;
                _isCancelVisible = value;
                OnPropertyChanged("IsCancelVisible");
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
        /// Gets or sets IsDeleteVisible
        /// </summary>		
        public bool IsDeleteVisible
        {
            get
            {
                return _isDeleteVisible;
            }
            set
            {
                if (_isDeleteVisible == value) return;
                _isDeleteVisible = value;
                OnPropertyChanged("IsDeleteVisible");
            }
        }

        private bool _isUncancelVisible;

        public bool IsUncancelVisible
        {
            get { return _isUncancelVisible; }
            set 
            {
                if (_isUncancelVisible == value) return;
                _isUncancelVisible = value;
                OnPropertyChanged("IsUncancelVisible");
            }
        }
        
        
        #endregion

        #region constructor

        /// <summary>
        /// Get a new instance of the class
        /// </summary>
        public SecurityPaperListViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                SecurityPaperFilters = new SecurityPaperFilters();
                GetSecurityPapers(0);
                IsNewVisible = App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin);
            }
            else
            {
                SecurityPaperList = new ObservableCollection<SecurityPaper>();
            }
        }
        #endregion

        #region private methods

        private void ExecuteUncancellCommand()
        {
            SecurityPaperCommentChildWindow commentWindow = new SecurityPaperCommentChildWindow();
            SecurityPaperCommentViewModel commentModel = new SecurityPaperCommentViewModel(SecurityPaperAction.UnCancel);
            commentWindow.DataContext = commentModel;
            commentModel.Proxy = _proxy;
            commentModel.CloseWindow += (s, e) => commentWindow.Close();
            commentModel.RefeshSecurityPaperList += (s, e) => ExecuteResetCommand();
            commentModel.SecurityPapers = SecurityPaperList.Where(x => x.Checked).ToList();
            commentWindow.Show();
        }

        /// <summary>
        /// Execute EditCommand
        /// </summary>
        private void ExecuteEditCommand()
        {
            SecurityPaperEditionChildWindow editWindow = new SecurityPaperEditionChildWindow();
            SecurityPaperEditionViewModel editViewModel = new SecurityPaperEditionViewModel();
            editWindow.DataContext = editViewModel;
            editViewModel.Context = _proxy;
            editViewModel.SelectedSecurityPapers = SecurityPaperList.Where(x => x.Checked).ToList();
            editViewModel.CloseWindow += (s, e) => editWindow.Close();
            editViewModel.RefeshSecurityPaperList += (s, e) => ExecuteResetCommand();
            editWindow.Show();
        }

        /// <summary>
        /// Show or hide export to excel button
        /// </summary>
        /// <param name="visibility">Visiblie or Collapsed</param>
        private void ExecuteShowHideExportButton(Visibility visibility)
        {
            if (ShowHideExportButton != null)
                ShowHideExportButton(this, new ContextEditionEventArgs<Visibility>(visibility));
        }

        /// <summary>
        /// Execute DeleteCommand
        /// </summary>
        private void ExecuteDeleteCommand()
        {
           
            List<int> CheckedSecurityPaperList = new List<int>();
            foreach (SecurityPaper item in SecurityPaperList)
            {
                if (item.Checked)
                {
                    CheckedSecurityPaperList.Add(item.SecurityPaperId);
                }
            }
            QuestionDisplay(Strings.SecurityPaperDeleteMessage, delegate
            {
                // if yes is selected all the checked security papers will be deleted
                IsBusy = true;
                _proxy.DeleteSecurityPaperList(CheckedSecurityPaperList, CheckedSecurityPaperLisCompleted, null);
            }, delegate 
            {
                // if no is selected all the checked security papers will be unchecked
                CheckAll = false;
            });
            
        }
        /// <summary>
        /// Executed when DeleteSecurityPaperList method is completed
        /// </summary>
        /// <param name="operation"></param>
        private void CheckedSecurityPaperLisCompleted(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, delegate
             {
                 ExecuteResetCommand();
                 IsBusy = false;
             });
        }

        /// <summary>
        /// Execute NewCommand
        /// </summary>
        private void ExecuteNewCommand()
        {
            SecurityPaperChildWindow newSecurityPaper = new SecurityPaperChildWindow();
            SecurityPaperViewModel securityPaperModel = new SecurityPaperViewModel();
            newSecurityPaper.DataContext = securityPaperModel;
            securityPaperModel.CloseWindow += (s, e) =>  newSecurityPaper.Close();
            securityPaperModel.RefeshSecurityPaperList += (s, e) => ExecuteResetCommand();
            newSecurityPaper.Show();
        }

        /// <summary>
        /// Execute CancelCommand
        /// </summary>
        private void ExecuteCancelCommand()
        {
            SecurityPaperCommentChildWindow commentWindow = new SecurityPaperCommentChildWindow();
            SecurityPaperCommentViewModel commentModel = new SecurityPaperCommentViewModel(SecurityPaperAction.Cancel);
            commentWindow.DataContext = commentModel;
            commentModel.Proxy = _proxy;
            commentModel.CloseWindow += (s, e) => commentWindow.Close();
            commentModel.RefeshSecurityPaperList += (s, e) => ExecuteResetCommand();
            commentModel.SecurityPapers = SecurityPaperList.Where(x => x.Checked).ToList();
            commentWindow.Show();

        }

        /// <summary>
        /// Execute MisPrintedCommand
        /// </summary>
        private void ExecuteMisPrintedCommand()
        {
            SecurityPaperCommentChildWindow commentWindow = new SecurityPaperCommentChildWindow();
            SecurityPaperCommentViewModel commentModel = new SecurityPaperCommentViewModel(SecurityPaperAction.Misprinted);
            commentWindow.DataContext = commentModel;
            commentModel.Proxy = _proxy;
            commentModel.CloseWindow += (s, e) => commentWindow.Close();
            commentModel.RefeshSecurityPaperList += (s, e) => ExecuteResetCommand();
            commentModel.SecurityPapers = SecurityPaperList.Where(x => x.Checked).ToList();
            commentWindow.Show();
        }
        /// <summary>
        /// Check or uncheck all items of list
        /// </summary>
        private void CheckAllAction()
        {
            if (SecurityPaperList != null)
            {
                foreach (var item in SecurityPaperList)
                {
                    item.Checked = CheckAll;
                }
            }
        }


        /// <summary>
        /// Go to the server and get the list of security papers
        /// </summary>
        /// <param name="currentPage">Current page</param>
        private void GetSecurityPapers(int selectedPage)
        {
            IsBusy = true;
            int currentIndex = selectedPage * App.CurrentUser.PageSize;

            if (GridViewModel.PageIndex > 1 && selectedPage == 0)
                GridViewModel.PageIndex = 1;

            EntityQuery<SecurityPaper> query = _proxy.GetSecurityPaperListQuery(SecurityPaperFilters.IssuanceDateFrom,
                SecurityPaperFilters.IssuanceDateTo,
                SecurityPaperFilters.SelectedEntryPointId, SecurityPaperFilters.Issued,
                SecurityPaperFilters.NotIssued, SecurityPaperFilters.Cancelled, SecurityPaperFilters.MisPrinted, 
                SecurityPaperFilters.Number)
                .Skip(currentIndex).Take(App.CurrentUser.PageSize);

            query.IncludeTotalCount = true;
            _proxy.Load(query, LoadBehavior.RefreshCurrent, CompletedGetSecurityPapers, null);
        }

        /// <summary>
        /// Callback method to get security papers
        /// </summary>
        /// <param name="operation">Operation</param>
        private void CompletedGetSecurityPapers(LoadOperation<SecurityPaper> operation)
        {
            HandleLoadOperation(operation, delegate 
            {
                SecurityPaperList = new ObservableCollection<SecurityPaper>(operation.Entities);
                foreach (var item in SecurityPaperList)
                {
                    item.PropertyChanged += SecurityPaperListPropertyChanged;
                }

                GridViewModel.TotalRecords = (uint)operation.TotalEntityCount;
                CheckAll = false;
                CheckActionButtons();
                IsBusy = false;
                ExecuteShowHideExportButton(operation.TotalEntityCount > 0 ? Visibility.Visible : Visibility.Collapsed);
            });
        }

        /// <summary>
        /// Is executed when PropertyChanged event is called
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event Arguments</param>
        private void SecurityPaperListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Checked")
            {
                CheckActionButtons();
            }
        }

        /// <summary>
        /// Check each button if it can be displayed or not.
        /// </summary>
        private void CheckActionButtons()
        {
            IsEditVisible = SecurityPaperList.Any(x => x.Checked) &&
            SecurityPaperList.Count(x => x.Checked && x.Status == SecurityPaperStatusEnum.NotIssued) > 0
            && SecurityPaperList.Count(x => x.Checked && x.Status != SecurityPaperStatusEnum.NotIssued) == 0
            && (App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin));

            IsDeleteVisible = SecurityPaperList.Any(x => x.Checked) &&
            SecurityPaperList.Count(x => x.Checked && x.Status == SecurityPaperStatusEnum.Issued) == 0
            && (App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin));

            IsMisPrintedVisible = SecurityPaperList.Any(x => x.Checked) && App.CurrentUser.IsBorderAgentOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);

            IsCancelVisible = SecurityPaperList.Any(x => x.Checked)
                && SecurityPaperList.Count(x => x.Checked && x.Status == SecurityPaperStatusEnum.Cancelled) == 0
                && App.CurrentUser.IsBorderAgentOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);
            
            IsUncancelVisible = SecurityPaperList.Any(x => x.Checked) &&
                SecurityPaperList.Count(x => x.Checked && x.Status == SecurityPaperStatusEnum.Cancelled) > 0
            && SecurityPaperList.Count(x => x.Checked && x.Status != SecurityPaperStatusEnum.Cancelled) == 0
                && App.CurrentUser.IsBorderAgentOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);
        }

        /// <summary>
        /// Control of pagination
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void GridViewModelPageIndexChanged(object sender, PageIndexChangedArgs e)
        {
            GetSecurityPapers((int)e.PageNumber());
        }

        /// <summary>
        /// Perform the exportation to excel
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void GridViewModelLaunchExcelExportation(object sender, EventArgs e)
        {
            IsBusy = true;
            _proxy.ExportSecutiryPapers(SecurityPaperFilters.IssuanceDateFrom,
                SecurityPaperFilters.IssuanceDateTo,
                SecurityPaperFilters.SelectedEntryPointId, SecurityPaperFilters.Issued,
                SecurityPaperFilters.NotIssued, SecurityPaperFilters.Cancelled, SecurityPaperFilters.MisPrinted, 
                SecurityPaperFilters.Number,
                CompletedExportSecutiryPapers,null);
        }

        /// <summary>
        /// Callback method for ExportSecutiryPapers
        /// </summary>
        /// <param name="operation">Operation</param>
        private void CompletedExportSecutiryPapers(InvokeOperation<string> operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                IsBusy = false;
                DownloadExcelFile(operation.Value);
            });
        }

        /// <summary>
        /// Download the report
        /// </summary>
        /// <param name="item">document item parameter</param>
        private void DownloadExcelFile(string path)
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

        /// <summary>
        /// Execute search command
        /// </summary>
        private void ExecuteSearchCommand()
        {
            GetSecurityPapers(0);
        }

        /// <summary>
        /// Execute reset command
        /// </summary>
        private void ExecuteResetCommand()
        {
            SecurityPaperFilters.ResetFilters();
            ExecuteSearchCommand();
            CheckAll = false;
        }
        #endregion
    }
}
