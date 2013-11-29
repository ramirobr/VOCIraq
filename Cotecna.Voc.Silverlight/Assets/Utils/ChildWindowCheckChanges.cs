using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cotecna.Voc.Silverlight
{
    public static class ChildWindowHelper
    {
        public static void CheckChangesToClose(this ChildWindow child, System.ComponentModel.CancelEventArgs e)
        {
            var vmBase = child.DataContext as ViewModel;
            if (vmBase != null && vmBase.HasChanges)
            {
                SaveConfirmation saveConfirmation = new SaveConfirmation();
                saveConfirmation.Show();
                saveConfirmation.Closed += (s, t) =>
                {
                    ChildWindow w = (ChildWindow)s;
                    if (w.DialogResult.HasValue && w.DialogResult.Value)
                    {
                        child.DialogResult = false;
                        vmBase.CancelChanges();
                    }
                };
                if (e != null)
                    e.Cancel = true;
            }
            else
                child.DialogResult = false;
        }
    }
}
