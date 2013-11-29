using System.ComponentModel.DataAnnotations;

namespace Cotecna.Voc.Business
{
    /// <summary>
    /// Extension to define validation attributes
    /// </summary>
    [MetadataType(typeof(EntryPointMetadata))]
    public partial class EntryPoint
    {
        private sealed class EntryPointMetadata
        {
            [Key]
            public int EntryPointId { get; set; }
        }
    }
}
