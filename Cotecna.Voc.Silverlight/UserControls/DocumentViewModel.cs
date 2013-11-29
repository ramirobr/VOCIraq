
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    public class DocumentViewModel : ViewModelChildWindow<Document>
    {
        #region Private Fields
        private Document _document;

        #endregion

        #region Properties
        /// <summary>
        /// Document to edit
        /// </summary>
        public Document Document
        {
            get
            {
                return _document;
            }
            set
            {
                if (_document != value)
                {
                    _document = value;
                    OnPropertyChanged("Document");
                }
            }
        }
        #endregion

        #region Constructor
        public DocumentViewModel(Document document)
            : base(document)
        {
            Document = document;
            AreValidationErrorsWatched = true;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Custom Validations have to be added to general validations
        /// </summary>
        protected override void ValidateData()
        {

            if (Document.Description == string.Empty)
                AddError("Description", Strings.DocumentDescriptionIsMandatory);
            else
                RemoveError("Description", Strings.DocumentDescriptionIsMandatory);
        }
        #endregion

        #region Public Methods

        #endregion


    }
}
