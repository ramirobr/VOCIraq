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
    
    public partial class V_DICTIONARY
    {
        public short DIC_ID { get; set; }
        public int REF_ID { get; set; }
        public int CONTRACT_MASK { get; set; }
        public string DIC_CODE { get; set; }
        public string DIC_DESCRIPTION_FRENCH { get; set; }
        public string DIC_DESCRIPTION_ENGLISH { get; set; }
        public string DIC_DESCRIPTION_SPANICH { get; set; }
        public bool IS_DELETED_YN { get; set; }
        public System.DateTime CREATION_DATE { get; set; }
        public Nullable<int> CREATOR { get; set; }
        public System.DateTime LASTKEY_DATE { get; set; }
        public Nullable<int> LASTKEYER { get; set; }
        public byte EXPORT_CODE { get; set; }
        public Nullable<int> EXPORT_OFF { get; set; }
        public byte LASTEXPORT_CODE { get; set; }
        public Nullable<System.DateTime> LASTEXPORT_DATE { get; set; }
    }
}
