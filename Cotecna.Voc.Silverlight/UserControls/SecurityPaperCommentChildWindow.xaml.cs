using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cotecna.Voc.Silverlight
{
    public partial class SecurityPaperCommentChildWindow : ChildWindow
    {
        public SecurityPaperCommentChildWindow()
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

