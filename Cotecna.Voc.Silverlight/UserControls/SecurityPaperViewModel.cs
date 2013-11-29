using System;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;

namespace Cotecna.Voc.Silverlight
{
    public class SecurityPaperViewModel : ViewModel
    {
        #region events
        /// <summary>
        /// Close the windows
        /// </summary>
        public event EventHandler CloseWindow;
        /// <summary>
        /// Refresh the security paper list after save
        /// </summary>
        public event EventHandler RefeshSecurityPaperList;
        #endregion

        #region private fields
        private bool? _isVChecked;
        private string _rangeFrom;
        private string _rangeTo;
        private ObservableCollection<EntryPoint> _entryPointList;
        private int _selectedEntryPointId;
        private ICommand _saveCommand;
        private ICommand _cancelCommand;
        private VocContext _proxy = new VocContext();
        #endregion

        #region properties
        
        /// <summary>
        /// Gets or sets IsVChecked
        /// </summary>		
        public bool? IsVChecked
        {
            get
            {
                return _isVChecked;
            }
            set
            {
                _isVChecked = value;
                OnPropertyChanged(() => IsVChecked);
                HasChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets RangeFrom
        /// </summary>		
        public string RangeFrom
        {
            get
            {
                return _rangeFrom;
            }
            set
            {
                _rangeFrom = value;
                OnPropertyChanged(() => RangeFrom);
                HasChanges = true;
            }
        }
        
        /// <summary>
        /// Gets or sets RangeTo
        /// </summary>		
        public string RangeTo
        {
            get
            {
                return _rangeTo;
            }
            set
            {
                _rangeTo = value;
                OnPropertyChanged(() => RangeTo);
                HasChanges = true;
            }
        }
        
        /// <summary>
        /// Gets or sets EntryPointList
        /// </summary>		
        public ObservableCollection<EntryPoint> EntryPointList
        {
            get
            {
                return _entryPointList;
            }
            set
            {
                _entryPointList = value;
                OnPropertyChanged(() => EntryPointList);
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
                _selectedEntryPointId = value;
                OnPropertyChanged(() => SelectedEntryPointId);
                HasChanges = true;
            }
        }
        #endregion

        #region commands
        /// <summary>
        /// Gets SaveCommand
        /// </summary>
        public ICommand SaveCommand
        {
            get 
            {
                if (_saveCommand == null)
                    _saveCommand = new DelegateCommand(ExecuteSaveCommand);
                return _saveCommand; 
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
        #endregion

        #region constructor
        /// <summary>
        /// Is the instance of the class
        /// </summary>
        public SecurityPaperViewModel()
        {
            EntryPointList = new ObservableCollection<EntryPoint>(StaticReferences.GetEntryPoints());
        }
        #endregion

        #region private methods
        /// <summary>
        /// Execute Save Command
        /// </summary>
        private void ExecuteSaveCommand()
        {
            //remove errors
            RemoveAllErrors();
            //validate all
            if (ValidateData())
            {
                //take the information
                int from = int.Parse(RangeFrom);
                int to = int.Parse(RangeTo);
                string initialLetter = IsVChecked.GetValueOrDefault() ? Strings.V : Strings.P;
                IsBusy = true;
                //call to server method
                _proxy.CreateSecurityPapers(initialLetter, from, to, SelectedEntryPointId, CompletedCreateSecurityPapers, null);
            }
        }

        /// <summary>
        /// Callback method for CompletedCreateSecurityPapers
        /// </summary>
        /// <param name="operation">Operation</param>
        private void CompletedCreateSecurityPapers(InvokeOperation<ValidationMessage> operation)
        {
            HandleInvokeOperation(operation, delegate 
            {
                IsBusy = false;
                if (operation.Value.Status != StatusProcess.Success)
                {
                    //if is not ok, show a message
                    AlertDisplay(operation.Value.Message);
                }
                else
                {
                    HasChanges = false;
                    //if is ok, refresh the list of security papers
                    if (RefeshSecurityPaperList != null)
                        RefeshSecurityPaperList(this, EventArgs.Empty);

                    //close pop-up
                    ExecuteCancelCommand();
                }

            },() => IsBusy = false);
        }

        /// <summary>
        /// Execute Cancel Command
        /// </summary>
        private void ExecuteCancelCommand()
        {
            if (CloseWindow != null)
                CloseWindow(this, EventArgs.Empty);
        }

        /// <summary>
        /// Validate all fields
        /// </summary>
        /// <returns></returns>
        private bool ValidateData()
        {
            if (IsVChecked == null)
                AddError("IsVChecked", Strings.IsVChecked);
            if(string.IsNullOrEmpty(RangeFrom))
                AddError("RangeFrom", Strings.NoValueWritten);
            if (string.IsNullOrEmpty(RangeTo))
                AddError("RangeTo", Strings.NoValueWritten);
            if (SelectedEntryPointId == 0)
                AddError("SelectedEntryPointId", Strings.SelectedEntryPointId);
            return !HasErrors;
        }

        #endregion
    }
}
