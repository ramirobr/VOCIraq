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
    public partial class ReleaseNoteChildWindow : ChildWindow
    {
        public ReleaseNoteChildWindow()
        {
            InitializeComponent();
            this.Loaded += ReleaseNoteChildWindow_Loaded;
        }

        void ReleaseNoteChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ReleaseNoteViewModel).CheckableControlChanged += ReleaseNoteChildWindow_CheckableControlChanged;
        }

        void ReleaseNoteChildWindow_CheckableControlChanged(object sender, EventArgs e)
        {
            ReleaseNoteViewModel releaseNoteVM = sender as ReleaseNoteViewModel;

            // Since the Security papers are mandatory, 
            // adding an error in the _validationSummary control if we haven't selected any available security paper
            if (releaseNoteVM.IssuedSecurityPapers.Count == 0)
            {
                if (!_validationSummary.Errors.Any(r => r.Message.Contains(releaseNoteVM.CheckableControlValidationErrorMessage)))
                _validationSummary.Errors.Add(new ValidationSummaryItem(releaseNoteVM.CheckableControlValidationErrorMessage));

                _ExpanderControl.Focus();
            }
            else
            {
                ValidationSummaryItem vItem = _validationSummary.Errors.FirstOrDefault(r => r.Message.Contains(releaseNoteVM.CheckableControlValidationErrorMessage));
                _validationSummary.Errors.Remove(vItem);
            }

            releaseNoteVM.HasValidationErrors = _validationSummary.HasErrors;
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

