using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Cotecna.Voc.FileAccess
{
    [ServiceContract]
    public interface IFileAccessService
    {
        [OperationContract]
        void InvokeService();
    }
}
