using System;

namespace Cotecna.Voc.Business
{ 
    public class CertificateFilterModel
    {
        /// <summary>
        /// Gets or Set CertificateNumber
        /// </summary>		
        public string CertificateNumber
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or Set IssuanceDateFrom
        /// </summary>		
        public DateTime? IssuanceDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets IssuanceDateTo
        /// </summary>		
        public DateTime? IssuanceDateTo
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets SelectedEntryPointId
        /// </summary>		
        public int SelectedEntryPointId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets SelectedOffice
        /// </summary>		
        public int SelectedOffice
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets Published
        /// </summary>		
        public bool Published
        {
            get;
            set;
        }


        /// <summary>
        /// Gets o sets Unpublished
        /// </summary>		
        public bool Unpublished
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets MyDocuments
        /// </summary>		
        public bool MyDocuments
        {
            get;
            set;
        }



        /// <summary>
        /// Gets or sets Conform
        /// </summary>		
        public bool Conform
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets NonConform
        /// </summary>		
        public bool NonConform
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Cancelled
        /// </summary>		
        public bool Cancelled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Created
        /// </summary>		
        public bool Created
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets Requested
        /// </summary>		
        public bool Requested
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Approved
        /// </summary>		
        public bool Approved
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Rejected
        /// </summary>		
        public bool Rejected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Ongoing
        /// </summary>		
        public bool Ongoing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Closed
        /// </summary>		
        public bool Closed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Invoiced
        /// </summary>		
        public bool Invoiced
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Invoiced
        /// </summary>		
        public bool NonInvoiced
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ComdivNumber
        /// </summary>
        public string ComdivNumber { get; set; }
    }
}
