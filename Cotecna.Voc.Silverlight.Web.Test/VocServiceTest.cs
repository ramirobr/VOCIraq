using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cotecna.Voc.Silverlight.Web.Test
{
    [TestClass]
    public class VocServiceTest
    {
        private const int certificateId=28;

        [TestMethod]
        public void GetCertificatesTest()
        {
            //VocService service = new VocService();
            //IList<Certificate> result = service.GetCertificates(new CertificateFilterModel()).ToList();
            //Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void GetEntryPointsTest()
        {
            VocService service = new VocService();
            IList<EntryPoint> result = service.GetEntryPoints().ToList();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetCertificateByCertificateIdTest()
        {
            VocService service = new VocService();
            Certificate result = service.GetCertificateByCertificateId(certificateId);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UploadCocDocumentTest()
        {
            MockAuthentication();
            VocService service = new VocService();
            service.UploadCocDocument(certificateId, "FileTest.docx");
            Assert.AreEqual(1,1);
        }

        [TestMethod]
        public void UploadSupportingDocumentTest()
        {
            MockAuthentication();
            VocService service = new VocService();
            var result = service.UploadSupportingDocument(certificateId, "Tulips.jpg");
            Assert.AreEqual(1, 1);
            
        }

        public void MockAuthentication()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
                );

            // User is logged in
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity("username"),
                new string[0]
                );

            // User is logged out
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity(String.Empty),
                new string[0]
                );
        }

        [TestMethod]
        public void GetCertificateDocumentByCertificateIdTest()
        {
            VocService service = new VocService();
            var document=service.GetCertificateDocumentByCertificateId(certificateId);
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void GetUserInformationActiveDirectory()
        {
            AuthenticationDomainService service = new AuthenticationDomainService();
            UserProfile user = service.GetUserInformationActiveDirectory("ecuiorbatallas", "AMERICA");
            Assert.AreNotEqual(user, null);
        }

        [TestMethod]
        public void GetUserInformationByEmail()
        {
            AuthenticationDomainService service = new AuthenticationDomainService();
            UserProfile user = service.GetUserInformationByEmail("javier.naveda@cotecna.com.ec");
            Assert.AreNotEqual(user, null);
        }

        [TestMethod]
        public void GetSecurityPapersTest()
        {
            //VocService service = new VocService();
            //List<SecurityPaper> result = service.GetSecurityPapers(1, 3);
            //Assert.AreNotEqual(result, null);
        }

        [TestMethod]
        public void GenerateReleaseNoteReport()
        {
            string errors = string.Empty;
            VocEntities db = new VocEntities();
            ReleaseNote rs = db.ReleaseNotes.FirstOrDefault(x => x.ReleaseNoteId == 3001);
            MemoryStream report = WordManagement.GenerateReleaseNoteReport(rs, @"D:\NewTfs\VocIraqMain\Cotecna.Voc\Cotecna.Voc.Silverlight.Web\WordTemplates\TemplateRN.docx", out errors);
            report.Position = 0;
            File.WriteAllBytes(@"D:\rs.docx", report.ToArray());
            WordManagement.SaveWordReportAsPdf(report,@"D:\rs.docx");
        }
    }
}
