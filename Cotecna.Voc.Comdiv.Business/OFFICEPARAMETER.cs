//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cotecna.Voc.Comdiv.Business
{
    using System;
    using System.Collections.Generic;
    
    public partial class OFFICEPARAMETER
    {
        public int PAR_ID { get; set; }
        public int OFF_ID { get; set; }
        public string PAR_CODE { get; set; }
        public string PAR_VALUE { get; set; }
        public Nullable<bool> PAR_COMPUTE { get; set; }
        public bool IS_DELETED_YN { get; set; }
        public System.DateTime CREATION_DATE { get; set; }
        public string CREATOR { get; set; }
        public Nullable<int> CREATOR_ID { get; set; }
        public string CREA_WKSTN { get; set; }
        public System.DateTime LASTKEY_DATE { get; set; }
        public string LASTKEYER { get; set; }
        public Nullable<int> LASTKEYER_ID { get; set; }
        public string LASTKEY_WKSTN { get; set; }
        public byte EXPORT_CODE { get; set; }
        public Nullable<int> EXPORT_OFF { get; set; }
        public byte LASTEXPORT_CODE { get; set; }
        public Nullable<System.DateTime> LASTEXPORT_DATE { get; set; }
        public System.Guid rowguid { get; set; }
    }
}
