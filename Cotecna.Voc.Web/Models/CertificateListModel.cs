using Cotecna.Voc.Business;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Cotecna.Voc.Web.Models
{
    /// <summary>
    /// Contain the information to be displayed in the certificate search view
    /// </summary>
    public class CertificateListModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets the list of certificates.
        /// </summary>
        /// <value>The certificates.</value>
        public PaginatedList<CertificateDocument> Certificates { get; set; }

        #region Filter Properties
        /// <summary>
        /// Gets or sets the certificate number filter.
        /// </summary>        
        public string CertificateNumber { get; set; }

        /// <summary>
        /// Gets or sets the issuance date from
        /// </summary>        
        public string IssuanceDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the issuance date to
        /// </summary>        
        public string IssuanceDateTo { get; set; }

        /// <summary>
        /// Gets or sets the list of certificate status.
        /// </summary>
        /// <value>The list of certificate status.</value>
        public SelectList CertificateStatus { get; set; }

        /// <summary>
        /// Gets or sets the list of entry points.
        /// </summary>
        /// <value>The list of entry points.</value>
        public SelectList EntryPoints { get; set; }

        /// <summary>
        /// Gets or sets the entry point selected
        /// </summary>        
        public int? EntryPointSelected { get; set; }

        /// <summary>
        /// Gets or sets the certificate status selected
        /// </summary>        
        public string CertificateStatusSelected { get; set; }

        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Searh the certificate list in base the filter criteria
        /// </summary>
        /// <param name="selectedPage">Selected page</param>
        /// <param name="isExport">When the data is exported to Microsoft Excel</param>
        public void SearchCertificateList(int selectedPage, bool isExport = false)
        {
            PaginatedList<CertificateDocument> result = new PaginatedList<CertificateDocument>();
            int pageSize = result.PageSize;
            int currentIndex = (selectedPage - 1) * pageSize;
            using (VocEntities db = new VocEntities())
            {
                var query =
                (from certificate in db.Certificates
                 join document in db.Documents on certificate.CertificateId equals document.CertificateId into outer
                 from document in outer.Where(f => f.IsDeleted == false && f.IsSupporting == false).DefaultIfEmpty()
                 where certificate.IsDeleted == false && certificate.IsPublished == true
                 select new CertificateDocument 
                 { 
                     Certificate = certificate, 
                     FileName = document.Filename, 
                     FilePath = document.FilePath, 
                     EntryPoint = certificate.EntryPoint 
                 });
                
                query = FilterQuery.CreateCertificateFilters(query, this);

                List<CertificateDocument> queryList = null;
                if (!isExport)
                {
                    queryList = query.OrderByDescending(item => item.Certificate.Sequential.Substring(6, 2))
                    .ThenByDescending(item => item.Certificate.Sequential.Substring(9, 5))
                    .Skip(currentIndex)
                    .Take(pageSize)
                    .ToList();
                }
                else
                {
                    queryList = query.OrderByDescending(item => item.Certificate.Sequential.Substring(6, 2))
                    .ThenByDescending(item => item.Certificate.Sequential.Substring(9, 5))
                    .ToList();
                }

                result.Collection = queryList;

                //set the quantity of elements without pagination
                result.TotalCount = query.Count();

                //set the number of pages
                result.NumberOfPages = (int)Math.Ceiling((double)result.TotalCount / (double)result.PageSize);

                result.Page = selectedPage;
                
                Certificates = result;

            }
        }       

        #endregion
    }

    /// <summary>
    /// Item to be displayed in each row of the certificate list
    /// </summary>
    public class CertificateDocument
    {
        /// <summary>
        /// Certificate information
        /// </summary>
        public Certificate Certificate { get; set; }
        /// <summary>
        /// Part of the path where the certificate pdf file is stored
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// File name of the certificate pdf file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Certificate entry point
        /// </summary>
        public EntryPoint EntryPoint { get; set; }

        public int test { get; set; }

    }

    /// <summary>
    /// Include dinamic conditions in a LINQ query according the filters selected in the screen
    /// </summary>
    public static class FilterQuery
    {
        public static IQueryable<CertificateDocument> CreateCertificateFilters(this IQueryable<CertificateDocument> query, CertificateListModel certificateListModel)
        {
            IQueryable<CertificateDocument> filters = query;
            //Filter Certificate Number
            if (!string.IsNullOrEmpty(certificateListModel.CertificateNumber))
                filters = filters.Where(y => y.Certificate.Sequential.Contains(certificateListModel.CertificateNumber));

            //Filter Certificate Issuance date from
            if (!string.IsNullOrEmpty(certificateListModel.IssuanceDateFrom))
            {
                DateTime f2;
                bool success = DateTime.TryParse(certificateListModel.IssuanceDateFrom, new System.Globalization.CultureInfo("fr-FR"), System.Globalization.DateTimeStyles.None, out f2);
                if (success)
                {
                    filters = filters.Where(z => z.Certificate.IssuanceDate >= f2);
                }
            }

            //Filter Certificate Issuance date to
            if (!string.IsNullOrEmpty(certificateListModel.IssuanceDateTo))
            {
                DateTime f2;
                bool success = DateTime.TryParse(certificateListModel.IssuanceDateTo, new System.Globalization.CultureInfo("fr-FR"), System.Globalization.DateTimeStyles.None, out f2);
                if (success)
                {
                    f2 = f2.AddDays(1);
                    filters = filters.Where(z => z.Certificate.IssuanceDate < f2);
                }
            }

            //Filter Certificate Status
            if (!string.IsNullOrEmpty(certificateListModel.CertificateStatusSelected))
            {
                List<CertificateStatusEnum> statusx = new List<CertificateStatusEnum>();
                
                certificateListModel.CertificateStatusSelected.Split(',').ToList().ForEach(x =>
                        {
                            statusx.Add((CertificateStatusEnum)Convert.ToInt32(x));
                        });
                filters = filters.Where(y => statusx.Contains(y.Certificate.CertificateStatusId));
            }

            //Filter Entry Point
            if (certificateListModel.EntryPointSelected.HasValue)
                filters = filters.Where(y => y.Certificate.EntryPointId == certificateListModel.EntryPointSelected.Value);

            return filters;
        }
    }


}