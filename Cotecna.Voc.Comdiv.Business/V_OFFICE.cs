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
    
    public partial class V_OFFICE
    {
        public int OFF_ID { get; set; }
        public string OFF_CODE { get; set; }
        public string OFF_DESCRIPTION { get; set; }
        public Nullable<int> OFF_MAIN { get; set; }
        public string OFF_NAME { get; set; }
        public string OFF_ADDRESS { get; set; }
        public string OFF_ZIPCODE { get; set; }
        public string OFF_CITY { get; set; }
        public string OFF_STATE { get; set; }
        public Nullable<short> OFF_COUNTRY { get; set; }
        public Nullable<short> OFF_CURRENCY { get; set; }
        public byte OFF_LANGUAGE { get; set; }
        public Nullable<float> OFF_TIMELAG_SUMMER { get; set; }
        public Nullable<float> OFF_TIMELAG_WINTER { get; set; }
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