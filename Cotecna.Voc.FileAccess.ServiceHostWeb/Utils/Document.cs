//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cotecna.Voc.FileAccess.Utils
{
    using System;
    using System.Collections.Generic;
    
    public partial class Document
    {
        public int DocumentId { get; set; }
        public int CertificateId { get; set; }
        public string Filename { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public bool IsSupporting { get; set; }
        public string CreationBy { get; set; }
        public System.DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public string ModificationBy { get; set; }
        public Nullable<System.DateTime> ModificationDate { get; set; }
    }
}
