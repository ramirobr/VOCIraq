//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cotecna.Voc.Business
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReleaseNote
    {
        public ReleaseNote()
        {
            this.SecurityPapers = new HashSet<SecurityPaper>();
            this.Documents = new HashSet<Document>();
        }
    
        public int ReleaseNoteId { get; set; }
        public int CertificateId { get; set; }
        public Nullable<int> PartialNumber { get; set; }
        public string Goods { get; set; }
        public Nullable<int> NumberOfContainers { get; set; }
        public string Containers { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public Nullable<ResultEnum> DocumentaryCheckResultId { get; set; }
        public Nullable<ResultEnum> PhysicalCheckResultId { get; set; }
        public Nullable<bool> VisualInspectionMade { get; set; }
        public Nullable<ResultEnum> OverallResultId { get; set; }
        public Nullable<NoteIssuedEnum> NoteIssuedId { get; set; }
        public System.DateTime IssuanceDate { get; set; }
        public Nullable<bool> JointSamplingMade { get; set; }
        public string CreationBy { get; set; }
        public System.DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public string ModificationBy { get; set; }
        public Nullable<System.DateTime> ModificationDate { get; set; }
        public string ImporterName { get; set; }
        public Nullable<bool> VisuallyCheck { get; set; }
        public string ContainersDetails { get; set; }
        public string ImportDocumentDetails { get; set; }
        public Nullable<bool> PartialComplete { get; set; }
        public Nullable<int> NumberLineItems { get; set; }
        public string Comments { get; set; }
        public Nullable<ShipmentTypeEnum> ShipmentType { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> ReceivedQuantity { get; set; }
        public Nullable<decimal> RemainingQuantity { get; set; }
        public Nullable<decimal> PaidFees { get; set; }
    
        public virtual Certificate Certificate { get; set; }
        public virtual ICollection<SecurityPaper> SecurityPapers { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}
