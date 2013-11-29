using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cotecna.Voc.Web;
using Cotecna.Voc.Web.Controllers;
using Cotecna.Voc.Web.Models;
using Cotecna.Voc.Web.Filters;
using WebMatrix.WebData;
using WebMatrix.Data;
using Cotecna.Voc.Business;

namespace Cotecna.Voc.Web.Tests.Controllers
{
    [TestClass]
    public class ControllerTest
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestDefaultSearchCertificates()
        {
            CertificateController controller = new CertificateController();
            ActionResult result = controller.Index();
            CertificateListModel model = (CertificateListModel)((System.Web.Mvc.ViewResultBase)(result)).Model;
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Certificates.Page, 1);
        }

        [TestMethod]
        public void TestSearchCertificatesSpecificPage()
        {
            CertificateController controller = new CertificateController();
            CertificateListModel model = new CertificateListModel();
            ActionResult result = controller.SearchCertificateGrid(model,2);
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            if (model.Certificates.TotalCount > model.Certificates.PageSize)
            {
                Assert.AreEqual(model.Certificates.Page, 2);
            }
            else
            {
                Assert.AreEqual(model.Certificates.Page, 1);
            }
        }

        [TestMethod]
        public void TestSearchCertificatesWithNoResults()
        {
            CertificateController controller = new CertificateController();
            CertificateListModel model = new CertificateListModel();
            model.CertificateNumber = "NUMBER0000001";
            ActionResult result = controller.SearchCertificateGrid(model, 1);
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Certificates.Page, 1);
            Assert.AreEqual(model.Certificates.TotalCount, 0);
            Assert.AreEqual(model.Certificates.NumberOfPages, 0);
            Assert.AreEqual(model.Certificates.Collection.Count, 0);
        }

        [TestMethod]
        public void TestSearchCertificatesWithSpecificFilters()
        {
            CertificateController controller = new CertificateController();
            CertificateListModel model = new CertificateListModel();
            //Test with certificate number == 1
            model.CertificateNumber = "1";
            ActionResult result = controller.SearchCertificateGrid(model, 1);
            if (model.Certificates.Collection.Count > 0)
            {
                int countResults = model.Certificates.Collection.Where(item1 => !item1.Certificate.Sequential.Contains("1")).ToList().Count;
                Assert.AreEqual(countResults, 0);
            }

            //Test with certificate status different to 1,2,3
            model = new CertificateListModel();
            model.CertificateStatusSelected = "1,2,3";
            result = controller.SearchCertificateGrid(model, 1);
            if (model.Certificates.Collection.Count > 0)
            {
                int countResults = model.Certificates.Collection.Where(item1 => item1.Certificate.CertificateStatusId != CertificateStatusEnum.Cancelled && item1.Certificate.CertificateStatusId != CertificateStatusEnum.Conform && item1.Certificate.CertificateStatusId != CertificateStatusEnum.NonConform).ToList().Count;
                Assert.AreEqual(countResults, 0);
            }

            //Test with entry point == 1
            model = new CertificateListModel();
            model.EntryPointSelected = 1;
            result = controller.SearchCertificateGrid(model, 1);
            if (model.Certificates.Collection.Count > 0)
            {
                int countResults = model.Certificates.Collection.Where(item1 => item1.Certificate.EntryPointId !=1).ToList().Count;
                Assert.AreEqual(countResults, 0);
            }

            //Test with issuance date in a range
            model = new CertificateListModel();
            //model.IssuanceDateFrom = new DateTime(2012, 1, 1);
            //model.IssuanceDateTo = new DateTime(2012, 1, 31);
            result = controller.SearchCertificateGrid(model, 1);
            if (model.Certificates.Collection.Count > 0)
            {
                int countResults = model.Certificates.Collection.Where(item1 => item1.Certificate.IssuanceDate < new DateTime(2012, 1, 1) || item1.Certificate.IssuanceDate > new DateTime(2012, 1, 31)).ToList().Count;
                Assert.AreEqual(countResults, 0);
            }

        }

        [TestMethod]
        public void TestSearchDetailFirstCertificateReturnedInTheSearch()
        {
            CertificateController controller = new CertificateController();
            CertificateListModel model = new CertificateListModel();
            ActionResult result = controller.SearchCertificateGrid(model, 1);
            var firstCertificate = model.Certificates.Collection.FirstOrDefault();
            if (firstCertificate != null)
            {
                ActionResult resultDetail = controller.ViewCertificateDetail(firstCertificate.Certificate.CertificateId);
                CertificateDetailModel modelDetail = (CertificateDetailModel)((System.Web.Mvc.ViewResultBase)(resultDetail)).Model;
                //Compare the certificate number displayed in the grid with the one obtained when the user access to the detail screen of the certificate
                Assert.AreEqual(firstCertificate.Certificate.Sequential, modelDetail.Certificate.Sequential);
                //Compare the certificate status displayed in the grid with the one obtained when the user access to the detail screen of the certificate
                Assert.AreEqual(firstCertificate.Certificate.CertificateStatusId, modelDetail.Certificate.CertificateStatusId);
                //Compare the entry point displayed in the grid with the one obtained when the user access to the detail screen of the certificate
                Assert.AreEqual(firstCertificate.Certificate.EntryPointId, modelDetail.Certificate.EntryPointId);

                if (modelDetail.Documents.TotalCount > 0)
                {
                    Assert.AreEqual(modelDetail.Documents.Page, 1);
                    Assert.AreNotEqual(modelDetail.Documents.Collection.Count, 0);
                }
            }
        }

        [TestMethod]
        public void TestVerifySearchOrderedBySequentialNumber()
        {
            CertificateController controller = new CertificateController();
            CertificateListModel model = new CertificateListModel();
            ActionResult result = controller.SearchCertificateGrid(model, 1);
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            IList<int> sequentials = new List<int>();
            int onlyNumber = 0;
            int itemFound = 0;

            foreach (var itemCollection in model.Certificates.Collection)
            {
               //Get the number from the string
               onlyNumber = Convert.ToInt32(String.Concat(itemCollection.Certificate.Sequential.Where(Char.IsDigit)));
                //Verify if there is a previous number less than the current one then ERROR
               itemFound = sequentials.FirstOrDefault(item => item < onlyNumber);
               Assert.AreEqual(itemFound, 0);
               sequentials.Add(onlyNumber);
            }            
        }
        
    }
}
