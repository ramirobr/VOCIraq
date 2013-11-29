using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cotecna.Voc.Business;
using Cotecna.Voc.Web.Models;
using System.IO;
using Cotecna.Voc.Web.Common;

namespace Cotecna.Voc.Web.Controllers
{
    public class CertificateController : BaseController
    {
        #region Fields
        private VocEntities db = new VocEntities();
        #endregion

        #region Methods
        /// <summary>
        /// Get the list of certificates without filter data when the list screen is displayed
        /// </summary>
        /// <returns>Certificate list view</returns>
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Index()
        {
            CertificateListModel model = new CertificateListModel();
            //Get the entry points to be loaded in the screen
            IList<EntryPoint> query = (from entryPoints in db.EntryPoints where entryPoints.IsDeleted == false select entryPoints).ToList();
            model.EntryPoints = new SelectList(query, "EntryPointId", "Name");
            //Search the certificates in the page 1
            model.SearchCertificateList(1);
            return View(model);
        }

        /// <summary>
        /// Get the list of certificates accordint the filter data selected by the user
        /// </summary>
        /// <param name="model">Filter data</param>
        /// <param name="selectedPage">Page selected chosen for the search</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public PartialViewResult SearchCertificateGrid(CertificateListModel model, int? selectedPage)
        {
            model.SearchCertificateList(selectedPage.HasValue?selectedPage.Value:1);
            return PartialView("_CertificateGrid", model);
        }

        /// <summary>
        /// Get the pdf certificate and return to the view
        /// </summary>
        /// <param name="fileSectionPath">Part of the path where the pdf is located</param>
        /// <param name="fileName">Pdf file name</param>
        public void ViewCertificate(string fileSectionPath, string fileName)
        {
            string templatePath=string.Empty ;
            if(fileSectionPath.StartsWith("/"))
                templatePath = string.Format("{0}{1}/{2}", Cotecna.Voc.Web.Properties.Settings.Default.PathDocument, fileSectionPath, fileName);
            else
                templatePath = string.Format("{0}/{1}/{2}", Cotecna.Voc.Web.Properties.Settings.Default.PathDocument, fileSectionPath, fileName);

            templatePath = templatePath.Replace('/', '\\');
            //wait until fileaccess get file
            while (!System.IO.File.Exists(templatePath))
            {
                System.Threading.Thread.Sleep(5000);
            }

            byte[] byteArray = System.IO.File.ReadAllBytes(templatePath);

            //Show the document for download
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("cache-control", "must-revalidate");
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", fileName));
            Response.AddHeader("Content-Length", byteArray.Length.ToString());
            string extension = Path.GetExtension(fileName);
            if (extension.ToLower() == ".pdf")
                Response.ContentType = "application/pdf";
            else
                Response.ContentType = "application/otc-stream";
            Response.BinaryWrite(byteArray);
            //Response.End();
            //No view here!
        }

        /// <summary>
        /// Check the pdf certificate file exists in the server
        /// </summary>
        /// <param name="fileSectionPath">Part of the path where the pdf is located</param>
        /// <param name="fileName">Pdf file name</param>
        /// <returns>Ok when it exists; otherwise return "fail"</returns>
        public string CheckFile(string fileSectionPath, string fileName)
        {
            string onlyFilePath= string.Format("{0}/{1}", fileSectionPath, fileName);
            string templatePath = string.Format("{0}/{1}", Cotecna.Voc.Web.Properties.Settings.Default.PathDocument, onlyFilePath);
            if (!System.IO.File.Exists(templatePath))
            {
                //add request.
                var request = new FileRequest() { FullName = onlyFilePath, CreationBy = "WebSite", CreationDate = DateTime.Now, IsDeleted = false, IsRequested=true };
                db.FileRequests.Add(request);
                db.SaveChanges();
                return "fail";
            }
            return "ok";
        }

        /// <summary>
        /// Display the detail information of a specific certificate
        /// </summary>
        /// <param name="certificateId">Certificate id</param>
        /// <returns>View and model with certificate information</returns>
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ViewCertificateDetail(int certificateId)
        {
            CertificateDetailModel certificateDetailModel = new CertificateDetailModel();
            //Get the certificate data with its documents
            certificateDetailModel.GetCertificateDetail(certificateId,true);
            return View("Certificate",certificateDetailModel);            
        }

        /// <summary>
        /// Get the documents related to a specific certificate
        /// </summary>
        /// <param name="certificateId">Certificate id</param>
        /// <param name="selectedPage">Selected page to execute the search(pagination)</param>
        /// <returns>Partial view that contains the document grid and the model</returns>
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public PartialViewResult SearchCertificateDocuments(int certificateId, int selectedPage)
        {
            CertificateDetailModel certificateDetailModel = new CertificateDetailModel();
            certificateDetailModel.GetCertificateDetail(certificateId, false);
            certificateDetailModel.Certificate = new Certificate { CertificateId = certificateId };
            return PartialView("_CertificateDocument", certificateDetailModel);
        }

        /// <summary>
        /// Search the certificate list without pagination
        /// </summary>
        /// <param name="model">Certificate model</param>
        /// <param name="selectedPage">Selected page</param>
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public void SearchCertificateExport(CertificateListModel model, int? selectedPage)
        {
            model.SearchCertificateList(1, true);
            MemoryStream report = ExcelManagement.GenerateCertificateReport(model, Server.MapPath("~/Images/Logo-Voc-Iraq.png"));
            report.Position = 0;
            string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, "CertificateReport" + currentDateTime + ".xlsx");
            Session.Add("CertificateReport", result);
        }

        /// <summary>
        /// Download the report
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Client")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public FileStreamResult DownloadExcelReport()
        {
            MicrosoftExcelStreamResult result = Session["CertificateReport"] as MicrosoftExcelStreamResult;
            Session.Remove("CertificateReport");
            return result;
        }

        #endregion
    }
}