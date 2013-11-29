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
using System.IO;
using Cotecna.Voc.Business;
using System.Security;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Certificate window that display certificate information to be managed
    /// </summary>
    public partial class CertificateWindow : UserControl
    {
        CertificateViewModel _model;
        OpenFileDialog _openDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateWindow"/> class.
        /// </summary>
        public CertificateWindow()
        {
            InitializeComponent();
            _openDialog = new OpenFileDialog();

            Loaded += OnWindowsLoaded;
        }

        void OnWindowsLoaded(object sender, RoutedEventArgs e)
        {
            _model = DataContext as CertificateViewModel;
            _model.DocumentEditionRequested += OnDocumentEditionRequested;
            _model.ReleaseNoteEditionRequested += OnReleaseNoteEditionRequested;
            _model.ActivateDocumentsTab += ModelActivateDocumentsTab;
            _model.ReadOnlyModeChanged += delegate 
            { 
                GridResults.Columns.GetByName("colEdit").Visibility = _model.IsUploadSuppDocumentsEnable ? Visibility.Visible : Visibility.Collapsed;
            };            
            Loaded -= OnWindowsLoaded;
        }

        /// <summary>
        /// Activate tab documents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelActivateDocumentsTab(object sender, EventArgs e)
        {
            tabDocuments.IsSelected = true;
            tabReleaseNotes.IsSelected = false;
        }

        /// <summary>
        /// Upload certificate clicked
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetPath = GetDocumentFilePath(_model.Certificate);

                _openDialog.Multiselect = false;
                _openDialog.Filter = "Word files (*.docx)|*.docx";
                var file = _openDialog.ShowDialog();
                if (file.Value == true)
                {
                    string fileName=_openDialog.File.Name;
                    var existingDocument = _model.DocumentList.FirstOrDefault(x => x.IsSupporting == false && x.Filename != fileName);
                    //Upload information to database
                    _model.Upload(fileName);

                    if (!Directory.Exists(targetPath))
                        Directory.CreateDirectory(targetPath);
                    string targetFile = string.Concat(targetPath, fileName);
                    if(File.Exists(targetFile))
                        File.SetAttributes(targetFile, FileAttributes.Normal);
                    File.Copy(_openDialog.File.FullName, targetFile, true);
                    File.SetAttributes(targetFile, FileAttributes.ReadOnly);

                    if (existingDocument != null)
                    {
                        string pathPrevious=string.Concat(targetPath,existingDocument.Filename);
                        //delete previous document
                        if (File.Exists(pathPrevious))
                        {
                            File.SetAttributes(pathPrevious, FileAttributes.Normal);
                            File.Delete(pathPrevious);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.CreateNew(ex);
            }
        }

        /// <summary>
        /// Upload supporting documents clicked
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void UploadSupportingDocuments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string targetPath = string.Concat(mainPath, newSecuential, "\\");
                string targetPath = GetDocumentFilePath(_model.Certificate);

                _openDialog.Multiselect = true;
                _openDialog.Filter = "All files (*.*)|*.*";
                var file = _openDialog.ShowDialog();
                if (file.Value == true)
                {
                    _model.CounterDocumentFails = 0;
                    foreach (var supportingFile in _openDialog.Files)
                    {
                        _model.UploadSupportingDocument(supportingFile.Name);

                        if (!Directory.Exists(targetPath))
                            Directory.CreateDirectory(targetPath);
                        string targetFile = string.Concat(targetPath, supportingFile.Name);
                        if(!File.Exists(targetFile))
                            File.Copy(supportingFile.FullName, targetFile, false);                        
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.CreateNew(ex);
            }
        }

        /// <summary>
        /// Gets the string path to save the documents of a certificate 
        /// </summary>
        /// <param name="certificate">Current certificate</param>
        /// <returns></returns>
        private string GetDocumentFilePath(Certificate certificate)
        {
            List<WorkflowStatusEnum> status = new List<WorkflowStatusEnum>()
            {
                WorkflowStatusEnum.Approved,
                WorkflowStatusEnum.Ongoing,
                WorkflowStatusEnum.Closed
            };
            
            string path = string.Empty;
            if (status.Contains(certificate.WorkflowStatusId) && certificate.CertificateStatusId == CertificateStatusEnum.Conform)
            {
                string entryPointName = StaticReferences.GetEntryPoints().FirstOrDefault(x => x.EntryPointId == certificate.EntryPointId).Name;
                path = string.Concat(App.CurrentUser.FilePath, entryPointName, "\\", certificate.Sequential, "\\");
            }
            else if (status.Contains(certificate.WorkflowStatusId) && certificate.CertificateStatusId == CertificateStatusEnum.NonConform)
            {
                path = string.Concat(App.CurrentUser.FilePath, certificate.Sequential, "\\");
            }
            else
            {
                string certificateIdentifier = string.IsNullOrEmpty(certificate.Sequential) ? certificate.CertificateId.ToString() : certificate.Sequential;
                path = string.Concat(App.CurrentUser.FilePath, certificateIdentifier, "\\");
            }
            return path;
        }

        private string FixComdivNumberInformation(Certificate certificate)
        {
            string fixedComdivNumber = certificate.ComdivNumber;
            fixedComdivNumber = fixedComdivNumber.Replace("/", "");
            fixedComdivNumber = fixedComdivNumber.Replace("_", "");
            fixedComdivNumber = fixedComdivNumber.Replace("-", "");
            fixedComdivNumber = fixedComdivNumber.Replace("(", "");
            fixedComdivNumber = fixedComdivNumber.Replace(")", "");
            fixedComdivNumber = fixedComdivNumber.Replace("[", "");
            fixedComdivNumber = fixedComdivNumber.Replace("]", "");
            fixedComdivNumber = fixedComdivNumber.Replace("*", "");
            fixedComdivNumber = fixedComdivNumber.Replace("+", "");
            fixedComdivNumber = fixedComdivNumber.Replace(".", "");
            fixedComdivNumber = fixedComdivNumber.Replace("%", "");
            fixedComdivNumber = fixedComdivNumber.Replace("&", "");
            fixedComdivNumber = fixedComdivNumber.Replace("=", "");
            fixedComdivNumber = fixedComdivNumber.Replace(@"\", "");
            fixedComdivNumber = fixedComdivNumber.Replace("#", "");
            fixedComdivNumber = fixedComdivNumber.Replace(",", "");
            fixedComdivNumber = fixedComdivNumber.Replace(";", "");
            fixedComdivNumber = fixedComdivNumber.Replace(":", "");
            return fixedComdivNumber;
        }

      
        /// <summary>
        /// When request to display a document to edit
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Contains the view model to the new page to be displayed</param>
        private void OnDocumentEditionRequested(object sender, ContextEditionEventArgs<DocumentViewModel> e)
        {
            var edition = new DocumentChildWindow();
            edition.DataContext = e.Context;
            edition.Show();
            e.Context.CloseEditableWindow = edition.Close;
        }

        /// <summary>
        /// When request to display a release note to edit
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Contains the view model to the new page to be displayed</param>
        private void OnReleaseNoteEditionRequested(object sender, ContextEditionEventArgs<ReleaseNoteViewModel> e)
        {
            var edition = new ReleaseNoteChildWindow();
            edition.DataContext = e.Context;
            edition.Show();
            e.Context.CanceledItem += (s, ev) => 
            {
                edition.Close();
            };
        }        
    }
}
