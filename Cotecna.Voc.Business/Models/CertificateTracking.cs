using System.ComponentModel.DataAnnotations;

namespace Cotecna.Voc.Business
{
    [MetadataType(typeof(CertificateTrackingMetadata))]
    public partial class CertificateTracking
    {
        private sealed class CertificateTrackingMetadata
        {
            [Key]
            public int CertificateTranckingId { get; set; }
        }
    }
}
