using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;


namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Certificate view model what manage certificate information
    /// </summary>
    public class CertificateViewModel : ViewModel
    {
        #region Private Fields
        VocContext _context = new VocContext();
        VocContext _documentcontext = new VocContext();
        private Certificate _certificate;
        private Office _office;
        private Document _currentDocument;
        private ReleaseNoteViewModel _releaseNoteViewModel;
        private CertificateStatusEnum? _selectedCertificateType;
        private int? _selectedEntryPoint;
        private bool _isComdivNumberEnabled;
        private bool? _isInvoiced;
        private bool _isFOBValueEnable;
        private bool _isuncancelledVisible;
        private bool _isSyncComdivVisible;
        private bool _isSyncStackPanelVisible;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets IsSyncStackPanelVisible
        /// </summary>
        public bool IsSyncStackPanelVisible
        {
            get 
            { 
                return _isSyncStackPanelVisible; 
            }
            set 
            { 
                _isSyncStackPanelVisible = value;
                OnPropertyChanged(() => IsSyncStackPanelVisible);
            }
        }
        

        /// <summary>
        /// Gets or sets IsSyncComdivVisible
        /// </summary>
        public bool IsSyncComdivVisible
        {
            get 
            { 
                return _isSyncComdivVisible; 
            }
            set 
            { 
                _isSyncComdivVisible = value;
                OnPropertyChanged(() => IsSyncComdivVisible);
            }
        }
        

        /// <summary>
        /// Gets or sets IsuncancelledVisible
        /// </summary>
        public bool IsuncancelledVisible
        {
            get 
            { 
                return _isuncancelledVisible; 
            }
            set 
            { 
                _isuncancelledVisible = value;
                OnPropertyChanged(() => IsuncancelledVisible);
            }
        }
        

        /// <summary>
        /// Get or set IsComdivNumberEnabled
        /// </summary>
        public bool IsComdivNumberEnabled
        {
            get 
            { 
                return _isComdivNumberEnabled; 
            }
            set 
            { 
                _isComdivNumberEnabled = value;
                OnPropertyChanged(() => IsComdivNumberEnabled);
            }
        }
        
        /// <summary>
        /// Get or set IsInvoiced
        /// </summary>
        public bool? IsInvoiced
        {
            get 
            { 
                return _isInvoiced; 
            }
            set 
            { 
                _isInvoiced = value;
                HasChanges = true;
                OnPropertyChanged(() => IsInvoiced);
                _certificate.IsInvoiced = _isInvoiced;
            }
        }
        

        /// <summary>
        /// Get or set SelectedEntryPoint
        /// </summary>
        public int? SelectedEntryPoint
        {
            get 
            { 
                return _selectedEntryPoint; 
            }
            set 
            { 
                _selectedEntryPoint = value;
                HasChanges = true;
                OnPropertyChanged(() => SelectedEntryPoint);
                _certificate.EntryPointId = _selectedEntryPoint;
            }
        }

        /// <summary>
        /// Gets or sets ComdivNumber
        /// </summary>
        public string ComdivNumber
        {
            get
            {
                return _certificate != null ? _certificate.ComdivNumber : string.Empty;
            }
            set
            {
                if (_certificate != null)
                {
                    HasChanges = true;
                    _certificate.ComdivNumber = value;
                    OnPropertyChanged("ComdivNumber");
                }
            }
        }

        /// <summary>
        /// Gets or sets fob value
        /// </summary>
        public string FOBValue
        {
            get
            {
                string _fobValue = string.Empty;
                if (_certificate != null && _certificate.FOBValue.HasValue)
                    _fobValue = _certificate.FOBValue.Value.ToString("F2", new CultureInfo("en-US"));
                return _fobValue;
            }
            set
            {
                if (_certificate != null && (!string.IsNullOrEmpty(value) && !value.Equals("0")))
                {
                    HasChanges = true;
                    _certificate.FOBValue = decimal.Parse(value, new CultureInfo("en-US"));
                    OnPropertyChanged("FOBValue");
                }
                else
                {
                    _certificate.FOBValue = null;
                    OnPropertyChanged("FOBValue");
                }
            }
        }

        /// <summary>
        /// Gets or sets IsFOBValueEnable
        /// </summary>
        public bool IsFOBValueEnable
        {
            get 
            { 
                return _isFOBValueEnable; 
            }
            set 
            { 
                _isFOBValueEnable = value;
                OnPropertyChanged(() => IsFOBValueEnable);
            }
        }
        
        

        /// <summary>
        /// Gets or sets SelectedCertificateType
        /// </summary>		
        public CertificateStatusEnum? SelectedCertificateType
        {
            get
            {
                return _selectedCertificateType;
            }
            set
            {
                if (value != null)
                {
                    _selectedCertificateType = value;
                    HasChanges = true;
                    OnPropertyChanged(() => SelectedCertificateType);
                    _certificate.CertificateStatusId = _selectedCertificateType.Value;
                }
            }
        }

        public bool IsNew { get; set; }
        
        /// <summary>
        /// Gets or sets CertificateType
        /// </summary>
        public CertificateStatusEnum CertificateType { get; set; }
        /// <summary>
        /// Text o edit hyperlink in Release Note grid
        /// </summary>
        public string EditReleaseNoteText
        {
            get
            {
                return _certificate.WorkflowStatusId != WorkflowStatusEnum.Closed && App.CurrentUser.IsBorderAgentOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)
                        ? Strings.Edit : Strings.Details;
            }
        }

        /// <summary>
        /// Gets or sets the counter when document failed to upload
        /// </summary>
        public int CounterDocumentFails { get; set; }

        /// <summary>
        /// Gets or sets list of entry points 
        /// </summary>
        public ObservableCollection<EntryPoint> EntryPoints { get; set; }

        /// <summary>
        /// Gets or sets document list
        /// </summary>
        public ObservableCollection<Document> DocumentList { get; set; }

        /// <summary>
        /// Gets or sets ReleaseNotesList
        /// </summary>
        public ObservableCollection<ReleaseNote> ReleaseNotesList { get; set; }

        /// <summary>
        /// Gets the text of Publish button (Publish/unPublish)
        /// </summary>
        public string PublishText
        {
            get
            {
                return _certificate == null ? string.Empty :
                        _certificate.IsPublished ? Strings.Unpublish : 
                        Strings.Publish; 
            } 
        }

        /// <summary>
        /// Gets or sets certificate to Edit
        /// </summary>
        public Certificate Certificate
        {
            get
            {
                return _certificate;
            }
            set
            {
                if (_certificate != value)
                {
                    _certificate = value;
                    _certificate.PropertyChanged += Certificate_PropertyChanged;
                    OnPropertyChanged("Certificate");
                }
            }
        }

        /// <summary>
        /// Gets or sets office to Edit
        /// </summary>
        public Office Office
        {
            get
            {
                return _office;
            }
            set
            {                
                _office = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the New Release buttons bool 
        /// </summary>
        public bool IsNewReleaseNoteVisible { get; set; } 

        /// <summary>
        /// Gets or sets a value indicating whether the Certificate buttons bool 
        /// </summary>
        public bool IsSaveVisible { get; set; } 
       
        /// <summary>
        /// Gets or sets a value indicating whether download is visible
        /// </summary>
        public bool IsDownloadVisible { get; set; }  
      
        /// <summary>
        /// Gets or sets a value indicating whether upload is visible
        /// </summary>
        public bool IsUploadVisible { get; set; }


        private bool _isReleaseVisible;
        /// <summary>
        /// Gets or sets a value indicating whether release tab control is visible
        /// </summary>
        public bool IsReleaseVisible
        {
            get
            {
                return _isReleaseVisible;
            }
            set
            {
                _isReleaseVisible = value;
                OnPropertyChanged(() => IsReleaseVisible);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether conform is visible
        /// </summary>
        public bool IsTypeCertifRadioButtomVisible { get; set; } 
        /// <summary>
        /// Gets or sets a value indicating whether cancelled is visible
        /// </summary>
        public bool IsTypeCertifTextVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether publish is visible
        /// </summary>        
        public bool IsPublishVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether request is visible
        /// </summary>
        public bool IsRequestVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reject is visible
        /// </summary>
        public bool IsRejectVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether approve is visible
        /// </summary>
        public bool IsApproveVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether close is visible
        /// </summary>
        public bool IsCloseVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Unclose is visible
        /// </summary>
        public bool IsUncloseVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entry point and invoiced fields enable
        /// </summary>
        public bool IsCertificateHeaderEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Delete is visible
        /// </summary>
        public bool IsDeleteVisible { get; set; }

        private bool _isRecallVisible;
        /// <summary>
        /// Gets or sets a value indicating whether recall is visible
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

        string _label;
        /// <summary>
        /// Gets text to be displayed as header of tab item
        /// </summary>
        public string Label
        {
            get { return _label; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Certificate Information section is enabled
        /// </summary>
        public bool IsCertifInfoHeaderEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Document List section is enabled
        /// </summary>
        public bool IsUploadSuppDocumentsEnable { get; set; }

        private string _totalNetWeight;
        /// <summary>
        /// Gets or sets TotalNetWeight
        /// </summary>		        
        public string TotalNetWeight
        {
            get
            {
                return _totalNetWeight;
            }
            set
            {                
                _totalNetWeight = value;
                OnPropertyChanged("TotalNetWeight");
            }
        }

        private bool _checkAllDocuments;
        /// <summary>
        /// Gets or sets CheckAllDocuments
        /// </summary>		
        public bool CheckAllDocuments
        {
            get
            {
                return _checkAllDocuments;
            }
            set
            {
                _checkAllDocuments = value;
                OnPropertyChanged("CheckAllDocuments");
                CheckAllDocumentsAction();
            }
        }

        private bool _checkAllReleaseNotes;
        /// <summary>
        /// Gets or sets CheckAllReleaseNotes
        /// </summary>
        public bool CheckAllReleaseNotes
        {
            get 
            { 
                return _checkAllReleaseNotes; 
            }
            set 
            {
                _checkAllReleaseNotes = value;
                OnPropertyChanged("CheckAllReleaseNotes");
                CheckAllReleaNoteList();
            }
        }

        private bool _isDeleteReleaseNoteVisible;
        /// <summary>
        /// Gets or sets IsDeleteReleaseNoteVisible
        /// </summary>
        public bool IsDeleteReleaseNoteVisible
        {
            get 
            { 
                return _isDeleteReleaseNoteVisible; 
            }
            set 
            { 
                _isDeleteReleaseNoteVisible = value;
                OnPropertyChanged(() => IsDeleteReleaseNoteVisible);
            }
        }
        
        private bool _isDeleteDocumentVisible;
        /// <summary>
        /// Gets or sets IsDeleteDocumentVisible
        /// </summary>		
        public bool IsDeleteDocumentVisible
        {
            get
            {
                return _isDeleteDocumentVisible;
            }
            set
            {
                if (_isDeleteDocumentVisible == value) return;
                _isDeleteDocumentVisible = value;
                OnPropertyChanged("IsDeleteDocumentVisible");
            }
        }

        /// <summary>
        /// Gets CanEditRoUser
        /// </summary>
        public bool CanEditRoUser 
        {
            get
            {
                return  App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) ||
                        _certificate.OfficeId == App.CurrentUser.OfficeId;
            }
        }

        
        
        #endregion

        #region Commands


        private ICommand _syncComdivCommand;

        public ICommand SyncComdivCommand
        {
            get 
            {
                if (_syncComdivCommand == null)
                    _syncComdivCommand = new DelegateCommand(ExecuteSyncComdivCommand);
                return _syncComdivCommand; 
            }
        }
        

        private ICommand _saveCommand;
        /// <summary>
        /// Gets save command
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new DelegateCommand(Save);
                return _saveCommand;
            }
        }

        private ICommand _downloadCommand;
        /// <summary>
        /// Gets download command
        /// </summary>
        public ICommand DownloadCommand
        {
            get
            {
                if (_downloadCommand == null)
                    _downloadCommand = new DelegateCommand<Document>(Download);
                return _downloadCommand;
            }
        }

        private ICommand _publishCommand;
        /// <summary>
        /// Gets publish/unpublish command
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
      

        private ICommand _editDocumentCommand;
        /// <summary>
        /// Gets edit Document Description
        /// </summary>
        public ICommand EditDocumentCommand
        {
            get
            {
                if (_editDocumentCommand == null)
                    _editDocumentCommand = new DelegateCommand<Document>(EditDocument);
                return _editDocumentCommand;
            }
        }


        private ICommand _editReleaseNoteCommand;
        /// <summary>
        /// Gets edit ReleaseNote Description
        /// </summary>
        public ICommand EditReleaseNoteCommand
        {
            get
            {
                if (_editReleaseNoteCommand == null)
                    _editReleaseNoteCommand = new DelegateCommand<ReleaseNote>(EditReleaseNote);
                return _editReleaseNoteCommand;
            }
        }

        private ICommand _requestCommand;
        /// <summary>
        /// Gets execute the click event of the request button
        /// </summary>
        public ICommand RequestCommand
        {
            get 
            {
                if (_requestCommand == null)
                    _requestCommand = new DelegateCommand(ExecuteRequestCommand);
                return _requestCommand; 
            }
        }
        
        private ICommand _rejectCommand;
        /// <summary>
        /// Gets execute the click event of the reject button
        /// </summary>
        public ICommand RejectCommand
        {
            get 
            {
                if (_rejectCommand == null)
                    _rejectCommand = new DelegateCommand(ExecuteRejectCommand);
                return _rejectCommand; 
            }
        }

        private ICommand _approveCommand;
        /// <summary>
        /// Gets execute the click event of the approve button
        /// </summary>
        public ICommand ApproveCommand
        {
            get
            {
                if (_approveCommand == null)
                    _approveCommand = new DelegateCommand(ExecuteApproveCommand);
                return _approveCommand;
            }
        }

        private ICommand _recallCommand;
        /// <summary>
        /// Gets execute the click event of the recall button
        /// </summary>
        public ICommand RecallCommand
        {
            get
            {
                if (_recallCommand == null)
                    _recallCommand = new DelegateCommand(ExecuteRecallCommand);
                return _recallCommand;
            }
        }

        private ICommand _newReleaseNoteCommand;
        /// <summary>
        /// Gets execute the click event of the NewReleaseNote button
        /// </summary>
        public ICommand NewReleaseNoteCommand
        {
            get
            {
                if (_newReleaseNoteCommand == null)
                    _newReleaseNoteCommand = new DelegateCommand(ExecuteNewReleaseNoteCommand);
                return _newReleaseNoteCommand;
            }
        }

        private ICommand _closeCommand;
        /// <summary>
        /// Gets execute the click event of the close button
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

        private ICommand _UncloseCommand;
        /// <summary>
        /// Gets execute the click event of the Unclose button
        /// </summary>
        public ICommand UncloseCommand
        {
            get
            {
                if (_UncloseCommand == null)
                    _UncloseCommand = new DelegateCommand(ExecuteUncloseCommand);
                return _UncloseCommand;
            }
        }

        private ICommand _DeleteCommand;
        /// <summary>
        /// Gets execute the click event of the Delete button
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                    _DeleteCommand = new DelegateCommand(ExecuteDeleteCommand);
                return _DeleteCommand;
            }
        }

        private ICommand _viewReleaseNoteCommand;
        /// <summary>
        /// Gets view ReleaseNote Command
        /// </summary>
        public ICommand ViewReleaseNoteCommand
        {
            get
            {
                if (_viewReleaseNoteCommand == null)
                    _viewReleaseNoteCommand = new DelegateCommand<ReleaseNote>(ViewReleaseNote);
                return _viewReleaseNoteCommand;
            }
        }

        private ICommand _deleteDocumentsCommand;

        /// <summary>
        /// Gets DeleteDocumentsCommand
        /// </summary>
        public ICommand DeleteDocumentsCommand
        {
            get 
            {
                if (_deleteDocumentsCommand == null)
                    _deleteDocumentsCommand = new DelegateCommand(ExecuteDeleteDocumentsCommand);
                return _deleteDocumentsCommand; 
            }
        }

        private ICommand _uncancelledCommand;

        /// <summary>
        /// Gets UncancelledCommand
        /// </summary>
        public ICommand UncancelledCommand
        {
            get
            {
                if (_uncancelledCommand == null)
                    _uncancelledCommand = new DelegateCommand(ExecuteUncancelledCommand);
                return _uncancelledCommand;
            }
        }

        private ICommand _deleteReleaseNoteCommand;
        /// <summary>
        /// Gets DeleteReleaseNoteCommand
        /// </summary>
        public ICommand DeleteReleaseNoteCommand
        {
            get 
            {
                if (_deleteReleaseNoteCommand == null)
                    _deleteReleaseNoteCommand = new DelegateCommand(ExecuteDeleteReleaseNoteCommand);
                return _deleteReleaseNoteCommand; 
            }
        }
        
        
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event used to notify to display the edit release note screen
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<ReleaseNoteViewModel>> ReleaseNoteEditionRequested;
        /// <summary>
        /// Event used to notify to display the edit screen
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<DocumentViewModel>> DocumentEditionRequested;

        /// <summary>
        /// Event used to notify the readonly mode changed
        /// </summary>
        internal event EventHandler ReadOnlyModeChanged;        

        /// <summary>
        /// Event used to notify that save is completed
        /// </summary>
        internal event EventHandler OnSaveCompleted;

        /// <summary>
        /// Event used to notify that delete is completed
        /// </summary>
        internal event EventHandler OnDeleteCompleted;
        
        /// <summary>
        /// Event used to notify when busy indicator is activated
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<bool>> ChangeIsBusy;

        /// <summary>
        /// Event used to activate documents tab after a recall
        /// </summary>
        internal event EventHandler ActivateDocumentsTab;

        #endregion

        #region Constructor

        public CertificateViewModel()
        {
            DocumentList = new ObservableCollection<Document>();
            ReleaseNotesList = new ObservableCollection<ReleaseNote>();
        }


        #endregion

        #region Private Methods
        /// <summary>
        /// Execute delete release notes
        /// </summary>
        private void ExecuteDeleteReleaseNoteCommand()
        {
            int releaseNoteCount = ReleaseNotesList.Count(x => x.Checked);
            string message = releaseNoteCount == 1 ? Strings.DeleteOneReleaseNoteMessage : string.Format(Strings.DeleteSomeReleaseNoteMessage, releaseNoteCount);
            QuestionDisplay(message, () => 
            {
                IsBusy = true;
                int[] selectedReleaseNotes = ReleaseNotesList.Where(x => x.Checked).Select(x => x.ReleaseNoteId).ToArray();
                _context.DeleteReleaseNotes(selectedReleaseNotes, CompletedDeleteReleaseNotes, null);
            },() => CheckAllReleaseNotes = false);
        }

        /// <summary>
        /// call back method for DeleteReleaseNotes
        /// </summary>
        /// <param name="operation">Invoke operation</param>
        private void CompletedDeleteReleaseNotes(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                CheckAllReleaseNotes = false;
                _context.Load(_context.GetReleaseNotesByCertificateIdQuery(Certificate.CertificateId), LoadBehavior.RefreshCurrent, GetReleaseNotesByCertificateIdCompleted, null);                
            });
        }

        /// <summary>
        /// Execute the synchorinization with comdiv
        /// </summary>
        private void ExecuteSyncComdivCommand()
        {
            if (App.CurrentUser.CanSync)
            {
                IsBusy = true;
                _context.SynchcroniseWithComdiv(_certificate, CompletedSynchcroniseWithComdiv, null);
            }
            else
            {
                AlertDisplay(Strings.SyncComdivNotPossible);
            }
        }

        /// <summary>
        /// Call back method for SynchcroniseWithComdiv
        /// </summary>
        /// <param name="operation">Invoke operation</param>
        private void CompletedSynchcroniseWithComdiv(InvokeOperation<ValidationMessage> operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                IsBusy = false;
                ValidationMessage result = operation.Value;
                if (result.Status != StatusProcess.Success)
                {
                    AlertDisplay(result.Message);
                    return;
                }
                GlobalAccessor.Instance.MessageStatus = Strings.SyncCompletedMessage;
                AlertDisplay(Strings.SyncCompletedMessage);
                //reload the certificate
                _context.Load(_context.GetCertificateByCertificateIdQuery(_certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadCertificateCompleted, null);
            });
        }

        /// <summary>
        /// Execute uncancelled command
        /// </summary>
        private void ExecuteUncancelledCommand()
        {
            Certificate.WorkflowStatusId = WorkflowStatusEnum.Created;
            Certificate.CertificateStatusId = CertificateStatusEnum.Conform;
            Certificate.IssuanceDate = null;
            IsBusy = true;
            OnChangeIsBusy(IsBusy);
            IsNew = true;
            _context.SubmitChanges(SaveCompleted, null);
            _context.SaveCertificateTrancking(_certificate.CertificateId, TrackingStatusEnum.Created);
        }


        /// <summary>
        /// Call the event to activate busy indicator
        /// </summary>
        /// <param name="value">true: is visible| false is hidden</param>
        private void OnChangeIsBusy(bool value)
        {
            if (ChangeIsBusy != null)
                ChangeIsBusy(this, new ContextEditionEventArgs<bool>(value));
        }

        /// <summary>
        /// Execute the click event of the button new
        /// </summary>
        private void ExecuteNewReleaseNoteCommand()
        {
            ReleaseNote newReleaseNote = new ReleaseNote { CertificateId = _certificate.CertificateId, Certificate = _certificate, IssuanceDate = DateTime.Now };
            ReleaseNoteViewModel model = new ReleaseNoteViewModel(newReleaseNote);
            model.SavedItem += OnReleaseNoteSaved;
            model.ReloadDocuments += ReleaseDocumentReloadDocuments;
            ReleaseNoteEditionRequested(this, new ContextEditionEventArgs<ReleaseNoteViewModel>(model));
        }

        /// <summary>
        /// Refresh document list
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void ReleaseDocumentReloadDocuments(object sender, EventArgs e)
        {
            IsBusy = true;
            _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(Certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);
        }


        /// <summary>
        /// ReleaseNote description saved
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnReleaseNoteSaved(object sender, EventArgs e)
        {
            ReleaseNoteViewModel model = sender as ReleaseNoteViewModel;
            ReleaseNote releaseNote = model.ReleaseNote;

            //If certificate staus is Approved change to Ongoing
            if (_certificate.WorkflowStatusId == WorkflowStatusEnum.Approved)
            {
                _certificate.WorkflowStatusId = WorkflowStatusEnum.Ongoing;
                model.IsNewReleaseNote = true;
            }

            if (!_certificate.IsInvoiced.GetValueOrDefault() && (releaseNote.PaidFees == null || releaseNote.PaidFees.Value == 0))
            {
                AlertDisplay(Strings.PaidFeesMandatory);
                return;
            }

            //If the release note is new, then attach to context
            if (releaseNote.EntityState == EntityState.Detached)
                _context.ReleaseNotes.Add(releaseNote);
            
            model.IsBusy = true;
            _context.SubmitChanges(ReleaseNoteSaveCompleted,model);
        }


        /// <summary>
        /// Execute when a ReleaseNote was saved
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void ReleaseNoteSaveCompleted(SubmitOperation operation)
        {
            HandleSubmitOperation(operation, () =>
            {
                ReleaseNoteViewModel model = operation.UserState as ReleaseNoteViewModel;
                GlobalAccessor.Instance.MessageStatus = Strings.ReleaseNoteSavedStatusMessage;
                _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(Certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);

                // Updating Issued security papers
                _releaseNoteViewModel = model;
                _releaseNoteViewModel.IsBusy = true;
                _documentcontext.UpdateIssuedSecurityPapersOnReleaseNote(model.ReleaseNote.ReleaseNoteId, model.IssuedSecurityPapers, CompletedUpdateIssuedSecurityPapers, null);
                if (model.IsNewReleaseNote)
                    _context.SaveCertificateTrancking(Certificate.CertificateId, TrackingStatusEnum.Ongoing, null, null);

                RaiseOnSaveCompleted();
                model.IsBusy = false;
            });
        }

        private void CompletedUpdateIssuedSecurityPapers(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                _releaseNoteViewModel.HasChanges = false;
                _releaseNoteViewModel.ChangesFromCheckableControl = false;
                _releaseNoteViewModel.SavePrintButtonsVisibilityRefresh();
                _releaseNoteViewModel.IsBusy = false;
                _releaseNoteViewModel = null;
            });
        }

        /// <summary>
        /// Execute when a certificate was loaded
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void LoadCertificateCompleted(LoadOperation<Certificate> operation)
        {
            HandleLoadOperation(operation, () =>
            {
                Certificate = operation.Entities.FirstOrDefault();
                CertificateType = Certificate.CertificateStatusId;
                SelectedCertificateType = Certificate.CertificateStatusId;
                _selectedEntryPoint = Certificate.EntryPointId;
                _isInvoiced = Certificate.IsInvoiced;
                OnPropertyChanged("IsInvoiced");
                OnPropertyChanged("SelectedEntryPoint");
                OnPropertyChanged("ComdivNumber");
                OnPropertyChanged("FOBValue");
                HasChanges = false;
                SetHeaderButtonsVisibility();
                SetScreenButtonsVisibility();
                VerifyReadOnlyMode();
                if (operation.UserState != null && operation.UserState.ToString().Equals("Approve"))
                {
                    //send a notification to coordinators when the certificate is approved
                    _context.SendEmailApprove(_certificate.Sequential, App.CurrentUser.OfficeId, SendEmailApproveCompleted, null);

                    if (!_certificate.IsInvoiced.GetValueOrDefault() && _certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                        _context.SendEmailLoNoBorderFees(_certificate.EntryPointId.GetValueOrDefault(), _certificate.Sequential);

                    RaiseOnSaveCompleted();
                }
                _context.GetOfficeByCertificateId(Certificate.OfficeId.GetValueOrDefault(), LoadOfficeCompleted, null);
            });
        }

        /// <summary>
        /// Execute when an office was loaded
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void LoadOfficeCompleted(InvokeOperation<Office> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                _office = operation.Value;
                _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(Certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);

            });
        }

        /// <summary>
        /// Execute when a documents were loaded
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void LoadDocumentCompleted(LoadOperation<Document> operation)
        {
            HandleLoadOperation(operation, () =>
            {
                _currentDocument = null;
                DocumentList.Clear();
                foreach (var document in operation.Entities.OrderBy(x => x.IsSupporting).ThenBy(x => x.CreationDate))
                    DocumentList.Add(document);

                foreach (Document item in DocumentList)
                {
                    item.PropertyChanged += DocumentPropertyChanged;
                }

                _context.Load(_context.GetReleaseNotesByCertificateIdQuery(Certificate.CertificateId), LoadBehavior.RefreshCurrent, GetReleaseNotesByCertificateIdCompleted, null);
            });
        }

        /// <summary>
        /// Handle property changed event
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void DocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Checked")
            {
                if (DocumentList != null)
                {
                    IsDeleteDocumentVisible = DocumentList.Any(x => x.Checked && x.DocumentType.GetValueOrDefault() != DocumentTypeEnum.Certificate)
                        && DocumentList.Count(x => x.Checked && x.DocumentType.GetValueOrDefault() == DocumentTypeEnum.Certificate) == 0
                        && IsUploadSuppDocumentsEnable;
                }
            }
        }


        /// <summary>
        /// Check all documents
        /// </summary>
        private void CheckAllDocumentsAction()
        {
            if (DocumentList != null)
            {
                foreach (Document item in DocumentList)
                {
                    item.Checked = CheckAllDocuments;
                }
            }
        }

        /// <summary>
        /// Check all release notes
        /// </summary>
        private void CheckAllReleaNoteList()
        {
            if (ReleaseNotesList != null && ReleaseNotesList.Count > 0)
            {
                foreach (ReleaseNote item in ReleaseNotesList)
                {
                    item.Checked = CheckAllReleaseNotes;
                }
            }
        }

        /// <summary>
        /// Execute delete documents command
        /// </summary>
        private void ExecuteDeleteDocumentsCommand()
        {
            int selectedDocuments = DocumentList.Count(x => x.Checked);
            string message = selectedDocuments == 1 ? Strings.DeleteMultipleDocumentsQuestionDocOne : string.Format(Strings.DeleteMultipleDocumentsQuestion, selectedDocuments);
            QuestionDisplay(message, () =>
            {
                IsBusy = true;
                int[] selectedIds = DocumentList.Where(x => x.Checked).Select(x => x.DocumentId).ToArray();
                _documentcontext.DeleteSelectedDocuments(selectedIds, CompletedDeleteSelectedDocuments, null);
            }, () => CheckAllDocuments = false);
        }

        /// <summary>
        /// Callback method for DeleteSelectedDocuments
        /// </summary>
        /// <param name="operation">Operation</param>
        private void CompletedDeleteSelectedDocuments(InvokeOperation<ValidationMessage> operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                CheckAllDocuments = false;
                IsBusy = false;
                ValidationMessage message = operation.Value;
                //will show messages if exists
                if (message.Status != StatusProcess.Success)
                {
                    AlertDisplay(message.Message);
                }
                _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(Certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);
            });
        }

        /// <summary>
        /// Execute when release notes list was loaded
        /// </summary>
        /// <param name="operation"></param>
        private void GetReleaseNotesByCertificateIdCompleted(LoadOperation<ReleaseNote> operation)
        {
            HandleLoadOperation(operation, () => 
            {
                ReleaseNotesList.Clear();
                foreach (var item in operation.Entities)
                    ReleaseNotesList.Add(item);
                //atach PropertyChanged of each item to a delegate for handling it
                foreach (ReleaseNote item in ReleaseNotesList)
                {
                    item.PropertyChanged += ReleaseNotePropertyChanged;
                }

                TotalNetWeight = string.Format(Strings.TotalNetWeight, ReleaseNotesList.Sum(x => x.NetWeight.GetValueOrDefault()));
                GlobalAccessor.Instance.MessageStatus = Strings.Ready;
                IsBusy = false;
            });
        }

        /// <summary>
        /// Execute PropertyChanged event
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Event arguments</param>
        private void ReleaseNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDeleteReleaseNoteVisible = ReleaseNotesList.Any(x => x.Checked) && _certificate.WorkflowStatusId == WorkflowStatusEnum.Ongoing;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Method executed when VerifyRequiredDocumentsInCertificate is completed
        /// </summary>
        /// <param name="operation">operation result</param>
        private void VerifyRequiredDocumentsInCertificateCompleted(InvokeOperation<bool> operation)
        {
            //if the certificate has required documents, the system will continue with the process
            if (operation.Value)
            {
                switch (operation.UserState.ToString())
                {
                    case "Approve":
                        //show a question message
                        QuestionDisplay(Strings.CertificateApproveMessage, delegate
                        {
                            _context.ApproveCertificate(_certificate.CertificateId, App.CurrentUser.Name, ApproveCertificateCompleted, null);
                        }, () => IsBusy = false);
                        break;
                    case "Request":
                        _context.VerifyIssuersInUserOffice(App.CurrentUser.OfficeId, VerifyIssuersInUserOfficeCompleted, operation.UserState);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                IsBusy = false;
                switch (operation.UserState.ToString())
                {
                    case "Approve":
                        //show an alert message if the validation is not ok.
                        AlertDisplay(Strings.CertificateDocumentValidator);
                        break;
                    case "Request":
                        //show an alert message if the validation is not ok.
                        AlertDisplay(Strings.CertificateDocumentRequestValidator);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Callback method for VerifyIssuersInUser
        /// </summary>
        /// <param name="operation">Operation</param>
        private void VerifyIssuersInUserOfficeCompleted(InvokeOperation<bool> operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                if (operation.Value)
                {
                    //show a question message
                    QuestionDisplay(Strings.CertificateRequestMessage, delegate
                    {
                        //if the answer is yes, the system will change the workflow status to requested.
                        Certificate.WorkflowStatusId = WorkflowStatusEnum.Requested;
                        Save();
                        //Save tracking
                        _context.SaveCertificateTrancking(Certificate.CertificateId, TrackingStatusEnum.Requested, CompletedSaveCertificateTrancking, operation.UserState);
                    }, () => IsBusy = false);
                }
                else
                {
                    //show a question message
                    QuestionDisplay(Strings.NoIssuersRequestForApproval, delegate
                    {
                        //if the answer is yes, the system will change the workflow status to requested.
                        Certificate.WorkflowStatusId = WorkflowStatusEnum.Requested;
                        Save();
                        //Save tracking
                        _context.SaveCertificateTrancking(Certificate.CertificateId, TrackingStatusEnum.Requested);
                    }, () => IsBusy = false);
                }
            });
        }

        /// <summary>
        /// Callback method when SaveCertificateTrancking method completes the task
        /// </summary>
        /// <param name="operation">Invoke operation</param>
        private void CompletedSaveCertificateTrancking(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                if (operation.UserState != null && operation.UserState.ToString() == "Request")
                {
                    //send the email
                    _context.SendEmailRequest(_certificate.ComdivNumber, App.CurrentUser.OfficeId, SendEmailRequestCompleted, null);
                }
                else if (operation.UserState != null && operation.UserState.ToString() == "Reject")
                {
                    //send confirmation email
                    _context.SendEmailReject(Certificate.Sequential, App.CurrentUser.OfficeId, SendEmailRejectCompleted, null);
                }
            });
        }

        /// <summary>
        /// Callback for SendEmailRequest method
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void SendEmailRequestCompleted(InvokeOperation<string> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                string result = operation.Value;
                if (!string.IsNullOrEmpty(result))
                {
                    AlertDisplay(result);
                }   
            });
        }

        /// <summary>
        /// Method is executed when ApproveCertificate is completed
        /// </summary>
        /// <param name="operation">operation result</param>
        private void ApproveCertificateCompleted(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                string errorMessage = operation.Value.ToString();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    AlertDisplay(errorMessage);
                    return;
                }
                //reload the certificate
                _context.Load(_context.GetCertificateByCertificateIdQuery(_certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadCertificateCompleted, "Approve");

            });

        }

        /// <summary>
        /// Callback method for SendEmailApprove
        /// </summary>
        /// <param name="operation">Operation</param>
        private void SendEmailApproveCompleted(InvokeOperation<string> operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                string errorMessage = operation.Value.ToString();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    AlertDisplay(errorMessage);
                }

                GlobalAccessor.Instance.MessageStatus = Strings.CertificateUpdateStatusMessage;
            });
        }

        

        private void RaiseOnSaveCompleted()
        {
            HasChanges = false;
            if (OnSaveCompleted != null)
                OnSaveCompleted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set Download / Upload / !Save buttons bool 
        /// </summary>
        private void SetHeaderButtonsVisibility()
        {
            IsSaveVisible = _certificate.HasChanges || HasChanges;
            IsDownloadVisible = !_certificate.HasChanges && !HasChanges && !string.IsNullOrEmpty(_certificate.ComdivNumber);
            IsUploadVisible = !_certificate.HasChanges && !HasChanges && !string.IsNullOrEmpty(_certificate.ComdivNumber);

            if (_certificate.IsCancelled || _certificate.IsNCR && (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer) || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin)))
            {
                IsCertifInfoHeaderEnabled = false;
            }
            else
            {
                IsCertifInfoHeaderEnabled = true;
            }

            IsComdivNumberEnabled = (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer)
                                                || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin));

            IsFOBValueEnable = (_certificate.CertificateStatusId == CertificateStatusEnum.Conform ||
                _certificate.CertificateStatusId == CertificateStatusEnum.New  ||
                _certificate.CertificateStatusId == CertificateStatusEnum.NonConform) &&
                                                (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer)
                                                || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin));

            OnPropertyChanged("IsSaveVisible");
            OnPropertyChanged("IsCertifInfoHeaderEnabled");
            OnPropertyChanged("IsDownloadVisible");
            OnPropertyChanged("IsUploadVisible");
        }

        /// <summary>
        /// Set screen buttons bool 
        /// </summary>
        private void SetScreenButtonsVisibility()
        {
            IsRequestVisible = !_certificate.HasChanges && !HasChanges && _certificate.CanBeRequested && CanEditRoUser
                                && App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)
                                ? true : false;

            IsRejectVisible = !_certificate.HasChanges && !HasChanges && _certificate.CanBeRejected && CanEditRoUser &&
                                !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)
                                && (App.CurrentUser.IsInRole(UserRoleEnum.Issuer)
                                || App.CurrentUser.IsInRole(UserRoleEnum.Admin)
                                || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin))
                                ? true : false;

            IsApproveVisible = !_certificate.HasChanges && !HasChanges && _certificate.CanBeApproved && CanEditRoUser
                                && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)
                                && (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin)
                                || App.CurrentUser.IsInRole(UserRoleEnum.Issuer))
                                ? true : false;

            IsPublishVisible = !_certificate.HasChanges && !HasChanges && _certificate.CanBePublished && CanEditRoUser
                                && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)
                                && (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin)
                                || App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer))
                                ? true : false;

            IsRecallVisible = (!_certificate.HasChanges && !HasChanges && _certificate.CanBeRecalled && CanEditRoUser
                                && App.CurrentUser.IsIssuerOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
                                ? true : false;

            IsCloseVisible = !_certificate.HasChanges && !HasChanges && _certificate.CanBeClosed && CanEditRoUser
                                && App.CurrentUser.IsBorderAgentOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)
                                ? true : false;

            IsUncloseVisible = !_certificate.HasChanges && !HasChanges && _certificate.CanBeUnclosed && CanEditRoUser
                                && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor) &&
                                (_certificate.CertificateStatusId == CertificateStatusEnum.Conform && (App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent) || App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin)) 
                                || (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin)))
                                ? true : false;

            IsDeleteVisible = !_certificate.HasChanges && !HasChanges && _certificate.CanBeDeleted && CanEditRoUser
                                && App.CurrentUser.IsCoordinatorOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)
                                ? true : false;

            IsNewReleaseNoteVisible = App.CurrentUser.IsBorderAgentOrSuperior && CanEditRoUser
                                && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor) &&
                                (_certificate.WorkflowStatusId == WorkflowStatusEnum.Approved || _certificate.WorkflowStatusId == WorkflowStatusEnum.Ongoing)
                                && _certificate.CertificateStatusId == CertificateStatusEnum.Conform
                                ? true : false;

            IsuncancelledVisible = !_certificate.HasChanges && !HasChanges && _certificate.CertificateStatusId == CertificateStatusEnum.Cancelled && !_certificate.IsPublished;

            IsSyncComdivVisible = !_certificate.HasChanges && !HasChanges && !_certificate.IsSynchronized.GetValueOrDefault()
                  && ((_certificate.WorkflowStatusId >= WorkflowStatusEnum.Approved && _certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                 ||(_certificate.WorkflowStatusId == WorkflowStatusEnum.Closed && _certificate.CertificateStatusId == CertificateStatusEnum.NonConform)) 
                 && !(App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent) || App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Supervisor));

            OnPropertyChanged("IsRequestVisible");
            OnPropertyChanged("IsApproveVisible");
            OnPropertyChanged("IsRejectVisible");
            OnPropertyChanged("IsPublishVisible");
            OnPropertyChanged("PublishText");
            OnPropertyChanged("IsCloseVisible");
            OnPropertyChanged("IsUncloseVisible");
            OnPropertyChanged("IsDeleteVisible");
            OnPropertyChanged("IsNewReleaseNoteVisible");
            OnPropertyChanged("EditReleaseNoteText");
            

            IsReleaseVisible = _certificate.WorkflowStatusId == WorkflowStatusEnum.Approved 
                || _certificate.WorkflowStatusId == WorkflowStatusEnum.Ongoing
                || _certificate.WorkflowStatusId == WorkflowStatusEnum.Closed;
        }

        /// <summary>
        /// Method that validate if the certificate information can be edited depending on the work flow and user role
        /// </summary>
        /// <returns></returns>
        private bool CanEditCertificateInformation()
        {

            if ((App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || 
                App.CurrentUser.IsInRole(UserRoleEnum.Issuer)) && _certificate.CanBeEdited)
            {
                return true;
            }
            else if (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) && _certificate.CanBeEditedCoordinator)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// Method that validate if support documents can be uploaded
        /// </summary>
        /// <returns></returns>
        private bool EnableSupportDocsUploading()
        {

            if (!(App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent) || App.CurrentUser.IsInRole(UserRoleEnum.LOAdmin) || App.CurrentUser.IsInRole(UserRoleEnum.Supervisor)) 
                && !_certificate.IsPublished)
            {
                return true;
            }            
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Verify if the screen is read only mode
        /// </summary>
        private void VerifyReadOnlyMode()
        {
            bool canEditCertifInfo = CanEditCertificateInformation() && CanEditRoUser;
            bool isSuppDocumentsEnabled = EnableSupportDocsUploading() && CanEditRoUser;

            IsTypeCertifRadioButtomVisible = canEditCertifInfo;// || (!canEditCertifInfo && _certificate.IsCancelled);
            IsTypeCertifTextVisible = !(IsTypeCertifRadioButtomVisible);
            OnPropertyChanged("IsTypeCertifRadioButtomVisible");
            OnPropertyChanged("IsTypeCertifTextVisible");

            IsCertifInfoHeaderEnabled = !(
                                        _certificate.IsCancelled ||
                                            (_certificate.IsNCR &&
                                                (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer) 
                                                || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin)))
                                        ) && canEditCertifInfo;

            IsComdivNumberEnabled = canEditCertifInfo &&  
                                                (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer) 
                                                || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin));

            IsFOBValueEnable = canEditCertifInfo &&  
                (_certificate.CertificateStatusId == CertificateStatusEnum.Conform ||
                _certificate.CertificateStatusId == CertificateStatusEnum.NonConform) &&
                                                (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) || App.CurrentUser.IsInRole(UserRoleEnum.Issuer)
                                                || App.CurrentUser.IsInRole(UserRoleEnum.Admin) || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin));

            IsUploadSuppDocumentsEnable = isSuppDocumentsEnabled;
            OnPropertyChanged("IsCertifInfoHeaderEnabled");
            OnPropertyChanged("IsUploadSuppDocumentsEnable");

            IsDownloadVisible = !_certificate.IsCancelled && canEditCertifInfo;
            IsUploadVisible = !_certificate.IsCancelled && canEditCertifInfo;
            IsSyncStackPanelVisible = App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin) ? false : true;
            OnPropertyChanged("IsDownloadVisible");
            OnPropertyChanged("IsUploadVisible");                       

            if (ReadOnlyModeChanged != null)
                ReadOnlyModeChanged(this, new EventArgs());
        }

        /// <summary>
        /// This method is executed when any property of certificate has been changed.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Certificate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (_certificate.IsCancelled)
            {
                SelectedEntryPoint = null;
                IsInvoiced = null;
                FOBValue = null;
            }
            else if (_certificate.IsNCR &&
                (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator) ||
                App.CurrentUser.IsInRole(UserRoleEnum.Issuer)
                || App.CurrentUser.IsInRole(UserRoleEnum.Admin)
                || App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin)))
            {
                SelectedEntryPoint = null;
                IsInvoiced = null;
            }
            RemoveAllErrors();
            SetHeaderButtonsVisibility();
            SetScreenButtonsVisibility();

        }

        /// <summary>
        /// Insert/Update certificate 
        /// </summary>
        private void Save()
        {
            VerifyErrors();
            if (HasErrors) return;

            HasChanges = false;
            if (_certificate.IsCancelled && CertificateType != CertificateStatusEnum.Cancelled)
            {
                QuestionDisplay(Strings.CertificateCancellEdition, delegate
                 {
                     IsBusy = true;
                     OnChangeIsBusy(IsBusy);
                     if (_certificate.EntityState == EntityState.Detached)
                     {
                         _context.Certificates.Add(_certificate);
                         IsNew = true;
                     }
                     _context.SubmitChanges(SaveCompleted, null);
                 });
            }
            else
            {
                IsBusy = true;
                OnChangeIsBusy(IsBusy);
                if (_certificate.EntityState == EntityState.Detached)
                {
                    _context.Certificates.Add(_certificate);
                    IsNew = true;
                }
                _context.SubmitChanges(SaveCompleted,null);
            }
        }

        /// <summary>
        /// Verify errors in the certificate information
        /// </summary>
        private void VerifyErrors()
        {
            RemoveAllErrors();
            if (string.IsNullOrEmpty(ComdivNumber) && _certificate.CertificateStatusId != CertificateStatusEnum.Cancelled)
                AddError("ComdivNumber", Strings.ComdivNumberMandatory);

            if (SelectedCertificateType == null)
                AddError("SelectedCertificateType", Strings.SelectedCertificateTypeMandatory);

            if (_certificate.CertificateStatusId == CertificateStatusEnum.Conform || _certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
            {
                if (string.IsNullOrEmpty(FOBValue))
                    AddError("FOBValue", Strings.FOBValueMandatory);
            }

            if (_certificate.CertificateStatusId == CertificateStatusEnum.Conform)
            {
                if (SelectedEntryPoint == null)
                    AddError("SelectedEntryPoint", Strings.EntryPointMandatory);

                if (IsInvoiced == null)
                    AddError("IsInvoiced", Strings.IsInvoicedMandatory);
            }
        }

        /// <summary>
        /// Publish/Unpublish certificate 
        /// </summary>
        private void Publish()
        {
            TrackingStatusEnum status;
            _certificate.IsPublished = !_certificate.IsPublished;
            status = _certificate.IsPublished ? TrackingStatusEnum.Published : TrackingStatusEnum.Unpublished;
            Save();
            _context.SaveCertificateTrancking(Certificate.CertificateId, status, null, null);
        }

        /// <summary>
        /// Download the corresponding template
        /// </summary>
        /// <param name="item">document item parameter</param>
        private void Download(Document item)
        {
            //if item null, downoad new word template
            if (item == null)
            {
                var serviceUri = System.Windows.Application.Current.Host.Source.AbsoluteUri;
                string currentUri = serviceUri.Replace(serviceUri.Split('/').Last(), "");
                currentUri = currentUri.Replace("ClientBin/", "");
                string fullFilePath = string.Format("{0}DownloadFile.aspx?CertificateId={1}",
                        currentUri,
                        Certificate.CertificateId);

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
            //When exist document, download this document
            else
            {
                DisplayCertificateFile(item);
            }

        }

        /// <summary>
        /// Execute when a certificate was saved
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void SaveCompleted(SubmitOperation operation)
        {
            HandleSubmitOperation(operation, () =>
            {
                GlobalAccessor.Instance.MessageStatus = Strings.CertificateUpdateStatusMessage;
                IsBusy = false;
                HasChanges = false;
                OnChangeIsBusy(IsBusy);
                SetHeaderButtonsVisibility();
                SetScreenButtonsVisibility();
                DeleteCertificateDocuments();
                VerifyReadOnlyMode();
                CertificateType = _certificate.CertificateStatusId;
                RaiseOnSaveCompleted();
            });
        }

        /// <summary>
        /// Delete mandatory documents only if the certificate is cancelled
        /// </summary>
        private void DeleteCertificateDocuments()
        {
            if (_certificate.CertificateStatusId == CertificateStatusEnum.Cancelled)
            {
                //go to delete mandatory documents
                _context.DeleteMandatoryDocuments(_certificate.CertificateId, DeleteCertificateDocumentsCompleted, null);
            }
        }

        /// <summary>
        /// Callback method for DeleteMandatoryDocuments
        /// </summary>
        /// <param name="operation">invoke operation</param>
        private void DeleteCertificateDocumentsCompleted(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                //after delete, refresh document list
                _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(Certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);
            });    
        }

        /// <summary>
        /// On completed upload document
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void UploadDone(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                bool result = (bool)operation.Value;

                if (result)
                {
                _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(_certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);
                GlobalAccessor.Instance.MessageStatus = Strings.Uploaded;
                }
                else
                {
                    AlertDisplay(Strings.SupportingFileRepeated);
                }
            });
        }


        /// <summary>
        /// On completed upload supporting document
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void UploadSupportingDone(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () =>
            {

                Document doc = operation.Value as Document;

                if (doc == null)
                {
                    _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(_certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);
                    GlobalAccessor.Instance.MessageStatus = Strings.Uploaded;
                }
                else
                {
                    if (doc.IsSupporting)
                    {
                        if (CounterDocumentFails == 0)
                            AlertDisplay(Strings.SupportingFileRepeated);
                        CounterDocumentFails++;
                    }
                    else
                    {
                        AlertDisplay(Strings.CertificateFileRepeated);
                    }
                }
            });
        }

        /// <summary>
        /// Command to edit ReleaseNote
        /// </summary>
        /// <param name="item">ReleaseNote item parameter</param>
        private void EditReleaseNote(ReleaseNote item)
        {
            if (ReleaseNoteEditionRequested != null)
            {
                item.Certificate = _certificate;
                ReleaseNoteViewModel model = new ReleaseNoteViewModel(item);
                model.SavedItem += OnReleaseNoteSaved;
                model.ReloadDocuments += ReleaseDocumentReloadDocuments;
                ReleaseNoteEditionRequested(this, new ContextEditionEventArgs<ReleaseNoteViewModel>(model));
            }
        }


        /// <summary>
        /// Command to edit document
        /// </summary>
        /// <param name="item">Document item parameter</param>
        private void EditDocument(Document item)
        {
            if (DocumentEditionRequested != null)
            {
                DocumentViewModel model = new DocumentViewModel(item);
                model.SavedItem += OnDocumentSaved;
                DocumentEditionRequested(this, new ContextEditionEventArgs<DocumentViewModel>(model));
            }
        }

        /// <summary>
        /// Document description saved
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnDocumentSaved(object sender, EventArgs e)
        {
            _documentcontext.SubmitChanges(UpdateDocumentCompleted, "Update");
        }

        /// <summary>
        /// When DeleteDocument was completed
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void UpdateDocumentCompleted(SubmitOperation operation)
        {
            HandleSubmitOperation(operation, () =>
            {
                _documentcontext.Load(_documentcontext.GetDocumentsByCertificateIdQuery(_certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadDocumentCompleted, null);
                GlobalAccessor.Instance.MessageStatus = string.Format(
                                                         (operation.UserState.ToString() == "Delete") ? Strings.DeletedSuccessfully : Strings.UpdatedSuccessfully
                                                        , Strings.Document);
            });
        }

        /// <summary>
        /// On completed recall certificate
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void RecallCertificateCompleted(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                string errorMessage = operation.Value.ToString();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    AlertDisplay(errorMessage);
                    IsBusy = false;
                    _context.Load(_context.GetCertificateByCertificateIdQuery(_certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadCertificateCompleted, null);
                    RaiseActivateDocumentsTab(); 
                    return;
                }
                _context.Load(_context.GetCertificateByCertificateIdQuery(_certificate.CertificateId), LoadBehavior.RefreshCurrent, LoadCertificateCompleted, null);
                GlobalAccessor.Instance.MessageStatus = Strings.CertificateUpdateStatusMessage;
                RaiseOnSaveCompleted();
                RaiseActivateDocumentsTab();
            });
        }

        private void RaiseActivateDocumentsTab()
        {
            if (ActivateDocumentsTab != null)
                ActivateDocumentsTab(this, EventArgs.Empty);

        }

        /// <summary>
        /// Get the list of all documents by Id 
        /// </summary>
        private void ExportDocumentsToExcel(int certificateId)
        {
            IsBusy = true;
            //get the paginated list of certificates
            _context.ExportCertificateDocuments(certificateId, ExportDocumentsToExcelCompleted, null);
        }

        /// <summary>
        /// Completed method for ExportDocumentsToExcel
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void ExportDocumentsToExcelCompleted(InvokeOperation<string> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                string path = operation.Value;
                IsBusy = false;
                //DownloadDocumentsExcelFile(path);
            });
        }

        /// <summary>
        /// Download the corresponding template
        /// </summary>
        /// <param name="item">document item parameter</param>
        private void DownloadDocumentsExcelFile(string path)
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
        /// Command to view the document associated with the ReleaseNote
        /// </summary>
        /// <param name="item">ReleaseNote item parameter</param>
        private void ViewReleaseNote(ReleaseNote item)
        {
            IsBusy = true;
            _context.GetDocumentByReleaseNoteId(item.ReleaseNoteId, CompletedViewReleaseNote, null);
        }

        /// <summary>
        /// Callback method for ViewReleaseNote
        /// </summary>
        /// <param name="operation">Operation</param>
        private void CompletedViewReleaseNote(InvokeOperation<Document> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                IsBusy = false;
                if (operation.Value != null)
                {
                    Document doc = operation.Value;
                    var serviceUri = System.Windows.Application.Current.Host.Source.AbsoluteUri;
                    string currentUri = serviceUri.Replace(serviceUri.Split('/').Last(), "");
                    currentUri = currentUri.Replace("ClientBin/", "");
                    string fullFilePath = string.Format("{0}DownloadFile.aspx?filename={1}", currentUri, doc.Filename);

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
                else
                {
                    AlertDisplay(Strings.DocumentDoesNotExist);
                }
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateViewModel"/> class. Constructor of ViewModel Screen with one argument
        /// </summary>
        /// <param name="certificateId">Identity of Certificate to edit</param>
        /// <param name="label">Label to display in tab control</param>
        public void Initialize(int certificateId, string label)
        {
            _label = label;
            IsBusy = true;
            EntryPoints = StaticReferences.GetEntryPoints();
            DocumentList = new ObservableCollection<Document>();
            _context.Load(_context.GetCertificateByCertificateIdQuery(certificateId), LoadBehavior.RefreshCurrent, LoadCertificateCompleted, null);
        }

        public void InitializeNew(Certificate certificate)
        {
            _label = "New Certificate";
            Certificate = certificate;
            EntryPoints = StaticReferences.GetEntryPoints();
            DocumentList = new ObservableCollection<Document>();
            IsNew = true;
            SetHeaderButtonsVisibility();
            IsTypeCertifRadioButtomVisible = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateViewModel"/> class. 
        /// Constructor used to creates a new viewmodel to reuse its public methods but not used as view model for certificate window.
        /// For instance, used the public method request since the certificate list.
        /// </summary>
        /// <param name="context">context parameter</param>
        public void Initialize(VocContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Upload the template filled
        /// </summary>
        /// <param name="fileName">File name</param>
        public void Upload(string fileName)
        {
            //it is implemented in paralell with the copy of file in order to copy the file inclusive when this method fails.
            //It could fail in offline status and user should countinue working.
            IsBusy = true;
            _context.UploadCocDocument(Certificate.CertificateId, fileName, UploadDone, null);
        }

        /// <summary>
        /// Upload supporting document
        /// </summary>
        /// <param name="fileName">Supporting document file name</param>
        public void UploadSupportingDocument(string fileName)
        {            
            IsBusy = true;            
            _context.UploadSupportingDocument(Certificate.CertificateId, fileName, UploadSupportingDone, null);
        }


        /// <summary>
        /// Delete Current Document
        /// </summary>
        public void DoDeleteCurrentDocument()
        {
            string path = string.Concat(App.CurrentUser.FilePath, _currentDocument.FilePath, _currentDocument.Filename).Replace("/", "\\");
            if (!File.Exists(@path))
            {
                AlertDisplay(string.Concat(Strings.FileNotExist, _currentDocument.Filename));
                return;
            }
            else
            {
                _currentDocument.IsDeleted = true;
                File.SetAttributes(@path, FileAttributes.Normal);
                File.Delete(@path);
                _documentcontext.SubmitChanges(UpdateDocumentCompleted, "Delete");
            }
        }

        /// <summary>
        /// Display certificate file
        /// </summary>
        /// <param name="item">Document file information</param>
        public void DisplayCertificateFile(Document item)
        {
            if (item == null)
            {
                AlertDisplay(Strings.CertificateNotHaveDocumentDisplay);
                return;
            }
            string path = string.Concat(App.CurrentUser.FilePath, item.FilePath, item.Filename).Replace("/", "\\"); 
            //Check if it is border agent and try to open a document of different entry point, return document using a webpage
            bool isBorderAgent = App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin);
            bool isFileFromServer = false;
            //This applies for border agents less agents of LO entrypoint. LO entry point always open files from its machine
            if (isBorderAgent && !App.CurrentUser.IsEntryPointLo)
            {
                if (App.CurrentUser.EntryPointId.HasValue && Certificate.EntryPointId.HasValue)
                {
                    if (App.CurrentUser.EntryPointId != Certificate.EntryPointId)
                    {
                        path = GetUrlToOpenFileFromServer(item, path);
                        isFileFromServer = true;
                    }
                }
            }
            if (!isFileFromServer)
            {
                if (!File.Exists(@path))
                {
                    if (isBorderAgent)
                    {
                        path = GetUrlToOpenFileFromServer(item, path);
                        isFileFromServer = true;
                    }
                    else
                    {
                    AlertDisplay(string.Concat(Strings.FileNotExist, item.Filename));
                    return;
                }
            }
            }
            if (App.Current.IsRunningOutOfBrowser)
            {
                MyHyperlinkButton button = new MyHyperlinkButton();
                button.NavigateUri = new Uri(path);
                button.TargetName = "_blank";
                button.ClickMe();
            }
            else
                //this option not work for in browser
                System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(path), "_blank");

            if (isFileFromServer)
                GlobalAccessor.Instance.MessageStatus = string.Format(Strings.OpeningDocumentFromServer, item.Filename);
            else
            GlobalAccessor.Instance.MessageStatus = string.Format(Strings.OpeningDocument, item.Filename);
        }

        private static string GetUrlToOpenFileFromServer(Document item, string path)
        {
            var serviceUri = System.Windows.Application.Current.Host.Source.AbsoluteUri;
            string currentUri = serviceUri.Replace(serviceUri.Split('/').Last(), "");
            currentUri = currentUri.Replace("ClientBin/", "");
            path = string.Format("{0}DownloadFile.aspx?DocumentId={1}",
                    currentUri,
                   item.DocumentId
                    );
            return path;
        }

        /// <summary>
        /// Approve the Certificate
        /// </summary>
        public void ExecuteApproveCommand()
        {
            //show an alert message if the user has not signatue assigned.
            if (!App.CurrentUser.HasSignature)
            {
                AlertDisplay(Strings.CertificateApproveSignatureValidationError);
                return;
            }

            //show an alert message if the certificate has not entry point selected and invoced setted.
            if (_certificate.EntryPointId == null && _certificate.IsInvoiced == null && !_certificate.IsNCR && string.IsNullOrEmpty(_certificate.ComdivNumber))
            {
                AlertDisplay(string.Format(Strings.CertificateApproveValidationErrorPlural, Strings.EntryPoint + "\" and \"" + Strings.Invoiced + "\" and \"" + Strings.ComdivNumber));
                return;
            }
            else if (string.IsNullOrEmpty(_certificate.ComdivNumber))
            {
                AlertDisplay(Strings.ComdivNumberMandatoryMessage);
                return;
            }
            else if (_certificate.EntryPointId == null && !_certificate.IsNCR)
            {
                AlertDisplay(string.Format(Strings.CertificateApproveValidationError, Strings.EntryPoint));
                return;
            }
            else if (_certificate.IsInvoiced == null && !_certificate.IsNCR)
            {
                AlertDisplay(Strings.NotInvoiceMessage);
                return;
            }
            IsBusy = true;
            _context.VerifyRequiredDocumentsInCertificate(_certificate.CertificateId, VerifyRequiredDocumentsInCertificateCompleted, "Approve");
        }

        /// <summary>
        /// Execute the click event for the request button
        /// </summary>
        public void ExecuteRejectCommand()
        {
            //show a question message
            QuestionDisplay(Strings.CertificateRejectMessage, delegate
            {
                //if the answer is yes, the system will change the workflow status to rejected.
                Certificate.WorkflowStatusId = WorkflowStatusEnum.Rejected;
                Save();
                _context.SaveCertificateTrancking(Certificate.CertificateId, TrackingStatusEnum.Rejected, CompletedSaveCertificateTrancking, "Rejected");
                
            });
        }

        /// <summary>
        /// Callback method for SendEmailReject
        /// </summary>
        /// <param name="operation">Operation</param>
        private void SendEmailRejectCompleted(InvokeOperation<string> operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                string result = operation.Value;
                if (!string.IsNullOrEmpty(result))
                    AlertDisplay(result);
            });
        }

        /// <summary>
        /// Execute the click event for the request button
        /// </summary>
        public void ExecuteRequestCommand()
        {
            _context.VerifyRequiredDocumentsInCertificate(_certificate.CertificateId, VerifyRequiredDocumentsInCertificateCompleted, "Request");
        }

        /// <summary>
        /// Recall the Certificate
        /// </summary>
        public void ExecuteRecallCommand()
        {
            if (_certificate.IsPublished == true)
            {
                AlertDisplay(Strings.CertificateRecallPublishValidationError);
                return;
            }
            //show a question message
            QuestionDisplay(Strings.CertificateRecallMessage, delegate
            {
                _context.RecallCertificate(_certificate.CertificateId, App.CurrentUser.Name, RecallCertificateCompleted, null);
            });
        }

        /// <summary>
        /// Change the workflow status of the certificate to close
        /// </summary>
        public void ExecuteCloseCommand()
        {
            //show a question message
            QuestionDisplay(Strings.CloseQuestionMessage, delegate
            {
                Certificate.WorkflowStatusId = WorkflowStatusEnum.Closed;
                Save();
                _context.SaveCertificateTrancking(Certificate.CertificateId, TrackingStatusEnum.Closed, CompletedSaveCertificateTrancking, null);
            });
        }

        /// <summary>
        /// Change the workflow status of the certificate to Unclose
        /// </summary>
        public void ExecuteUncloseCommand()
        {
            if (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) && _certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
            {
                if (_certificate.IsPublished == true)
                {
                    AlertDisplay(Strings.CertificateNCRUnclosePublished);
                    return;
                }
                else
                {
                    //show a question message
                    QuestionDisplay(Strings.UnCloseQuestionMessage, delegate
                    {
                        _context.RecallCertificate(_certificate.CertificateId, App.CurrentUser.Name, RecallCertificateCompleted, null);
                        
                    });
                }
                
            }
            else
            {
                //show a question message
                QuestionDisplay(Strings.UnCloseQuestionMessage, delegate
                {

                    Certificate.WorkflowStatusId = WorkflowStatusEnum.Ongoing;
                    Save();
                    _context.SaveCertificateTrancking(Certificate.CertificateId, TrackingStatusEnum.Ongoing, null, null);
                });
            }
            
        }


        /// <summary>
        /// Change the workflow status of the certificate to Delete
        /// </summary>
        public void ExecuteDeleteCommand()
        {
            //show a question message
            QuestionDisplay(Strings.DeleteQuestionMessage, delegate
            {
                IsBusy = true;
                OnChangeIsBusy(IsBusy);
                _context.DeleteCertificate(_certificate.CertificateId, App.CurrentUser.Name, DeleteCertificateCompleted, null);
            });
        }
        
        /// <summary>
        /// Method is executed when DeleteCertificate is completed
        /// </summary>
        /// <param name="operation">operation result</param>
        private void DeleteCertificateCompleted(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                IsBusy = false;
                OnChangeIsBusy(IsBusy);

                string errorMessage = operation.Value.ToString();
                if (!string.IsNullOrEmpty(errorMessage))
                {                    
                    ErrorWindow.CreateNew(errorMessage);
                    return;
                }

                GlobalAccessor.Instance.MessageStatus = Strings.CertificateDeleteStatusMessage;
                RaiseOnSaveCompleted();
                if (OnDeleteCompleted != null)
                    OnDeleteCompleted(this, EventArgs.Empty);
            });


        }
        
        
        #endregion

    }

    /// <summary>
    /// Class created to call a web page using out of browser
    /// </summary>
    public class MyHyperlinkButton : System.Windows.Controls.HyperlinkButton
    {
        /// <summary>
        /// Go to web page set
        /// </summary>
        public void ClickMe()
        {
            base.OnClick();
        }
    }


}
