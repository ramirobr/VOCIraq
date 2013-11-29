using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotecna.Voc.FileAccess.Invoker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (FileServiceReference.FileAccessServiceClient proxy = new FileServiceReference.FileAccessServiceClient())
                {
                    proxy.InvokeService();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());                
            }
        }
    }
}
