using System.Windows.Controls;

namespace Cotecna.Voc.Silverlight
{
    public partial class SecurityPaperChildWindow : ChildWindow
    {
        public SecurityPaperChildWindow()
        {
            InitializeComponent();
        }

        //Check that screen does not have changes before closing it.
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
                this.CheckChangesToClose(e);
            base.OnClosing(e);
        }
    }
}

