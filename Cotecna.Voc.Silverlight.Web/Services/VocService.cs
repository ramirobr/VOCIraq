
namespace Cotecna.Voc.Silverlight.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Cotecna.Voc.Business;
    using System.Web;
    using Cotecna.Voc.Silverlight.Web.Properties;
    using System.Transactions;
    using System.IO;
    using System.Data;
    using System.Web.Hosting;
    using Cotecna.Voc.Silverlight.Web.Resources;
    using LinqKit;
    using System.Text.RegularExpressions;
    using System.Configuration;
    using Cotecna.Voc.Comdiv.Business;

    
    [EnableClientAccess()]
    public class VocService : CustomDomainService
    {
        private VocEntities ctx = new VocEntities();

        /// <summary>
        /// Allow count the total item of Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">Query to Count</param>
        /// <returns>Total number of item</returns>
        protected override int Count<T>(IQueryable<T> query)
        {
            return query.Count();
        }

        /// <summary>
        /// Get the list of certificates
        /// </summary>
        /// <returns></returns>
        public IQueryable<Certificate> GetCertificates(string certificateNumber, DateTime? issuanceDateFrom,
            DateTime? issuanceDateTo, int selectedEntryPointId, int selectedOffice, bool published,
            bool unpublished, bool myDocuments, bool conform, bool nonConform, bool cancelled,
            bool created, bool requested, bool approved, bool rejected, bool ongoing, 
            bool closed,bool invoiced,bool nonInvoiced, string comdivNumber)
        {
            CertificateFilterModel filters = new CertificateFilterModel
            {
                Approved = approved,
                Cancelled = cancelled,
                CertificateNumber = certificateNumber,
                Closed = closed,
                Conform = conform,
                Created = created,
                IssuanceDateFrom = issuanceDateFrom,
                IssuanceDateTo = issuanceDateTo,
                MyDocuments = myDocuments,
                NonConform = nonConform,
                Ongoing = ongoing,
                Published = published,
                Rejected = rejected,
                Requested = requested,
                SelectedEntryPointId = selectedEntryPointId,
                SelectedOffice = selectedOffice,
                Unpublished = unpublished,
                Invoiced = invoiced,
                NonInvoiced = nonInvoiced,
                ComdivNumber = comdivNumber
            };
            return ctx.Certificates.Where(x => x.IsDeleted == false)
                .FilterCertificate(filters).OrderByUser();
        }

        /// <summary>
        /// Retrieve the Entry Point List
        /// </summary>
        /// <returns>Query of Entry Point List</returns>
        [Query]
        public IQueryable<EntryPoint> GetEntryPoints()
        {
            return ctx.EntryPoints.Where(entry => entry.IsDeleted == false);
        }

        /// <summary>
        /// Retrieve the Office List
        /// </summary>
        /// <returns>Query of Office List</returns>
        [Query]
        public IQueryable<Office> GetOffices()
        {
            return ctx.Offices.Where(office => office.IsDeleted == false);
        }

        /// <summary>
        /// Get's the office by office id
        /// </summary>
        /// <param name="officeId"></param>
        /// <returns>Office if it already exists</returns>
        [Invoke]
        public Office GetOfficeByOfficeId(int officeId)
        {   
            Office office = ctx.Offices.Where(x=> x.OfficeId==officeId).FirstOrDefault();
            return office;
        }

        /// <summary>
        /// Returns a list of Validation Messages according to the fields of the office that already exist.
        /// </summary>
        /// <param name="officeId">Office Id</param>
        /// <param name="officeName">Office Name</param>
        /// <param name="officeCode">Office Code</param>
        /// <returns>List of Validation Messages</returns>
        [Invoke]
        public List<ValidationMessage> OfficeAlreadyExists(int officeId, string officeName, string officeCode)
        {
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            
            if (!string.IsNullOrEmpty(officeName) && !string.IsNullOrEmpty(officeCode))
            {
                if (officeId == 0)
                {
                    // This validation will be executed when we are going to create a new office.
                    if (ctx.Offices.Any(x => x.OfficeName.Replace(" ", string.Empty).CompareTo(officeName.Replace(" ", string.Empty)) == 0))
                    {
                        validationMessage = new ValidationMessage();
                        validationMessage.Identifier =  ServiceResource.OfficeName;
                        validations.Add(validationMessage);
                    }
                    if (ctx.Offices.Any(x => x.OfficeCode.Replace(" ", string.Empty).CompareTo(officeCode.Replace(" ", string.Empty)) == 0))
                    {
                        validationMessage = new ValidationMessage();
                        validationMessage.Identifier = ServiceResource.OfficeCode;
                        validations.Add(validationMessage);
                    }
                }
                else
                {
                    // This validation will be executed when we are updating an office.
                    if (ctx.Offices.Any(x => x.OfficeName.Replace(" ", string.Empty).CompareTo(officeName.Replace(" ", string.Empty))  == 0 && x.OfficeId != officeId))
                    {
                        validationMessage = new ValidationMessage();
                        validationMessage.Identifier = ServiceResource.OfficeName;
                        validations.Add(validationMessage);
                    }
                    if (ctx.Offices.Any(x => x.OfficeCode.Replace(" ", string.Empty).CompareTo( officeCode.Replace(" ", string.Empty)) == 0 && x.OfficeId != officeId))
                    {
                        validationMessage = new ValidationMessage();
                        validationMessage.Identifier = ServiceResource.OfficeCode;
                        validations.Add(validationMessage);
                    }
                }
            }
            return validations;

        }

        /// <summary>
        /// Retrieve the Office List based on filters done by the user
        /// </summary>
        /// <param name="officeName">Office name</param>
        /// <param name="officeCode">Office code</param>
        /// <param name="active">Is active</param>
        /// <param name="inactive">Is inactive</param>
        /// <param name="isRegionalOffice">Is regional office</param>
        /// <returns>Query of offices</returns>
        [Query]
        public IQueryable<Office> GetFilteredOffices(string officeName, string officeCode, bool active, bool inactive, bool isRegionalOffice)
        {
            ctx.Configuration.AutoDetectChangesEnabled = true;
            IQueryable<Office> officeQuery = ctx.Offices;

            if (!string.IsNullOrEmpty(officeName))
                officeQuery = officeQuery.Where(x => x.OfficeName.Contains(officeName));
            if (!string.IsNullOrEmpty(officeCode))
                officeQuery = officeQuery.Where(x => x.OfficeCode.Contains(officeCode));

            if (active || inactive)
            {
                var predicateActiveInactive = PredicateBuilder.False<Office>();

                if (active) // it means that the office was not deleted
                    predicateActiveInactive = predicateActiveInactive.Or(o => o.IsDeleted == false);
                if (inactive) //it means that the office was already deleted
                    predicateActiveInactive = predicateActiveInactive.Or(o => o.IsDeleted == true);

                officeQuery = officeQuery.AsExpandable().Where(predicateActiveInactive);
            }

            //shows or not regional offices
            if (isRegionalOffice)
                officeQuery = officeQuery.Where(x => x.OfficeType == OfficeTypeEnum.RegionalOffice);

            return officeQuery;
        }

        /// <summary>
        /// Retieve the Certificate by its identifier
        /// </summary>
        /// <param name="certificateId">Certificate</param>
        /// <returns>Certificate Entity</returns>
        public Certificate GetCertificateByCertificateId(int certificateId)
        {
            return ctx.Certificates.Find(certificateId);
        }

        
        /// <summary>
        /// Retieve the Document List attached to certificate
        /// </summary>
        /// <param name="certificateId">Certificate Identifier</param>
        /// <returns>Query of Document List</returns>
        [Query]
        public IQueryable<Document> GetDocumentsByCertificateId(int certificateId)
        {
            return ctx.Documents.Where(x=> x.CertificateId==certificateId && x.IsDeleted==false && x.DocumentType != DocumentTypeEnum.ReleaseNote);
        }

        /// <summary>
        /// Retieve the Release Note List attached to certificate
        /// </summary>
        /// <param name="certificateId">Certificate Identifier</param>
        /// <returns>Query of Release Note List</returns>
        [Query]
        public IQueryable<ReleaseNote> GetReleaseNotesByCertificateId(int certificateId)
        {
            return ctx.ReleaseNotes.Where(x => x.CertificateId == certificateId && x.IsDeleted == false);
        }

        /// <summary>
        /// Perform an approval with the validations of the process to a list of certificates
        /// </summary>
        /// <param name="certificates">List of certificates to approve</param>
        /// <returns>Validation messages</returns>
        [Invoke]
        public List<ValidationMessage> ApproveCertificateList(List<Certificate> certificates)
        {
            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            StatusProcess status = StatusProcess.Success;

            foreach (var certificate in certificates)
            {
                status = StatusProcess.Success;
                messages = string.Empty;
                //show an alert message if the user has not signatue assigned.
                if (!user.HasSignature)
                {
                    messages += ServiceResource.CertificateApproveSignatureValidationError;
                    status = StatusProcess.GenericError;
                }
                else if (certificate.EntryPointId == null && certificate.IsInvoiced == null && certificate.CertificateStatusId != CertificateStatusEnum.NonConform)
                {
                    messages += string.Format(ServiceResource.CertificateApproveValidationErrorPlural, ServiceResource.EntryPoint + "\" and \"" + ServiceResource.Invoiced) ;
                    status = StatusProcess.Error;
                }
                else if (certificate.EntryPointId == null && certificate.CertificateStatusId != CertificateStatusEnum.NonConform)
                {
                    messages += string.Format(ServiceResource.CertificateApproveValidationError, ServiceResource.EntryPoint);
                    status = StatusProcess.Error;
                }
                else if (certificate.IsInvoiced == null && certificate.CertificateStatusId != CertificateStatusEnum.NonConform)
                {
                    messages += ServiceResource.NotInvoiceMessage ;
                    status = StatusProcess.Error;
                }
                if (status == StatusProcess.Success)
                {
                    //verify mandatory documents
                    if (VerifyRequiredDocumentsInCertificate(certificate.CertificateId))
                    {
                        //approve certificate
                        string appoveResult = ApproveCertificate(certificate.CertificateId, user.Name);
                        //verify result of approval
                        if (!string.IsNullOrEmpty(appoveResult))
                        {
                            messages += appoveResult ;
                            status = StatusProcess.Error;
                        }
                        else
                        {
                            //send an email with the approval information
                            string emailResult = SendEmailApprove(certificate.Sequential, user.OfficeId);
                            if (!string.IsNullOrEmpty(emailResult))
                            {
                                messages += emailResult;
                                status = StatusProcess.GenericWarning;
                            }
                            //send no bordeer fees email
                            if (certificate.CertificateStatusId == CertificateStatusEnum.Conform && !certificate.IsInvoiced.GetValueOrDefault())
                                SendEmailLoNoBorderFees(certificate.EntryPointId.GetValueOrDefault(), certificate.Sequential);
                        }
                    }
                    else
                    {
                        messages += ServiceResource.CertificateDocumentValidator ;
                        status = StatusProcess.Error;
                    }
                }
                validationMessage = new ValidationMessage
                {
                    Identifier = certificate.ComdivNumber,
                    Status = status,
                    Message = messages
                };
                validations.Add(validationMessage);

            }
            return validations;
        }

        /// <summary>
        /// Recall a certificate list
        /// </summary>
        /// <param name="certificates">List of certificates to recall</param>
        /// <returns>Validation messages</returns>
        [Invoke]
        public List<ValidationMessage> RecallCertificateList(List<Certificate> certificates)
        {
            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            StatusProcess status = StatusProcess.Success;

            foreach (var certificate in certificates)
            {
                messages = string.Empty;
                if (certificate.IsPublished)
                {
                    messages += ServiceResource.CertificateRecallPublishValidationError ;
                    status = StatusProcess.Error;
                }
                if (status == StatusProcess.Success)
                {
                    string recallResult = RecallCertificate(certificate.CertificateId, user.Name);
                    if(!string.IsNullOrEmpty(recallResult))
                    {
                        messages += recallResult ;
                        status = StatusProcess.Error;
                    }
                }
                validationMessage = new ValidationMessage
                {
                    Identifier = certificate.Sequential,
                    Status = status,
                    Message = messages
                };
                validations.Add(validationMessage);
            }

            return validations;
        }
              
        /// <summary>
        /// Approve Certificate
        /// </summary>
        /// <param name="certificateId">Certificate Identifier to be approved</param>
        /// <param name="userName">User Name who approves the certificate</param>
        /// <returns>Error message</returns>
        [Invoke]
        public string ApproveCertificate(int certificateId, string userName)
        {
            string errorValidation = string.Empty;
            try
            {
                //get the current user
                VocUser currentUser = HttpContext.Current.Cache.Get("LoggedUser" + userName) as VocUser;
                var userSignature = AuthenticationDomainService.GetSignatureByUser(userName);
                var officeSignet = GetSignetByOfficeId(currentUser.OfficeId);

                using (TransactionScope ts = new TransactionScope())
                {
                    using (VocEntities context = new VocEntities())
                    {
                        var certificate = context.Certificates.FirstOrDefault(x => x.CertificateId == certificateId);
                        var certificateDocument = context.Documents.FirstOrDefault(x => x.CertificateId == certificateId && x.IsDeleted == false && x.IsSupporting == false);
                        string newFilePath = string.Empty;
                        string fixedComdivNumber = certificate.CertificateId.ToString();
                        string sourcePath = string.Empty;

                        //Update Certificate fields
                        if (certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
                            certificate.WorkflowStatusId = WorkflowStatusEnum.Closed;
                        else
                            certificate.WorkflowStatusId = WorkflowStatusEnum.Approved;

                        if (string.IsNullOrEmpty(certificate.Sequential))
                        {
                            certificate.Sequential = GetNewSecuentialNumber();

                            newFilePath = certificateDocument.FilePath.Replace(fixedComdivNumber, certificate.Sequential);
                            sourcePath = GetDocumentFilePathRequest(certificate, false);
                        }
                        else
                        {
                            newFilePath = certificateDocument.FilePath;
                            sourcePath = GetSourcePathWithEntryPoint(certificate, true);
                        }

                        certificate.IssuanceDate = DateTime.Now;
                        certificate.ApprovedBy = userName;

                        //Set the word document as deleted
                        certificateDocument.IsDeleted = true;

                        //Create new item to pdf document
                        Document document = new Document();
                        document.CertificateId = certificateDocument.CertificateId;
                        document.IsSupporting = certificateDocument.IsSupporting;
                        document.DocumentType = DocumentTypeEnum.Certificate;
                        document.Filename = certificateDocument.Filename.Replace(".docx", ".pdf");
                        document.FilePath = newFilePath;
                        document.Description = certificateDocument.Description;
                        document.CreationBy = HttpContext.Current.User.Identity.Name;
                        document.CreationDate = DateTime.Now;

                        if (certificate.EntryPoint != null)
                        {
                            document.FilePath = Path.Combine(certificate.EntryPoint.Name, newFilePath);

                            var docs = ctx.Documents.Where(x => x.CertificateId == certificateId && x.IsDeleted == false && x.IsSupporting == true).ToList();
                            foreach (var doc in docs)
                            {
                                if (certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                                    doc.FilePath = Path.Combine(certificate.EntryPoint.Name, newFilePath);
                                else
                                    doc.FilePath = newFilePath;
                            }
                        }
                        else
                        {
                            var docs = ctx.Documents.Where(x => x.CertificateId == certificateId && x.IsDeleted == false && x.IsSupporting == true).ToList();
                            foreach (var doc in docs)
                            {
                                doc.FilePath = newFilePath;
                            }
                        }
                        context.Documents.Add(document);
                        

                        string certificatePath = Path.Combine(Properties.Settings.Default.PathDocument, certificateDocument.FilePath, certificateDocument.Filename).Replace("/", "\\");

                        var ms = WordManagement.GenerateWordReport(certificate, certificatePath, out errorValidation, userSignature, currentUser.DisplayName, officeSignet, currentUser.OfficeName);
                        //If errorValidation, return this message and stop process
                        if (!string.IsNullOrEmpty(errorValidation))
                            return errorValidation;
                        
                        WordManagement.SaveWordReportAsPdf(ms, certificatePath);

                        string finalPath;
                        

                        if(certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                            finalPath = GetSourcePathWithEntryPoint(certificate, false);
                        else
                            finalPath = GetNCRApprovalPath(certificate.Sequential);

                        if (sourcePath != finalPath)
                        {
                            MoveAll(sourcePath, finalPath);
                            Directory.Delete(sourcePath);
                        }

                        if (certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
                            SaveCertificateTrancking(certificateId, TrackingStatusEnum.Closed);
                        else
                            SaveCertificateTrancking(certificateId, TrackingStatusEnum.Approved);

                        context.SaveChanges();
                        ts.Complete();
                        
                    }
                    
                }
                
            }
            catch (System.UriFormatException ex)
            {
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                errorValidation = ServiceResource.CertificateIncorrectFormat;
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                errorValidation =  ex.Message;               
            }
            return errorValidation;
        }

        /// <summary>
        /// Remove special characters in comdiv number
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static string FixComdivNumberInformation(Certificate certificate)
        {
            string fixedComdivNumber = certificate.ComdivNumber;
            fixedComdivNumber = Regex.Replace(fixedComdivNumber, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
            return fixedComdivNumber;
        }

        /// <summary>
        /// Recall the Certificate
        /// </summary>
        /// <param name="certificateId">Certificate Identifier to be recallled</param>
        /// <param name="userName">User Name who recalls the certificate</param>
        /// <returns>Error message</returns>
        [Invoke]
        public string RecallCertificate(int certificateId, string userName)
        {
            string logErrors = string.Empty;
            string certificateNumber = string.Empty;
            int officeId = 0;
            bool userCanSync = (HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser).CanSync;
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    using (VocEntities context = new VocEntities())
                    {
                        var certificate = context.Certificates.FirstOrDefault(x => x.CertificateId == certificateId);
                        var certificateDocumentPdf = context.Documents.FirstOrDefault(x => x.CertificateId == certificateId && 
                            x.IsDeleted == false && (x.IsSupporting == false || x.DocumentType.Value == DocumentTypeEnum.Certificate ));
                        var certificateDocumentWord = context.Documents.FirstOrDefault(x => x.CertificateId == certificateId && 
                            x.IsDeleted == true && (x.IsSupporting == false || x.DocumentType.Value == DocumentTypeEnum.Certificate));

                        //Update Certificate fields
                        certificate.WorkflowStatusId = WorkflowStatusEnum.Requested;
                        certificate.IssuanceDate = null;
                        certificate.ApprovedBy = string.Empty;
                        certificate.ModificationBy = userName;
                        certificate.ModificationDate = DateTime.Now;
                        if (userCanSync)
                            certificate.IsSynchronized = false;

                        //Recover the word document deleted
                        certificateDocumentWord.IsDeleted = false;
                        string filePath = Path.Combine(string.Concat(certificate.Sequential, "\\"));
                        certificateDocumentWord.FilePath = filePath;

                        //Remove the item as pdf document
                        context.Documents.Remove(certificateDocumentPdf);

                        //Come back the documents to original folder without entrypoint name
                        if (certificate.EntryPoint != null)
                        {
                            var docs = ctx.Documents.Where(x => x.CertificateId == certificateId && x.IsDeleted == false && x.IsSupporting == true).ToList();
                            foreach (var doc in docs)
                            {
                                doc.FilePath = filePath;
                            }
                        }

                        context.SaveChanges();
                        //save tracking
                        SaveCertificateTrancking(certificateId, TrackingStatusEnum.Requested);

                        string certificatePathPdf = Path.Combine(Properties.Settings.Default.PathDocument, certificateDocumentPdf.FilePath, certificateDocumentPdf.Filename).Replace("/", "\\");

                        if (File.Exists(certificatePathPdf))
                            File.Delete(certificatePathPdf);

                        //Come back the documents physically to original folder without entrypoint name
                        if (certificate.EntryPoint != null)
                        {
                            certificate.WorkflowStatusId = WorkflowStatusEnum.Approved;
                            string sourcePath = GetSourcePathWithEntryPoint(certificate, false);
                            MoveAll(sourcePath, GetSourcePathWithEntryPoint(certificate, true));
                            Directory.Delete(sourcePath);
                        }
                        certificateNumber = certificate.Sequential;
                        officeId = certificate.OfficeId.Value;
                        ts.Complete();
                    }
                    
                }

                logErrors = SendEmailRecall(certificateNumber, officeId);
            }
            catch (IOException ex)
            {
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                logErrors = ServiceResource.FileOpenMessage;
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                logErrors = ex.Message;
            }
            return logErrors;
        }

        /// <summary>
        /// Gets the string path to save the documents of a certificate at least approve
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="office"></param>
        /// <returns></returns>
        private string GetSourcePathWithEntryPoint(Certificate certificate,bool removeEntryPoint)
        {
            string path = string.Empty;
            path =  Path.Combine(Properties.Settings.Default.PathDocument, GetDocumentFilePath(certificate)).Replace("/", "\\");
            if (removeEntryPoint && certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                path = path.Replace(certificate.EntryPoint.Name + "\\", "");
            return path;
        }

        private string GetNCRApprovalPath(string sequential)
        {
            string path = string.Empty;
            path = string.Concat(Properties.Settings.Default.PathDocument, sequential,"\\");
            return path;
        }

        [Invoke]
        public bool VerifyIssuersInUserOffice(int officeId)
        {
            using (UsersContext context = new UsersContext())
            {
                return (from user in context.UserProfiles
                        join userRole in context.UserInRoles on user.UserId equals userRole.UserId
                        join role in context.Roles on userRole.RoleId equals role.RoleId
                        where (role.RoleId == (int)UserRoleEnum.Issuer || role.RoleId == (int)UserRoleEnum.Admin)
                        && user.IsActive == true
                        && user.Email != null && user.OfficeId == officeId
                        select user.Email).Any();
            }
        }

        /// <summary>
        /// Send an email notification when a request for aprroval is performed
        /// </summary>
        /// <param name="certificateNumber">The number of the certificate</param>
        /// <param name="officeId">The id of the office</param>
        /// <returns>A result</returns>
        [Invoke]
        public string SendEmailRequest(string certificateNumber,int officeId)
        {
            string response = string.Empty;
            using (UsersContext context = new UsersContext())
            {
                //get the list of issuers
                string[] issuerEmailList = (from user in context.UserProfiles
                                            join userRole in context.UserInRoles on user.UserId equals userRole.UserId
                                            join role in context.Roles on userRole.RoleId equals role.RoleId
                                            where (role.RoleId == (int)UserRoleEnum.Issuer || role.RoleId == (int)UserRoleEnum.Admin)
                                            && user.IsActive == true
                                            && user.Email != null && user.OfficeId == officeId
                                            select user.Email).ToArray();

                //if there are no issuers, return a warnig message
                if (issuerEmailList.Length == 0 || issuerEmailList == null)
                {
                    response = ServiceResource.NonExistIssuersMessage;
                }
                else
                {
                    try
                    {
                        //get the template
                        string emailTemplate = File.ReadAllText(HostingEnvironment.MapPath("~/Templates/RequestTemplate.html"));
                        //set certificate numbert
                        emailTemplate = emailTemplate.Replace("{COMDIV NUMBER}", certificateNumber);
                        //send the emails
                        EmailManagement.SendEmail(issuerEmailList, emailTemplate, ServiceResource.RequestApprovalSubjectMessage + " " + certificateNumber, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                        response = ServiceResource.EmailErrorMessage;
                    }
                }
            }
            return response;
        }

        [Invoke]
        public string SendEmailRecall(string certificateNumber, int officeId)
        {
            string response = string.Empty;
            using (UsersContext context = new UsersContext())
            {
                //get the list of issuers
                string[] issuerEmailList = (from user in context.UserProfiles
                                            join userRole in context.UserInRoles on user.UserId equals userRole.UserId
                                            join role in context.Roles on userRole.RoleId equals role.RoleId
                                            where (role.RoleId == (int)UserRoleEnum.Issuer || role.RoleId == (int)UserRoleEnum.Admin)
                                            && user.IsActive == true
                                            && user.Email != null && user.OfficeId == officeId
                                            select user.Email).ToArray();

                //if there are no issuers, return a warnig message
                if (issuerEmailList.Length == 0 || issuerEmailList == null)
                {
                    response = ServiceResource.NonExistIssuersMessage;
                }
                else
                {
                    try
                    {
                        //get the template
                        string emailTemplate = File.ReadAllText(HostingEnvironment.MapPath("~/Templates/RecallTemplate.html"));
                        //set certificate numbert
                        emailTemplate = emailTemplate.Replace("{CERTIFICATE NUMBER}", certificateNumber);
                        //send the emails
                        EmailManagement.SendEmail(issuerEmailList, emailTemplate, ServiceResource.RequestApprovalSubjectMessage + " " + certificateNumber, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);

                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                        response = ServiceResource.EmailErrorMessage;
                    }
                }
            }
            return response;
        }
        
        /// <summary>
        /// Send an email with the confirmation when a certificate is rejected
        /// </summary>
        /// <param name="certificateNumber">The number of the certificate</param>
        /// <param name="officeId">The id of the office</param>
        /// <returns>A result</returns>
        public string SendEmailReject(string certificateNumber, int officeId)
        {
            string response = string.Empty;
            using (UsersContext context = new UsersContext())
            {
                //get the list of coordinators
                string[] coordinatorEmailList = (from user in context.UserProfiles
                                                 join userRole in context.UserInRoles on user.UserId equals userRole.UserId
                                                 join role in context.Roles on userRole.RoleId equals role.RoleId
                                                 where (role.RoleId == (int)UserRoleEnum.Coordinator || role.RoleId == (int)UserRoleEnum.Admin) 
                                                 && user.IsActive == true
                                                 && user.Email != null && user.OfficeId == officeId
                                                 select user.Email).ToArray();

                //if there are no coordinators, return a warning message
                if (coordinatorEmailList.Length == 0 || coordinatorEmailList == null)
                {
                    response = ServiceResource.NotExistsCoordinatorsMessage;
                }
                else
                {
                    try
                    {
                        //get the template
                        string emailTemplate = File.ReadAllText(HostingEnvironment.MapPath("~/Templates/RejectTemplate.html"));
                        //set the sequential number
                        emailTemplate = emailTemplate.Replace("{CERTIFICATE NUMBER}", certificateNumber);
                        //send the emails
                        EmailManagement.SendEmail(coordinatorEmailList, emailTemplate, ServiceResource.RejectCertificateSubjectMessage + " " + certificateNumber, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);

                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                        response = ServiceResource.EmailErrorMessage;
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Send an email to coordinators when a certificate is approved
        /// </summary>
        /// <param name="certificateNumber">The number of the certificate</param>
        /// <param name="officeId">The id of the office</param>
        /// <returns>A result</returns>
        [Invoke]
        public string SendEmailApprove(string certificateNumber, int officeId)
        {
            string response = string.Empty;
            using (UsersContext context = new UsersContext())
            {
                //get the list of coordinators
                string[] coordinatorEmailList = (from user in context.UserProfiles
                                                 join userRole in context.UserInRoles on user.UserId equals userRole.UserId
                                                 join role in context.Roles on userRole.RoleId equals role.RoleId
                                                 where (role.RoleId == (int)UserRoleEnum.Coordinator || role.RoleId == (int)UserRoleEnum.Admin)
                                                 && user.IsActive == true
                                                 && user.Email != null && user.OfficeId == officeId
                                                 select user.Email).ToArray();

                //if there are no coordinators, return a warning message
                if (coordinatorEmailList.Length == 0 || coordinatorEmailList == null)
                {
                    response = ServiceResource.NotExistsCoordinatorsMessage;
                }
                else
                {
                    try
                    {
                        //get the template
                        string emailTemplate = File.ReadAllText(HostingEnvironment.MapPath("~/Templates/ApproveTemplate.html"));
                        //set the sequential number
                        emailTemplate = emailTemplate.Replace("{CERTIFICATE NUMBER}", certificateNumber);
                        //send the emails
                        EmailManagement.SendEmail(coordinatorEmailList, emailTemplate, ServiceResource.ApprovedCertificateSubject + " " + certificateNumber, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                        response = ServiceResource.EmailErrorMessage;
                    }
                    
                }
            }
            return response;
        }

        /// <summary>
        /// Send an email notification to all LO admins
        /// </summary>
        /// <param name="entryPointId">Entry point id</param>
        /// <param name="certificateNumber">Certificate number</param>
        /// <returns>A result</returns>
        [Invoke]
        public string SendEmailLoNoBorderFees(int entryPointId, string certificateNumber)
        {
            string response = string.Empty;
            string entryPointName = string.Empty;
            //Get entry point name
            using (VocEntities context =new VocEntities())
            {
                entryPointName = context.EntryPoints.FirstOrDefault(x => x.EntryPointId == entryPointId).Name;
            }
            using (UsersContext context = new UsersContext())
            {
                //get the list of LO
                string[] coordinatorEmailList = (from user in context.UserProfiles
                                                 join userRole in context.UserInRoles on user.UserId equals userRole.UserId
                                                 join role in context.Roles on userRole.RoleId equals role.RoleId
                                                 where role.RoleId == (int)UserRoleEnum.LOAdmin
                                                 && user.IsActive == true
                                                 && user.Email != null 
                                                 select user.Email).ToArray();

                //if there are no coordinators, return a warning message
                if (coordinatorEmailList.Length == 0 || coordinatorEmailList == null)
                {
                    response = ServiceResource.NotExistLOUsers;
                }
                else
                {
                    try
                    {
                        //get the template
                        string emailTemplate = File.ReadAllText(HostingEnvironment.MapPath("~/Templates/LOMessageTemplate.html"));
                        //set the sequential number
                        emailTemplate = emailTemplate.Replace("{CERTIFICATE NUMBER}", certificateNumber);
                        //set border point name
                        emailTemplate = emailTemplate.Replace("{BORDER POINT NAME}", entryPointName);
                        //send the emails
                        EmailManagement.SendEmail(coordinatorEmailList, emailTemplate, ServiceResource.NoFeesCertificate + " " + certificateNumber, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);

                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                        response = ServiceResource.EmailErrorMessage;
                    }
                }
            }
            return response;
        }
        
        /// <summary>
        /// Move document files
        /// </summary>
        /// <param name="sourceInfo">Document files source</param>
        /// <param name="targetInfo">Document files target</param>
        private static void MoveAll(string sourceInfo, string targetInfo)
        {
            DirectoryInfo source = new DirectoryInfo(sourceInfo);
            DirectoryInfo target = new DirectoryInfo(targetInfo);
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.MoveTo( Path.Combine(target.ToString(), fi.Name));
            }

        }

        /// <summary>
        /// Retrieve the specific cetificate document attached
        /// </summary>
        /// <param name="certificateId">Certificate Identifier</param>
        /// <returns>Certificate file</returns>
        [Invoke]
        public Document GetCertificateDocumentByCertificateId(int certificateId)
        {
            return ctx.Documents.Where(x => x.CertificateId == certificateId && x.IsDeleted == false && x.IsSupporting==false).FirstOrDefault();
        }

        /// <summary>
        /// Retrieve the specific office attached to the certificate
        /// </summary>
        /// <param name="certificateId">Certificate Identifier</param>
        /// <returns>Certificate file</returns>
        [Invoke]
        public Office GetOfficeByCertificateId(int certificateId)
        {
            return ctx.Offices.Where(x => x.OfficeId == certificateId).FirstOrDefault();
        }

        /// <summary>
        /// Upload certificate document
        /// </summary>
        /// <param name="certificateId">Certificate Identifier</param>
        /// <param name="fileName">File Name tu upload</param>
        [Invoke]        
        public bool UploadCocDocument(int certificateId, string fileName)
        {

            var supportingDocument = ctx.Documents
                .Where(x => x.CertificateId == certificateId
                    && x.IsDeleted == false
                    && x.IsSupporting == true
                    && x.Filename == fileName).FirstOrDefault();
            if (supportingDocument != null)
                return false;

            var document = ctx.Documents
                .Where(x => x.CertificateId == certificateId
                    && x.IsSupporting == false
                    && x.IsDeleted == false).FirstOrDefault();
            //create the coc document
            if (document == null)
            {
                document = new Document();
                document.CertificateId = certificateId;
                
                //todo modify to save with authenticated user
                document.CreationBy = HttpContext.Current.User.Identity.Name;
                ctx.Documents.Add(document);
            }

            document.FilePath = GetDocumentFilePath(ctx.Certificates.Find(certificateId));

            document.IsSupporting = false;
            document.DocumentType = DocumentTypeEnum.Certificate;
            //modify for creating or updating a document
            document.CreationDate = DateTime.Now;
            //the first time the description is the same name
            document.Description = Path.GetFileNameWithoutExtension(fileName);
            document.Filename = fileName;

            ctx.SaveChanges();
            return true;

        }

        /// <summary>
        /// Get document file path
        /// </summary>
        /// <param name="certificate">current certificate</param>
        /// <returns></returns>
        private string GetDocumentFilePath(Certificate certificate)
        {
            List<WorkflowStatusEnum> status = new List<WorkflowStatusEnum>()
            {
                WorkflowStatusEnum.Approved,
                WorkflowStatusEnum.Ongoing,
                WorkflowStatusEnum.Closed
            };
            string path = string.Empty;
            if (status.Contains(certificate.WorkflowStatusId) && certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                path = Path.Combine(certificate.EntryPoint.Name, string.Concat(certificate.Sequential, "\\"));
            else if (status.Contains(certificate.WorkflowStatusId) && certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
                path = Path.Combine(string.Concat(certificate.Sequential, "\\"));
            else
                path = string.Concat(string.IsNullOrEmpty(certificate.Sequential) ? certificate.CertificateId.ToString() : certificate.Sequential, "\\");
            return path;
        }

        /// <summary>
        /// Get document file path in status request
        /// </summary>
        /// <param name="certificate">Current certificate</param>
        /// <param name="isShort">Is the short path or not</param>
        /// <returns></returns>
        private string GetDocumentFilePathRequest(Certificate certificate,bool isShort)
        {
            string path = string.Empty;
            if (isShort)
                path = string.Concat(certificate.CertificateId.ToString(), "\\");
            else
                path = Path.Combine(Settings.Default.PathDocument, string.Concat(certificate.CertificateId.ToString(), "\\"));
            return path;
        }

        /// <summary>
        /// Upload supporting document
        /// </summary>
        /// <param name="certificateId">Certificate Identifier</param>
        /// <param name="fileName">File Name tu upload</param>
        /// <returns>File Name when it already exist</returns>
        [Invoke]
        public Document UploadSupportingDocument(int certificateId, string fileName)
        {
            string fileNameRepeated = string.Empty;
            var document = ctx.Documents
                .Where(x => x.CertificateId == certificateId
                    && x.IsDeleted == false
                    && x.Filename == fileName).FirstOrDefault();
            //create the document
            if (document == null)
            {
                document = new Document();
                document.CertificateId = certificateId;
                document.FilePath = GetDocumentFilePath(ctx.Certificates.Find(certificateId));
                document.IsSupporting = true;
                document.DocumentType = DocumentTypeEnum.SupportingDocument;
                //the first time the description is the same name
                document.Description = Path.GetFileNameWithoutExtension(fileName);
                document.CreationBy = HttpContext.Current.User.Identity.Name;
                ctx.Documents.Add(document);

                //modify for creating or updating a document
                document.CreationDate = DateTime.Now;
                document.Filename = fileName;
                ctx.SaveChanges();
                return null;
            }
            return document;
        }

        /// <summary>
        /// Create a new certificate and return the new one.
        /// </summary>
        /// <param name="userName">User name </param>
        /// <returns>Certificate</returns>
        [Invoke]
        public Certificate CreateCertificate(string userName)
        {
            //get the current user
            VocUser currentUser = HttpContext.Current.Cache.Get("LoggedUser" + userName) as VocUser;

            if (currentUser.OfficeId <= 0)
                return null;

            int nextNumber = 0;
            string newSecuential = string.Empty;

            //get the list of certificates
            var certificateList = ctx.Certificates.ToList();
            //get deleted certificates
            var deletedCertertificates = certificateList.Where(cert => cert.IsDeleted).OrderBy(cert => cert.Sequential.Substring(6,2)).ThenBy(cert => cert.Sequential.Substring(9, 5));
            //get non-deleted certificates
            var actualCertificates = certificateList.Where(cert => !cert.IsDeleted).OrderBy(cert => cert.Sequential.Substring(6,2)).ThenBy(cert => cert.Sequential.Substring(9, 5));

            //get sequential number 
            nextNumber = GetSequential(deletedCertertificates, actualCertificates);
            int currentYear = DateTime.Now.Date.Year;
            //concat final sequential number IRQ-CO13-1
            newSecuential = string.Concat("IRQ-CO", currentYear.ToString().Substring(2,2), "-",nextNumber.ToString("00000"));
            

            //create the certificate
            Certificate newCertificate = new Certificate
            {
                Sequential = newSecuential,
                WorkflowStatusId = WorkflowStatusEnum.Created,
                CertificateStatusId = CertificateStatusEnum.Conform,
                OfficeId = currentUser.OfficeId,
                CreationBy = HttpContext.Current.User.Identity.Name,
                CreationDate = DateTime.Now
            };
            //save changes
            ctx.Certificates.Add(newCertificate);
            ctx.SaveChanges();

            SaveCertificateTrancking(newCertificate.CertificateId, TrackingStatusEnum.Created);

            return newCertificate;
        }

        /// <summary>
        /// Get a new sequential number
        /// </summary>
        /// <returns></returns>
        private string GetNewSecuentialNumber()
        {
            string newSecuential = string.Empty;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                int nextNumber = 0;

                using (VocEntities context = new VocEntities())
                {
                    //get the list of certificates
                    var certificateList = context.Certificates.ToList();
                    //get deleted certificates
                    var deletedCertertificates = certificateList.Where(cert => cert.IsDeleted && cert.Sequential != null).OrderBy(cert => cert.Sequential.Substring(6, 2)).ThenBy(cert => cert.Sequential.Substring(9, 5));
                    //get non-deleted certificates
                    var actualCertificates = certificateList.Where(cert => !cert.IsDeleted && cert.Sequential != null).OrderBy(cert => cert.Sequential.Substring(6, 2)).ThenBy(cert => cert.Sequential.Substring(9, 5));

                    //get sequential number 
                    nextNumber = GetSequential(deletedCertertificates, actualCertificates);
                    int currentYear = DateTime.Now.Date.Year;
                    //concat final sequential number IRQ-CO13-1
                    newSecuential = string.Concat("IRQ-CO", currentYear.ToString().Substring(2, 2), "-", nextNumber.ToString("00000"));
                }
                
                transaction.Complete();
            }
            return newSecuential;
        }

        /// <summary>
        /// Delete mandatory documents in the certificate
        /// </summary>
        /// <param name="certificateId">Certificate id</param>
        [Invoke]
        public void DeleteMandatoryDocuments(int certificateId)
        {
            using (VocEntities context = new VocEntities())
            {
                //get the list of mandatory documents
                var documentList = context.Documents.Where(x => x.IsDeleted == false 
                    && x.IsSupporting == false
                    && x.CertificateId == certificateId).ToList();

                //if contains documents,the system will perform the delete operation
                if (documentList.Count > 0)
                {
                    //set isdeleted in each document and delete the file
                    foreach (var document in documentList)
                    {
                        //set flag is deleted in true
                        document.IsDeleted = true;
                        //get the path of the document
                        string documentPath = Path.Combine(Properties.Settings.Default.PathDocument, document.FilePath, document.Filename).Replace("/", "\\");

                        //if the file exists
                        if (File.Exists(documentPath))
                        {
                            //set the read/write permission
                            File.SetAttributes(documentPath, FileAttributes.Normal);
                            //delete the file
                            File.Delete(documentPath);
                        }
                    }
                    //save the changes in the database
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Verify if certificate has a file attached
        /// </summary>
        /// <param name="certificateId">Certificate identifier</param>
        /// <returns>Flag true or false</returns>
        [Invoke]
        public bool VerifyRequiredDocumentsInCertificate(int certificateId)
        {
            return ctx.Documents.Any(doc => doc.CertificateId == certificateId && doc.IsSupporting == false && doc.IsDeleted == false);
        }

        /// <summary>
        /// Close the list of certificates
        /// </summary>
        /// <param name="certificates">list of certificates to be closed</param>
        /// <returns></returns>
        [Invoke]
        public List<ValidationMessage> CloseCertificateList(List<Certificate> certificates)
        {
            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            foreach (var certificate in certificates)
            {
                messages = string.Empty;
                using (TransactionScope ts = new System.Transactions.TransactionScope())
                {
                    var certifVar = ctx.Certificates.FirstOrDefault(x => x.CertificateId == certificate.CertificateId);
                    certifVar.WorkflowStatusId = WorkflowStatusEnum.Closed;
                    ctx.SaveChanges();
                    SaveCertificateTrancking(certificate.CertificateId, TrackingStatusEnum.Closed);
                    ts.Complete();
                }

                validationMessage = new ValidationMessage
                {
                    Identifier = certificate.Sequential,
                    Status = StatusProcess.Success,
                    Message = messages
                };
                validations.Add(validationMessage);
            }
            return validations;
        }

        /// <summary>
        /// UnClose the list of certificates
        /// </summary>
        /// <param name="certificates">list of certificates to be unclosed</param>
        /// <returns></returns>
        [Invoke]
        public List<ValidationMessage> UNCloseCertificateList(List<Certificate> certificates)
        {
            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            StatusProcess status = StatusProcess.Success;
            foreach (var certificate in certificates)
            {
                messages = string.Empty;

                if (user.IsInRole(UserRoleEnum.SuperAdmin) && certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
                {
                    if (certificate.IsPublished)
                    {
                        messages = ServiceResource.CertificateUnpublisPublishValidationError;
                        status = StatusProcess.Error;
                    }
                    if (status != StatusProcess.Error)
                    {
                        messages = RecallCertificate(certificate.CertificateId, user.Name);
                        if (!string.IsNullOrEmpty(messages))
                            status = StatusProcess.Error;
                    }
                }
                else
                {
                    using (TransactionScope ts = new System.Transactions.TransactionScope())
                    {
                        var certifVar = ctx.Certificates.FirstOrDefault(x => x.CertificateId == certificate.CertificateId);
                        certifVar.WorkflowStatusId = WorkflowStatusEnum.Ongoing;
                        ctx.SaveChanges();
                        SaveCertificateTrancking(certificate.CertificateId, TrackingStatusEnum.Ongoing);
                        ts.Complete();
                    }
                }

                validationMessage = new ValidationMessage
                {
                    Identifier = certificate.Sequential,
                    Status = status,
                    Message = messages
                };
                validations.Add(validationMessage);
            }
            return validations;
        }

        /// <summary>
        /// Make request of the list of certificates
        /// </summary>
        /// <param name="certificates">list of certificates to be requested</param>
        /// <returns></returns>
        [Invoke]
        public List<ValidationMessage> RequestCertificateList(List<Certificate> certificates)
        {

            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            StatusProcess status = StatusProcess.Success;
            foreach (var certificate in certificates)
            {
                messages = string.Empty;
                if (VerifyRequiredDocumentsInCertificate(certificate.CertificateId))
                    status = StatusProcess.Success;
                else
                    status = StatusProcess.Error;

                //if the certificate has required documents, the system will continue with the process
                if (status == StatusProcess.Success)
                {
                    using (TransactionScope ts = new System.Transactions.TransactionScope())
                    {
                        var certifVar = ctx.Certificates.FirstOrDefault(x => x.CertificateId == certificate.CertificateId);
                        certifVar.WorkflowStatusId = WorkflowStatusEnum.Requested;
                        ctx.SaveChanges();
                        SaveCertificateTrancking(certifVar.CertificateId, TrackingStatusEnum.Requested);
                        ts.Complete();
                    }
                    string sendMailResult = SendEmailRequest(certificate.ComdivNumber, user.OfficeId);

                    if (!string.IsNullOrEmpty(sendMailResult))
                    {
                        messages = sendMailResult;
                        status = StatusProcess.GenericWarning;
                    }                      
                }
                else
                {
                    messages = ServiceResource.CertificateDocumentValidator;
                    status = StatusProcess.Error;
                }

                validationMessage = new ValidationMessage
                {
                    Identifier = certificate.ComdivNumber,
                    Status = status,
                    Message = messages
                };
                validations.Add(validationMessage);

            }
            return validations;
        }

        /// <summary>
        /// Reject the list of certificates
        /// </summary>
        /// <param name="certificates">list of certificates to be rejected</param>
        /// <returns></returns>
        [Invoke]
        public List<ValidationMessage> RejectCertificateList(List<Certificate> certificates)
        {
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            StatusProcess status = StatusProcess.Success;
            foreach (var certificate in certificates)
            {
                messages = string.Empty;
                using (TransactionScope ts = new System.Transactions.TransactionScope())
                {
                    var certifVar = ctx.Certificates.FirstOrDefault(x => x.CertificateId == certificate.CertificateId);
                    certifVar.WorkflowStatusId = WorkflowStatusEnum.Rejected;
                    ctx.SaveChanges();
                    SaveCertificateTrancking(certificate.CertificateId, TrackingStatusEnum.Rejected);
                    ts.Complete();
                }
                string sendMailResult = SendEmailReject(certificate.Sequential, user.OfficeId);

                if (!string.IsNullOrEmpty(sendMailResult))
                {
                    messages = sendMailResult;
                    status = StatusProcess.GenericWarning;
                }   
                validationMessage = new ValidationMessage
                {
                    Identifier = certificate.Sequential,
                    Status = status,
                    Message = messages
                };
                validations.Add(validationMessage);
            }

            return validations;
        }
        
        /// <summary>
        /// Get the new sequential number
        /// </summary>
        /// <param name="deletedCertertificates">Deleted certificates</param>
        /// <param name="actualCertificates">Non-deleted certificates</param>
        /// <returns>int</returns>
        private int GetSequential(IOrderedEnumerable<Certificate> deletedCertertificates, IOrderedEnumerable<Certificate> actualCertificates)
        {
            int nextNumber = 0;
            int currentYear = int.Parse(DateTime.Now.Date.Year.ToString().Substring(2, 2));
            string numericSection = string.Empty;
            bool foundOld = false;
            var lastCertificate = actualCertificates.LastOrDefault();
            int certificateYear = 0;
            
            if (lastCertificate != null)
                certificateYear = int.Parse(lastCertificate.Sequential.Substring(6, 2));
            else
                certificateYear = currentYear;
            
            //verify if exist a number deleted
            if (deletedCertertificates.Count() > 0 && certificateYear == currentYear)
            {
                foreach (var certificate in deletedCertertificates)
                {
                    //get the deleted number
                    numericSection = certificate.Sequential.Substring(9, 5);
                    if (!actualCertificates.Any(cert => cert.Sequential.Contains(numericSection)))
                    {
                        nextNumber = int.Parse(numericSection);
                        foundOld = true;
                        break;
                    }
                }
                if (!foundOld)
                {
                    //get a new number
                    if (lastCertificate != null)
                    {
                        numericSection = lastCertificate.Sequential.Substring(9, 5);
                        nextNumber = int.Parse(numericSection);
                    }
                    nextNumber += 1;
                }
            }
            else
            {
                //get a new number
                if (lastCertificate != null && certificateYear == currentYear)
                {
                    numericSection = lastCertificate.Sequential.Substring(9, 5);
                    nextNumber = int.Parse(numericSection);
                }
                nextNumber += 1;
            }
            return nextNumber;
        }

        /// <summary>
        /// Get signet by office
        /// </summary>
        /// <returns></returns>
        public byte[] GetSignetByOfficeId(int officeId)
        {
            return ctx.Offices.Where(x => x.OfficeId == officeId).Select(x => x.OfficeStamp).FirstOrDefault();
        }
              
        /// <summary>
        /// Delete the Certificate
        /// </summary>
        /// <param name="certificateId">Certificate Identifier to be deleted</param>
        /// <param name="userName">User Name who delete the certificate</param>
        /// <returns>Error message</returns>
        [Invoke]
        public string DeleteCertificate(int certificateId, string userName)
        {
            string logErrors = string.Empty;
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            try
            {
                using (TransactionScope ts = new System.Transactions.TransactionScope())
                {

                    //Retrieve the Certificate Information
                    var certificate = ctx.Certificates.FirstOrDefault(x => x.CertificateId == certificateId);
                    var certificateDocuments = ctx.Documents.Where(x => x.CertificateId == certificateId);

                    //Update Certificate IsDeleted flag
                    certificate.IsDeleted=true;
                    certificate.ModificationBy = userName;
                    certificate.ModificationDate = DateTime.Now;                    
                    ctx.SaveChanges();

                    //Delete physical documents
                    foreach (var document in certificateDocuments)
                    {
                        document.IsDeleted = true;
                        document.ModificationBy = user.Name;
                        document.ModificationDate = DateTime.Now;

                        ctx.SaveChanges();

                        string documentPath = Path.Combine(Properties.Settings.Default.PathDocument, document.FilePath, document.Filename).Replace("/", "\\");

                        if (File.Exists(documentPath))
                        {
                            File.SetAttributes(documentPath, FileAttributes.Normal);
                            File.Delete(documentPath);
                        }
                    }

                    //Delete Documents Directory
                    string filePath = GetDocumentFilePath(certificate);
                    string sourcePath = Path.Combine(Properties.Settings.Default.PathDocument, filePath).Replace("/", "\\");
                    if (Directory.Exists(sourcePath))
                    {                        
                        Directory.Delete(sourcePath, true);
                    }

                    ts.Complete();

                }
            }
            catch (IOException ex)
            {
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                logErrors = ServiceResource.IOErrorMessage;
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                logErrors = ServiceResource.DeleteCertificateExeption + ex.Message;                
            }
            return logErrors;
        }

        /// <summary>
        /// Delete the Certificate
        /// </summary>
        /// <param name="certificateId">Certificate Identifier to be deleted</param>
        /// <param name="userName">User Name who delete the certificate</param>
        /// <returns>Error message</returns>
        [Invoke]
        public List<ValidationMessage> DeleteCertificateList(List<Certificate> certificates)
        {

            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            StatusProcess status = StatusProcess.Success;
            foreach (var certifAux in certificates)
            {
                status = StatusProcess.Success;
                messages = string.Empty;
                try
                {
                    using (TransactionScope ts = new System.Transactions.TransactionScope())
                    {

                        //Retrieve the Certificate Information
                        var certificate = ctx.Certificates.FirstOrDefault(x => x.CertificateId == certifAux.CertificateId);
                        var certificateDocuments = ctx.Documents.Where(x => x.CertificateId == certifAux.CertificateId);

                        //Update Certificate IsDeleted flag
                        certificate.IsDeleted = true;
                        certificate.ModificationBy = user.Name;
                        certificate.ModificationDate = DateTime.Now;
                        ctx.SaveChanges();

                        //Delete physical documents
                        foreach (var document in certificateDocuments)
                        {
                            string documentPath = Path.Combine(Properties.Settings.Default.PathDocument, document.FilePath, document.Filename).Replace("/", "\\");

                            if (File.Exists(documentPath))
                            {
                                File.SetAttributes(documentPath, FileAttributes.Normal);
                                File.Delete(documentPath);
                            }
                        }

                        //Delete Documents Directory
                        string filePath = GetDocumentFilePath(certificate);
                        string sourcePath = Path.Combine(Properties.Settings.Default.PathDocument, filePath).Replace("/", "\\");
                        if (Directory.Exists(sourcePath))
                        {
                            Directory.Delete(sourcePath, true);
                        }

                        ts.Complete();
                    }
                }
                catch (IOException ex)
                {
                    ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                    messages =  ServiceResource.IOErrorMessage;
                    status = StatusProcess.Error;
                }
                catch (Exception ex)
                {
                    ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
                    messages =  ex.Message ;
                    status = StatusProcess.Success;
                }

                validationMessage = new ValidationMessage
                {
                    Identifier = certifAux.Sequential,
                    Status = status,
                    Message = messages
                };
                validations.Add(validationMessage);
            }

            return validations;

        }

        /// <summary>
        /// Publish or unpublish a list of certificates
        /// </summary>
        /// <param name="certificateIdList">List of certificate ids</param>
        /// <param name="isPublished">Is publish or unpublish</param>
        [Invoke]
        public List<ValidationMessage> PublisUnpublishCertificateList(List<Certificate> certificateList, bool isPublished)
        {
            string messages = string.Empty;
            List<ValidationMessage> validations = new List<ValidationMessage>();
            ValidationMessage validationMessage = null;
            using (VocEntities context = new VocEntities())
            {
                //iterate in the list of ids
                foreach (var certificate in certificateList)
                {
                    messages = string.Empty;
                    if (certificate.IsPublished != isPublished)
                    {
                        //get the certificate with the corresponding id
                        var currentCertificate = context.Certificates.FirstOrDefault(x => x.CertificateId == certificate.CertificateId);
                        if (currentCertificate != null)
                        {
                            //set is published
                            currentCertificate.IsPublished = isPublished;
                            //perform the update operation
                            context.SaveChanges();
                            TrackingStatusEnum status = isPublished ? TrackingStatusEnum.Published : TrackingStatusEnum.Unpublished;
                            SaveCertificateTrancking(certificate.CertificateId, status);
                        }
                    }
                    else
                    {
                        messages += isPublished ? ServiceResource.PublishValidationError
                            : ServiceResource.UnpublishValidationError;

                    }
                    validationMessage = new ValidationMessage
                    {
                        Identifier = certificate.Sequential,
                        Status= certificate.IsPublished != isPublished ? StatusProcess.Success: StatusProcess.Error,
                        Message = messages
                    };
                    validations.Add(validationMessage);
                }
            }
            return validations;
        }


        /// <summary>
        /// Generate release note report
        /// </summary>
        /// <param name="releaseNoteId">Release note id</param>
        /// <returns></returns>
        [Invoke]
        public ValidationMessage GenerateReleaseNoteReport(int releaseNoteId)
        {
            ValidationMessage message = new ValidationMessage();
            StatusProcess status = StatusProcess.Success;
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            using (TransactionScope ts = new System.Transactions.TransactionScope())
            {
                string errors = string.Empty;
                ReleaseNote releaseNote = ctx.ReleaseNotes.FirstOrDefault(x => x.ReleaseNoteId == releaseNoteId);
                Certificate certificate = ctx.ReleaseNotes.FirstOrDefault(x => x.ReleaseNoteId == releaseNoteId).Certificate;
                string entryPointName = ctx.ReleaseNotes.FirstOrDefault(x => x.ReleaseNoteId == releaseNoteId).Certificate.EntryPoint.Name;
                string fileName = "RN_" + string.Format("{0}_{1:000}", certificate.Sequential, releaseNote.PartialNumber.GetValueOrDefault()) + ".docx";
                string filePath = Settings.Default.PathDocument + entryPointName + @"\" + certificate.Sequential;
                string fullPath = filePath + @"\" + fileName;
                MemoryStream report = WordManagement.GenerateReleaseNoteReport(releaseNote, HostingEnvironment.MapPath("~/WordTemplates/TemplateRN.docx"), out errors);
                if (string.IsNullOrEmpty(errors))
                {
                    report.Position = 0;
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                    File.WriteAllBytes(fullPath, report.ToArray());
                    fullPath = filePath + @"\" + fileName.Replace(".docx", ".pdf");
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                    WordManagement.SaveWordReportAsPdf(report, fullPath);
                    fullPath = filePath + @"\" + fileName.Replace(".pdf", ".docx");
                    File.Delete(fullPath);
                    fileName = fileName.Replace(".docx", ".pdf");
                    if (!ctx.Documents.Any(x => x.Filename == fileName && x.CertificateId == certificate.CertificateId))
                    {
                        Document document = new Document();
                        document.CertificateId = certificate.CertificateId;
                        document.Filename = fileName;
                        document.Description = fileName.Replace(".pdf", "");
                        document.FilePath = entryPointName + @"\" + certificate.Sequential + @"\";
                        document.IsSupporting = true;
                        document.CreationBy = HttpContext.Current.User.Identity.Name;
                        document.CreationDate = DateTime.Now;
                        document.ReleaseNoteId = releaseNoteId;
                        document.DocumentType = DocumentTypeEnum.ReleaseNote;
                        ctx.Documents.Add(document);
                    }
                    else
                    {
                        // If the document already exists we need to change the IsDeleted status
                        Document document = ctx.Documents.Where(x => x.ReleaseNoteId == releaseNoteId).FirstOrDefault();
                        if (document != null)
                        {
                            document.IsDeleted = false;
                            document.ModificationBy = user.Name;
                            document.ModificationDate = DateTime.Now;
                        }

                    }
                    ctx.SaveChanges();
                }
                else
                {
                    status = StatusProcess.Error;
                }
                fullPath = fullPath.Replace(".docx", ".pdf");
                message.Identifier = fileName;
                message.Message = errors;
                message.Status = status;
                ts.Complete();
            }

            return message;
        }

        /// <summary>
        /// Deletes the pdf document associated to the Release Note once the Release Note is saved
        /// </summary>
        /// <param name="releaseNoteId">Release Note Id</param>
        /// <returns></returns>
        [Invoke]
        public void DeleteReleaseNoteReport(int releaseNoteId)
        {
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            using (TransactionScope ts = new System.Transactions.TransactionScope())
            {
                Document doc = GetDocumentByReleaseNoteId(releaseNoteId);
                if (doc != null)
                {
                    string fullPath = Settings.Default.PathDocument + doc.FilePath + doc.Filename;
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        if (File.Exists(fullPath))
                        {
                            // Delete the file and update the IsDeleted status
                            File.Delete(fullPath);
                            doc.IsDeleted = true;
                            doc.ModificationBy = user.Name;
                            doc.ModificationDate = DateTime.Now;
                        }
                        ctx.SaveChanges();
                    }
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// Deletes the security papers selected
        /// </summary>
        /// <param name="securityPapersSelected">security papers selected</param>
        [Invoke]
        public void DeleteSecurityPaperList(List<int> securityPapersSelected)
        {
            VocUser user = HttpRuntime.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            using (TransactionScope ts = new System.Transactions.TransactionScope())
            {
                SecurityPaper secPaper = new SecurityPaper();
                foreach (int item in securityPapersSelected)
                {
                    secPaper = ctx.SecurityPapers.Where(x => x.SecurityPaperId == item).FirstOrDefault();
                    if (secPaper != null && secPaper.IsDeleted == false)
                    {
                        secPaper.IsDeleted = true;
                        secPaper.ModificationBy = user.Name;
                        secPaper.ModificationDate = DateTime.Now;
                    }
                }
                ctx.SaveChanges();
                ts.Complete();
            }
        }

        /// <summary>
        /// Gets a document by Release Note Id
        /// </summary>
        /// <param name="releaseNoteId">Release Note Id</param>
        /// <returns></returns>
        [Invoke]
        public Document GetDocumentByReleaseNoteId(int releaseNoteId)
        {
            return ctx.Documents.Where(x => x.ReleaseNoteId == releaseNoteId && x.IsDeleted == false).FirstOrDefault();
        }

        /// <summary>
        /// Get the list of securityPapers filteres
        /// </summary>
        /// <param name="issuanceDateFrom">Issuance Date</param>
        /// <param name="issuanceDateTo">issuanceDateTo</param>
        /// <param name="selectedEntryPoint">Entry point id</param>
        /// <param name="issued">Issued Status</param>
        /// <param name="notIssued">Not issued Status</param>
        /// <param name="cancelled">Cancelled Status</param>
        /// <param name="misPrinted">MisPrinted Status</param>
        /// <returns>IQueryable of SecurityPaper</returns>
        [Query]
        public IQueryable<SecurityPaper> GetSecurityPaperList(DateTime? issuanceDateFrom, DateTime? issuanceDateTo, int selectedEntryPoint,
            bool issued, bool notIssued, bool cancelled, bool misPrinted, string number)
        {
            //get query
            IQueryable<SecurityPaper> query = ctx.SecurityPapers.Where(x => x.IsDeleted == false);
            //filter by date
            if (issuanceDateFrom != null && issuanceDateTo != null)
            {
                DateTime finalDate = issuanceDateTo.GetValueOrDefault().AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.ModificationDate >= issuanceDateFrom && x.ModificationDate <= finalDate);
            }
            //filter by entry point
            if (selectedEntryPoint > 0)
                query = query.Where(x => x.EntryPointId == selectedEntryPoint);
            //filter by status with an or clause
            if (issued || notIssued || cancelled || misPrinted)
            {
                var predicateStatus = PredicateBuilder.False<SecurityPaper>();
                if (issued)
                    predicateStatus = predicateStatus.Or(x => x.Status == SecurityPaperStatusEnum.Issued);
                if (notIssued)
                    predicateStatus = predicateStatus.Or(x => x.Status == SecurityPaperStatusEnum.NotIssued);
                if (cancelled)
                    predicateStatus = predicateStatus.Or(x => x.Status == SecurityPaperStatusEnum.Cancelled);
                if (misPrinted)
                    predicateStatus = predicateStatus.Or(x => x.Status == SecurityPaperStatusEnum.MissPrinted);

                query = query.AsExpandable().Where(predicateStatus);
            }

            //filter by number
            if (!string.IsNullOrEmpty(number))
                query = query.Where(x => x.SecurityPaperNumber.Contains(number));

            //order query
            query = query.OrderBy(x => x.SecurityPaperNumber).ThenBy(x => x.CreationDate).ThenBy(x => x.ReleaseNote.Certificate.Sequential);

            return query;
        }

        /// <summary>
        /// Create one or more security papers in a valid range
        /// </summary>
        /// <param name="initialLetter">Initial letter</param>
        /// <param name="rangeFrom">From</param>
        /// <param name="rangeTo">To</param>
        /// <param name="selectedEntryPointId">Selected entry point</param>
        /// <returns>ValidationMessage</returns>
        [Invoke]
        public ValidationMessage CreateSecurityPapers(string initialLetter, int rangeFrom, int rangeTo, int selectedEntryPointId)
        {
            ValidationMessage validationMessage = new ValidationMessage();
            StatusProcess status = StatusProcess.Success;
            string securityPaperNumber = string.Empty;
            SecurityPaper sucurityPaper = null;
            string message = string.Empty;

            if (rangeTo < rangeFrom)
            {
                status = StatusProcess.Error;
                message += ServiceResource.NotValidRange;
            }

            if (status == StatusProcess.Success)
            {
                using (TransactionScope ts = new System.Transactions.TransactionScope(TransactionScopeOption.Required,new TimeSpan(0,30,0)))
                {
                    for (int i = rangeFrom -1; i < rangeTo; i++)
                    {
                        securityPaperNumber = string.Format("{0}{1}", initialLetter, (i+1).ToString("000000000"));

                        if (ctx.SecurityPapers.Any(x => x.IsDeleted == false && x.SecurityPaperNumber == securityPaperNumber))
                        {
                            status = StatusProcess.Error;
                            message += ServiceResource.RangeAlreadyExists;
                            break;
                        }
                        else
                        {
                            sucurityPaper = new SecurityPaper
                            {
                                SecurityPaperNumber = securityPaperNumber,
                                Status = SecurityPaperStatusEnum.NotIssued,
                                EntryPointId = selectedEntryPointId,
                                CreationBy = HttpContext.Current.User.Identity.Name,
                                CreationDate = DateTime.Now
                            };

                            ctx.SecurityPapers.Add(sucurityPaper);
                        }
                    }
                    if (status == StatusProcess.Success)
                    {
                        ctx.SaveChanges();
                        ts.Complete();
                    }
                }
            }
            
            validationMessage.Status = status;
            validationMessage.Message = message;

            return validationMessage;
        }

        /// <summary>
        /// Gets the Security papers used in a Release Note
        /// </summary>
        /// <param name="releaseNoteId"></param>
        /// <returns></returns>
        [Invoke]
        public List<SecurityPaper> GetSecurityPapersByRN(int releaseNoteId)
        {
            return ctx.ReleaseNotes.FirstOrDefault(rn => rn.ReleaseNoteId == releaseNoteId).SecurityPapers.ToList<SecurityPaper>();
        }

        /// <summary>
        /// Method for update Issued Security Papers when saving or updating a Release Note
        /// </summary>
        /// <param name="releaseNoteId"></param>
        /// <param name="issuedSecurityPapers"></param>
        [Invoke]
        public void UpdateIssuedSecurityPapersOnReleaseNote(int releaseNoteId, List<SecurityPaper> issuedSecurityPapers)
        {
            ReleaseNote releaseNote = ctx.ReleaseNotes.FirstOrDefault(rn => rn.ReleaseNoteId == releaseNoteId);

            // Updating Security Papers not being used by the original RN
            foreach (SecurityPaper sp in releaseNote.SecurityPapers)
            {
                if (!issuedSecurityPapers.Any(p => p.SecurityPaperId == sp.SecurityPaperId))
                {
                    SecurityPaper securityPaper = releaseNote.SecurityPapers.FirstOrDefault(p => p.SecurityPaperId == sp.SecurityPaperId);
                    securityPaper.Status = SecurityPaperStatusEnum.NotIssued;
                    securityPaper.ReleaseNoteId = null;

                    securityPaper.ModificationBy = HttpContext.Current.User.Identity.Name;
                    securityPaper.ModificationDate = DateTime.Now;
                }
            }

            // Update to Issued the new selected Security Papers, and relate each of them with its Release note
            foreach (SecurityPaper sp in issuedSecurityPapers)
            {
                SecurityPaper securityPaper = ctx.SecurityPapers.FirstOrDefault(p => p.SecurityPaperId == sp.SecurityPaperId);
                securityPaper.Status = SecurityPaperStatusEnum.Issued;
                securityPaper.ReleaseNoteId = releaseNoteId;

                securityPaper.ModificationBy = HttpContext.Current.User.Identity.Name;
                securityPaper.ModificationDate = DateTime.Now;
            }

            ctx.SaveChanges();
        }

        /// <summary>
        /// Retreives the history of a certificate
        /// </summary>
        /// <param name="certificateId">Id of certificate</param>
        /// <returns></returns>
        [Query]
        public IQueryable<CertificateTracking> GetTrackingListByCertificateId(int certificateId)
        {
            return ctx.CertificateTrackings.Where(x => x.CertificateId == certificateId && x.IsDeleted == false)
                .OrderBy(x => x.CertificateTranckingId);
        }

        /// <summary>
        /// Save the trancking for certificate changes
        /// </summary>
        /// <param name="certificateId">Id of certificate</param>
        /// <param name="newStatus">New Status</param>
        [Invoke]
        public void SaveCertificateTrancking(int certificateId, TrackingStatusEnum newStatus)
        {
            CertificateTracking certificateTracking = new CertificateTracking
            {
                CertificateId = certificateId,
                TrackingDate = DateTime.Now,
                TrackingBy = HttpContext.Current.User.Identity.Name,
                TrackingStatus =  newStatus,
                CreationBy = HttpContext.Current.User.Identity.Name,
                CreationDate = DateTime.Now
            };
            ctx.CertificateTrackings.Add(certificateTracking);
            ctx.SaveChanges();
        }

        /// <summary>
        /// Save the trancking for certificate changes
        /// </summary>
        /// <param name="certificateIds">List of ids</param>
        /// <param name="newStatus">New status</param>
        [Invoke]
        public void SaveCertificateTranckingList(int[] certificateIds, TrackingStatusEnum newStatus)
        {
            foreach (int certificateId in certificateIds)
            {
                CertificateTracking certificateTracking = new CertificateTracking
                {
                    CertificateId = certificateId,
                    TrackingDate = DateTime.Now,
                    TrackingBy = HttpContext.Current.User.Identity.Name,
                    TrackingStatus = newStatus,
                    CreationBy = HttpContext.Current.User.Identity.Name,
                    CreationDate = DateTime.Now
                };
                ctx.CertificateTrackings.Add(certificateTracking);
            }
            ctx.SaveChanges();
        }

        /// <summary>
        /// Delete selected documents
        /// </summary>
        /// <param name="selectedIds">Ids of selected documents</param>
        /// <returns>validation messages</returns>
        [Invoke]
        public ValidationMessage DeleteSelectedDocuments(int[] selectedIds)
        {
            ValidationMessage message = null;
            string strMessage = string.Empty;
            using (TransactionScope transaction = new TransactionScope())
            {
                using (VocEntities context = new VocEntities())
                {
                    //Get selected documents
                    List<Document> selectedDocuments = context.Documents.Where(x => selectedIds.Contains(x.DocumentId)).ToList();
                    foreach (Document doc in selectedDocuments)
                    {                        
                        strMessage = string.Empty;
                        //get the path of the document
                        string path = string.Concat(Settings.Default.PathDocument, doc.FilePath, doc.Filename).Replace("/", "\\");
                        //if the document not exists, write an error message
                        if (!File.Exists(@path))
                        {
                            strMessage = string.Format(ServiceResource.DeleteDocumentsFailMessage, doc.Filename) + Environment.NewLine;
                        }
                        else
                        {
                            //otherwise delete the document
                            doc.IsDeleted = true;
                            File.SetAttributes(@path, FileAttributes.Normal);
                            File.Delete(@path);
                        }
                    }
                    //save changes
                    context.SaveChanges();
                    //commit transaction
                    transaction.Complete();
                    //build validation message
                    message = new ValidationMessage
                    {
                        Message = strMessage,
                        Status = string.IsNullOrEmpty(strMessage) ? StatusProcess.Success : StatusProcess.Error
                    };
                }
            }
            return message;
        }

        /// <summary>
        /// Remove a regional office code in local offices
        /// </summary>
        /// <param name="officeId">Id of current office</param>
        [Invoke]
        public void RemoveRegionalOffice(int officeId)
        {
            using (VocEntities context = new VocEntities())
            {
                Office currentOffice = context.Offices.FirstOrDefault(x => x.OfficeId == officeId);
                if (currentOffice != null)
                {
                    if (currentOffice.OfficeType.GetValueOrDefault() == OfficeTypeEnum.RegionalOffice)
                    {
                        List<Office> localOffices = context.Offices.Where(x => x.RegionalOfficeId == officeId).ToList();
                        foreach (Office office in localOffices)
                        {
                            office.RegionalOfficeId = null;
                        }
                        context.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// This method send to comdiv the information of the current certificate
        /// </summary>
        /// <param name="certificate">Current certificate</param>
        /// <returns>Validation Message</returns>
        [Invoke]
        public ValidationMessage SynchcroniseWithComdiv(Certificate certificate)
        {
            ValidationMessage result = new ValidationMessage() { Status = StatusProcess.Success };
            VocUser currentUser = HttpContext.Current.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            try
            {
                string comdivConnectionString = string.Empty;
                string entryPointName = string.Empty;
                //Verify if the user can sync
                if (!currentUser.CanSync)
                {
                    result.Status = StatusProcess.Error;
                    result.Message = ServiceResource.SyncComdivNotPossible;
                    return result;
                }

                using (VocEntities vocContext = new VocEntities())
                {
                    //Get office information
                    Office certificateOffice = vocContext.Offices.FirstOrDefault(x => x.OfficeId == certificate.OfficeId);
                    //get and build the connection string
                    comdivConnectionString = ConfigurationManager.ConnectionStrings["ComdivEntities"].ConnectionString;
                    comdivConnectionString = comdivConnectionString.Replace("data source=", "data source=" + certificateOffice.ServerName);
                    comdivConnectionString = comdivConnectionString.Replace("initial catalog=", "initial catalog=" + certificateOffice.DatabaseName);
                    if (certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                    {
                        //get the name of the entry point
                        entryPointName = vocContext.EntryPoints.FirstOrDefault(x => x.EntryPointId == certificate.EntryPointId).Name;
                    }
                }

                using (ComdivEntities comdivContext = new ComdivEntities(comdivConnectionString))
                {
                    //get coc on comdiv
                    COC comdivCertificate = comdivContext.COCs.FirstOrDefault(x => x.COC_NUMBER == certificate.ComdivNumber);
                    DICTIONARY comdivEntryPoint = null;
                    DICTIONARY comdivCertificateType = null;
                    ITEM comdivItem = null;
                    //verify if the coc exist
                    if (comdivCertificate == null)
                    {
                        result.Status = StatusProcess.Error;
                        result.Message = ServiceResource.SyncCertificateNotFound;
                        return result;
                    }
                    if (certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                    {
                        //verify if the entrypoint exist
                        comdivEntryPoint = comdivContext.DICTIONARies.FirstOrDefault(x => x.DIC_DESCRIPTION == entryPointName && !x.IS_DELETED_YN);
                        if (comdivEntryPoint == null)
                        {
                            result.Status = StatusProcess.Error;
                            result.Message = ServiceResource.SyncEntrypointNotFound;
                            return result;
                        }
                    }
                    string certificateTypeName = GetCertificateTypeComdiv(certificate.CertificateStatusId);
                    comdivCertificateType = comdivContext.DICTIONARies.FirstOrDefault(x => x.DIC_DESCRIPTION == certificateTypeName && !x.IS_DELETED_YN);
                    //verify if the certificate type exists
                    if (comdivCertificateType == null)
                    {
                        result.Status = StatusProcess.Error;
                        result.Message = ServiceResource.SyncCertificateTypeNotFound;
                        return result;
                    }
                    comdivContext.Set<COC>().Attach(comdivCertificate);
                    comdivContext.Entry(comdivCertificate).State = EntityState.Unchanged;
                    //update the certificate informaiton
                    if (certificate.CertificateStatusId == CertificateStatusEnum.Conform)
                        comdivCertificate.COC_ENTRYPOINT_ID = comdivEntryPoint.DIC_ID;

                    comdivCertificate.COC_CERTIFICATETYPE = comdivCertificateType.DIC_ID;
                    comdivCertificate.COC_ISSUING_DATE = certificate.IssuanceDate;
                    comdivCertificate.COC_INVOICED_YN = certificate.IsInvoiced;
                    comdivCertificate.COC_WEB_NUMBER = certificate.Sequential;

                    if (certificate.CertificateStatusId == CertificateStatusEnum.Conform || certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
                    {
                        //verify if fob value exists
                        comdivItem = comdivContext.ITEMs.FirstOrDefault(x => x.ITM_ID == comdivCertificate.ITM_ID);
                        short currencyId = comdivContext.V_DICTIONARY.FirstOrDefault(x => x.DIC_CODE == "USD" && x.REF_ID == 4 && !x.IS_DELETED_YN).DIC_ID;
                        if (comdivItem == null)
                        {
                            result.Status = StatusProcess.Error;
                            result.Message = ServiceResource.SyncFOBValueNotFound;
                            return result;
                        }
                        comdivContext.Set<ITEM>().Attach(comdivItem);
                        comdivContext.Entry(comdivItem).State = EntityState.Unchanged;
                        //update fob value
                        comdivItem.ITM_VALUE = certificate.FOBValue;
                        //update currency
                        comdivItem.ITM_CURR_ID = currencyId;
                        
                    }
                    //save changes
                    comdivContext.SaveChanges();
                }
                //set the certificate as synchronized 
                certificate.IsSynchronized = true;
                UpdateCertificate(certificate);
            }
            catch (Exception ex)
            {
                result.Status = StatusProcess.Error;
                result.Message = ServiceResource.SyncGeneralProblem;
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
            }
            return result;
        }

        private string GetCertificateTypeComdiv(CertificateStatusEnum certificateType)
        {
            string type = string.Empty;
            switch (certificateType)
            {
                case CertificateStatusEnum.Conform:
                    type = ServiceResource.COC;
                    break;
                case CertificateStatusEnum.NonConform:
                    type = ServiceResource.NRC;
                    break;
                case CertificateStatusEnum.Cancelled:
                    type = ServiceResource.CANCEL;
                    break;
                default:
                    type = ServiceResource.COC;
                    break;
            }
            return type;
        }

        /// <summary>
        /// Synchronize with comdiv a list of certificates
        /// </summary>
        /// <param name="certificates">List of certificates</param>
        /// <returns></returns>
        [Invoke]
        public List<ValidationMessage> SynchronizeWithComdivList(List<Certificate> certificates)
        {
            List<ValidationMessage> result = new List<ValidationMessage>();
            ValidationMessage message = null;
           
            foreach (Certificate certificate in certificates)
            {
                message = SynchcroniseWithComdiv(certificate);
                message.Identifier = certificate.Sequential;
                result.Add(message);
            }
            
            return result;
        }

        /// <summary>
        /// Delete release notes
        /// </summary>
        /// <param name="releaseNoteIds">Ids of release notes</param>
        [Invoke]
        public void DeleteReleaseNotes(int[] releaseNoteIds)
        {
            string userName = HttpContext.Current.User.Identity.Name;
            using (VocEntities context = new VocEntities())
            {
                //Delete release note repors
                foreach (int id in releaseNoteIds)
                {
                    DeleteReleaseNoteReport(id);
                }
                //get current release notes
                List<ReleaseNote> currentReleaseNotes = context.ReleaseNotes.Where(x => releaseNoteIds.Contains(x.ReleaseNoteId)).ToList();
                //Delete release notes
                foreach (ReleaseNote item in currentReleaseNotes)
                {
                    item.IsDeleted = true;
                    item.ModificationBy = userName;
                    item.ModificationDate = DateTime.Now;
                }
                context.SaveChanges();
            }
        }

        #region Export To Excel

        /// <summary>
        ///  Method to export a file excel with certificates with the current filters
        /// </summary>
        [Invoke]
        public string ExportReleaseNotesToExcel(string certificateNumber, DateTime? issuanceDateFrom,
            DateTime? issuanceDateTo, int selectedEntryPointId, int selectedOffice, bool published,
            bool unpublished, bool myDocuments, bool conform, bool nonConform, bool cancelled,
            bool created, bool requested, bool approved, bool rejected, bool ongoing,
            bool closed, bool invoiced, bool nonInvoiced, string comdivNumber)
        {
            List<Certificate> certificateList = GetCertificates(certificateNumber, issuanceDateFrom,
           issuanceDateTo, selectedEntryPointId, selectedOffice, published,
             unpublished, myDocuments, conform, nonConform, cancelled,
             created, requested, approved, rejected, ongoing,
             closed, invoiced, nonInvoiced, comdivNumber).ToList();

            string issuanceDateFromString, issuanceDateToString, officeName;
            issuanceDateFromString = issuanceDateFrom.HasValue ? issuanceDateFrom.Value.ToString("dd/MM/yyyy") :
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Count() > 0 ?
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Min().ToString("dd/MM/yyyy") :
                string.Empty;
            issuanceDateToString = issuanceDateTo.HasValue ? issuanceDateTo.Value.ToString("dd/MM/yyyy") :
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Count() > 0 ?
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Max().ToString("dd/MM/yyyy") :
                string.Empty;
            officeName = selectedOffice > 0 ? ctx.Offices.FirstOrDefault(off => off.OfficeId == selectedOffice).OfficeName : ServiceResource.All;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("issuanceDateFrom", issuanceDateFromString);
            parameters.Add("issuanceDateTo", issuanceDateToString);
            parameters.Add("officeName", officeName);

            MemoryStream report = ExcelManagement.GenerateReleaseNotesReport(certificateList, HostingEnvironment.MapPath("~/Images/Logo-Voc-Iraq.png"), parameters);
            report.Position = 0;

            string userName = HttpContext.Current.User.Identity.Name;
            userName = userName.Replace("\\", "");
            userName += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            userName = userName.Replace("/", "_");
            userName = userName.Replace(":", "_");
            string NameDocument = "ReleaseNotes" + userName + ".xlsx";
            string folder = AuthenticationDomainService.GetSourcePathExcelFile();
            string fullPath = folder + @"\" + NameDocument;

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            File.WriteAllBytes(fullPath, report.ToArray());

            return NameDocument;
        }





        /// <summary>
        ///  Method to export a file excel with certificates with the current filters
        /// </summary>
        [Invoke]
        public string ExportCertificatesToExcel(string certificateNumber, DateTime? issuanceDateFrom,
            DateTime? issuanceDateTo, int selectedEntryPointId, int selectedOffice, bool published,
            bool unpublished, bool myDocuments, bool conform, bool nonConform, bool cancelled,
            bool created, bool requested, bool approved, bool rejected, bool ongoing,
            bool closed, bool invoiced, bool nonInvoiced,string comdivNumber)
        {
            List<Certificate> certificateList = GetCertificates(certificateNumber, issuanceDateFrom,
           issuanceDateTo, selectedEntryPointId, selectedOffice, published,
             unpublished, myDocuments, conform, nonConform, cancelled,
             created, requested, approved, rejected, ongoing,
             closed, invoiced, nonInvoiced, comdivNumber).ToList();

            string issuanceDateFromString, issuanceDateToString, officeName;
            issuanceDateFromString = issuanceDateFrom.HasValue ? issuanceDateFrom.Value.ToString("dd/MM/yyyy") :
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Count() > 0 ?
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Min().ToString("dd/MM/yyyy") :
                string.Empty;
            issuanceDateToString = issuanceDateTo.HasValue ? issuanceDateTo.Value.ToString("dd/MM/yyyy") : 
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Count() > 0 ?
                certificateList.Where(x => x.IssuanceDate != null).Select(x => x.IssuanceDate.GetValueOrDefault()).Max().ToString("dd/MM/yyyy") :
                string.Empty;
            officeName = selectedOffice > 0 ? ctx.Offices.FirstOrDefault(off => off.OfficeId == selectedOffice).OfficeName : ServiceResource.All;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("issuanceDateFrom", issuanceDateFromString);
            parameters.Add("issuanceDateTo", issuanceDateToString);
            parameters.Add("officeName", officeName);

            MemoryStream report = ExcelManagement.GenerateReport(certificateList, "Certificates", typeof(Certificate), HostingEnvironment.MapPath("~/Images/Logo-Voc-Iraq.png"), parameters);
            report.Position = 0;

            string userName = HttpContext.Current.User.Identity.Name;
            userName = userName.Replace("\\", "");
            userName += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            userName = userName.Replace("/", "_");
            userName = userName.Replace(":", "_");
            string NameDocument = "Certificates" + userName + ".xlsx";
            string folder = AuthenticationDomainService.GetSourcePathExcelFile();
            string fullPath = folder + @"\" + NameDocument;

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            File.WriteAllBytes(fullPath, report.ToArray());

            return NameDocument;
        }

        /// <summary>
        /// Method to export a file excel with documents to the certificate
        /// </summary>
        /// <param name="certificateId">certificate id</param>
        [Invoke]
        public string ExportCertificateDocuments(int certificateId)
        {
            List<Document> documentList = GetDocumentsByCertificateId(certificateId).ToList();
            MemoryStream report = ExcelManagement.GenerateReport(documentList, "Documents", typeof(Document), HostingEnvironment.MapPath("~/Images/Logo-Voc-Iraq.png"));
            report.Position = 0;

            string userName = HttpContext.Current.User.Identity.Name;
            userName = userName.Replace("\\", "");
            userName += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            userName = userName.Replace("/", "_");
            userName = userName.Replace(":", "_");
            string NameDocument = "DocumentsCertifificate" + certificateId + userName + ".xlsx";
            string folder = AuthenticationDomainService.GetSourcePathExcelFile();
            string fullPath = folder + @"\" + NameDocument ;

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            File.WriteAllBytes(fullPath, report.ToArray());

            return NameDocument;
        }

        /// <summary>
        /// Export security paper list to excel
        /// </summary>
        /// <param name="entryPointId"></param>
        /// <returns></returns>
        [Invoke]
        public string ExportSecutiryPapers(DateTime? issuanceDateFrom, DateTime? issuanceDateTo, int selectedEntryPoint,
            bool issued, bool notIssued, bool cancelled, bool misPrinted,string number)
        {
            List<SecurityPaper> securityPapers = GetSecurityPaperList(issuanceDateFrom,issuanceDateTo,selectedEntryPoint,
            issued,notIssued,cancelled,misPrinted,number).ToList();
            string reportName = string.Empty;

            string minDate, maxDate;
            minDate = minDate = securityPapers.Where(x => x.ModificationDate != null).Select(x => x.ModificationDate.GetValueOrDefault()).Count() > 0 ?
                securityPapers.Where(x => x.ModificationDate != null).Select(x => x.ModificationDate.GetValueOrDefault()).Min().ToString("dd/MM/yyyy") : string.Empty;
            maxDate = maxDate = securityPapers.Where(x => x.ModificationDate != null).Select(x => x.ModificationDate.GetValueOrDefault()).Count() > 0 ?
                securityPapers.Where(x => x.ModificationDate != null).Select(x => x.ModificationDate.GetValueOrDefault()).Max().ToString("dd/MM/yyyy") :string.Empty;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("modificationDateFrom", minDate);
            parameters.Add("modificationDateTo", maxDate);

            MemoryStream report = ExcelManagement.GenerateReport(securityPapers, "Security papers", typeof(SecurityPaper), HostingEnvironment.MapPath("~/Images/Logo-Voc-Iraq.png"), parameters);
            report.Position = 0;
            string userName = HttpContext.Current.User.Identity.Name;
            userName = userName.Replace("\\", "");
            userName += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            userName = userName.Replace("/", "_");
            userName = userName.Replace(":", "_");
            reportName = "SecurityPapers" + userName + ".xlsx";
            string folder = AuthenticationDomainService.GetSourcePathExcelFile();
            string fullPath = folder + @"\" + reportName;

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            File.WriteAllBytes(fullPath, report.ToArray());
            return reportName;
        }


        
        #endregion

        #region Persistance


        #region Certificate Persistance
        [Delete]
        public void DeleteCertificate(Certificate certificate)
        {
            throw new NotImplementedException("Not supported");
        }

        [Insert]
        public void InsertCertificate(Certificate certificate)
        {
            VocUser currentUser = HttpContext.Current.Cache.Get("LoggedUser" + HttpContext.Current.User.Identity.Name) as VocUser;
            certificate.CreationBy = HttpContext.Current.User.Identity.Name;
            certificate.CreationDate = DateTime.Now;
            certificate.WorkflowStatusId = WorkflowStatusEnum.Created;
            certificate.OfficeId = currentUser.OfficeId;
            if (certificate.CertificateStatusId == CertificateStatusEnum.Cancelled)
            {
                certificate.Sequential = GetNewSecuentialNumber();
                certificate.IssuanceDate = DateTime.Now;
                certificate.WorkflowStatusId = WorkflowStatusEnum.Closed;
            }
            Insert(certificate);
            if (certificate.CertificateStatusId == CertificateStatusEnum.Cancelled)
                SaveCertificateTrancking(certificate.CertificateId, TrackingStatusEnum.Closed);
            else
                SaveCertificateTrancking(certificate.CertificateId, TrackingStatusEnum.Created);
        }

        [Update]
        public void UpdateCertificate(Certificate certificate)
        {
            if (certificate.CertificateStatusId == CertificateStatusEnum.Cancelled)
                certificate.WorkflowStatusId = WorkflowStatusEnum.Closed;
            
            certificate.ModificationBy = HttpContext.Current.User.Identity.Name;
            certificate.ModificationDate = DateTime.Now;
            Update(certificate);
            if (certificate.CertificateStatusId == CertificateStatusEnum.Cancelled)
                SaveCertificateTrancking(certificate.CertificateId, TrackingStatusEnum.Closed);
        }

        #endregion

        #region Document Persistance
        [Delete]
        public void DeleteDocument(Document document)
        {
            throw new NotImplementedException("Not supported");
        }

        [Insert]
        public void InsertDocument(Document document)
        {
            throw new NotImplementedException("Not supported");
        }

        [Update]
        public void UpdateDocument(Document document)
        {
            document.ModificationBy = HttpContext.Current.User.Identity.Name;
            document.ModificationDate = DateTime.Now;
            Update(document);
        }

        #endregion

        #region ReleaseNote Persistance
        [Delete]
        public void DeleteReleaseNote(ReleaseNote releaseNote)
        {
            throw new NotImplementedException("Not supported");
        }

        [Insert]
        public void InsertReleaseNote(ReleaseNote releaseNote)
        {
            var lastReleaseNote = ctx.ReleaseNotes.Where(x => x.CertificateId == releaseNote.CertificateId && x.IsDeleted == false)
                                                            .OrderByDescending(x => x.PartialNumber).FirstOrDefault();

            releaseNote.PartialNumber = lastReleaseNote == null ? 1 : lastReleaseNote.PartialNumber + 1;
            releaseNote.CreationBy = HttpContext.Current.User.Identity.Name;
            releaseNote.CreationDate = DateTime.Now;
            Insert(releaseNote);
        }

        [Update]
        public void UpdateReleaseNote(ReleaseNote releaseNote)
        {
            releaseNote.ModificationBy = HttpContext.Current.User.Identity.Name;
            releaseNote.ModificationDate = DateTime.Now;
            Update(releaseNote);
            DeleteReleaseNoteReport(releaseNote.ReleaseNoteId);
        }

        #endregion

        #region Office Persistance
        [Insert]
        public void InsertOffice(Office office)
        {
            office.CreationBy = HttpContext.Current.User.Identity.Name;
            office.CreationDate = DateTime.Now;
            Insert(office);
        }

        [Update]
        public void UpdateOffice(Office office)
        {
            office.ModificationBy = HttpContext.Current.User.Identity.Name;
            office.ModificationDate = DateTime.Now;
            Update(office);
        }

        [Delete]
        public void DeleteOffice(Office office)
        {
            throw new NotImplementedException("Not supported");
        }

        #endregion Office Persistance

        [Update]
        public void UpdateSecurityPaper(SecurityPaper securityPaper)
        {
            securityPaper.ModificationBy = HttpContext.Current.User.Identity.Name;
            securityPaper.ModificationDate = DateTime.Now;
            Update(securityPaper);
        }


        /// <summary>
        /// Generic Update Method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected void Update<TEntity>(TEntity entity) where TEntity : class
        {
            ctx.Set<TEntity>().Attach(entity);
            ctx.Entry(entity).State = EntityState.Modified;
            ctx.SaveChanges();
        }

        /// <summary>
        /// Generic Insert Method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        protected void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            ctx.Set<TEntity>().Add(entity);
            ctx.SaveChanges();
        }


        #endregion

    }
}