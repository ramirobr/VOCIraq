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
    
    public partial class HSDETAIL
    {
        public int HSD_ID { get; set; }
        public Nullable<int> HSC_ID { get; set; }
        public short HSD_ORIG { get; set; }
        public Nullable<System.DateTime> HSD_EFF_DATE { get; set; }
        public Nullable<int> HSD_UNIT { get; set; }
        public Nullable<System.DateTime> HSD_END_DATE { get; set; }
        public string HSD_DESCRIPTION { get; set; }
        public Nullable<bool> HSD_MDV_YN { get; set; }
        public Nullable<short> HSD_GOODS_TYPE { get; set; }
        public bool IS_DELETED_YN { get; set; }
        public System.DateTime CREATION_DATE { get; set; }
        public Nullable<int> CREATOR { get; set; }
        public System.DateTime LASTKEY_DATE { get; set; }
        public Nullable<int> LASTKEYER { get; set; }
        public byte EXPORT_CODE { get; set; }
        public int EXPORT_OFF { get; set; }
        public Nullable<byte> LASTEXPORT_CODE { get; set; }
        public Nullable<System.DateTime> LASTEXPORT_DATE { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime LASTKEY_DATEUTC { get; set; }
    
        public virtual HSCODE HSCODE { get; set; }
    }
}
