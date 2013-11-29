using Cotecna.Voc.Silverlight;
namespace Cotecna.Voc.Business
{
    public partial class Document : IEditableDataComparer<Document>, ICloneable<Document>
    {
        public bool Equals(Document other)
        {
            bool different = Filename != other.Filename ||
                             FilePath != other.FilePath ||
                             Description != other.Description;

            return different ? false : true;
        }

        public void Clone(Document xSource)
        {
            DocumentId = xSource.DocumentId;
            Filename = xSource.Filename;
            FilePath = xSource.FilePath;
            Description = xSource.Description;
            IsSupporting = xSource.IsSupporting;
            IsDeleted = xSource.IsDeleted;
            DocumentType = xSource.DocumentType;
        }

        private bool _checked;

        /// <summary>
        /// Gets or sets Checked
        /// </summary>		
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                RaisePropertyChanged("Checked");
            }
        }
    }
}
