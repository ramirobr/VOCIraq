using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Main page that manage install screen logic
    /// </summary>
    public partial class MainPage : UserControl
    {
        #region private fields
        private MainPageViewModel _context;

        private CertificateList _certificateList;
        private UserList _userList;
        private OfficeList _officeList;
        private SecurityPaperList _securityPaperList;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(MainPage_Loaded);
            GlobalAccessor.Instance.TopLevelMenuSelected += Instance_TopLevelMenuSelected;
            StartInstall();

        }
        #endregion

        #region Private and internal methods

        /// <summary>
        /// Start install process to work OOB only. If it is debug mode, it allows to work inbrowser
        /// </summary>
        private void StartInstall()
        {
            if (Application.Current.IsRunningOutOfBrowser ||   System.Diagnostics.Debugger.IsAttached || App.IsForTest==true)
            {
                //USE MainPage. First call login
                LoginViewModel viewModel = new LoginViewModel();
                viewModel.ChangeScreen += ViewModelChangeScreen;
                viewModel.AuthenticateInSystem();
                GlobalAccessor.Instance.MessageStatus = Strings.Authenticating;
            }
            else
                MainContentControl.Content = new Install();

        }

        /// <summary>
        /// On authentication succesful
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ViewModelChangeScreen(object sender, EventArgs e)
        {
            GlobalAccessor.Instance.MessageStatus = string.Empty;
            _context.LoggedUser = "Hi, " + App.CurrentUser.DisplayName;
            GotoView(VocViews.CertificateList);
        }
        /// <summary>
        /// On loaded mainpage
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_context == null)
            {
                _context = this.DataContext as MainPageViewModel;
                _context.GoToView += context_GoToView;
            }
        }

        /// <summary>
        /// instanciate xamls object according the top menu selected by yser
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Contains the menu selected</param>
        void Instance_TopLevelMenuSelected(object sender, ChangeViewEventArgs e)
        {
            switch (e.View)
            {
                case VocViews.CertificateList:
                    GotoView(VocViews.CertificateList);
                    break;
                case VocViews.UserAdministration:
                    GotoView(VocViews.UserAdministration);
                    break;
                case VocViews.OfficeAdministration:
                    GotoView(VocViews.OfficeAdministration);
                    break;
                case VocViews.SecurityPaper:
                    GotoView(VocViews.SecurityPaper);
                    break;
                case VocViews.Help:
                    GotoView(VocViews.Help);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Event fired according the menu selected by user
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">xaml to be displayed</param>
        private void context_GoToView(object sender, ChangeViewEventArgs e)
        {
            GotoView(e.View);
        }

        /// <summary>
        /// Display the corresponding view
        /// </summary>
        /// <param name="view">The xaml to be displayed. CrmsViews enum type</param>
        internal void GotoView(VocViews view)
        {
            switch (view)
            {
                case VocViews.CertificateList:
                    if (_certificateList == null)
                        _certificateList = new CertificateList();
                    MainContentControl.Content = _certificateList;
                    GlobalAccessor.Instance.FillTopMenus(VocMenuGroup.All);
                    break;
                case VocViews.UserAdministration:
                    if (_userList == null)
                        _userList = new UserList();
                    MainContentControl.Content = _userList;
                    GlobalAccessor.Instance.FillTopMenus(VocMenuGroup.All);
                    break;
                case VocViews.OfficeAdministration:
                    if (_officeList == null)
                        _officeList = new OfficeList();
                    MainContentControl.Content = _officeList;
                    GlobalAccessor.Instance.FillTopMenus(VocMenuGroup.All);
                    break;
                case VocViews.SecurityPaper:
                    if (_securityPaperList == null)
                        _securityPaperList = new SecurityPaperList();
                    MainContentControl.Content = _securityPaperList;
                    GlobalAccessor.Instance.FillTopMenus(VocMenuGroup.All);
                    break;
                case VocViews.Help:
                    MyHyperlinkButton button = new MyHyperlinkButton();
                    button.NavigateUri = new Uri(Strings._UrlHelp);
                    button.TargetName = "_blank";
                    button.ClickMe();
                    GlobalAccessor.Instance.FillTopMenus(VocMenuGroup.All);
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
