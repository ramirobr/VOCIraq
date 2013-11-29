using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace Cotecna.Voc.Business
{
    /// <summary>
    /// Extension to define validation attributes
    /// </summary>
    [MetadataType(typeof(ReleaseNoteMetadata))]
    public partial class ReleaseNote
    {

        private sealed class ReleaseNoteMetadata
        {
            [Key]
            public int ReleaseNoteId { get; set; }
            
            [Required]
            [Range(0, Int32.MaxValue)]
            [Display(Name = "Number of containers")]
            public Nullable<int> NumberOfContainers { get; set; }

            [Required]
            [StringLength(512)]
            [Display(Name = "Containers details")]
            public string Containers { get; set; }

            
            [Display(Name = "Net weight")]
            public decimal? NetWeight { get; set; }

            [Required]
            [StringLength(512)]
            [Display(Name = "Goods")]
            public string Goods { get; set; }

            [Required]
            [Display(Name = "Documentary  check result")]
            public Nullable<ResultEnum> DocumentaryCheckResultId { get; set; }
            
            [Required]
            [Display(Name = "Physical check result")]
            public Nullable<ResultEnum> PhysicalCheckResultId { get; set; }
            
            [Required]
            [Display(Name = "Visual Inspection")]
            public Nullable<bool> VisualInspectionMade { get; set; }
            
            [Required]
            [Display(Name = "Overall Result")]
            public Nullable<ResultEnum> OverallResultId { get; set; }
            
            [Required]
            [Display(Name = "Note Issued")]
            public Nullable<NoteIssuedEnum> NoteIssuedId { get; set; }

            [Required]
            [Display(Name = "Joint sampling")]
            public Nullable<bool> JointSamplingMade { get; set; }

            [Required]
            [StringLength(512)]
            [Display(Name = "Importer Name")]
            public string ImporterName { get; set; }

            [Required]
            [Display(Name = "Visually Check the packages")]
            public Nullable<bool> VisuallyCheck { get; set; }

            [Required]
            [Display(Name = "Partial / Complete")]
            public Nullable<bool> PartialComplete { get; set; }

            [Required]
            [Display(Name = "Unit")]
            [StringLength(64)]
            public string Unit { get; set; }

            [Required]
            [Display(Name = "Received Quantity")]
            public int? ReceivedQuantity { get; set; }

            [Required]
            [Display(Name = "Remaining Quantity")]
            public int? RemainingQuantity { get; set; }

            [Required]
            [Display(Name = "Shipment Type")]
            public Nullable<ShipmentTypeEnum> ShipmentType { get; set; }
        }
    }
}
