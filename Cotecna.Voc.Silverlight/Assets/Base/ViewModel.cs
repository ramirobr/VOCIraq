using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Common class used as base class for all View Models
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        #pragma warning disable
        


        ///// <summary>
        ///// The office code of the logged in user
        ///// </summary>
        //protected string OfficeCode
        //{
        //    get
        //    {
        //        if (WebContext.Current.User != null)
        //            return WebContext.Current.User.OfficeCode;
        //        return "";
        //    }
        //}

        #region HasChanges
        bool _haschanges;

        /// <summary>
        /// A property that shows if the context has changes
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return _haschanges;
            }
            set
            {
                _haschanges = value;
                OnPropertyChanged(() => this.HasChanges);
            }
        }
        #endregion

        /// <summary>
        /// Called when the user agrees to abandon the changes s/he made
        /// </summary>
        public virtual void CancelChanges()
        {

        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Raise the PropertyChanged event for the 
        /// specified property.
        /// </summary>
        /// <param name="propertyName">
        /// A string representing the name of 
        /// the property that changed.</param>
        /// <remarks>
        /// Only raise the event if the value of the property 
        /// has changed from its previous value</remarks>
        protected void OnPropertyChanged(string propertyName)
        {
            // Validate the property name in debug builds
            VerifyProperty(propertyName);

            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                Validate(propertyName);
            }
        }

        /// <summary>
        /// Verifies whether the current class provides a property with a given
        /// name. This method is only invoked in debug builds, and results in
        /// a runtime exception if the <see cref="OnPropertyChanged"/> method
        /// is being invoked with an invalid property name. This may happen if
        /// a property's name was changed but not the parameter of the property's
        /// invocation of <see cref="OnPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            Type type = this.GetType();

            // Look for a *public* property with the specified name
            System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
            if (pi == null)
            {
                // There is no matching property - notify the developer
                string msg = "OnPropertyChanged was invoked with invalid " +
                                "property name {0}. {0} is not a public " +
                                "property of {1}.";
                msg = String.Format(msg, propertyName, type.FullName);
                System.Diagnostics.Debug.Assert(1 != 1, msg);
            }
        }
        #endregion

        private string GetPropertyNameFromExpression<T>(Expression<Func<T>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member.Name;
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            OnPropertyChanged(GetPropertyNameFromExpression(property));
        }

        protected bool SetValue<T>(ref T property, T value, Expression<Func<T>> propertyDelegate)
        {
            if (Object.Equals(property, value))
            {
                return false;
            }
            property = value;

            OnPropertyChanged(propertyDelegate);

            return true;
        }

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || _errors == null ||
                 !_errors.ContainsKey(propertyName)) return null;
            return _errors[propertyName];
        }

        public IEnumerable GetAllErrors()
        {
            List<string> errors = new List<string>();
            if (_errors != null)
            {
                foreach (var item in _errors)
                {
                    errors.AddRange(item.Value);
                }
            }
            return errors;
        }

        //[Display(AutoGenerateField = false)]
        //public virtual bool HasErrors
        //{
        //    get { return _errors.Any(); }
        //}

        #endregion

        string _changes;
        /// <summary>
        /// A message generally used to inform the user what is happening or happened
        /// </summary>
        public string ScreenInformation { get { return _changes; } set { _changes = value; OnPropertyChanged(() => this.ScreenInformation); } }

        private string _message;
        /// <summary>
        /// Gets or sets message information
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(() => Message);
            }
        }

        private string _errorMessage;
        /// <summary>
        /// When HasErrors property is set to true, the consequence of the error will appear here 
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                SetValue(ref _errorMessage, value, () => ErrorMessage);
            }
        }

        private bool _hasErrors;
        /// <summary>
        /// Common property that notifies of an error.
        /// </summary>
        public bool HasErrors
        {
            get { return _hasErrors || _errors.Any(); }
            set
            {
                SetValue(ref _hasErrors, value, () => HasErrors);
            }
        }

        private bool _isBusy;
        /// <summary>
        /// A common property to indicate that a loading process is on the way
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetValue(ref _isBusy, value, () => IsBusy);
            }
        }


        /// <summary>
        /// Operation to be executed when an asynchronous opeartion begins
        /// </summary>        
        protected virtual void BeginAsynchronousOperation(bool clearScreenInformation = true)
        {
            this.IsBusy = true;
            if (clearScreenInformation)
                ScreenInformation = "";
        }

        /// <summary>
        /// Handles the load operation when it is completed
        /// </summary>
        /// <param name="operation">the LoadOperation object when is returned after the asynchronous operation ended</param>
        /// <param name="successfulAction">An action to be executed if no errors were found</param>
        /// <param name="errorAction">An action to be executed when errors are found and the inmediately after the error window appears</param>
        protected virtual void HandleLoadOperation(LoadOperation operation, Action successfulAction, Action errorAction = null)
        {
            this.IsBusy = false;
            if (operation.HasError)
            {
                operation.MarkErrorAsHandled();
                ErrorMessage = operation.Error.Message;
                HasErrors = true;
                ErrorWindow.CreateNew(operation.Error);
                if (errorAction != null)
                    errorAction();
                return;
            }

            successfulAction();
        }

        /// <summary>
        /// Handles the submit operation when it is completed
        /// </summary>
        /// <param name="operation">the SubmitOperation object when is returned after the asynchronous operation ended</param>
        /// <param name="successfulAction">An action to be executed if no errors were found</param>
        /// <param name="errorAction">An action to be executed when errors are found and the inmediately after the error window appears</param>
        protected virtual void HandleSubmitOperation(SubmitOperation operation, Action successfulAction, Action errorAction = null)
        {
            this.IsBusy = false;
            if (operation.HasError)
            {
                operation.MarkErrorAsHandled();
                ErrorMessage = operation.Error.Message;
                HasErrors = true;
                ErrorWindow.CreateNew(operation.Error);
                if (errorAction != null)
                    errorAction();
                return;
            }
            successfulAction();
        }

        /// <summary>
        /// Handles the invoke operation when it is completed
        /// </summary>
        /// <param name="operation">the SubmitOperation object when is returned after the asynchronous operation ended</param>
        /// <param name="successfulAction">An action to be executed if no errors were found</param>
        /// <param name="errorAction">An action to be executed when errors are found and the inmediately after the error window appears</param>
        protected virtual void HandleInvokeOperation(InvokeOperation operation, Action successfulAction, Action errorAction = null)
        {
            this.IsBusy = false;
            if (operation.HasError)
            {
                operation.MarkErrorAsHandled();
                ErrorMessage = operation.Error.Message;
                HasErrors = true;
                ErrorWindow.CreateNew(operation.Error);
                if (errorAction != null)
                    errorAction();
                return;
            }
            successfulAction();
        }

        /// <summary>
        /// Display Information messages
        /// </summary>
        /// <param name="error">Message to be displayed</param>
        protected static void InformationDisplay(string message)
        {
            var informative = new InformativeChildWindow();
            informative.Message = message;
            informative.ButtonSelection = InformativeChildWindow.ButtonSelections.OnlyOk;
            informative.Title = Strings.Information;
            informative.Image = InformativeChildWindow.StyleImages.Information;
            informative.Show();
        }


        /// <summary>
        /// Display alert messages
        /// </summary>
        /// <param name="error">Message to be displayed</param>
        protected static void AlertDisplay(string message)
        {
            var informative = new InformativeChildWindow();
            informative.Message = message;
            informative.ButtonSelection = InformativeChildWindow.ButtonSelections.OnlyOk;
            informative.Title = Strings.Alert;
            informative.Image = InformativeChildWindow.StyleImages.Information;
            informative.Show();
        }

        /// <summary>
        /// Display a question meesage
        /// </summary>
        /// <param name="message">Message for showing</param>
        /// <param name="executeOnClose">Method to execute if the answer is yes</param>
        /// <param name="executeOnCancel">Mehtod to execute if the answer is no</param>
        protected static void QuestionDisplay(string message, Action executeOnClose, Action executeOnCancel = null)
        {
            var informative = new InformativeChildWindow();
            informative.Message = message;
            informative.ButtonSelection = InformativeChildWindow.ButtonSelections.YesNo;
            informative.Title = Strings.Question;
            informative.Image = InformativeChildWindow.StyleImages.Question;
            informative.Show();
            informative.Closed += (s, e) =>
            {
                if (informative.DialogResult.GetValueOrDefault())
                {
                    executeOnClose();
                }
                else
                {
                    if (executeOnCancel != null)
                        executeOnCancel();
                }
            };
        }

        #region Validate Error
        protected virtual void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Insert(0, errorMessage);
            }
            NotifyErrorsChanged(propertyName);
        }

        protected virtual void RemoveError(string propertyName, string errorMessage)
        {
            if (_errors.ContainsKey(propertyName) && _errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Remove(errorMessage);
                if (_errors[propertyName].Count == 0)
                    _errors.Remove(propertyName);
                NotifyErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Removes all the existing errors from the errors dictionary
        /// </summary>
        protected virtual void RemoveAllErrors()
        {
            List<string> currentErrors = _errors.Keys.ToList();
            if (currentErrors.Count > 0)
            {
                foreach (string propertyName in currentErrors)
                {
                    if (_errors.ContainsKey(propertyName))
                    {
                        _errors[propertyName].Clear();
                        if (_errors[propertyName].Count == 0)
                            _errors.Remove(propertyName);
                        NotifyErrorsChanged(propertyName);
                    }
                }
            }
        }


        private readonly  Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        private void NotifyErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void Validate(string propertyName)
        {
            var value = GetPropertyValue(propertyName);
            RemovePropertyErrors(propertyName);
            List<ValidationResult> list = new List<ValidationResult>();
            Validator.TryValidateProperty(value, new ValidationContext(this, null, null) { MemberName = propertyName }, list);
            foreach (var item in list)
                this.AddError(item.MemberNames.First(), item.ErrorMessage);
            NotifyErrorsChanged(propertyName);
        }

        protected void Validate()
        {
            _errors.Clear();
            List<ValidationResult> list = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this, null, null), list, true);

            foreach (var item in list)
            {
                this.AddError(item.MemberNames.First(), item.ErrorMessage);
                NotifyErrorsChanged(item.MemberNames.First());
            }
        }

        private object GetPropertyValue(string propertyName)
        {
            var type = this.GetType();

            var propertyInfo = type.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Couldn't find any property called {0} on type {1}", propertyName, type));
            }

            return propertyInfo.GetValue(this, null);
        }

        public void RemovePropertyErrors(string propertyName)
        {
            if (_errors != null && _errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                NotifyErrorsChanged(propertyName);
            }
        }
        #endregion
    }
}
