using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Web.Services;

namespace Cotecna.Voc.Silverlight.Web
{
    public partial class DownloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get parameters
            string fileIdQuery = Request.QueryString["CertificateId"];
            string documentIdQuery = Request.QueryString["DocumentId"];
            string exportDocumentQuery = Request.QueryString["ExportDocument"];
            string filename = Request.QueryString["filename"];
            try
            {
                int certificateId;
                int documentId;
                if (int.TryParse(fileIdQuery, out certificateId))
                {
                    using (var ctx = new VocEntities())
                    {
                        var certificate = ctx.Certificates.FirstOrDefault(x => x.CertificateId == certificateId);
                        if (certificate != null)
                        {
                            string fileName = string.Empty;
                            if(certificate.CertificateStatusId==CertificateStatusEnum.Conform)
                                fileName="Coc.docx";
                            else
                                fileName = "NCR.docx";
                            string templates = Server.MapPath(VirtualPathUtility.ToAbsolute("~/WordTemplates/"));
                            string templatePath = templates + fileName;
                            string errorValidation = string.Empty;
                            var file = WordManagement.GenerateWordReport(certificate, templatePath,out errorValidation);

                            if (!string.IsNullOrEmpty(errorValidation))
                            {
                                lblDisplayFilesError.Text = errorValidation;
                                return;
                            }
                           byte[] getContent = file.ToArray();
                           string certificateIdentifier = VocService.FixComdivNumberInformation(certificate);
                           ReturnFile(certificateIdentifier + "_" + fileName, getContent);
                        }
                    }
                }
                else if(int.TryParse(documentIdQuery, out documentId))
                {
                    using (var ctx=new VocEntities())
                    {
                        var doc=ctx.Documents.Where(x=>x.DocumentId==documentId).FirstOrDefault();
                        if (doc != null)
                        {
                            string path = Properties.Settings.Default.PathDocument + doc.FilePath + doc.Filename;
                            if (!File.Exists(path))
                            {
                                lblDisplayFilesError.Text = "Not exist the file: " + doc.Filename;
                                return;
                            }
                            byte[] byteArray = File.ReadAllBytes(path);

                            ReturnFile(doc.Filename, byteArray);
                        }
                        else
                        {
                            lblDisplayFilesError.Text = "Not found the document required";
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(exportDocumentQuery))
                {
                    string NameDocument= exportDocumentQuery;
                    string folder = AuthenticationDomainService.GetSourcePathExcelFile();
                    string fullPath = folder + @"\" + NameDocument ;

                    if (!File.Exists(fullPath))
                    {
                        lblDisplayFilesError.Text = "file not found";
                        return;
                    }
                    byte[] byteArray = File.ReadAllBytes(fullPath);

                    ReturnFile(NameDocument, byteArray);
                }
                else if (!string.IsNullOrEmpty(filename))
                {
                    VocEntities db = new VocEntities();
                    Document doc = db.Documents.FirstOrDefault(x => x.Filename == filename && x.IsDeleted == false);
                    string path = Properties.Settings.Default.PathDocument + doc.FilePath + doc.Filename;
                    
                    if (!File.Exists(path))
                    {
                        lblDisplayFilesError.Text = "file not found";
                        return;
                    }
                    byte[] byteArray = File.ReadAllBytes(path);

                    ReturnFile(filename, byteArray);
                }

            }
            catch (Exception ex)
            {
                lblDisplayFilesError.Text = ex.Message;
            }
        }

        private void ReturnFile(string fileName, byte[] getContent)
        {
            Response.ClearHeaders();
            Response.AddHeader("content-disposition", string.Format(CultureInfo.CurrentCulture, "attachment; filename={0}", fileName));
            Response.ContentType = GetContentType(fileName);
            Response.ClearContent();
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            Response.BinaryWrite(getContent);
            Response.Flush();
            Response.Close();
        }

        #region GetContentType
        [SuppressMessage("Microsoft.Performance", "CA1822")]
        public string GetContentType(string strextension)
        {
            if (string.IsNullOrEmpty(strextension)) throw new ArgumentNullException("strextension");
            string contentType;
            switch (strextension.ToLower(CultureInfo.CurrentCulture))
            {
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".doc":
                    contentType = "application/ms-word";
                    break;
                case ".docx":
                    contentType = "application/vnd.ms-word.document.12";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case ".ppt":
                    contentType = "application/vnd.ms-powerpoint";
                    break;
                case ".zip":
                    contentType = "application/zip";
                    break;
                case ".txt":
                    contentType = "text/plain";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            return contentType;
        }
        #endregion
    }
}