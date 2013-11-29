using System;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cotecna.Voc.Silverlight.Web.Services
{
    public partial class VocContext
    {
        partial void OnCreated()
        {
            var proxy = (WebDomainClient<IVocServiceContract>)this.DomainClient;
            proxy.ChannelFactory.Endpoint.Binding.SendTimeout = new TimeSpan(0, 30, 0);
        }
    }
}
