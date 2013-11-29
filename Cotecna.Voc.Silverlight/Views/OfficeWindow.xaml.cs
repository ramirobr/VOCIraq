using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class OfficeWindow : UserControl
    {
        OfficeViewModel _model;
        OpenFileDialog _openDialog;

        public OfficeWindow()
        {
            InitializeComponent();
            _openDialog = new OpenFileDialog();
            Loaded += OnWindowsLoaded;

        }

        void OnWindowsLoaded(object sender, RoutedEventArgs e)
        {
            _model = DataContext as OfficeViewModel;
            Loaded -= OnWindowsLoaded;
        }

        /// <summary>
        /// Upload signature clicked
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void UploadOfficeSignature_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                _openDialog.Multiselect = false;
                _openDialog.Filter = "JPG files (*.jpg)|*.jpg";
                var file = _openDialog.ShowDialog();
                if (file.Value == true)
                {
                    byte[] signatureArray = File.ReadAllBytes(_openDialog.File.FullName);

                    //Upload information to database
                    _model.UploadOfficeSignature(signatureArray);

                }
            }
            catch (Exception ex)
            {
                ErrorWindow.CreateNew(ex);
            }
        }
    }
}
