using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.DomainServices.Client;
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
using Cotecna.Voc.Silverlight.Web.Services;
using System.Collections.ObjectModel;

namespace Cotecna.Voc.Silverlight
{
    public class OfficeViewModel : ViewModel
    {
        #region Private Fields
        string _label;
        private Office _office;
        private bool _isSaveVisible;
        VocContext _context;
        private string _officeHeaderText;
        private string _officeInstructionsText;
        private bool _isEnabled;
        #endregion

        #region Properties
        /// <summary>
        /// Gets text to be displayed as header of tab item
        /// </summary>
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged("Label");
            }
        }

        
        /// <summary>
        /// Gets / Sets Office
        /// </summary>		
        public Office Office
        {
            get
            {
                return _office;
            }
            set
            {
                if (_office != null)
                    _office.PropertyChanged -= Office_PropertyChanged;
                if (_office != value)
                {
                    _office = value;
                    OnPropertyChanged("Office");
                }
                _office.PropertyChanged += Office_PropertyChanged;
            }
        }

        
        
        /// <summary>
        /// Get's / Set's Office's Name
        /// </summary>		
        public string OfficeName
        {
            get
            {
                return Office.OfficeName;
            }
            set
            {
                Office.OfficeName = value;
                OnPropertyChanged("OfficeName");
            }
        }

        
        /// <summary>
        /// Get's / Set's Office's Code
        /// </summary>		
        public string OfficeCode
        {
            get
            {
                return Office.OfficeCode;
            }
            set
            {
                Office.OfficeCode = value;
                OnPropertyChanged("OfficeCode");
            }
        }

        
        /// <summary>
        /// Save button visibility
        /// </summary>		
        public bool IsSaveVisible
        {
            get
            {
                return _isSaveVisible;
            }
            set
            {
                if (_isSaveVisible == value) return;
                _isSaveVisible = value;
                OnPropertyChanged("IsSaveVisible");
            }
        }

        
        /// <summary>
        ///  Gets / Sets the header text for the Office screen
        /// </summary>		
        public string OfficeHeaderText
        {
            get
            {
                return _officeHeaderText;
            }
            set
            {
                if (_officeHeaderText == value) return;
                _officeHeaderText = value;
                OnPropertyChanged("OfficeHeaderText");
            }
        }

        
        /// <summary>
        /// Gets / Sets the instructions text for the Office screen
        /// </summary>		
        public string OfficeInstructionsText
        {
            get
            {
                return _officeInstructionsText;
            }
            set
            {
                if (_officeInstructionsText == value) return;
                _officeInstructionsText = value;
                OnPropertyChanged("OfficeInstructionsText");
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

        private List<DictionaryEnum> _officeTypeList;
        /// <summary>
        /// Gets or sets OfficeTypeList
        /// </summary>		
        public List<DictionaryEnum> OfficeTypeList
        {
            get
            {
                return _officeTypeList;
            }
            set
            {
                if (_officeTypeList == value) return;
                _officeTypeList = value;
                OnPropertyChanged("OfficeTypeList");
            }
        }

        /// <summary>
        /// Gets or sets SelectedOfficeType
        /// </summary>		
        public int SelectedOfficeType
        {
            get
            {
                return _office.OfficeType.HasValue ? (int)_office.OfficeType.Value : (int)0;
            }
            set
            {
                if (_office != null)
                {
                    _office.OfficeType = (OfficeTypeEnum)value;
                    OnPropertyChanged("Office");
                }
                OnPropertyChanged("SelectedOfficeType");
            }
        }

        private ObservableCollection<Office> _regionalOfficeList;
        /// <summary>
        /// Gets or sets RegionalOfficeList
        /// </summary>		
        public ObservableCollection<Office> RegionalOfficeList
        {
            get
            {
                return _regionalOfficeList;
            }
            set
            {
                if (_regionalOfficeList == value) return;
                _regionalOfficeList = value;
                OnPropertyChanged("RegionalOfficeList");
            }
        }

        private bool _isLocalOffice;
        /// <summary>
        /// Gets or sets IsLocalOffice
        /// </summary>		
        public bool IsLocalOffice
        {
            get
            {
                return _isLocalOffice;
            }
            set
            {
                if (_isLocalOffice == value) return;
                _isLocalOffice = value;
                OnPropertyChanged("IsLocalOffice");
            }
        }

        private bool _isRegionalOffice;
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

        public int? RegionalOfficeId
        {
            get
            {
                return _office.RegionalOfficeId;
            }
            set
            {
                _office.RegionalOfficeId = value;
                OnPropertyChanged("RegionalOfficeId");
            }
        }

        #endregion

        #region constructor
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public OfficeViewModel()
        {
            //Initialize office type list
            OfficeTypeList = new List<DictionaryEnum>();
            //Initialize regional office list
            RegionalOfficeList = new ObservableCollection<Business.Office>(StaticReferences.GetRegionalOffices());
            OfficeTypeConverter converter = new OfficeTypeConverter();
            //Add office types
            Enum.GetValues(typeof(OfficeTypeEnum)).Cast<OfficeTypeEnum>().OrderBy(x => x.ToString()).ToList().ForEach(data => 
            {
                OfficeTypeList.Add(new DictionaryEnum
                {
                    DictionaryEnumId = (int)data,
                    DictionaryEnumName = converter.Convert(data,null,null,null).ToString()
                });
            });
            OfficeTypeList.Insert(0, new DictionaryEnum { DictionaryEnumId = -1, DictionaryEnumName = " " });
            RegionalOfficeList.Insert(0, new Office { OfficeId = -1, OfficeName = " " });
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Event used to notify that save is completed
        /// </summary>
        internal event EventHandler OnSaveCompleted;

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves office's information
        /// </summary>
        private void Save()
        {
            if (ValidateData())
                return;
            IsBusy = true;

            _context.OfficeAlreadyExists(Office.OfficeId, OfficeName, OfficeCode, OfficeAlreadyExistsCompleted, null);
        }

        /// <summary>
        /// Executes when validation if the office already exists is performed
        /// </summary>
        /// <param name="operation"></param>
        private void OfficeAlreadyExistsCompleted(InvokeOperation<List<ValidationMessage>> operation)
        {
            HandleInvokeOperation(operation, () =>
            {
                if (operation.Value.Count == 1)
                {
                    // We have at least one field that already exists.
                    foreach (ValidationMessage error in operation.Value)
                    {
                        if (error.Identifier == Strings.OfficeName)
                        {
                            AlertDisplay(Strings.OfficeNameAlreadyExists);
                            return;
                        }
                        else
                        {
                            if (error.Identifier == Strings.OfficeCode)
                            {
                                AlertDisplay(Strings.OfficeCodeAlreadyExists);
                                return;
                            }
                        }

                    }

                }
                else
                {
                    if (operation.Value.Count == 2)
                    {
                        // Both fields already exist.
                        AlertDisplay(Strings.OfficeAlreadyExists);
                        return;
                    }
                }

                //remove regional office
                _context.RemoveRegionalOffice(_office.OfficeId, RemoveRegionalOfficeCompleted, null);
                
            });
        }

        /// <summary>
        /// Callback method for RemoveRegionalOffice
        /// </summary>
        /// <param name="operation">Invoke operation</param>
        private void RemoveRegionalOfficeCompleted(InvokeOperation operation)
        {
            HandleInvokeOperation(operation, () => 
            {
                // if it is a new office add it to the context
                if (Office.EntityState == EntityState.Detached)
                    _context.Offices.Add(Office);

                _context.SubmitChanges(SaveCompleted, null);
            });
        }

        /// <summary>
        /// Executes when an office was saved
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void SaveCompleted(SubmitOperation operation)
        {
            HandleSubmitOperation(operation, () =>
            {
                GlobalAccessor.Instance.MessageStatus = Strings.OfficeSaveStatusMessage;

                // Refreshes the list of offices when save is completed
                _context.Load(_context.GetOfficesQuery().OrderBy(x => x.OfficeId), LoadBehavior.RefreshCurrent, CompleteGetOfficesQuery, null);

            });
        }

        /// <summary>
        /// Executes when the office has been saved to refresh the list of offices
        /// </summary>
        /// <param name="operation">Operation result</param>
        private void CompleteGetOfficesQuery(LoadOperation<Office> operation)
        {
            HandleLoadOperation(operation, delegate 
            {
                StaticReferences.ReloadOffices(operation.Entities);
                if (OnSaveCompleted != null)
                    OnSaveCompleted(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Validates Office's data
        /// </summary>
        /// <returns></returns>
        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(Office.OfficeName))
                AddError("OfficeName", Strings.OfficeNameIsMandatory);
            else
                RemoveError("OfficeName", Strings.OfficeNameIsMandatory);

            if (string.IsNullOrEmpty(Office.OfficeCode))
                AddError("OfficeCode", Strings.OfficeCodeIsMandatory);
            else
                RemoveError("OfficeCode", Strings.OfficeCodeIsMandatory);

            if (SelectedOfficeType != (int)OfficeTypeEnum.LocalOffice && SelectedOfficeType != (int)OfficeTypeEnum.RegionalOffice)
                AddError("SelectedOfficeType", Strings.OfficeTypeMandatory);
            else
                RemoveError("SelectedOfficeType", Strings.OfficeTypeMandatory);

            return HasErrors;

        }

        /// <summary>
        /// This method is executed when any property of the Office has been changed.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Office_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetVisibility((Office.HasChanges || Office.EntityState == EntityState.Detached) && !App.CurrentUser.IsInRole(UserRoleEnum.Supervisor));
            if (e.PropertyName == "OfficeType")
            {
                if (_office.OfficeType.Value == OfficeTypeEnum.LocalOffice)
                {
                    IsLocalOffice = false;
                    IsRegionalOffice = true;
                }
                else
                {
                    _office.RegionalOfficeId = null;
                    IsLocalOffice = true;
                    IsRegionalOffice = false;
                    OnPropertyChanged("Office");
                }
            }
        }

        /// <summary>
        /// Set Visibility of save button
        /// </summary>
        /// <param name="isVisible">visible or not</param>
        private void SetVisibility(bool isVisible)
        {
            IsSaveVisible = isVisible;
        }

        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the OfficeViewModel
        /// </summary>
        /// <param name="office">Office</param>
        /// <param name="context">Context</param>
        /// <param name="isEdit">is it new or a modification ?</param>
        public void Initialize(Office office, VocContext context,bool isEdit = false)
        {
            _context = context;
            Office = office;
            IsLocalOffice = true;
            IsRegionalOffice = false;
            if (!isEdit)
                Label = Strings.NewOffice;
            else
            {
                Label = Office.OfficeName;
                if (RegionalOfficeList.Any(x => x.OfficeId == office.OfficeId))
                {
                    var elementToRemove = RegionalOfficeList.First(x => x.OfficeId == office.OfficeId);
                    RegionalOfficeList.Remove(elementToRemove);
                }

                if (_office.OfficeType.Value == OfficeTypeEnum.LocalOffice)
                {
                    IsLocalOffice = false;
                    IsRegionalOffice = true;
                }
                else
                {
                    IsLocalOffice = true;
                    IsRegionalOffice = false;
                }
            }
            
            SetVisibility(false);
            if (App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                OfficeHeaderText = Strings.ViewOffice;
                OfficeInstructionsText = Strings.ViewOfficeInstructions;
                IsEnabled = false;
            }
            else
            {
                OfficeHeaderText = Strings.EditOffice;
                OfficeInstructionsText = Strings.EditOfficeInstructions;
                IsEnabled = true;
            }
        }

        /// <summary>
        /// Sets the signature to the office
        /// </summary>
        /// <param name="signatureArray"></param>
        public void UploadOfficeSignature(byte[] signatureArray)
        {
            Office.OfficeStamp = signatureArray;
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
                    _saveCommand = new DelegateCommand(Save);
                return _saveCommand;
            }
        }
        #endregion
    }
}
