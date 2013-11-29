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
    
    public partial class INVOICE
    {
        public INVOICE()
        {
            this.INVITEMs = new HashSet<INVITEM>();
            this.INVOICEPAYMENTs = new HashSet<INVOICEPAYMENT>();
        }
    
        public int INV_ID { get; set; }
        public int ITM_ID { get; set; }
        public Nullable<byte> INV_NUMBER { get; set; }
        public string INV_NO { get; set; }
        public string INV_CONTRACT_NO { get; set; }
        public Nullable<System.DateTime> INV_DATE { get; set; }
        public Nullable<System.DateTime> INV_DUE_DATE { get; set; }
        public Nullable<byte> INV_REMIND_LEVEL { get; set; }
        public Nullable<int> INV_CURR_ID { get; set; }
        public Nullable<int> INV_TYPE_ID { get; set; }
        public Nullable<int> INV_PAY_TYPE_ID { get; set; }
        public string INV_CREDIT_REF { get; set; }
        public Nullable<int> INV_PAY_NAME_ID { get; set; }
        public Nullable<int> INV_PAY_ADD_ID { get; set; }
        public Nullable<int> INV_PAY_CONT_ID { get; set; }
        public string INV_PAY_REF { get; set; }
        public bool INV_COMD_SEL { get; set; }
        public string INV_COMD_DESCRIP { get; set; }
        public bool INV_QTY_SEL { get; set; }
        public Nullable<decimal> INV_QTY { get; set; }
        public Nullable<int> INV_QTY_UNIT_ID { get; set; }
        public bool INV_TRANSP_SEL { get; set; }
        public string INV_TRANSP_DESCRIP { get; set; }
        public bool INV_POI_SEL { get; set; }
        public string INV_POI_DESCRIP { get; set; }
        public bool INV_VAL_SEL { get; set; }
        public Nullable<int> INV_VAL_CURR_ID { get; set; }
        public Nullable<decimal> INV_VAL_AMOUNT { get; set; }
        public bool INV_WHS_SEL { get; set; }
        public string INV_WHS_DESCRIP { get; set; }
        public bool INV_TOI_SEL { get; set; }
        public string INV_TOI_DESCRIP { get; set; }
        public bool INV_OTH1_SEL { get; set; }
        public string INV_OTH1_TITLE { get; set; }
        public string INV_OTH1_DESCRIP { get; set; }
        public bool INV_OTH2_SEL { get; set; }
        public string INV_OTH2_TITLE { get; set; }
        public string INV_OTH2_DESCRIP { get; set; }
        public bool INV_BODY_SEL { get; set; }
        public string INV_BODY_TEXT { get; set; }
        public Nullable<decimal> INV_APPLIC_VAT { get; set; }
        public Nullable<int> INV_VAT_SECTION_NO { get; set; }
        public Nullable<short> INV_PAY_DELAY { get; set; }
        public string INV_BK_ACCOUNT_NR { get; set; }
        public string INV_BK_IBAN { get; set; }
        public string INV_BK_NAME { get; set; }
        public string INV_BK_VIA { get; set; }
        public string INV_PAID { get; set; }
        public Nullable<decimal> INV_PAID_AMOUNT { get; set; }
        public Nullable<System.DateTime> INV_PAID_DATE { get; set; }
        public Nullable<System.DateTime> INV_PAID_KEY_DATE { get; set; }
        public string INV_RMK_TEXT { get; set; }
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
        public Nullable<bool> INV_MANAGER_VAL { get; set; }
        public Nullable<bool> INV_EXPORTED_YN { get; set; }
        public Nullable<decimal> INV_EXRATE_TO_USD { get; set; }
        public Nullable<decimal> INV_EXRATE_TO_CHF { get; set; }
        public Nullable<decimal> INV_EXRATE_TO_LOC { get; set; }
        public Nullable<int> INV_CURR2_ID { get; set; }
        public System.Guid rowguid { get; set; }
        public Nullable<bool> INV_PRINT_FINAL_CLIENT_YN { get; set; }
        public Nullable<bool> ReadyToExportYN { get; set; }
        public Nullable<bool> ExportedToNavisionYN { get; set; }
        public Nullable<decimal> INV_OTHER_TAXES { get; set; }
        public Nullable<bool> INV_RMK_SEL { get; set; }
        public string TaxNumber { get; set; }
        public Nullable<int> CustomerMarket { get; set; }
        public Nullable<bool> INV_PAY_REF_SEL { get; set; }
        public string INV_DESTINATION { get; set; }
        public Nullable<bool> INV_DESTINATION_SEL { get; set; }
        public Nullable<bool> AutoNumber { get; set; }
        public Nullable<bool> IntercoInvoice { get; set; }
    
        public virtual ADDRESS ADDRESS { get; set; }
        public virtual COMPANY COMPANY { get; set; }
        public virtual CONTACT CONTACT { get; set; }
        public virtual ICollection<INVITEM> INVITEMs { get; set; }
        public virtual ICollection<INVOICEPAYMENT> INVOICEPAYMENTs { get; set; }
        public virtual ITEM ITEM { get; set; }
    }
}