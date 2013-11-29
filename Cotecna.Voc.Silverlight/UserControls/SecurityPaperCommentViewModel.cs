using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;
using System;
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
using System.Collections.Generic;

namespace Cotecna.Voc.Silverlight
{

    public enum SecurityPaperAction
    {
        Misprinted,
        Cancel,
        UnCancel
    }

    public class SecurityPaperCommentViewModel : ViewModel
    {
        #region events
        /// <summary>
        /// Close current window
        /// </summary>
        public event EventHandler CloseWindow;
        /// <summary>
        /// Refresh the list of security papers
        /// </summary>
        public event EventHandler RefeshSecurityPaperList;
        #endregion

        #region private fields
        private string _screenTitle;
        private string _screenInstruction;
        private string _comment;
        private ICommand _saveCommand;
        private ICommand _cancelCommand;
        #endregion

        #region properties
        
        /// <summary>
        /// Get or set Action
        /// </summary>
        public SecurityPaperAction Action { get; set; }

        /// <summary>
        /// Gets or sets SecurityPapersIds
        /// </summary>
        public List<SecurityPaper> SecurityPapers { get; set; }

        /// <summary>
        /// Gets or sets Proxy
        /// </summary>
        public VocContext Proxy { get; set; }

        /// <summary>
        /// Gets or sets ScreenTitle
        /// </summary>		        
        public string ScreenTitle
        {
            get
            {
                return _screenTitle;
            }
            set
            {
                if (_screenTitle == value) return;
                _screenTitle = value;
                OnPropertyChanged("ScreenTitle");
            }
        }

        /// <summary>
        /// Gets or sets ScreenInstruction
        /// </summary>		
        public string ScreenInstruction
        {
            get
            {
                return _screenInstruction;
            }
            set
            {
                if (_screenInstruction == value) return;
                _screenInstruction = value;
                OnPropertyChanged("ScreenInstruction");
            }
        }

        /// <summary>
        /// Gets or sets Comment
        /// </summary>		
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                if (_comment == value) return;
                _comment = value;
                OnPropertyChanged("Comment");
                HasChanges = true;
            }
        }

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
        /// Get an instance and set the title and the instruction
        /// </summary>
        /// <param name="action">Action to permform</param>
        public SecurityPaperCommentViewModel(SecurityPaperAction action)
        {
            Action = action;
            ScreenTitle = string.Format(Strings.ScurityPaperCancellMissPrintedTitle, action == SecurityPaperAction.Cancel ? Strings.Cancel : Strings.MisPrint);
            ScreenInstruction = string.Format(Strings.ScurityPaperCancellMissPrintedInstruction, action == SecurityPaperAction.Cancel ? Strings.Cancel : Strings.MisPrint);
        }
        #endregion

        #region private methods

        /// <summary>
        /// Execute Save Command
        /// </summary>
        private void ExecuteSaveCommand()
        {
            RemoveAllErrors();
            if (ValidateData())
            {
                IsBusy = true;
                SecurityPaperStatusEnum status = SecurityPaperStatusEnum.NotIssued;
                switch (Action)
                {
                    case SecurityPaperAction.Misprinted:
                        status = SecurityPaperStatusEnum.MissPrinted;
                        break;
                    case SecurityPaperAction.Cancel:
                        status = SecurityPaperStatusEnum.Cancelled;
                        break;
                    case SecurityPaperAction.UnCancel:
                        status = SecurityPaperStatusEnum.NotIssued;
                        break;   
                }
                foreach (SecurityPaper item in SecurityPapers)
                {
                    item.Status = status;
                    item.Comment = Comment;
                }
                Proxy.SubmitChanges(CompletedSubmitSecurityPapers, null);
            }
        }

        /// <summary>
        /// Callback method MisprintedCancelSecurityPapers
        /// </summary>
        /// <param name="operation">Operation</param>
        private void CompletedSubmitSecurityPapers(SubmitOperation operation)
        {
            HandleSubmitOperation(operation, delegate 
            {
                IsBusy = false;
                HasChanges = false;
                if (RefeshSecurityPaperList != null)
                    RefeshSecurityPaperList(this, EventArgs.Empty);
                ExecuteCancelCommand();
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
        /// Validate the screen
        /// </summary>
        /// <returns>Bool</returns>
        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(Comment))
            {
                AddError("Comment", Strings.CommentMandatory);
            }
            return !HasErrors;
        }
        #endregion
    }
}
