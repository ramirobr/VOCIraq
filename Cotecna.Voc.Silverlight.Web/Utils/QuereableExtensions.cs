using Cotecna.Voc.Business;
using LinqKit;
using System;
using System.Linq;
using System.Web;

namespace Cotecna.Voc.Silverlight.Web
{
    public static class QuereableExtensions
    {
        /// <summary>
        /// Apply filters in Certificates query
        /// </summary>
        /// <param name="query">Query to apply filters</param>
        /// <param name="filters">List of filters to apply</param>
        /// <returns>Filtered query</returns>
        public static IQueryable<Certificate> FilterCertificate(this IQueryable<Certificate> query, CertificateFilterModel filters)
        {
            //take the user that is logged
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            IQueryable<Certificate> queryfiltered = query;
            
            //Certificate status condition
            if (user.IsInRole(UserRoleEnum.BorderAgent) || user.IsInRole(UserRoleEnum.LOAdmin))
                queryfiltered = FilterCertificateStatusBoderAgent(filters, queryfiltered);
            else
                queryfiltered = FilterCertificateStatus(filters, queryfiltered);

            //Worflow conditon
            queryfiltered = FilterWorkflowStatus(filters, queryfiltered);
            //publish and non publish condition
            queryfiltered = FilterPublishNonPublish(filters, queryfiltered);
            //filter by invoiced or non invoiced
            queryfiltered = FilterInvoiced(filters, queryfiltered);

            //filter by comdiv number
            if (!string.IsNullOrEmpty(filters.ComdivNumber))
                queryfiltered = queryfiltered.Where(c => c.ComdivNumber.Contains(filters.ComdivNumber));

            //filter by certificate number
            if (!string.IsNullOrEmpty(filters.CertificateNumber))
                queryfiltered = queryfiltered.Where(c => c.Sequential.Contains(filters.CertificateNumber));

            //filter by dates
            if (filters.IssuanceDateFrom != null && filters.IssuanceDateTo != null)
            {
                DateTime finalDate =filters.IssuanceDateTo.GetValueOrDefault().AddDays(1).AddSeconds(-1);
                queryfiltered = queryfiltered.Where(c => c.IssuanceDate >= filters.IssuanceDateFrom && c.IssuanceDate <= finalDate);
            }
            //filter by entry point
            if (filters.SelectedEntryPointId > 0)
                queryfiltered = queryfiltered.Where(c => c.EntryPointId == filters.SelectedEntryPointId);

            //filter by office
            queryfiltered = FilterByOffice(filters, user, queryfiltered);

            //filter by creation by of approveded by
            if (filters.MyDocuments)
                queryfiltered = queryfiltered.Where(c => c.ApprovedBy == user.Name || c.CreationBy == user.Name);
            
            return queryfiltered;
        }

        /// <summary>
        /// Filter by office
        /// </summary>
        /// <param name="filters">Filter parameters</param>
        /// <param name="user">Logged user</param>
        /// <param name="queryfiltered">Query to filter</param>
        /// <returns></returns>
        private static IQueryable<Certificate> FilterByOffice(CertificateFilterModel filters, VocUser user, IQueryable<Certificate> queryfiltered)
        {
            //filter by office
            if (filters.SelectedOffice > 0)
                queryfiltered = queryfiltered.Where(c => c.OfficeId == filters.SelectedOffice);
            else if (filters.SelectedOffice == 0 && user.IsRoUser && user.IsInRole(UserRoleEnum.Coordinator, UserRoleEnum.Issuer))
            {
                VocEntities context = new VocEntities();
                var localOffices = context.Offices.Where(x => x.RegionalOfficeId == user.OfficeId).Select(x => x.OfficeId).ToList();
                var roOffice = context.Offices.FirstOrDefault(x => x.OfficeId == user.OfficeId).OfficeId;
                localOffices.Add(roOffice);
                context.Dispose();
                queryfiltered = queryfiltered.Where(c => localOffices.Contains(c.OfficeId.Value));
            }
            return queryfiltered;
        }


        /// <summary>
        /// Order the Certificates by UserRole
        /// </summary>
        /// <param name="query">Query to order</param>
        /// <param name="param">params for order query</param>
        /// <returns></returns>
        public static IQueryable<Certificate> OrderByUser(this IQueryable<Certificate> query)
        {
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;

            IQueryable<Certificate> queryordered = query;
            if (user.IsInRole(UserRoleEnum.Coordinator, UserRoleEnum.Issuer, UserRoleEnum.Admin))
            {
                queryordered = queryordered.OrderByDescending(c => c.CreationDate);
            }
            else if (user.IsInRole(UserRoleEnum.SuperAdmin, UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin, UserRoleEnum.Supervisor))
            {
                queryordered = queryordered.OrderByDescending(c => c.IssuanceDate)
                    .ThenByDescending(c => c.Sequential.Substring(6, 2))
                    .ThenByDescending(c => c.Sequential.Substring(9, 5));
            }

            return queryordered;
        }

