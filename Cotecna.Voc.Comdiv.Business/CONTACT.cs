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
    
    public partial class CONTACT
    {
        public CONTACT()
        {
            this.HEADERs = new HashSet<HEADER>();
            this.INVOICEs = new HashSet<INVOICE>();
            this.ITPs = new HashSet<ITP>();
        }
    
        public int CON_ID { get; set; }
        public int ADD_ID { get; set; }
        public string CON_FIRSTNAME { get; set; }
        public string CON_LASTNAME { get; set; }
        public Nullable<int> CON_TITLE_ID { get; set; }
        public string CON_TEL { get; set; }
        public string CON_MOBILE { get; set; }
        public string CON_FAX { get; set; }
        public string CON_EMAIL { get; set; }
        public Nullable<int> CON_LANGUAGE_ID { get; set; }
        public string CON_POSITION { get; set; }
        public Nullable<bool> CON_NO_MORE_VALID_YN { get; set; }
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
        public Nullable<int> CON_SEND_PREFERRED { get; set; }
        public System.Guid rowguid { get; set; }
    
        public virtual ADDRESS ADDRESS { get; set; }
        public virtual ICollection<HEADER> HEADERs { get; set; }
        public virtual ICollection<INVOICE> INVOICEs { get; set; }
        public virtual ICollection<ITP> ITPs { get; set; }
    }
}
