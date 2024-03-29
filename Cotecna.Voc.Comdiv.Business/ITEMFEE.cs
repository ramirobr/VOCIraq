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
    
    public partial class ITEMFEE
    {
        public ITEMFEE()
        {
            this.ITEMCOSTs = new HashSet<ITEMCOST>();
        }
    
        public int ITF_ID { get; set; }
        public int ITM_ID { get; set; }
        public int ITF_NUMBER { get; set; }
        public int ITF_TYPE_ID { get; set; }
        public int ITF_DESC_ID { get; set; }
        public Nullable<int> ITF_BASIS_ID { get; set; }
        public Nullable<decimal> ITF_QUANTITY { get; set; }
        public Nullable<int> ITF_CURR_ID { get; set; }
        public Nullable<decimal> ITF_AMOUNT { get; set; }
        public Nullable<int> ITF_SUB_DESC_ID { get; set; }
        public string ITF_DESCRIPTION_REMARK { get; set; }
        public string ITF_BASIS_REMARK { get; set; }
        public Nullable<bool> ITF_EXPORT_INV_YN { get; set; }
        public Nullable<decimal> ITF_DISCOUNT { get; set; }
        public Nullable<decimal> ITF_EXRATE_FEE_TO_USD { get; set; }
        public Nullable<decimal> ITF_EXRATE_FEE_TO_CHF { get; set; }
        public Nullable<decimal> ITF_EXRATE_FEE_TO_LOC { get; set; }
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
        public Nullable<decimal> ITF_ExrateToTotalFeeCurrency { get; set; }
        public Nullable<int> COC_ID { get; set; }
    
        public virtual COC COC { get; set; }
        public virtual ITEM ITEM { get; set; }
        public virtual ICollection<ITEMCOST> ITEMCOSTs { get; set; }
    }
}