        /// <summary>
        /// Filter by invoiced or non invoiced
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="queryfiltered">Query to filter</param>
        /// <returns>Query filtered</returns>
        private static IQueryable<Certificate> FilterInvoiced(CertificateFilterModel filters, IQueryable<Certificate> queryfiltered)
        {
            //filter by invoice
            if (filters.Invoiced || filters.NonInvoiced)
            {
                var predicateInvoiced = PredicateBuilder.False<Certificate>();
                if (filters.Invoiced)
                    predicateInvoiced = predicateInvoiced.Or(c => c.IsInvoiced == true);
                if (filters.NonInvoiced)
                    predicateInvoiced = predicateInvoiced.Or(c => c.IsInvoiced == false);
                queryfiltered = queryfiltered.AsExpandable().Where(predicateInvoiced);
            }
            return queryfiltered;
        }

        /// <summary>
        /// Filter by publish or non publish
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="queryfiltered">Query to filter</param>
        /// <returns>Query filtered</returns>
        private static IQueryable<Certificate> FilterPublishNonPublish(CertificateFilterModel filters, IQueryable<Certificate> queryfiltered)
        {
            //publish non-publish condition
            if (filters.Published || filters.Unpublished)
            {
                var predicatePublisUnpublish = PredicateBuilder.False<Certificate>();
                if (filters.Published)
                    predicatePublisUnpublish = predicatePublisUnpublish.Or(c => c.IsPublished == true);
                if (filters.Unpublished)
                    predicatePublisUnpublish = predicatePublisUnpublish.Or(c => c.IsPublished == false);
                queryfiltered = queryfiltered.AsExpandable().Where(predicatePublisUnpublish);
            }
            return queryfiltered;
        }

        /// <summary>
        /// Filter by workflow status
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="queryfiltered">Query to filter</param>
        /// <returns>Query filtered</returns>
        private static IQueryable<Certificate> FilterWorkflowStatus(CertificateFilterModel filters, IQueryable<Certificate> queryfiltered)
        {
            //workflow status condition
            if (filters.Created || filters.Requested || filters.Approved || filters.Rejected || filters.Ongoing || filters.Closed)
            {
                var predicateWorkflowStatus = PredicateBuilder.False<Certificate>();
                if (filters.Created)
                    predicateWorkflowStatus = predicateWorkflowStatus.Or(c => c.WorkflowStatusId == WorkflowStatusEnum.Created);
                if (filters.Requested)
                    predicateWorkflowStatus = predicateWorkflowStatus.Or(c => c.WorkflowStatusId == WorkflowStatusEnum.Requested);
                if (filters.Approved)
                    predicateWorkflowStatus = predicateWorkflowStatus.Or(c => c.WorkflowStatusId == WorkflowStatusEnum.Approved);
                if (filters.Rejected)
                    predicateWorkflowStatus = predicateWorkflowStatus.Or(c => c.WorkflowStatusId == WorkflowStatusEnum.Rejected);
                if (filters.Ongoing)
                    predicateWorkflowStatus = predicateWorkflowStatus.Or(c => c.WorkflowStatusId == WorkflowStatusEnum.Ongoing);
                if (filters.Closed)
                    predicateWorkflowStatus = predicateWorkflowStatus.Or(c => c.WorkflowStatusId == WorkflowStatusEnum.Closed);
                queryfiltered = queryfiltered.AsExpandable().Where(predicateWorkflowStatus);
            }
            return queryfiltered;
        }

        /// <summary>
        /// Filter by certificate status
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="queryfiltered">Query to filter</param>
        /// <returns>Query filtered</returns>
        private static IQueryable<Certificate> FilterCertificateStatus(CertificateFilterModel filters, IQueryable<Certificate> queryfiltered)
        {
            if (filters.Conform || (filters.NonConform || filters.Cancelled))
            {
                var predicateCertificateStatus = PredicateBuilder.False<Certificate>();
                if (filters.Conform)
                    predicateCertificateStatus = predicateCertificateStatus.Or(c => c.CertificateStatusId == CertificateStatusEnum.Conform);
                if (filters.NonConform)
                    predicateCertificateStatus = predicateCertificateStatus.Or(c => c.CertificateStatusId == CertificateStatusEnum.NonConform);
                if (filters.Cancelled)
                    predicateCertificateStatus = predicateCertificateStatus.Or(c => c.CertificateStatusId == CertificateStatusEnum.Cancelled);
                queryfiltered = queryfiltered.AsExpandable().Where(predicateCertificateStatus);
            }
            return queryfiltered;
        }

        /// <summary>
        /// Filter by certificate status for a border agent
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="queryfiltered">Query to filter</param>
        /// <returns>Query filtered</returns>
        private static IQueryable<Certificate> FilterCertificateStatusBoderAgent(CertificateFilterModel filters, IQueryable<Certificate> queryfiltered)
        {
            var predicateCertificateStatus = PredicateBuilder.False<Certificate>();
            if (filters.Conform || filters.NonConform)
            {
                if (filters.Conform)
                    predicateCertificateStatus = predicateCertificateStatus.Or(c => c.CertificateStatusId == CertificateStatusEnum.Conform);
                if (filters.NonConform)
                    predicateCertificateStatus = predicateCertificateStatus.Or(c => c.CertificateStatusId == CertificateStatusEnum.NonConform);
                queryfiltered = queryfiltered.AsExpandable().Where(predicateCertificateStatus);
            }
            
            queryfiltered = queryfiltered.Where(c => c.CertificateStatusId != CertificateStatusEnum.Cancelled);
            
            return queryfiltered;
        }
   
    }
}