using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Cotecna.Voc.Business;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Singleton configuration class
    /// </summary>
    public class GlobalAccessor : INotifyPropertyChanged
    {
        /// <summary>
        /// Constant to set the number of records which will determinate the page size in grids
        /// </summary>
        public const int PageSize = 20;


        #region Properties

        string _message;

        /// <summary>
        /// Gets or sets the message that will be showed in the status bar
        /// </summary>
        public string MessageStatus
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged("MessageStatus");
                }
            }
        }

        private bool _isOnline = NetworkInterface.GetIsNetworkAvailable();
        /// <summary>
        /// Gets a value indicating whether is the EntityManager connected 
        /// </summary>
        public bool IsOnline
        {
            get
            {

                return NetworkInterface.GetIsNetworkAvailable();

            }
            private set
            {
                if (_isOnline != value)
                {
                    _isOnline = value;
                    OnPropertyChanged("IsOnline");
                }
            }
        }

        /// <summary>
        /// Gets the unique instance of this class in the Voc application
        /// </summary>
        public static GlobalAccessor Instance
        {
            get;
            private set;
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalAccessor"/> class.
        /// </summary>
        public GlobalAccessor()
        {
            _list = new ObservableCollection<TopLevelMenu>();
            Instance = this;

            if (!DesignerProperties.IsInDesignTool)
            {
                NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
            }
        }

        /// <summary>
        /// On changed network address
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            bool b = NetworkInterface.GetIsNetworkAvailable();
            if (b)
            {
                IsOnline = true;
                MessageStatus = Strings.NetworkOnline;
            }
            else
            {
                IsOnline = false;
                MessageStatus = Strings.NetworkOffline;
            }
        }



        #region INotifyPropertyChanged

        /// <summary>
        /// Event fired when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Throw the event property changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Manage Top Menus
        /// <summary>
        /// Gets the command action when a top menu was clicked
        /// </summary>
        public ICommand TopLevelMenuClicked
        {
            get
            {
                return new DelegateCommand<VocViews>(
                  (mainView) =>
                  {
                      //Launch event to be handled by some view.
                      if (TopLevelMenuSelected != null)
                          TopLevelMenuSelected(null, new ChangeViewEventArgs(mainView));

                  }
               );
            }
        }

        /// <summary>
        /// Event notifying when a Top Menu was Selected
        /// </summary>
        internal event EventHandler<ChangeViewEventArgs> TopLevelMenuSelected;

        /// <summary>
        /// Load these menus according to authorization
        /// </summary>
        private void LoadAllMenus()
        {
            _list.Add(new TopLevelMenu
            {
                InnerName = VocViews.CertificateList,
                Label = Strings.Certificate,
                ContentSource = LoadXamlResource("CertificateImage.xaml"),
                Group = VocMenuGroup.All
            });
            if (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin, UserRoleEnum.Admin, UserRoleEnum.LOAdmin, UserRoleEnum.Supervisor))
            _list.Add(new TopLevelMenu
            {
                InnerName = VocViews.UserAdministration,
                Label = Strings.UserAdministration,
                ContentSource = LoadXamlResource("UserAdministrationImage.xaml"),
                Group = VocMenuGroup.All
            });
            if (App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin, UserRoleEnum.Supervisor))
                _list.Add(new TopLevelMenu
                {
                    InnerName = VocViews.OfficeAdministration,
                    Label = Strings.OfficeAdministration,
                    ContentSource = LoadXamlResource("OfficeImage.xaml"),
                    Group = VocMenuGroup.All
                });
            if (App.CurrentUser.IsInRole(UserRoleEnum.BorderAgent, UserRoleEnum.LOAdmin, UserRoleEnum.SuperAdmin, UserRoleEnum.Supervisor))
            {
                _list.Add(new TopLevelMenu
                {
                    InnerName = VocViews.SecurityPaper,
                    Label = Strings.SecurityPapers,
                    ContentSource = LoadXamlResource("SecurityPaperImage.xaml"),
                    Group = VocMenuGroup.All
                });
            }
            _list.Add(new TopLevelMenu
            {
                InnerName = VocViews.Help,
                Label = Strings.Help,
                ContentSource = LoadXamlResource("HelpImage.xaml"),
                Group = VocMenuGroup.All
            });
        }

        /// <summary>
        /// Access this method to filter top menus according to page
        /// </summary>
        /// <param name="group">menu group name</param>
        internal void FillTopMenus(VocMenuGroup group)
        {
            if (_list.Count == 0)
                LoadAllMenus();

            if (group != VocMenuGroup.None)
            {
                foreach (var item in _list)
                {
                    if (item.Group == VocMenuGroup.All || item.Group == group)
                        item.IsVisible = true;
                    else
                        item.IsVisible = false;
                }
            }
            else
            {
                foreach (var item in _list)
                    item.IsVisible = false;
            }
        }

        /// <summary>
        /// Loads a xaml resource
        /// </summary>
        /// <param name="name">Name of image</param>
        /// <returns>Resource required</returns>
        internal static object LoadXamlResource(string name)
        {
            var uri = new Uri("/Cotecna.Voc.Silverlight;component/Assets/Images/" + name, UriKind.Relative);
            var streamResourceInfo = Application.GetResourceStream(uri);

            string xaml = null;

            var resourceStream = streamResourceInfo.Stream;

            using (var streamReader = new System.IO.StreamReader(resourceStream))
            {
                xaml = streamReader.ReadToEnd();
                return XamlReader.Load(xaml);
            }

        }


        ObservableCollection<TopLevelMenu> _list;
        /// <summary>
        /// Gets the public list of top menu items
        /// </summary>
        public ObservableCollection<TopLevelMenu> TopLevelMenus { get { return _list; } }
        #endregion


    }

    /// <summary>
    /// An object which represents a top level menu
    /// </summary>
    public class TopLevelMenu : ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TopLevelMenu"/> class.
        /// </summary>
        public TopLevelMenu()
        {
            _isVisible = false;
        }

        /// <summary>
        /// Gets or sets group
        /// </summary>
        public VocMenuGroup Group { get; set; }
        /// <summary>
        /// Gets or sets label
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets image source
        /// </summary>
        public string ImageSource { get; set; }
        /// <summary>
        /// Gets or sets inner name
        /// </summary>
        public VocViews InnerName { get; set; }
        /// <summary>
        /// Gets or sets content source
        /// </summary>
        public object ContentSource { get; set; }

        private bool _isVisible;
        /// <summary>
        /// Gets or sets a value indicating whether is visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }
    }

    /// <summary>
    /// Possible Values for Voc views
    /// </summary>
    public enum VocViews
    {
        /// <summary>
        /// Certificate List menu / Default
        /// </summary>
        CertificateList,
        /// <summary>
        /// User administration menu
        /// </summary>
        UserAdministration,
        /// <summary>
        /// Office administration menu
        /// </summary>
        OfficeAdministration,
        /// <summary>
        /// Show the list of securtiryPapers
        /// </summary>
        SecurityPaper,
        /// <summary>
        /// Open help document
        /// </summary>
        Help

    }

    /// <summary>
    /// Possible Values for the Voc Menu groups
    /// </summary>
    public enum VocMenuGroup
    {
        /// <summary>
        /// None group
        /// </summary>
        None,
        /// <summary>
        /// Application group
        /// </summary>
        Application,
        /// <summary>
        /// All group
        /// </summary>
        All
    }


    /// <summary>
    /// Arguments sent when a view changes
    /// </summary>
    internal class ChangeViewEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets view
        /// </summary>
        public VocViews View { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeViewEventArgs"/> class.
        /// </summary>
        /// <param name="view">Voc view</param>
        public ChangeViewEventArgs(VocViews view)
        {
            View = view;
        }
    }
}
