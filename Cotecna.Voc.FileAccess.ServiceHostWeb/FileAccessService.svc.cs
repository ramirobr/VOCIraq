using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cotecna.Voc.FileAccess.Utils;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;

namespace Cotecna.Voc.FileAccess
{
    public class FileAccessService : IFileAccessService
    {
        /// <summary>
        /// Wake up the FileAccess service and keep it running in the IIS
        /// </summary>
        public void InvokeService()
        {
            //Wake up the service 
            ConcurrencyUserThread.Instance.StartContinueProcess();
        }

        public void GetPhysicalFile()
        {
                byte[] fileBytes;
                using (Entities model = new Entities())
                {
                    var listRequested = model.FileRequests.Where(x => x.IsRequested == true).ToList();
                    foreach (var item in listRequested)
                    {
                        //only execute/retry files that are required less than 1 day in order to not fill the diskdrive with logs.
                        if (item.CreationDate.AddDays(1) < DateTime.Now)
                            continue;
                        if (string.IsNullOrEmpty(item.FullName))
                        {
                            var document = model.Documents.FirstOrDefault(x => x.DocumentId == item.DocumentId);
                            if (document != null)
                                item.FullName = string.Concat(document.FilePath, document.Filename);
                        }
                        string onlyFileName = item.FullName.Replace('/', '\\');
                        fileBytes = GetBytes(Path.Combine(Properties.Settings.Default.SourcePath, onlyFileName));
                        string targetFileFullName = Path.Combine(Properties.Settings.Default.TargetPath, onlyFileName);

                        if (File.Exists(targetFileFullName))
                            File.Delete(targetFileFullName);
                        string sudirectoryNeeded = Path.GetDirectoryName(targetFileFullName);
                        if (!Directory.Exists(sudirectoryNeeded))
                            Directory.CreateDirectory(sudirectoryNeeded);
                        System.IO.File.WriteAllBytes(targetFileFullName, fileBytes);
                        item.IsRequested = false;
                        item.ModificationDate = DateTime.Now;
                    }
                    model.SaveChanges();
                }
        }


        /// <summary>
        /// Convert physical file to a byte array
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] GetBytes(string path)
        {
            // read the file and return the byte array byte[]
            using (FileStream fs = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                return buffer;
            }
        }



       

    }
}
