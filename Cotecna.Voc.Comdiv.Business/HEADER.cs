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
    
    public partial class HEADER
    {
        public HEADER()
        {
            this.HEADER1 = new HashSet<HEADER>();
            this.ITEMs = new HashSet<ITEM>();
        }
    
        public int HDR_ID { get; set; }
        public Nullable<byte> HDR_TYPE { get; set; }
        public string HDR_NO { get; set; }
        public Nullable<int> OFF_ID { get; set; }
        public string HDR_LOCAL_NO { get; set; }
        public Nullable<int> HDR_SUBTYPE { get; set; }
        public Nullable<System.DateTime> HDR_DATE { get; set; }
        public byte HDR_STATUS { get; set; }
        public Nullable<int> HDR_OFFER_ID { get; set; }
        public Nullable<int> HDR_CLI_NAME_ID { get; set; }
        public Nullable<int> HDR_CLI_ADD_ID { get; set; }
        public Nullable<int> HDR_CLI_CONT_ID { get; set; }
        public string HDR_CLI_REF { get; set; }
        public Nullable<int> HDR_COORD_ID { get; set; }
        public Nullable<int> HDR_PAY_NAME_ID { get; set; }
        public Nullable<int> HDR_PAY_ADD_ID { get; set; }
        public Nullable<int> HDR_PAY_CONT_ID { get; set; }
        public Nullable<int> HDR_FCLI_NAME_ID { get; set; }
        public Nullable<int> HDR_FCLI_ADD_ID { get; set; }
        public Nullable<int> HDR_FCLI_CONT_ID { get; set; }
        public Nullable<int> HDR_OFF_ID { get; set; }
        public Nullable<bool> HDR_NOTIFY_YN { get; set; }
        public Nullable<int> HDR_PROGRESS_ID { get; set; }
        public Nullable<int> HDR_DESTINATION_ID { get; set; }
        public string HDR_LC_NO { get; set; }
        public Nullable<decimal> HDR_LC_AMOUNT { get; set; }
        public Nullable<int> HDR_LC_CURR_ID { get; set; }
        public bool IS_DELETED_YN { get; set; }
        public string IS_LOCKED_BY { get; set; }
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
        public string HDR_PAYMENT_MODE { get; set; }
        public Nullable<int> HDR_SHIPPER_NAME_ID { get; set; }
        public Nullable<int> HDR_SHIPPER_ADD_ID { get; set; }
        public Nullable<int> HDR_SHIPPER_CONT_ID { get; set; }
        public Nullable<int> HDR_EXPORTER_NAME_ID { get; set; }
        public Nullable<int> HDR_EXPORTER_ADD_ID { get; set; }
        public Nullable<int> HDR_EXPORTER_CONT_ID { get; set; }
        public Nullable<int> HDR_NOMINATOR_NAME_ID { get; set; }
        public Nullable<int> HDR_NOMINATOR_ADD_ID { get; set; }
        public Nullable<int> HDR_NOMINATOR_CONT_ID { get; set; }
        public Nullable<int> HDR_TRADER_NAME_ID { get; set; }
        public Nullable<int> HDR_TRADER_ADD_ID { get; set; }
        public Nullable<int> HDR_TRADER_CONT_ID { get; set; }
        public Nullable<int> HDR_BUYER_NAME_ID { get; set; }
        public Nullable<int> HDR_BUYER_ADD_ID { get; set; }
        public Nullable<int> HDR_BUYER_CONT_ID { get; set; }
        public string DocumentPath { get; set; }
        public string VesselName { get; set; }
        public Nullable<int> HDR_IMPORTER_NAME_ID { get; set; }
        public Nullable<int> HDR_IMPORTER_ADD_ID { get; set; }
        public Nullable<int> HDR_IMPORTER_CONT_ID { get; set; }
    
        public virtual ADDRESS ADDRESS { get; set; }
        public virtual COMPANY COMPANY { get; set; }
        public virtual CONTACT CONTACT { get; set; }
        public virtual ICollection<HEADER> HEADER1 { get; set; }
        public virtual HEADER HEADER2 { get; set; }
        public virtual ICollection<ITEM> ITEMs { get; set; }
    }
}