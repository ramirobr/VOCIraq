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
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Silverlight.Controls;
using System.Collections.ObjectModel;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Certificate list that display certificates according filter criterias
    /// </summary>
    public partial class CertificateList : UserControl
    {

        private CertificateListViewModel _context;

        /// <summary>
        /// Tab item that is going to be closed
        /// </summary>
        private TabItem _closingTabItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateList"/> class.
        /// </summary>
        public CertificateList()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CertificateList_Loaded);
             GridResults.LoadingRow += GridResults_LoadingRow;
        }

        /// <summary>
        /// On loaded the certificate list screen
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void CertificateList_Loaded(object sender, RoutedEventArgs e)
        {
            if (_context == null)
            {
                _context = this.DataContext as CertificateListViewModel;
                _context.CertificateEditionRequested += CertificateEditionRequested;
                _context.GridActionCompleted += GridActionCompleted;
                _context.ShowHideExportButtons += ContextShowHideExportButtons;
                GridResults.Columns.GetByName("colPublishInformation").Visibility = _context.IsPublishVisible ? Visibility.Visible : Visibility.Collapsed;
                GridResults.Columns.GetByName("colSyncComdiv").Visibility = App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin) ? Visibility.Collapsed : Visibility.Visible;
                paginator.CanExportRN = App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin, UserRoleEnum.SuperAdmin,UserRoleEnum.Supervisor) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ContextShowHideExportButtons(object sender, ContextEditionEventArgs<Visibility> e)
        {
            paginator.CanExportExcel = e.Context;
            paginator.CanExportRN = App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin, UserRoleEnum.SuperAdmin, UserRoleEnum.Supervisor) 
                && e.Context == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Execute when a grid action button has been completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridActionCompleted(object sender, EventArgs e)
        {
            CertificateViewModel model = sender as CertificateViewModel;
            if (model != null)
            {
                string headerTitle = model.Label;
                var tabItem = tab.Items.FirstOrDefault(x => ((ClosableTabItem)x).Header.ToString() == headerTitle);
                if (tabItem!=null)
                tabItem_CloseClicked(tabItem, null);
            }
        }

        /// <summary>
        /// When request to display a certificate to edit
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Contains the view model to the new page to be displayed</param>
        private void CertificateEditionRequested(object sender, ContextEditionEventArgs<CertificateViewModel> e)
        {
            var edition = new CertificateWindow();
            edition.DataContext = e.Context;
            OpenClosableTabItem(e.Context.Label, edition);
        }

        #region Handle Tabs


        /// <summary>
        /// On tabitem clicked to close, check if it has changes to display a warning message
        /// </summary>
        /// <param name="sender">ClosableTabItem type</param>
        /// <param name="e">The parameter is not used.</param>
        void tabItem_CloseClicked(object sender, EventArgs e)
        {
            ViewModel vmBase;

            _closingTabItem = sender as ClosableTabItem;

            FrameworkElement fe = _closingTabItem.Content as FrameworkElement;
            Type contextType = fe.DataContext.GetType();

            vmBase = fe.DataContext as ViewModel;


            if (vmBase != null && vmBase.HasChanges)
            {

                SaveConfirmation saveConfirmation = new SaveConfirmation();
                saveConfirmation.Show();
                saveConfirmation.Closed += (s, t) =>
                {
                    ChildWindow w = (ChildWindow)s;
                    if (w.DialogResult.HasValue && w.DialogResult.Value)
                    {
                        RemoveTabItem();
                    }
                };
            }
            else
                RemoveTabItem();
        }


        /// <summary>
        /// Remove tab item when user decides to close it.
        /// </summary>
        private void RemoveTabItem()
        {
            _closingTabItem.Content = null;
            tab.Items.Remove(_closingTabItem);
        }

        /// <summary>
        /// Open a new or existing tab item
        /// </summary>
        /// <param name="header">Tab item header text</param>
        /// <param name="content">Content of tab. (Screen to be displayed)</param>
        private void OpenClosableTabItem(string header, FrameworkElement content)
        {
            //Look if it is open already            
            var pair = tab.TabItemsListIdentifiers.Where(x => x.Value == header);
            if (pair.Count() == 0)
            {

                var model = content.DataContext as CertificateViewModel;
                var tabItem = new ClosableTabItem { Header = header, IsSelected = true };
                if (model != null)
                    model.OnDeleteCompleted += delegate { tabItem_CloseClicked(tabItem, null); };

                tabItem.Content = content;
                tabItem.CloseClicked += tabItem_CloseClicked;
                tab.Items.Add(tabItem);
            }
            else //Activate when it is created
            {
                tab.MoveToTabItem(pair.First().Key);
            }
        }

        #endregion

        void GridResults_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            _context = this.DataContext as CertificateListViewModel;

            //Get the HyperLink Button of column with index 8
            HyperlinkButton editButton = GridResults.Columns.GetByName("colEditAction").GetCellContent(e.Row) as HyperlinkButton;

            if (editButton == null) return;

            //Get the certificate attached to current row
            Certificate certificate = e.Row.DataContext as Certificate;
            bool canEditRoUser = false;
            canEditRoUser = App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin) ||
                            certificate.OfficeId == App.CurrentUser.OfficeId;

            //Set the button text according to IsPublished flag

            if (App.CurrentUser.IsInRole(UserRoleEnum.Coordinator))
            {
                editButton.Content = certificate.CanBeEditedCoordinator && canEditRoUser ? Strings.Edit : Strings.Details;                
            }
            else
            {
                //Set the button text according to IsEditVisible            
                editButton.Content = _context.IsEditVisible && certificate.CanBeEdited && canEditRoUser ? Strings.Edit : Strings.Details;
            }

        }

        private void CheckBox_CheckedCell(object sender, RoutedEventArgs e)
        {
            CheckBox current = sender as CheckBox;
            int certId = int.Parse(current.Tag.ToString());
            ObservableCollection<Certificate> itemSource = GridResults.ItemsSource as ObservableCollection<Certificate>;
            itemSource.First(x => x.CertificateId == certId).Checked = true;
        }

        private void CheckBox_UncheckedCell(object sender, RoutedEventArgs e)
        {
            CheckBox current = sender as CheckBox;
            int certId = int.Parse(current.Tag.ToString());
            ObservableCollection<Certificate> itemSource = GridResults.ItemsSource as ObservableCollection<Certificate>;
            itemSource.First(x => x.CertificateId == certId).Checked = false;
        }

    }
}
