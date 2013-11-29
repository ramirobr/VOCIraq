using System;
using System.Linq;
using System.Collections.Generic;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using System.ComponentModel;
using System.Windows.Input;
using Cotecna.Voc.Silverlight.Web.Services;
using System.ServiceModel.DomainServices.Client;
using System.Collections.ObjectModel;

namespace Cotecna.Voc.Silverlight
{
    public class ReleaseNoteViewModel: ViewModelChildWindow<ReleaseNote>
    {
        #region events
        public event EventHandler ReloadDocuments;
        #endregion

        #region Private Fields
        private ReleaseNote _releaseNote;
        private List<ResultEnum> _resultList;
        private List<NoteIssuedEnum> _noteIssuedList;
        private bool _isPrintVisible;
        private bool _isSaveVisible;
        private ICommand _printCommand;
        private VocContext _proxy = new VocContext();
        private string _releaseNoteTitleWindow;
        private string _releaseNoteInstructionsText;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets IsNewReleaseNote
        /// </summary>
        public bool IsNewReleaseNote { get; set; }

        /// <summary>
        /// ReleaseNote to edit
        /// </summary>
        public ReleaseNote ReleaseNote
        {
            get
            {
                return _releaseNote;
            }
            set
            {
                if (_releaseNote != value)
                {
                    _releaseNote = value;
                    OnPropertyChanged("ReleaseNote");
                    _releaseNote.PropertyChanged += ReleaseNotePropertyChanged;
                }
            }
        }

        /// <summary>
        /// List of results
        /// </summary>
        public List<ResultEnum> ResultList
        {
            get
            {
                return _resultList;
            }
            set
            {
                if (_resultList != value)
                {
                    _resultList = value;
                    OnPropertyChanged("ResultList");
                }
            }
        }


        /// <summary>
        /// List of Note Issued
        /// </summary>
        public List<NoteIssuedEnum> NoteIssuedList
        {
            get
            {
                return _noteIssuedList;
            }
            set
            {
                if (_noteIssuedList != value)
                {
                    _noteIssuedList = value;
                    OnPropertyChanged("NoteIssuedList");
                }
            }
        }


        /// <summary>
        /// Show or hide Print button
        /// </summary>
        public bool IsPrintVisible
        {
            get
            {
                return _isPrintVisible;
            }
            set
            {
                _isPrintVisible = value;
                OnPropertyChanged("IsPrintVisible");
            }
        }

