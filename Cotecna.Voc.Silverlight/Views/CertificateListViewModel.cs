using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;
using Cotecna.Silverlight.Controls.Extension;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Web.Services;
using System.Linq;
using System.ComponentModel;
using System;
using System.Windows;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System.Collections.Generic;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Class ViewModel for the list of certificates
    /// </summary>
    public class CertificateListViewModel: ViewModel
    {
        #region fields
        private Certificate _selectedCertificate;
        private Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel _gridViewModel;
        private VocContext _context = new VocContext();
            
        #endregion

        #region properties


        /// <summary>
        /// Gets or sets CerticateListFilters
        /// </summary>
        public CertificateListFilters CertificateListFilters { get; set; }
        
        /// <summary>
        /// Gets or sets the selected certificate in the grid
        /// </summary>		
        public Certificate SelectedCertificate
        {
            get
            {
                return _selectedCertificate;
            }
            set
            {
                if (_selectedCertificate == value) return;
                _selectedCertificate = value;
                OnPropertyChanged("SelectedCertificate");
            }
        }

        private ObservableCollection<Certificate> _certificateList;
        /// <summary>
        /// Gets or sets the list of certificates
        /// </summary>		
        public ObservableCollection<Certificate> CertificateList
        {
            get
            {
                return _certificateList;
            }
            set
            {
                _certificateList = value;
                OnPropertyChanged(() => CertificateList);
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
                    _gridViewModel.LaunchExcelExportation += _gridViewModel_LaunchExcelExportation;
                    _gridViewModel.LaunchExportReleaseNotes += GridViewModelLaunchExportReleaseNotes;
                }
                return _gridViewModel;
            }
        }


        /// <summary>
        /// Gets a value indicating whether new option visible
        /// </summary>
        public bool IsNewVisible {             
            get {
                return App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin)
                        || App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer); 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether publish option visible
        /// </summary>
        public bool IsPublishVisible
        {
            get
            {
                return IsNewVisible || App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);
            }
        }

        /// <summary>
        /// Gets a value indicating whether edit option visible
        /// </summary>
        public bool IsEditVisible
        {
            get
            {
                return IsNewVisible;
            }
        }

        private bool _isPublishVisible;
        /// <summary>
        /// Gets or sets a value indicating whether publish button is visible
        /// </summary>
        public bool IsPublishActionVisible
        {
            get
            {
                return _isPublishVisible;
            }
            set
            {
                _isPublishVisible = value;
                OnPropertyChanged(() => IsPublishActionVisible);
            }
        }
        private bool _isUnpublishVisible;
        /// <summary>
        /// Gets or sets a value indicating whether unpublish button is visible
        /// </summary>
        public bool IsUnpublishVisible
        {
            get
            {
                return _isUnpublishVisible;
            }
            set
            {
                _isUnpublishVisible = value;
                OnPropertyChanged(() => IsUnpublishVisible);
            }
        }
        private bool _isRequestVisible;
        /// <summary>
        /// Gets or sets a value indicating whether request control is visible
        /// </summary>
        public bool IsRequestVisible
        {
            get
            {
                return _isRequestVisible;
            }
            set
            {
                _isRequestVisible = value;
                OnPropertyChanged(() => IsRequestVisible);
            }
        }
        private bool _isApproveVisible;
        /// <summary>
        /// Gets or sets a value indicating whether approve button is visible
        /// </summary>
        public bool IsApproveVisible
        {
            get
            {
                return _isApproveVisible;
            }
            set
            {
                _isApproveVisible = value;
                OnPropertyChanged(() => IsApproveVisible);
            }
        }
        private bool _isRecallVisible;
        /// <summary>
        /// Gets or sets a value indicating whether recall button is visible
        /// </summary>
        public bool IsRecallVisible
        {
            get
            {
                return _isRecallVisible;
            }
            set
            {
                _isRecallVisible = value;
                OnPropertyChanged(() => IsRecallVisible);
            }
        }
        private bool _isRejectVisible;
        /// <summary>
        /// Gets or sets a value indicating whether reject button is visible
        /// </summary>
        public bool IsRejectVisible
        {
            get
            {
                return _isRejectVisible;
            }
            set
            {
                _isRejectVisible = value;
                OnPropertyChanged(() => IsRejectVisible);
            }
        }
        private bool _isCloseVisible;
        /// <summary>
        /// Gets or sets a value indicating whether close button is visible
        /// </summary>
        public bool IsCloseVisible
        {
            get
            {
                return _isCloseVisible;
            }
            set
            {
                _isCloseVisible = value;
                OnPropertyChanged(() => IsCloseVisible);
            }
        }
        private bool _isUncloseVisible;
        /// <summary>
        /// Gets or sets a value indicating whether unclose button is visible
        /// </summary>
        public bool IsUncloseVisible
        {
            get
            {
                return _isUncloseVisible;
            }
            set
            {
                _isUncloseVisible = value;
                OnPropertyChanged(() => IsUncloseVisible);
            }
        }
        private bool _isDeleteVisible;
        /// <summary>
        /// Gets or sets a value indicating whether delete button is visible
        /// </summary>
        public bool IsDeleteVisible
        {
            get
            {
                return _isDeleteVisible;
            }
            set
            {
                _isDeleteVisible = value;
                OnPropertyChanged(() => IsDeleteVisible);
            }
        }

        private bool _isSendComdivVisible;

        /// <summary>
        /// Gets or sets IsSendComdivVisible
        /// </summary>
        public bool IsSendComdivVisible
        {
            get 
            { 
                return _isSendComdivVisible; 
            }
            set 
            { 
                _isSendComdivVisible = value;
                OnPropertyChanged(() => IsSendComdivVisible);
            }
        }
        

        private bool _checkAll;
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
                if (_checkAll != value)
                {
                    _checkAll = value;
                    OnPropertyChanged(() => CheckAll);
                    CheckAllAction();
                }
            }
        }

        #endregion

        #region commands

        private ICommand _sendComdivCommand;

        /// <summary>
        /// Gets SendComdivCommand command
        /// </summary>
        public ICommand SendComdivCommand
        {
            get
            {
                if (_sendComdivCommand == null)
                    _sendComdivCommand = new DelegateCommand(ExecuteSendComdivCommand);
                return _sendComdivCommand; 
            }
        }
        

        private ICommand _editCertificateCommand;
        /// <summary>
        /// Gets edit certificate command
        /// </summary>
        public ICommand EditCertificateCommand
        {
            get
            {
                if (_editCertificateCommand == null)
                    _editCertificateCommand = new DelegateCommand<Certificate>(EditCertificate);
                return _editCertificateCommand;
            }
        }

        private ICommand _refreshCommand;
        /// <summary>
        /// Gets refresh command
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                    _refreshCommand = new DelegateCommand(Refresh);
                return _refreshCommand;
            }
        }

        private ICommand _publishCommand;
        /// <summary>
        /// Gets publish command
        /// </summary>
        public ICommand PublishCommand
        {
            get
            {
                if (_publishCommand == null)
                    _publishCommand = new DelegateCommand(Publish);
                return _publishCommand;
            }
        }

        private ICommand _unpublishCommand;
        /// <summary>
        /// Gets unpublish command
        /// </summary>
        public ICommand UnpublishCommand
        {
            get
            {
                if (_unpublishCommand == null)
                    _unpublishCommand = new DelegateCommand(Unpublish);
                return _unpublishCommand;
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
                if(_newCommand == null)
                    _newCommand = new DelegateCommand(ExecuteNewCommand);
                return _newCommand; 
            }
        }

        private ICommand _viewFileCommand;
        
        /// <summary>
        /// Gets View file command
        /// </summary>
        public ICommand ViewFileCommand
        {
            get
            {
                if (_viewFileCommand == null)
                    _viewFileCommand = new DelegateCommand<Certificate>(ViewFile);
                return _viewFileCommand;
            }
        }

        private ICommand _requestCommand;
        /// <summary>
        /// Gets request command
        /// </summary>
        public ICommand RequestCommand
        {
            get
            {
                if (_requestCommand == null)
                    _requestCommand = new DelegateCommand(Request);
                return _requestCommand;
            }
        }

        private ICommand _approveCommand;
        /// <summary>
        /// Gets request command
        /// </summary>
        public ICommand ApproveCommand
        {
            get
            {
                if (_approveCommand == null)
                    _approveCommand = new DelegateCommand(Approve);
                return _approveCommand;
            }
        }

        private ICommand _rejectCommand;
        /// <summary>
        /// Gets request command
        /// </summary>
        public ICommand RejectCommand
        {
            get
            {
                if (_rejectCommand == null)
                    _rejectCommand = new DelegateCommand(Reject);
                return _rejectCommand;
            }
        }

        private ICommand _recallCommand;
        /// <summary>
        /// Gets recall command
        /// </summary>
        public ICommand RecallCommand
        {
            get
            {
                if (_recallCommand == null)
                    _recallCommand = new DelegateCommand(Recall);
                return _recallCommand;
            }
        }

        private ICommand _closeCommand;

        /// <summary>
        /// Gets close command
        /// </summary>
        public ICommand CloseCommand
        {
            get 
            {
                if (_closeCommand == null)
                    _closeCommand = new DelegateCommand(Close);
                return _closeCommand; 
            }
        }


        private ICommand _uncloseCommand;

        /// <summary>
        /// Gets Unclose command
        /// </summary>
        public ICommand UncloseCommand
        {
            get
            {
                if (_uncloseCommand == null)
                    _uncloseCommand = new DelegateCommand(Unclose);
                return _uncloseCommand;
            }
        }

        private ICommand _deleteCommand;

        /// <summary>
        /// Gets delete command
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                    _deleteCommand = new DelegateCommand(Delete);
                return _deleteCommand;
            }
        }

        

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

        private ICommand _viewHistoryCommand;

        /// <summary>
        /// Gets ViewHistoryCommand
        /// </summary>
        public ICommand ViewHistoryCommand
        {
            get 
            {
                if (_viewHistoryCommand == null)
                    _viewHistoryCommand = new DelegateCommand<Certificate>(ExecuteViewHistoryCommand);
                return _viewHistoryCommand; 
            }
        }
        

        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateListViewModel"/> class.
        /// </summary>
        public CertificateListViewModel()
        {
            CertificateList = new ObservableCollection<Certificate>();
            
            if (!DesignerProperties.IsInDesignTool)
            {
                CertificateListFilters = new CertificateListFilters();
                IsBusy = true;
                //Get the list of certificates
                GetCertificatesPaginated(0);
                
            }
            else
            {
                CertificateList.Add(new Certificate 
                    { Sequential="Test", WorkflowStatusId=WorkflowStatusEnum.Created }
                );
            }
        }
        #endregion

        #region private methods

        /// <summary>
        /// Send to comdiv a list of certificate
        /// </summary>
        private void ExecuteSendComdivCommand()
        {
            IsBusy = true;
            _context.SynchronizeWithComdivList(GetCheckedList(), CompletedExecuteSendComdivCommand, null);
        }

        /// <summary>
        /// Call back method for SynchronizeWithComdivList
        /// </summary>
        /// <param name="operation"></param>
        private void CompletedExecuteSendComdivCommand(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeSyncComdiv);
                Refresh();
            });
        }

        /// <summary>
        /// Execute ShowHideExportButtons event
        /// </summary>
        /// <param name="visibility">Visibility</param>
        private void ExecuteShowHideExportButtons(Visibility visibility)
        {
            if (ShowHideExportButtons != null)
                ShowHideExportButtons(this, new ContextEditionEventArgs<Visibility>(visibility));
        }

        private void GridViewModelLaunchExportReleaseNotes(object sender, EventArgs e)
        {
            IsBusy = true;
            _context.ExportReleaseNotesToExcel(CertificateListFilters.CertificateNumber,
                                            CertificateListFilters.IssuanceDateFrom, CertificateListFilters.IssuanceDateTo,
                                            CertificateListFilters.SelectedEntryPointId, CertificateListFilters.SelectedOffice,
                                            CertificateListFilters.Published, CertificateListFilters.Unpublished, CertificateListFilters.MyDocuments,
                                            CertificateListFilters.Conform, CertificateListFilters.NonConform, CertificateListFilters.Cancelled,
                                            CertificateListFilters.Created, CertificateListFilters.Requested, CertificateListFilters.Approved,
                                            CertificateListFilters.Rejected, CertificateListFilters.Ongoing, CertificateListFilters.Closed,
                                            CertificateListFilters.Invoiced, CertificateListFilters.NonInvoiced, CertificateListFilters.ComdivNumber,
                                            ExportCertificatesToExcelCompleted,null);
        }

        /// <summary>
        /// Show history pop up
        /// </summary>
        /// <param name="certificate">Certificate</param>
        private void ExecuteViewHistoryCommand(Certificate certificate)
        {
            CertificateTrackingChildWindow trackWindow = new CertificateTrackingChildWindow();
            CertificateTrackingViewModel trackModel = new CertificateTrackingViewModel(certificate.CertificateId);
            trackModel.CloseWindow += (s, e) => trackWindow.Close();
            trackWindow.DataContext = trackModel;
            trackWindow.Show();
        }

        /// <summary>
        /// Download certificates excel file exportation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _gridViewModel_LaunchExcelExportation(object sender, EventArgs e)
        {
            ExportCertificatesToExcel();
        }

        /// <summary>
        /// Check or uncheck all items of list
        /// </summary>
        private void CheckAllAction()
        {
            if (CertificateList != null)
            {
                foreach (var item in CertificateList)
                {
                    item.Checked = CheckAll;
                }
            }
        }

        /// <summary>
        /// Execute search command
        /// </summary>
        private void ExecuteSearchCommand()
        {
            IsBusy = true;
            //Get the list of certificates
            GetCertificatesPaginated(0);
        }

        /// <summary>
        /// Execute reset command
        /// </summary>
        private void ExecuteResetCommand()
        {
            CertificateListFilters.SetDefaultValues();
            CertificateListFilters.SetDefaultView();
            IsBusy = true;
            //Get the list of certificates
            GetCertificatesPaginated(0);
        }
        /// <summary>
        /// Control of pagination
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void GridViewModelPageIndexChanged(object sender, PageIndexChangedArgs e)
        {
            IsBusy = true;
            GetCertificatesPaginated((int)e.PageNumber());
        }

        /// <summary>
        /// Get the list of certificates paginated
        /// </summary>
        /// <param name="selectedPage">Page number in 0 base</param>
        private void GetCertificatesPaginated(int selectedPage)
        {
            //calculate the current index
            int currentIndex = selectedPage * App.CurrentUser.PageSize;
            //set the page on the paginator
            if (GridViewModel.PageIndex > 1 && selectedPage == 0)
                GridViewModel.PageIndex = 1;

            //get the paginated list of certificates
            EntityQuery<Certificate> query = _context.GetCertificatesQuery(CertificateListFilters.CertificateNumber,
                                            CertificateListFilters.IssuanceDateFrom, CertificateListFilters.IssuanceDateTo,
                                            CertificateListFilters.SelectedEntryPointId, CertificateListFilters.SelectedOffice,
                                            CertificateListFilters.Published, CertificateListFilters.Unpublished, CertificateListFilters.MyDocuments,
                                            CertificateListFilters.Conform, CertificateListFilters.NonConform, CertificateListFilters.Cancelled,
                                            CertificateListFilters.Created, CertificateListFilters.Requested, CertificateListFilters.Approved,
                                            CertificateListFilters.Rejected, CertificateListFilters.Ongoing, CertificateListFilters.Closed,
                                            CertificateListFilters.Invoiced, CertificateListFilters.NonInvoiced, CertificateListFilters.ComdivNumber)
                                            .Skip(currentIndex)
                                            .Take(App.CurrentUser.PageSize);
            query.IncludeTotalCount = true;
            _context.Load(query, LoadBehavior.RefreshCurrent, LoadCertificatesCompleted, null);

           // ExportCertificatesToExcel();
        }

        /// <summary>
        /// Get the list of all certificates 
        /// </summary>
        private void ExportCertificatesToExcel()
        {
            IsBusy = true;
            //get the paginated list of certificates
            _context.ExportCertificatesToExcel(CertificateListFilters.CertificateNumber,
                                            CertificateListFilters.IssuanceDateFrom, CertificateListFilters.IssuanceDateTo,
                                            CertificateListFilters.SelectedEntryPointId, CertificateListFilters.SelectedOffice,
                                            CertificateListFilters.Published, CertificateListFilters.Unpublished, CertificateListFilters.MyDocuments,
                                            CertificateListFilters.Conform, CertificateListFilters.NonConform, CertificateListFilters.Cancelled,
                                            CertificateListFilters.Created, CertificateListFilters.Requested, CertificateListFilters.Approved,
                                            CertificateListFilters.Rejected, CertificateListFilters.Ongoing, CertificateListFilters.Closed,
                                            CertificateListFilters.Invoiced, CertificateListFilters.NonInvoiced,CertificateListFilters.ComdivNumber,
                                            ExportCertificatesToExcelCompleted, null);
        } 

        /// <summary>
        /// Completed method for GetAllCertificates
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void ExportCertificatesToExcelCompleted(InvokeOperation<string> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                string path= operation.Value.ToString();
                IsBusy = false;
                DownloadCertificateExcelFile(path);

            });
        }


        /// <summary>
        /// Download the corresponding template
        /// </summary>
        /// <param name="item">document item parameter</param>
        private void DownloadCertificateExcelFile(string path)
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
        /// Refresh Screen
        /// </summary>
        private void Refresh()
        {
            IsBusy = true;
            //Get the list of certificates
            GetCertificatesPaginated((int)_gridViewModel.PageIndex - 1);
        }

        /// <summary>
        /// Execute the click event of the button new
        /// </summary>
        private void ExecuteNewCommand()
        {
            if(App.CurrentUser.OfficeId <= 0)
                AlertDisplay(Strings.CreateCertificateError);
            else
                EditCertificate(new Certificate());
        }
        
        /// <summary>
        /// Completed method for GetCertificatesQuery
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void LoadCertificatesCompleted(LoadOperation<Certificate> operation)
        {
            HandleLoadOperation(operation, delegate 
            {
                GridViewModel.TotalRecords = (uint)operation.TotalEntityCount;
                if (CertificateList != null)
                    CertificateList.Clear();
                CheckAll = false;
                CertificateList = new ObservableCollection<Certificate>(operation.Entities);
                foreach (var item in CertificateList)
                {
                    item.PropertyChanged += CertificateItemPropertyChanged;
                }
                HidaAllButtons();
                GlobalAccessor.Instance.MessageStatus = Strings.Ready;
                IsBusy = false;
                ExecuteShowHideExportButtons(operation.TotalEntityCount > 0 ? Visibility.Visible : Visibility.Collapsed);
            });
        }

        /// <summary>
        /// On certificate item has changed. Only check "Checked property"
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Check PropertyName information</param>
        private void CertificateItemPropertyChanged(object sender, PropertyChangedEventArgs e)
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
            if (CertificateList == null)
                return;
            var list = GetCheckedList();

            if (list.Count == 0)
            {
                HidaAllButtons();
                return;
            }
            if (IsPublishVisible)
            {
                IsPublishActionVisible = CanPublish(list);
                IsUnpublishVisible = CanUnpublish(list);
            }
            else
            {
                IsPublishActionVisible = CanPublish(list);
                IsUnpublishVisible = CanUnpublish(list);
            }
            if (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator))
            {
                IsRequestVisible = CanDisplayRequest(list);
            }
            else
            {
                IsRequestVisible = false;
            }
            if (App.CurrentUser.IsIssuerOrSuperior)
            {
                IsApproveVisible = CanApproveCertificateList(list);
                IsRecallVisible = CanRecallCertificateList(list);
            }
            else
            {
                IsApproveVisible = false;
                IsRecallVisible = false;
            }
            if (App.CurrentUser.IsIssuerOrSuperior)
            {
                IsRejectVisible = CanDisplayReject(list);
            }
            else
            {
                IsRejectVisible = false;
            }
            if (App.CurrentUser.IsBorderAgentOrSuperior)
            {
                IsCloseVisible = CanDisplayClose(list);
            }
            else
            {
                IsCloseVisible = false;
                IsUncloseVisible = false;
            }
            if (App.CurrentUser.IsCoordinatorOrSuperior)
            {
                IsDeleteVisible = CanDisplayDelete(list);
            }
            else
            {
                IsDeleteVisible = false;
            }
            IsUncloseVisible = CanDisplayUnClose(list);
            IsSendComdivVisible = CanDisplaySendComdiv(list);
        }

        /// <summary>
        /// Hide all action buttons
        /// </summary>
        private void HidaAllButtons()
        {
            IsPublishActionVisible = false;
            IsUnpublishVisible = false;
            IsRequestVisible = false;
            IsApproveVisible = false;
            IsRecallVisible = false;
            IsRejectVisible = false;
            IsCloseVisible = false;
            IsUncloseVisible = false;
            IsDeleteVisible = false;
            IsSendComdivVisible = false;
        }


        #region action methods

        /// <summary>
        /// Gets the successful label 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="actionLabel"></param>
        /// <returns></returns>
        public string GetSuccesLabel(List<ValidationMessage> messages, string actionLabel)
        {
            string label = string.Empty;
            int success = messages.Count(x => (x.Status == StatusProcess.Success) || (x.Status == StatusProcess.Warning) || (x.Status == StatusProcess.GenericWarning));

            if (success > 1)
                label = String.Format(Strings.MultiActionSuccessResultPlural, success, Strings.ItemTypeCertificates, actionLabel) + Environment.NewLine;
            else if (success == 1)
                label = String.Format(Strings.MultiActionSuccessResult, success, Strings.ItemTypeCertificate, actionLabel) + Environment.NewLine;

            foreach (var m in messages.Where(x => (x.Status == StatusProcess.Success) || (x.Status == StatusProcess.Warning) || (x.Status == StatusProcess.GenericWarning)))
            {
                label += m.Identifier + Environment.NewLine;
            }
            return label;
        }

        /// <summary>
        /// Gets the error label 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="actionLabel"></param>
        /// <returns></returns>
        public string GetErrorLabel(List<ValidationMessage> messages, string actionLabel)
        {
            string label = string.Empty;
            int nonSuccess = messages.Count(x => x.Status == StatusProcess.Error);

            if (nonSuccess > 1)
                label = String.Format(Strings.MultipleActionErrorResultPlural,Strings.ItemTypeCertificates, actionLabel) + Environment.NewLine;
            else if (nonSuccess == 1)
                label = String.Format(Strings.MultipleActionErrorResult, Strings.ItemTypeCertificate, actionLabel) + Environment.NewLine;

            foreach (var m in messages.Where(x => x.Status == StatusProcess.Error))
            {
                label += m.Identifier + ":" + m.Message + Environment.NewLine;
            }
            return label;
        }

        /// <summary>
        /// Gets the warning label 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="actionLabel"></param>
        /// <returns></returns>
        public string GetWarningLabel(List<ValidationMessage> messages, string actionLabel)
        {
            string label = string.Empty;
            int warnings = messages.Count(x => x.Status == StatusProcess.Warning);

            if (warnings > 1)
                label = String.Format(Strings.MultipleActionWarningsResultPlural,Strings.ItemTypeCertificates, actionLabel) + Environment.NewLine;
            else if (warnings == 1)
                label = String.Format(Strings.MultipleActionWarningsResult, Strings.ItemTypeCertificate, actionLabel) + Environment.NewLine;

            foreach (var m in messages.Where(x => x.Status == StatusProcess.Warning))
            {
                label += m.Identifier + ":" + m.Message + Environment.NewLine;
            }
            return label;
        }

        /// <summary>
        /// Gets the generic warning label 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="actionLabel"></param>
        /// <returns></returns>
        public string GetGenericWarningLabel(List<ValidationMessage> messages, string actionLabel)
        {
            string label = string.Empty;

            foreach (var m in messages.Where(x => x.Status == StatusProcess.GenericWarning))
            {
                label += Strings.MultipleActionGenericWarningsResult + Environment.NewLine;
                label += m.Message + Environment.NewLine;
                break;
            }
            return label;
        }

        /// <summary>
        /// Gets the generic error label 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="actionLabel"></param>
        /// <returns></returns>
        public string GetGenericErrorLabel(List<ValidationMessage> messages, string actionLabel)
        {
            string label = string.Empty;

            foreach (var m in messages.Where(x => x.Status == StatusProcess.GenericError))
            {
                label += Strings.MultipleActionGenericErrosResult + Environment.NewLine;
                label += m.Message + Environment.NewLine;
                break;
            }
            return label;
        }

        /// <summary>
        /// 
        /// </summary>
        private void showMultipleActionResult(List<ValidationMessage> result, string actionLabel)
        {
            List<ValidationMessage> messages = result;
            string label = string.Empty;

            string success = GetSuccesLabel(messages, actionLabel);
            string genericErrors = GetGenericErrorLabel(messages, actionLabel);
            string errors = GetErrorLabel(messages, actionLabel);
            string warnings = GetWarningLabel(messages, actionLabel);
            string genericWarnings = GetGenericWarningLabel(messages, actionLabel);

            if (!string.IsNullOrEmpty(success))
            {
                label += success + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(genericErrors))
            {
                if (!string.IsNullOrEmpty(label))
                    label += Environment.NewLine;

                label += genericErrors + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(errors))
            {
                if (!string.IsNullOrEmpty(label))
                    label += Environment.NewLine;

                label += errors + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(genericWarnings))
            {
                if (!string.IsNullOrEmpty(label))
                    label += Environment.NewLine;

                label += genericWarnings + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(warnings))
            {
                if (!string.IsNullOrEmpty(label))
                    label += Environment.NewLine;

                label += warnings + Environment.NewLine;
            }
            

            AlertDisplay(label);
                       
            Refresh();
        }

        /// <summary>
        /// Method executes RequestCertificateList 
        /// </summary>
        /// <param name="operation">operation result</param>
        public void RequestCertificateList(List<Certificate> certificates)
        {
            _context.RequestCertificateList(certificates, RequestCertificateListCompleted, null);
        }

        /// <summary>
        /// Method is executed when RequestCertificateList is completed
        /// </summary>
        /// <param name="operation"></param>
        private void RequestCertificateListCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeRequested);
                Refresh();
            });
        }

        /// <summary>
        /// if all de certificates in the list can be requested
        /// </summary>
        /// <param name="list">list of certificates</param>
        /// <returns></returns>
        public bool CanDisplayRequest(List<Certificate> certificates)
        {
            foreach (Certificate certificate in certificates)
            {
                if (!(certificate.CanBeRequested && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Method executes RejectCertificateList
        /// </summary>
        /// <param name="operation">operation result</param>
        public void RejectCertificateList(List<Certificate> certificates)
        {
            _context.RejectCertificateList(certificates, RejectCertificateListCompleted, null);
        }

        /// <summary>
        /// Method is executed when RejectCertificateList is completed
        /// </summary>
        /// <param name="operation"></param>
        private void RejectCertificateListCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeRejected);
                Refresh();
            });
        }

        /// <summary>
        /// if all de certificates in the list can be rejected
        /// </summary>
        /// <param name="list">list of certificates</param>
        /// <returns></returns>
        public bool CanDisplayReject(List<Certificate> certificates)
        {
            foreach (Certificate certificate in certificates)
            {
                if (!(certificate.CanBeRejected && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Methotd tha delete a list of certificates
        /// </summary>
        /// <param name="certificates"></param>
        public void DeleteCertificateList(List<Certificate> certificates)
        {
            QuestionDisplay(Strings.DeleteQuestionMessagePlural, delegate
            {
                _context.DeleteCertificateList(certificates, DeleteCertificateListCompleted, null);
            });
        }

        /// <summary>
        /// Method is executed when DeleteCertificateList is completed
        /// </summary>
        /// <param name="operation">operation result</param>
        private void DeleteCertificateListCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeDeleted);
                Refresh();
            });
        }

        /// <summary>
        /// if all de certificates in the list can be deleted
        /// </summary>
        /// <param name="list">list of certificates</param>
        /// <returns></returns>
        public bool CanDisplayDelete(List<Certificate> certificates)
        {
            foreach (Certificate certificate in certificates)
            {
                if (!(certificate.CanBeDeleted && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Method executes CloseCertificateList
        /// </summary>
        /// <param name="operation">operation result</param>
        public void CloseCertificateList(List<Certificate> certificates)
        {
            QuestionDisplay(Strings.CloseQuestionMessagePlural, delegate
            {
                _context.CloseCertificateList(certificates, CloseCertificateListCompleted, null);
            });
        }

        /// <summary>
        /// Method is executed when CloseCertificateList is completed
        /// </summary>
        /// <param name="operation"></param>
        private void CloseCertificateListCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeClosed);
                Refresh();
            });
        }

        /// <summary>
        /// if all de certificates in the list can be closed
        /// </summary>
        /// <param name="list">list of certificates</param>
        /// <returns></returns>
        public bool CanDisplayClose(List<Certificate> certificates)
        {
            foreach (Certificate certificate in certificates)
            {
                if (!(certificate.CanBeClosed && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Method executes UnCloseCertificateList
        /// </summary>
        /// <param name="operation">operation result</param>
        public void UnCloseCertificateList(List<Certificate> certificates)
        {
            QuestionDisplay(Strings.UnCloseQuestionMessagePlural, delegate
            {
                _context.UNCloseCertificateList(certificates, UnCloseCertificateListCompleted, null);
            });
        }

        /// <summary>
        /// Method is executed when UnCloseCertificateList is completed
        /// </summary>
        /// <param name="operation"></param>
        private void UnCloseCertificateListCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeUnclosed);
                Refresh();
            });
        }

        /// <summary>
        /// Verify is any certificate can be synchronized with comdiv
        /// </summary>
        /// <param name="certificates"></param>
        /// <returns></returns>
        public bool CanDisplaySendComdiv(List<Certificate> certificates)
        {
            foreach (Certificate certificate in certificates)
            {
                if (!(!certificate.HasChanges && !HasChanges && !certificate.IsSynchronized.GetValueOrDefault()
                  && ((certificate.WorkflowStatusId >= WorkflowStatusEnum.Approved && certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                 || (certificate.WorkflowStatusId == WorkflowStatusEnum.Closed && certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
                 || certificate.CertificateStatusId == CertificateStatusEnum.Cancelled) && !(App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent) || App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin))))

                    return false;
            }
            return true;
        }

        /// <summary>
        /// if all de certificates in the list can be unclosed
        /// </summary>
        /// <param name="list">list of certificates</param>
        /// <returns></returns>
        public bool CanDisplayUnClose(List<Certificate> certificates)
        {
            foreach (Certificate certificate in certificates)
            {
                if (!(certificate.CanBeUnclosed &&
                                (certificate.CertificateStatusId == CertificateStatusEnum.Conform && (App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent) || App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin))
                                || (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin)))))

                    return false;
            }
            return true;
        }

        /// <summary>
        /// Approve a list of certificates
        /// </summary>
        /// <param name="certificates">The list of certificates to be approved</param>
        private void ApproveCertificateList(List<Certificate> certificates)
        {
            QuestionDisplay(Strings.ApproveMultiselectQuestion, () =>
            {
                IsBusy = true;
                _context.ApproveCertificateList(certificates, ApproveCertificateListCompleted, null);
            });
        }

        /// <summary>
        /// Callback method for ApproveCertificateList
        /// </summary>
        /// <param name="operation">Operation</param>
        private void ApproveCertificateListCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeApproved);
                Refresh();
            });
        }

        /// <summary>
        /// Recall a list of certificates
        /// </summary>
        /// <param name="certificates">List of certificates to be recalled</param>
        private void RecallCertificateList(List<Certificate> certificates)
        {
            QuestionDisplay(Strings.RecallMultiselectQuestion, () => 
            {
            IsBusy = true;
            _context.RecallCertificateList(certificates, RecallCertificateListCompleted, null);
            });
            
        }

        /// <summary>
        /// Callback method for RecallCertificateList
        /// </summary>
        /// <param name="operation">Operation</param>
        private void RecallCertificateListCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, Strings.ActionTypeRecall);
                Refresh();
            });
        }

        /// <summary>
        /// Validate if i can approve one or several items in the selected list
        /// </summary>
        /// <param name="certificateList">List of certificates</param>
        /// <returns>bool</returns>
        private bool CanApproveCertificateList(List<Certificate> certificateList)
        {
            bool canApprove = true;
            foreach (var certificate in certificateList)
            {
                if (!(certificate.CanBeApproved && (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer))))
                {
                    canApprove =false;
                }
            }
            return canApprove;
        }

        /// <summary>
        /// Validate if i can recall one or several items in the selected list
        /// </summary>
        /// <param name="certificateList">List of certificates</param>
        /// <returns>bool</returns>
        private bool CanRecallCertificateList(List<Certificate> certificateList)
        {
            bool canRecall = true;
            foreach (var certificate in certificateList)
            {
                if (!(certificate.CanBeRecalled && App.CurrentUser.IsIssuerOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)))
                {
                    canRecall = false;
                }
            }
            return canRecall;
        }

        /// <summary>
        /// Perform the publish or unpublis operation
        /// </summary>
        /// <param name="certificateList">List of certificate ids</param>
        /// <param name="isPublished">Is publish or unpublish</param>
        private void PublishUnplishCertificate(List<Certificate> certificateList, bool isPublished)
        {
            IsBusy = true;
            _context.PublisUnpublishCertificateList(certificateList, isPublished, PublishUnplishCertificateCompleted, isPublished);
        }

        /// <summary>
        /// Callback method of PublisUnpublishCertificateList
        /// </summary>
        /// <param name="operation">Operation</param>
        private void PublishUnplishCertificateCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                bool isPublished = bool.Parse(operation.UserState.ToString());
                List<ValidationMessage> messages = operation.Value;
                showMultipleActionResult(messages, isPublished ? Strings.ActionTypePublished : Strings.ActionTypeUnpublished);
                Refresh();
            });
        }

        /// <summary>
        /// Validate if the the sytem can publish selected items
        /// </summary>
        /// <param name="certificateList">List of certifices</param>
        /// <returns>bool</returns>
        private bool CanPublish(List<Certificate> certificateList)
        {
            bool canPublish = true;
            foreach (var certificate in certificateList)
            {
                if (!(certificate.CanBePublished && !certificate.IsPublished && 
                    (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || 
                    App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer))))
                {
                    canPublish = false;
                }
            }
            return canPublish;
        }

        /// <summary>
        /// Validate if the system can unpublish selected items
        /// </summary>
        /// <param name="certificateList">List of certificates</param>
        /// <returns>bool</returns>
        private bool CanUnpublish(List<Certificate> certificateList)
        {
            bool canUnpublish = true;
            foreach (var certificate in certificateList)
            {
                if (!(certificate.CanBePublished && certificate.IsPublished &&
                    (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin) ||
                    App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer))))
                {
                    canUnpublish = false;
                }
            }
            return canUnpublish;
        }

        #endregion

        /// <summary>
        /// Get list of checked items
        /// </summary>
        /// <returns>List of items checked</returns>
        private List<Certificate> GetCheckedList()
        {
            return CertificateList.Where(x => x.Checked == true).ToList();
        }

        /// <summary>
        /// Edit the selected certificate
        /// </summary>
        /// <param name="item">certificate item</param>
        private void EditCertificate(Certificate item)
        {
            if (CertificateEditionRequested != null)
            {
                CertificateViewModel certificateViewModel = new CertificateViewModel();
                certificateViewModel.OnSaveCompleted += Model_OnSaveCompleted;

                if (item.EntityState == EntityState.Detached)
                    certificateViewModel.InitializeNew(item);
                else
                    certificateViewModel.Initialize(item.CertificateId, string.IsNullOrEmpty(item.Sequential) ? item.ComdivNumber : item.Sequential);
                CertificateEditionRequested(this, new ContextEditionEventArgs<CertificateViewModel>(certificateViewModel));
            }
        }

        /// <summary>
        /// View certificate file
        /// </summary>
        /// <param name="certificate">Certificate information</param>
        private void ViewFile(Certificate certificate)
        {
            _context.GetCertificateDocumentByCertificateId(certificate.CertificateId, GetDocumentDone,certificate);
        }

        /// <summary>
        /// On done getting document
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void GetDocumentDone(InvokeOperation<Document> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                CertificateViewModel certificateViewModel = new CertificateViewModel();
                certificateViewModel.Initialize(_context);
                certificateViewModel.Certificate = operation.UserState as Certificate;
                certificateViewModel.DisplayCertificateFile(operation.Value);
            });
        }


        /// <summary>
        /// On save done
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        void Model_OnSaveCompleted(object sender, EventArgs e)
        {
            Refresh();
            CertificateViewModel model= sender as CertificateViewModel;

            bool canClose = (model.Certificate.CertificateStatusId == CertificateStatusEnum.Conform &&
                model.Certificate.WorkflowStatusId == WorkflowStatusEnum.Approved) ||
                (model.Certificate.CertificateStatusId == CertificateStatusEnum.NonConform &&
                model.Certificate.WorkflowStatusId == WorkflowStatusEnum.Closed);

            //Label is null when action is executed from grid.
            if (GridActionCompleted != null && (model.Label == null || model.IsNew || canClose))
                GridActionCompleted(sender, EventArgs.Empty);

            if (model.IsNew || canClose)
            {
                CertificateViewModel certificateViewModel = new CertificateViewModel();
                certificateViewModel.OnSaveCompleted += Model_OnSaveCompleted;
                certificateViewModel.Initialize(model.Certificate.CertificateId, string.IsNullOrEmpty(model.Certificate.Sequential) ? model.Certificate.ComdivNumber : model.Certificate.Sequential);
                CertificateEditionRequested(this, new ContextEditionEventArgs<CertificateViewModel>(certificateViewModel));
            }
        }

        /// <summary>
        /// Publish certificate 
        /// </summary>
        private void Publish()
        {
            PublishUnplishCertificate(GetCheckedList(), true);
        }

        /// <summary>
        /// Unpublish certificate 
        /// </summary>
        private void Unpublish()
        {
            PublishUnplishCertificate(GetCheckedList(), false);
        }


        /// <summary>
        /// Request action
        /// </summary>
        private void Request()
        {
            var list = GetCheckedList();
            RequestCertificateList(list);
        }

        /// <summary>
        /// Approve process
        /// </summary>
        private void Approve()
        {
            ApproveCertificateList(GetCheckedList());
        }


        /// <summary>
        /// Reject process
        /// </summary>
        private void Reject()
        {
            var list = GetCheckedList();
            RejectCertificateList(list);
        }


        /// <summary>
        /// Recall process
        /// </summary>
        private void Recall()
        {
            RecallCertificateList(GetCheckedList());
        }


        /// <summary>
        /// Perform close process
        /// </summary>
        private void Close()
        {
            CloseCertificateList(GetCheckedList());
        }

        /// <summary>
        /// Perform Unclose process
        /// </summary>
        private void Unclose()
        {
            UnCloseCertificateList(GetCheckedList());
        }


        /// <summary>
        /// Perform Delete process
        /// </summary>
        private void Delete()
        {
            DeleteCertificateList(GetCheckedList());
        }

        #endregion

        #region Events


        /// <summary>
        /// Event used to notify to display the edit screen
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<CertificateViewModel>> CertificateEditionRequested;

        /// <summary>
        /// Event used to notify that Grid action has been completed
        /// </summary>
        internal event EventHandler GridActionCompleted;

        /// <summary>
        /// Show or hide export to excel buttons
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<Visibility>> ShowHideExportButtons;
        #endregion
    }
}
