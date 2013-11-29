using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cotecna.Voc.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.WebTesting;

namespace Cotecna.Voc.Silverlight.Web.Test
{
    [TestClass]
    public class TemplateTest
    {
        [TestMethod]
        public void GenerateWordTemplate()
        {
            Certificate cert=new Certificate();
            cert.CertificateStatusId = CertificateStatusEnum.Conform;
            cert.IssuanceDate=DateTime.Now;
            cert.Sequential="CHGVA125631";
            
            cert.EntryPoint=new EntryPoint{ Name="TestEntryPoint"};
            string templatePath=string.Concat(Directory.GetCurrentDirectory(),"\\","Coc.docx");
            string errorValidation = string.Empty;
            var res = WordManagement.GenerateWordReport(cert, templatePath,out errorValidation);
            Assert.AreEqual(errorValidation, string.Empty);
        }

    }
}
