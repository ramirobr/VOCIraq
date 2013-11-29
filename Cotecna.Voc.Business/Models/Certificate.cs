
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
namespace Cotecna.Voc.Business
{
    /// <summary>
    /// Extension to define validation attributes
    /// </summary>
    [MetadataType(typeof(CertificateMetadata))]
    public partial class Certificate
    {
        
        private sealed class CertificateMetadata
        {
            [Key]
            public int CertificateId { get; set; }

            public IEnumerable<ReleaseNote> ReleaseNotes { get; set; }

            //[Include]
            //[Association("Certificate_Documents", "CertificateId", "CertificateId")]  
            //public IEnumerable<Document> Documents { get; set; }
        }
    }
}
