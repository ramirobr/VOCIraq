using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using Cotecna.Voc.Business;
using Cotecna.Silverlight.Controls.Extension;
using Cotecna.Voc.Silverlight.Assets.Resources;
using Cotecna.Voc.Silverlight.Web.Services;

namespace Cotecna.Voc.Silverlight
{
    public class OfficeListViewModel : ViewModel 
    {
        #region Private Fields
        private Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel _gridViewModel;
        private VocContext _context = new VocContext();
        private ObservableCollection<Office> _officeList;
        private OfficeListFilters _officeListFilters;
        private bool _isNewVisible;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the list of Office
        /// </summary>		
        public ObservableCollection<Office> OfficeList
        {
            get
            {
                return _officeList;
            }
            set
            {
                _officeList = value;
                OnPropertyChanged(() => OfficeList);
            }
        }

        /// <summary>
        /// Gets or sets the filters to search Offices
        /// </summary>		
        public OfficeListFilters OfficeListFilters
        {
            get
            {
                return _officeListFilters;
            }
            set
            {
                if (_officeListFilters == value) return;
                _officeListFilters = value;
                OnPropertyChanged("OfficeListFilters");
            }
        }

        
        /// <summary>
        /// Gets or sets IsNewVisible
        /// </summary>		
        public bool IsNewVisible
        {
            get
            {
                return _isNewVisible;
            }
            set
            {
                if (_isNewVisible == value) return;
                _isNewVisible = value;
                OnPropertyChanged("IsNewVisible");
            }
        }
        #endregion

        #region Commands

        private ICommand _searchCommand;
        /// <summary>
        /// Get search command
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new DelegateCommand(ExecuteSearchCommand);
                return _searchCommand;
            }
        }

        private ICommand _resetCommand;

        /// <summary>
        /// Gets reset command
        /// </summary>
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                    _resetCommand = new DelegateCommand(ExecuteResetCommand);
                return _resetCommand;
            }
        }


        private ICommand _newCommand;

        /// <summary>
        /// Gets New command
        /// </summary>
        public ICommand NewCommand
        {
            get
            {
                if (_newCommand == null)
                    _newCommand = new DelegateCommand(ExecuteNewCommand);
                return _newCommand;
            }
        }


        private ICommand _editCommand;

        /// <summary>
        /// Gets Edit command
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                    _editCommand = new DelegateCommand<Office>(ExecuteEditCommand);
                return _editCommand;
            }
        } 

        #endregion

        #region Event Handlers
        /// <summary>
        /// Event used to notify to display the Office screen
        /// </summary>
        internal event EventHandler<ContextEditionEventArgs<OfficeViewModel>> OfficeEditionRequested;
        #endregion

        #region Constructor

        public OfficeListViewModel()
        {
            OfficeListFilters = new OfficeListFilters();
            OfficeList = new ObservableCollection<Office>();
            //Get the list of Offices
            IsBusy = true;
            GetOfficesPaginated(0);
            IsNewVisible = App.CurrentUser.IsInRole(UserRoleEnum.SuperAdmin);
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Execute search command
        /// </summary>
        private void ExecuteSearchCommand()
        {
            IsBusy = true;
            //Get the list of Office
            GetOfficesPaginated(0);
        }

        /// <summary>
        /// Execute reset command
        /// </summary>
        private void ExecuteResetCommand()
        {
            OfficeListFilters.SetDefaultValues();
            IsBusy = true;
            GetOfficesPaginated(0);
        }

        /// <summary>
        /// Execute the click event of the button new
        /// </summary>
        private void ExecuteNewCommand()
        {
            Office office = new Office();
            OfficeViewModel officeViewModel = new OfficeViewModel();

            if (OfficeEditionRequested != null)
            {
                officeViewModel.OnSaveCompleted += Model_OnSaveCompleted;
                officeViewModel.Initialize(office,_context);
                OfficeEditionRequested(this, new ContextEditionEventArgs<OfficeViewModel>(officeViewModel));
            }
        }

        /// <summary>
        /// Control of pagination
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void GridViewModelPageIndexChanged(object sender, PageIndexChangedArgs e)
        {
            IsBusy = true;
            GetOfficesPaginated((int)e.PageNumber());
        }

        /// <summary>
        /// Get the list of Offices paginated
        /// </summary>
        /// <param name="selectedPage">Page number in 0 base</param>
        private void GetOfficesPaginated(int selectedPage)
        {
            //calculate the current index
            int currentIndex = selectedPage * App.CurrentUser.PageSize;
            //set the page on the paginator
            if (GridViewModel.PageIndex > 1 && selectedPage == 0)
                GridViewModel.PageIndex = 1;
            
            //begin the pagination
            //get the paginated list of Offices
            EntityQuery<Office> query = _context.GetFilteredOfficesQuery(_officeListFilters.OfficeName,
                                                                         _officeListFilters.OfficeCode,
                                                                         _officeListFilters.Active,
                                                                         _officeListFilters.Inactive,
                                                                         _officeListFilters.IsRegionalOffice)
                                                                         .OrderBy(x => x.OfficeId)
                                                                         .Skip(currentIndex)
                                                                         .Take(App.CurrentUser.PageSize);
            query.IncludeTotalCount = true;
            _context.Load(query, LoadBehavior.RefreshCurrent, LoadOfficesCompleted, null);
        }

        /// <summary>
        /// Completed method for GetOfficesQuery
        /// </summary>
        /// <param name="operation"></param>
        private void LoadOfficesCompleted(LoadOperation<Office> operation)
        {
            GridViewModel.TotalRecords = (uint)operation.TotalEntityCount;
            OfficeList = new ObservableCollection<Office>(operation.Entities);
            GlobalAccessor.Instance.MessageStatus = Strings.Ready;
            IsBusy = false;
        }

        /// <summary>
        /// Edit the selected office
        /// </summary>
        /// <param name="item">user item</param>
        private void ExecuteEditCommand(Office office)
        {
            if (OfficeEditionRequested != null)
            {
                OfficeViewModel OfficeViewModel = new OfficeViewModel();
                OfficeViewModel.OnSaveCompleted += Model_OnSaveCompleted;
                OfficeViewModel.Initialize(office,_context,true);
                OfficeEditionRequested(this, new ContextEditionEventArgs<OfficeViewModel>(OfficeViewModel));
            }
        }

        /// <summary>
        /// On save done
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Model_OnSaveCompleted(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Refresh Office's list Screen
        /// </summary>
        private void Refresh()
        {
            IsBusy = true;
            //Get the list of offices
            GetOfficesPaginated(0);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets data context of paginator control
        /// </summary>		
        public Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel GridViewModel
        {
            get
            {
                if (_gridViewModel == null)
                {
                    _gridViewModel = new Cotecna.Voc.Silverlight.Assets.GridHeaderViewModel
                    {
                        PageSize = (uint)App.CurrentUser.PageSize,
                    };
                    _gridViewModel.PageIndexChanged += GridViewModelPageIndexChanged;
                    //_gridViewModel.LaunchExcelExportation += _gridViewModel_LaunchExcelExportation;
                }
                return _gridViewModel;
            }
        }

        #endregion
    }
}
