using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotecna.Voc.Business
{
    [MetadataType(typeof(OfficeMetadata))]
    public partial class Office
    {
        private sealed class OfficeMetadata
        {
            [Key]
            public int OfficeId { get; set; }
        }
    }
}
