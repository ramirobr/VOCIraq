using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// View Model base class for child windows wich need to interact with main screens.
    /// </summary>
    public class ViewModelChildWindow<T> : ViewModel where T : Entity, IEditableDataComparer<T>, ICloneable<T>
    {

        /// <summary>
        /// An action executed when an order to close the child window is received.
        /// Generally it is used to assign the close action to the View Model. Only the screen has 
        /// reference to the child window, so it is a job of the screen to assign the close action.
        /// </summary>
        public Action CloseEditableWindow { get; set; }

        public void OnSavedItem()
        {
            if (SavedItem != null)
                SavedItem(this, EventArgs.Empty);
        }

        public void OnCanceledItem()
        {
            if (CanceledItem != null)
                CanceledItem(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event is fired when the user confirms the action on the child windows (usually through a save button)
        /// </summary>
        public event EventHandler SavedItem;

        /// <summary>
        /// event is fired when user cancels
        /// </summary>
        public event EventHandler CanceledItem;

        /// <summary>
        /// Constructor which must be used in inherited classes by claaing base(...). 
        /// <para>It handles four options: </para>
        /// <para>1. (true, null) This case implies a New Object T and Generation of new Instance of T is executed here</para>
        /// <para>2. (true, ObjectInstanceOfT) This case implies a New Object T and Generation of new Instance is sent to this constructor</para>
        /// <para>3. (false, null) It throws an exception because edition is not posible with null objects.</para>
        /// <para>4. (false, ObjectInstanceOfT) This case implies an existing Object T being edited. This constructor will handle duplication of T and inform of changes.</para>
        /// </summary>
        /// <param name="original">The AirPassengerBaseObject instance</param>
        public ViewModelChildWindow(T original)
        {

            if (original == null)
                throw new ArgumentException("Entity for Edition cannot be null");
            BusinessObject = original;
            BusinessObject.ValidationErrors.Clear();
            CopyOriginal();
            _saveCommand = new DelegateCommand(ExecuteSave, CanSaveExecute);
        }


        #region Save Command

        private readonly DelegateCommand _saveCommand;

        private void ExecuteSave()
        {

            RetrieveData();

            // reapply all validations, as errors for un-initialized fields do not appear automatically
            System.Collections.Generic.List<ValidationResult> list = new System.Collections.Generic.List<ValidationResult>();
            Validator.TryValidateObject(BusinessObject, new ValidationContext(BusinessObject), list, true);
            foreach (var item in list)
            {
                AddError(item.MemberNames.First(), item.ErrorMessage);
            }

            // custom validation
            ValidateData();

            if (!HasErrors)
            {

                BusinessObject.PropertyChanged -= OnBusinesObjectPropertyChanged;

                if (HasChanges)
                {
                    // otherwise, closing the window requests a confirmation
                    CommitChanges();
                }

                FinalizeSave();

                LaunchCloseEventBecauseUserConfirms();
            }
        }

        /// <summary>
        /// User has confirmed the save; last chance to modify some data before closing the window. Get the data that you want to keep when the user confirms the changes
        /// </summary>
        protected virtual void FinalizeSave() { }
        
        private bool CanSaveExecute()
        {
            //if (BusinessObject.HasErrors)
            //    return false;            
            return true;
        }

        /// <summary>
        /// Save command which handles validation steps prior to launch the save process
        /// </summary>
        public DelegateCommand SaveCommand
        {
            get
            {
                return _saveCommand;
            }
        }

        #endregion
        #region Cancel Command

        public ICommand CancelCommand
        {
            get { return new DelegateCommand(TryToCloseTheWindowBecauseUserCanceled); }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A copy of the business object under edition
        /// </summary>
        private T _original;

        /// <summary>
        /// Makes a copy of the original BusinessObject, in order to be able to compare it for changes
        /// </summary>
        public void CopyOriginal()
        {

            _original = Activator.CreateInstance<T>();
            SimpleCopy(BusinessObject, ref _original);

            // we may be called several times => start by unsubscribing (if we never subscribed before, it has no effect)
            BusinessObject.PropertyChanged -= OnBusinesObjectPropertyChanged;
            BusinessObject.PropertyChanged += OnBusinesObjectPropertyChanged;

            HasChanges = false;
        }

        /// <summary>
        /// To be called if the user saves the changes. This way, the closing event will not ask the user if s/he wants to save
        /// </summary>
        public void CommitChanges()
        {

            _original.Clone(BusinessObject);
            UpdateHasChanges();
        }

        /// <summary>
        /// The user canceled the changes => overwrite the BusinessObject with the original values.
        /// </summary>
        public override void CancelChanges()
        {
            SimpleCopy(_original, BusinessObject);
            HasChanges = false;
        }

        protected void OnBusinesObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateHasChanges();
        }

        protected void UpdateHasChanges()
        {
            HasChanges = !_original.Equals(BusinessObject);
            _saveCommand.OnCanExecuteChanged();
        }

        /// <summary>
        /// Property representing a BusinessObject. Every change to this object's properties can launch the save process 
        /// if requested by the user. This object must be used in the call to the services for edit and create actions.
        /// </summary>
        public T BusinessObject { get; set; }

        #endregion

        #region Overridable Methods

        /// <summary>
        /// add your custom validation in the derived class/// </summary>
        protected virtual void ValidateData() { }

        /// <summary>
        /// Read data that is not handled by the binding system. Called when user saves AND cancels.
        /// </summary>
        protected virtual void RetrieveData() { }

        protected override void AddError(string propertyName, string errorMessage)
        {

            if (FindValidationResult(propertyName, errorMessage) == null)
            {
                // not already in collection
                ValidationResult newResult = new ValidationResult(errorMessage, new string[] { propertyName });
                BusinessObject.ValidationErrors.Add(newResult);
            }
        }

        private ValidationResult FindValidationResult(string propertyName, string errorMessage)
        {
            foreach (ValidationResult result in BusinessObject.ValidationErrors)
            {
                if (result.MemberNames.Contains(propertyName))
                {
                    if (result.ErrorMessage.Equals(errorMessage))
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        protected override void RemoveError(string propertyName, string errorMessage)
        {

            ValidationResult result = FindValidationResult(propertyName, errorMessage);
            if (result != null)
            {
                BusinessObject.ValidationErrors.Remove(result);
            }
        }

        public new bool HasErrors
        {
            get { return BusinessObject.HasValidationErrors || HasValidationErrors; }
        }

        /// <summary>
        /// This is meant to be bound on the validation summary in the child window
        /// </summary>
        public bool HasValidationErrors { get; set; }

        private bool _areValidationErrorsWatched;
        public bool AreValidationErrorsWatched
        {
            get
            {
                return _areValidationErrorsWatched;
            }
            set
            {
                if (_areValidationErrorsWatched != value)
                {
                    _areValidationErrorsWatched = value;
                    OnPropertyChanged("AreValidationErrorsWatched");
                }
            }
        }
        #endregion

        /// <summary>
        /// Copies an object to another
        /// </summary>
        /// <param name="source">The original object <c>T</c></param>
        /// <param name="destination">The destination object</param>
        protected void SimpleCopy(T source, ref T destination)
        {

            destination.Clone(source);
        }

        /// <summary>
        /// Copies an object to another
        /// </summary>
        /// <param name="source">The original object <c>T</c></param>
        /// <param name="destination">The destination object</param>
        /// <remarks>This method is used in cancell option</remarks>
        protected void SimpleCopy(T source, T destination)
        {
            destination.Clone(source);
        }

        private void TryToCloseTheWindowBecauseUserCanceled()
        {
            RetrieveData();

            OnCanceledItem();

            FireCloseEditableWindowEvent();
        }

        /// <summary>
        /// If the implementation of the view model corresponds to a child window you should launch the close event 
        /// to inform the user interface to close the child window.
        /// </summary>
        protected virtual void LaunchCloseEventBecauseUserConfirms()
        {
            OnSavedItem();

            FireCloseEditableWindowEvent();
        }

        private void FireCloseEditableWindowEvent()
        {
            //Call an action to for closing if there is one assigned.
            if (CloseEditableWindow != null)
            {
                CloseEditableWindow();
            }
        }
    }
}
