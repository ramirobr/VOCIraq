using System.ComponentModel;
using System.Windows.Controls;

namespace Cotecna.Voc.Silverlight
{
    public partial class CertificateTrackingChildWindow : ChildWindow
    {
        public CertificateTrackingChildWindow()
        {
            InitializeComponent();
        }
        //Check that screen does not have changes before closing it.
        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == null)
                this.CheckChangesToClose(e);
            base.OnClosing(e);
        }
    }
}

