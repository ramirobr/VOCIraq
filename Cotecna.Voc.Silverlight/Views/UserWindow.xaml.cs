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
    public partial class UserWindow : UserControl
    {

        UserViewModel _model;
        OpenFileDialog _openDialog;

        public UserWindow()
        {
            InitializeComponent();
            _openDialog = new OpenFileDialog();

            Loaded += OnWindowsLoaded;
        }

        void OnWindowsLoaded(object sender, RoutedEventArgs e)
        {
            _model = DataContext as UserViewModel;
            Loaded -= OnWindowsLoaded;
        }


        /// <summary>
        /// Upload signature clicked
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void UploadSignature_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                _openDialog.Multiselect = false;
                _openDialog.Filter = "JPG files (*.jpg)|*.jpg";
                var file = _openDialog.ShowDialog();
                if (file.Value == true)
                {
                    byte[] signatureArray= File.ReadAllBytes(_openDialog.File.FullName);

                    //Upload information to database
                    _model.UploadSignature(signatureArray);

                }
            }
            catch (Exception ex)
            {
                ErrorWindow.CreateNew(ex);
            }
        }

        /// <summary>
        /// When the user name field lost the focus, the system search if the user exists
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void txtUserLogin_LostFocus_1(object sender, RoutedEventArgs e)
        {
            UserViewModel vieModel = DataContext as UserViewModel;
            TextBox objSender = sender as TextBox;
            vieModel.SearchUserInformationActiveDirectory(objSender.Text);
        }

        /// <summary>
        /// Searches if the user exists when email field lost the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            UserViewModel viewModel = DataContext as UserViewModel;
            TextBox objSender = sender as TextBox;
            viewModel.SearchUserInformationByEmail(objSender.Text);
        }

        /// <summary>
        /// Sets ContentFocused prior to modify email field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MandatoryField_GotFocus(object sender, RoutedEventArgs e)
        {
            UserViewModel viewModel = DataContext as UserViewModel;
            TextBox objSender = sender as TextBox;
            viewModel.ContentFocused = objSender.Text;
        }
    }
}
