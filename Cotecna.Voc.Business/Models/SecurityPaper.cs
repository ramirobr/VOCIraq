using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotecna.Voc.Business
{
    /// <summary>
    /// DTO to show the information of security papers
    /// </summary>
    [MetadataType(typeof(SecurityPaperMetadata))]
    public partial class SecurityPaper
    {
        string _fileReference;

        public string FileReference
        {
            get
            {
                if (this.ReleaseNote != null)
                    _fileReference = this.ReleaseNote.Certificate.Sequential;
                return _fileReference;
            }
           
        }

        private DateTime? _issuanceDate;

        public DateTime? IssuanceDate
        {
            get 
            {
                if (this.ReleaseNote != null)
                    _issuanceDate = this.ReleaseNote.IssuanceDate;
                return _issuanceDate; 
            }
        }
        

        private sealed class SecurityPaperMetadata
        {
            [Key]
            public int SecurityPaperId { get; set; }

        }
    }
}