        /// <summary>
        /// Enabled / Disabled all fields
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _releaseNote.Certificate.WorkflowStatusId != WorkflowStatusEnum.Closed
                    && App.CurrentUser.IsBorderAgentOrSuperior && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);
            }
        }

        
        /// <summary>
        /// Gets or sets IsSaveVisible
        /// </summary>		
        public bool IsSaveVisible
        {
            get
            {
                return _isSaveVisible;
            }
            set
            {
                _isSaveVisible = value;
                OnPropertyChanged("IsSaveVisible");
            }
        }

        /// <summary>
        /// Print command
        /// </summary>
        public ICommand PrintCommand
        {
            get 
            {
                if (_printCommand == null)
                    _printCommand = new DelegateCommand(ExecutePrintCommand);
                return _printCommand; 
            }
        }

        
        /// <summary>
        /// Gets/ Sets title of the Release Note Window
        /// </summary>		
        public string ReleaseNoteTitleWindow
        {
            get
            {
                return _releaseNoteTitleWindow;
            }
            set
            {
                if (_releaseNoteTitleWindow == value) return;
                _releaseNoteTitleWindow = value;
                OnPropertyChanged("ReleaseNoteTitleWindow");
            }
        }

        
        /// <summary>
        /// Gets / Sets Release Note instructions text
        /// </summary>		
        public string ReleaseNoteInstructionsText
        {
            get
            {
                return _releaseNoteInstructionsText;
            }
            set
            {
                if (_releaseNoteInstructionsText == value) return;
                _releaseNoteInstructionsText = value;
                OnPropertyChanged("ReleaseNoteInstructionsText");
            }
        }

        /// <summary>
        /// Gets Total Quantiy
        /// </summary>		
        public decimal TotalQuantity
        {
            get
            {
                return _releaseNote.ReceivedQuantity.GetValueOrDefault() + _releaseNote.RemainingQuantity.GetValueOrDefault();
            }
        }

        private List<ShipmentTypeEnum> _shipmentTypeList;
        /// <summary>
        /// Gets or sets ShipmentTypeList
        /// </summary>		
        public List<ShipmentTypeEnum> ShipmentTypeList
        {
            get
            {
                return _shipmentTypeList;
            }
            set
            {
                if (_shipmentTypeList == value) return;
                _shipmentTypeList = value;
                OnPropertyChanged("ShipmentTypeList");
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of ViewModel
        /// </summary>
        /// <param name="releaseNote">Release item</param>
        public ReleaseNoteViewModel(ReleaseNote releaseNote)
            : base(releaseNote)
        {
            ResultList = new List<ResultEnum>();
            NoteIssuedList = new List<NoteIssuedEnum>();
            ShipmentTypeList = new List<ShipmentTypeEnum>();
            ReleaseNote = releaseNote;
            AreValidationErrorsWatched = true;
            InitialzeData();
            IsPrintVisible = releaseNote.PartialNumber != null && IsEnabled && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);

            SelectedSecurityPapersNumbers = "Empty list...";
            IssuedSecurityPapers = new List<SecurityPaper>();
            SecurityPapers = new ObservableCollection<CheckableSecurityPaper>();
            LoadNonUsedSecurityPapers();
            LoadSecurityPapersOnRN();
            ChangesFromCheckableControl = false;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Execute print command
        /// </summary>
        private void ExecutePrintCommand()
        {
            IsBusy = true;
            IsPrintVisible = false;
            _proxy.GenerateReleaseNoteReport(_releaseNote.ReleaseNoteId, CompletedGenerateReleaseNoteReport, null);
        }

        /// <summary>
        /// Callback method for GenerateReleaseNoteReport
        /// </summary>
        /// <param name="operation">Operation</param>
        private void CompletedGenerateReleaseNoteReport(InvokeOperation<ValidationMessage> operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                IsBusy = false;
                ValidationMessage message = operation.Value;
                if (message.Status == StatusProcess.Success)
                {
                    string path = message.Identifier;
                    
                    var serviceUri = System.Windows.Application.Current.Host.Source.AbsoluteUri;
                    string currentUri = serviceUri.Replace(serviceUri.Split('/').Last(), "");
                    currentUri = currentUri.Replace("ClientBin/", "");
                    string fullFilePath = string.Format("{0}DownloadFile.aspx?filename={1}", currentUri, path);

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

                    if (ReloadDocuments != null)
                        ReloadDocuments(this, EventArgs.Empty);
                    IsPrintVisible = true;
                }
                else
                {
                    AlertDisplay(message.Message);
                }
            });
        }

        /// <summary>
        /// Is executed when a property change
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void ReleaseNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SavePrintButtonsVisibilityRefresh();

            if (e.PropertyName == "ReceivedQuantity" || e.PropertyName == "RemainingQuantity")
            {
                OnPropertyChanged("TotalQuantity");
            }
        }

        /// <summary>
        /// Initialze the data
        /// </summary>
        private void InitialzeData()
        {
            //retrieve the list of Result
            Enum.GetValues(typeof(ResultEnum)).Cast<ResultEnum>().OrderBy(x => x.ToString()).ToList().ForEach(data =>
            {
                ResultList.Add(data);
            });

            //retrieve the Note issued List
            Enum.GetValues(typeof(NoteIssuedEnum)).Cast<NoteIssuedEnum>().OrderBy(x => x.ToString()).ToList().ForEach(data =>
            {
                NoteIssuedList.Add(data);
            });

            //retrieve the Note issued List
            Enum.GetValues(typeof(ShipmentTypeEnum)).Cast<ShipmentTypeEnum>().OrderBy(x => x.ToString()).ToList().ForEach(data =>
            {
                ShipmentTypeList.Add(data);
            });


            if (App.CurrentUser.IsInRole(UserRoleEnum.Supervisor) || !IsEnabled)
            {
                ReleaseNoteTitleWindow = Strings.ViewReleaseNote;
                ReleaseNoteInstructionsText = Strings.ViewReleaseNoteInstructions;
            }
            else
            {
                ReleaseNoteTitleWindow = Strings.EditReleaseNote;
                ReleaseNoteInstructionsText = Strings.EditReleaseNoteInstructions;
            }
        }
        #endregion

        #region Public Methods
        public void SavePrintButtonsVisibilityRefresh()
        {
            if (ChangesFromCheckableControl && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                IsSaveVisible = IsEnabled;
                IsPrintVisible = !IsSaveVisible && IsEnabled;

                if (IsSaveVisible)
                    HasChanges = true;
            }
            else if (_releaseNote.EntityState == EntityState.Detached && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                IsPrintVisible = !HasChanges && IsEnabled;
                IsSaveVisible = HasChanges && IsEnabled;
            }
            else
            {
                IsPrintVisible = !_releaseNote.HasChanges && IsEnabled && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);
                IsSaveVisible = _releaseNote.HasChanges && IsEnabled && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor);
            }
        }
        #endregion

        #region Protected Methods
        protected override void ValidateData()
        {
            base.ValidateData();

            // Security papers are mandatory
            if (IssuedSecurityPapers.Count == 0)
            {
                if (CheckableControlChanged != null)
                    CheckableControlChanged(this, null);

                HasChanges = true;
                ChangesFromCheckableControl = true;
                SavePrintButtonsVisibilityRefresh();
            }
            else
                HasValidationErrors = false;
        }
        #endregion


        #region CheckAbleSecurityPapers

        /// <summary>
        /// flag to know if changes on the UI come from the Available Security Papers(Expander control)
        /// </summary>
        public bool ChangesFromCheckableControl {get; set;}

        private string _selectedSecurityPapers;
        /// <summary>
        /// Sets the selected security papers numbers concatenated as string, for UI purposes
        /// </summary>
        public string SelectedSecurityPapersNumbers
        {
            get
            {
                return _selectedSecurityPapers;
            }
            set
            {
                _selectedSecurityPapers = value;
                OnPropertyChanged("SelectedSecurityPapersNumbers");
            }
        }

        public string CheckableControlValidationErrorMessage
        {
            get 
            {
                return Strings.SecurityPapersMandatory.Trim();
            }
        }

        /// <summary>
        /// List of Issued Security Papers that should be updated on Saving this Release Note
        /// </summary>
        public List<SecurityPaper> IssuedSecurityPapers { get; set; }

        /// <summary>
        /// Gets or Sets the availables security papers 
        /// </summary>
        public ObservableCollection<CheckableSecurityPaper> SecurityPapers { get; set; }

        /// <summary>
        /// Fills the collection with non-issued Security Papers, availables on a particular Entry Point
        /// </summary>
        private void LoadNonUsedSecurityPapers()
        {
            IsBusy = true;
            _proxy.Load(_proxy.GetSecurityPaperListQuery(null, null, ReleaseNote.Certificate.EntryPointId.Value, false, true, false, false,null).OrderBy(p => p.SecurityPaperId),
                                                          LoadBehavior.RefreshCurrent, CompletedGetSecurityPaperListQuery, null);
        }

        /// <summary>
        /// Callback for GetSecurityPaperListQuery
        /// </summary>
        /// <param name="operation"></param>
        private void CompletedGetSecurityPaperListQuery(LoadOperation<SecurityPaper> operation)
        {
            HandleLoadOperation(operation, delegate
            {
                foreach (SecurityPaper sp in operation.Entities)
                {
                    CheckableSecurityPaper checkedSP = new CheckableSecurityPaper(sp, false);
                    checkedSP.PropertyChanged += checkableSP_PropertyChanged;

                    SecurityPapers.Add(checkedSP);
                }

                IsBusy = false;
            });
        }

        /// <summary>
        /// Load the current security papers related with this release note
        /// </summary>
        private void LoadSecurityPapersOnRN()
        {
            // if not new Release note
            if (ReleaseNote.ReleaseNoteId != 0)
            {
                IsBusy = true;
                _proxy.GetSecurityPapersByRN(ReleaseNote.ReleaseNoteId, CompletedGetSecurityPapersByRN, null);
            }
        }

        /// <summary>
        /// CallBack for GetSecurityPapersByRN
        /// </summary>
        /// <param name="operation"></param>
        private void CompletedGetSecurityPapersByRN(InvokeOperation<List<SecurityPaper>> operation)
        {
            HandleInvokeOperation(operation, delegate
            {
                SelectedSecurityPapersNumbers = string.Empty;

                foreach (SecurityPaper sp in operation.Value)
                {
                    CheckableSecurityPaper checkableSP = new CheckableSecurityPaper(sp, true);
                    checkableSP.PropertyChanged += checkableSP_PropertyChanged;

                    IssuedSecurityPapers.Add(sp);
                    SecurityPapers.Insert(0, checkableSP);
                    SelectedSecurityPapersNumbers += checkableSP.SPaper.SecurityPaperNumber.ToString() + "; ";
                }

                IsBusy = false;
            });
        }

        /// <summary>
        /// Refresh the IssuedSecurityPapers list when changing the checkable security papers
        /// </summary>
        private void RefresSelectedSecurityPapers()
        {
            SelectedSecurityPapersNumbers = string.Empty;
            IssuedSecurityPapers = new List<SecurityPaper>();

            foreach (CheckableSecurityPaper checkableSP in SecurityPapers)
            {
                if (checkableSP.IsSelected)
                {
                    IssuedSecurityPapers.Add(checkableSP.SPaper);
                    SelectedSecurityPapersNumbers += checkableSP.SPaper.SecurityPaperNumber.ToString() + "; ";
                }
            }
        }

       
        /// <summary>
        /// Event Handler for changes on checkable security papers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkableSP_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsBusy = true;

            if (e.PropertyName.CompareTo("IsSelected") == 0)
            {
                ChangesFromCheckableControl = true;
                this.HasChanges = true;
                this.SavePrintButtonsVisibilityRefresh();
                RefresSelectedSecurityPapers();

                if (CheckableControlChanged != null)
                    CheckableControlChanged(this, null);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Occurs when a checkable item in the Expander control has changed 
        /// </summary>
        public event EventHandler CheckableControlChanged;

        #endregion

    }
}


