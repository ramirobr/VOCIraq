using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cotecna.Voc.FileAccess.Test
{
    [TestClass]
    public class FileAccessTest
    {
        [TestMethod]
        public void TestGetPhysicalFile()
        {
            try
            {
                FileAccessService service = new FileAccessService();
                service.GetPhysicalFile();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }
    }
}
