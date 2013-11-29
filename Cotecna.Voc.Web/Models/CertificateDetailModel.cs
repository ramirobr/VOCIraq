using Cotecna.Voc.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotecna.Voc.Web.Models
{
    /// <summary>
    /// Contains the information of the certificate and documents (certificate+supporting documents) related
    /// </summary>
    public class CertificateDetailModel
    {
        #region Properties
        /// <summary>
        /// Certificate information
        /// </summary>
        public Certificate Certificate { get; set; }
        /// <summary>
        /// Certificate or supporting documents
        /// </summary>
        public PaginatedList<Document> Documents { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Get the certificate data with its documents paginated
        /// </summary>
        /// <param name="certificateId">Certificate id</param>
        /// <param name="searchHeader">Flag to know if it is required to get the certificate information or only the documents</param>
        public void GetCertificateDetail(int certificateId, bool searchHeader)
        {
            PaginatedList<Document> result = new PaginatedList<Document>(); 
            using (VocEntities db = new VocEntities())
            {
                //Get the certificate data detail
                if (searchHeader)
                {
                    Certificate =
                    (from certificate in db.Certificates.Include("EntryPoint")
                     where certificate.CertificateId == certificateId
                     select certificate).FirstOrDefault();
                }

                //Get the documents related to the certificate
                var querydocs = (from documents in db.Documents
                                 where documents.CertificateId == certificateId && documents.IsDeleted == false
                                 orderby documents.Certificate.IssuanceDate descending
                                 select documents);

                result.TotalCount = querydocs.Count();

                var supportDocument = querydocs.Where(doc => doc.DocumentType == DocumentTypeEnum.SupportingDocument).ToList();
                var releaseNotes = querydocs.Where(doc => doc.DocumentType == DocumentTypeEnum.ReleaseNote).ToList();
                supportDocument.AddRange(releaseNotes);

                var certificatDoc = querydocs.Where(doc => doc.DocumentType == DocumentTypeEnum.Certificate).FirstOrDefault();
                if (certificatDoc != null) supportDocument.Insert(0, certificatDoc);

                result.Collection = supportDocument;

                Documents = result;
                    
            }
        }
        #endregion
    }
}