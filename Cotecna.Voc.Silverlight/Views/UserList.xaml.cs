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
using Cotecna.Silverlight.Controls;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    public partial class UserList : UserControl
    {
        private UserListViewModel _context;

        /// <summary>
        /// Tab item that is going to be closed
        /// </summary>
        private TabItem _closingTabItem;

        public UserList()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UserList_Loaded);
            GridResults.LoadingRow += GridResults_LoadingRow;
        }

        /// <summary>
        /// On loaded the User List screen
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void UserList_Loaded(object sender, RoutedEventArgs e)
        {
            if (_context == null)
            {
                _context = this.DataContext as UserListViewModel;
                _context.UserEditionRequested += UserEditionRequested;
                _context.ShowHideExportButton += ContextShowHideExportButton;
                GridResults.Columns.GetByName("colOffice").Visibility = _context.UserListFilters.IsOfficeVisible ? Visibility.Visible : Visibility.Collapsed;
                GridResults.Columns.GetByName("colEntryPoint").Visibility = _context.UserListFilters.IsEntryPointVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Show or hide export button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextShowHideExportButton(object sender, ContextEditionEventArgs<Visibility> e)
        {
            paginator.CanExportExcel = e.Context;
        }

        /// <summary>
        /// When request to display a user to edit
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Contains the view model to the new page to be displayed</param>
        private void UserEditionRequested(object sender, ContextEditionEventArgs<UserViewModel> e)
        {
            var edition = new UserWindow();
            edition.DataContext = e.Context;
            OpenClosableTabItem(e.Context.Label, edition);
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
                var model = content.DataContext as UserViewModel;
                var tabItem = new ClosableTabItem { Header = header, IsSelected = true };
                if (model != null)
                    model.OnSaveCompleted += delegate { tabItem_CloseClicked(tabItem,null); };

                tabItem.Content = content;
                tabItem.CloseClicked += tabItem_CloseClicked;
                tab.Items.Add(tabItem);
            }
            else //Activate when it is created
            {
                tab.MoveToTabItem(pair.First().Key);
            }
        }


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
        /// Sets the text to the hiperlink depending if the user is able to view o modify the information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridResults_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //Get the HyperLink Button to set the text accordingly
            HyperlinkButton editButton = GridResults.Columns.GetByName("colEdit").GetCellContent(e.Row) as HyperlinkButton;

            if (editButton == null) return;


            // Sets the text accordingly

            if (App.CurrentUser.IsInRole(UserRoleEnum.Supervisor))
            {
                editButton.Content = Strings.View;
            }
            else
            {
                editButton.Content = Strings.Edit;
            }

        }
    }
}
