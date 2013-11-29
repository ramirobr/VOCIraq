using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;

namespace Cotecna.Voc.Silverlight
{
    public class SecurityPaperEditionViewModel : ViewModel
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
        private ObservableCollection<EntryPoint> _entryPointList;
        private int _selectedEntryPointId;
        private ICommand _saveCommand;
        private ICommand _cancelCommand;
        //private VocContext _proxy = new VocContext();
        #endregion

        #region properties
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

        private string _screenTitle;
        /// <summary>
        /// Gets or sets ScreenTitle
        /// </summary>
        public string ScreenTitle
        {
            get { return _screenTitle; }
            set 
            { 
                _screenTitle = value;
                OnPropertyChanged(() => ScreenTitle);
            }
        }

        private string _screenInstruction;
        /// <summary>
        /// Gets or sets ScreenInstruction
        /// </summary>
        public string ScreenInstruction
        {
            get { return _screenInstruction; }
            set 
            { 
                _screenInstruction = value;
                OnPropertyChanged(() => ScreenInstruction);
            }
        }
        
        /// <summary>
        /// Gets or sets SelectedSecurityPapers
        /// </summary>
        public List<SecurityPaper> SelectedSecurityPapers { get; set; }

        /// <summary>
        /// Gets or sets db context
        /// </summary>
        public VocContext Context { get; set; }

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
        public SecurityPaperEditionViewModel()
        {
            EntryPointList = new ObservableCollection<EntryPoint>(StaticReferences.GetEntryPoints());
            ScreenTitle = Strings.SecurityPaperEditionTitle;
            ScreenInstruction = Strings.SecurityPaperEditionInstruction;
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
            if (SelectedEntryPointId == 0)
            {
                AddError("SelectedEntryPointId", Strings.EntryPointMandatory);
                return;
            }

            foreach (SecurityPaper item in SelectedSecurityPapers)
            {
                item.EntryPointId = SelectedEntryPointId;
            }
            IsBusy = true;
            Context.SubmitChanges(SubmitOperationCompleted, null);
        }

        /// <summary>
        /// Callback method for SubmitChanges
        /// </summary>
        /// <param name="operation"></param>
        private void SubmitOperationCompleted(SubmitOperation operation)
        {
            HandleSubmitOperation(operation, delegate 
            {
                IsBusy = false;
                if (RefeshSecurityPaperList != null)
                    RefeshSecurityPaperList(this, EventArgs.Empty);
                
                ExecuteCancelCommand();
            });
        }

        /// <summary>
        /// Execute Cancel Command
        /// </summary>
        private void ExecuteCancelCommand()
        {
            if (CloseWindow != null)
                CloseWindow(this, EventArgs.Empty);
        }
        #endregion
    }
}
