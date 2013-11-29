using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
namespace Cotecna.Voc.Business
{
    /// <summary>
    /// Extension to define validation attributes
    /// </summary>
    [MetadataType(typeof(DocumentMetadata))]
    public partial class Document
    {
        private sealed class DocumentMetadata
        {
            [Key]
            public int DocumentId { get; set; }

        }
    }
}
