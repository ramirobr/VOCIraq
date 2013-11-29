using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Printing;
using Infragistics.Controls.Grids;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Cotecna.Silverlight.Controls.Extension;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight.Assets
{
    /// <summary>
    /// Grid header view model used to manage the pagination process
    /// </summary>
    public class GridHeaderViewModel : ViewModelBase
    {
        #region Private Fields
        private string _pageNumber;
        private string _headerText = Strings.TotalRecords + " ";
        private uint _totalRecords;
        private uint _totalPages;
        private uint _pageSize;
        private uint _pageIndex = 1;
        private ObservableCollection<object> _data;

        private const string PathToImages = "/Cotecna.Voc.Silverlight;component/Assets/Images/";
        private const string ICON_FIRST = PathToImages + "first.png";
        private const string ICON_PREV = PathToImages + "previous.png";
        private const string ICON_NEXT = PathToImages + "next.png";
        private const string ICON_LAST = PathToImages + "last.png";
        private const string ICON_FIRST_DISABLED = PathToImages + "firstDisabled.png";
        private const string ICON_PREV_DISABLED = PathToImages + "previousDisabled.png";
        private const string ICON_NEXT_DISABLED = PathToImages + "nextDisabled.png";
        private const string ICON_LAST_DISABLED = PathToImages + "lastDisabled.png";
        private bool _isEnabled;
        //View
        //private MessageBoxWindow _messageBox;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridHeaderViewModel"/> class.
        /// </summary>
        public GridHeaderViewModel()
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets text in the header of the control (Total Records:)
        /// </summary>
        [DataMember]
        public string HeaderText
        {
            get 
            {
                if (String.IsNullOrEmpty(_headerText))
                    _headerText = Strings.TotalRecords;

                return _headerText;
            }
            set 
            {
                _headerText = value;
                OnPropertyChanged("HeaderText");
            }
        }

        
        /// <summary>
        /// Gets or sets the actual page index
        /// </summary>
        [DataMember]
        public uint PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                if (value == 0 && value < TotalPages)
                {
                    SetPageNumber();
                    OnPropertyChanged("PageIndex");
                    return;
                }
                if (value > TotalPages)
                {
                    SetPageNumber();
                    OnPropertyChanged("PageIndex");
                    return;
                }

                if (value <= TotalPages)
                    _pageIndex = value;

                SetPageNumber();
                OnPropertyChanged("PageIndex");
                OnPageIndexChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size of the page
        /// </summary>
        [DataMember]
        public uint PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
                SetInitialValues();
            }
        }

        /// <summary>
        /// Gets or sets the total number of pages of data
        /// </summary>
        [DataMember]
        public uint TotalPages
        {
            get
            {
                return _totalPages;
            }
            set
            {
                _totalPages = value;
                SetPageNumber();
                OnPropertyChanged("TotalPages"); 
            }
        }

        /// <summary>
        /// Gets or sets actual page number format: "Page 1 of n"
        /// </summary>
        [DataMember]
        public string PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                _pageNumber = value;
                OnPropertyChanged("PageNumber");
            }
        }

        /// <summary>
        /// Gets or sets the total number of records from the query
        /// </summary>
        [DataMember]
        public uint TotalRecords
        {
            get
            {
                return _totalRecords;
            }
            set
            {
                _totalRecords = value;
                EnableButtons();
                SetInitialValues();
                SetPageNumber();
                OnPropertyChanged("TotalRecords");
                OnPropertyChanged("MinPage");
            }
        }

        /// <summary>
        /// Enable/Disable the navigation buttons
        /// </summary>
        [DataMember]
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Gets goto first page button command
        /// </summary>
        public ICommand First
        {
            get
            {
                return new DelegateCommand(
                          () =>
                          {
                              ExecutePaging(1);
                          }
                    );
            }
        }



        /// <summary>
        /// Gets goto previous page button command
        /// </summary>
        public ICommand Previous
        {
            get
            {
                return new DelegateCommand(
                         () =>
                         {
                             ExecutePaging(2);
                         }
                   );
            }
        }

        /// <summary>
        /// Gets next delegatecommand
        /// </summary>
        public ICommand Next
        {
            get
            {
                return new DelegateCommand(
                         () =>
                         {
                             ExecutePaging(3);
                         }
                   );
            }
        }

        /// <summary>
        /// Gets Goto last page button command
        /// </summary>
        public ICommand Last
        {
            get
            {
                return new DelegateCommand(
                    () =>
                    {
                        ExecutePaging(4);
                    }
                    );
            }
        }

      


        /// <summary>
        /// Gets or sets the Grid that shows the data
        /// </summary>
        public ObservableCollection<object> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        /// <summary>
        /// Gets the minimun number page.
        /// </summary>
        /// <value>The min page.</value>
        public int MinPage
        {
            get { return _totalRecords > 0 ? 1 : 0; }
        }

        private ICommand _exportCommand;

        public ICommand ExportCommand
        {
            get 
            {
                if (_exportCommand == null)
                    _exportCommand = new RelayCommand(ExecuteExportCommand);
                return _exportCommand; 
            }
        }

        private ICommand _exportReleaseNotes;

        /// <summary>
        /// Export to excel all release notes
        /// </summary>
        public ICommand ExportReleaseNotes
        {
            get 
            {
                if(_exportReleaseNotes== null)
                    _exportReleaseNotes = new RelayCommand(ExecuteExportReleaseNotes);
                return _exportReleaseNotes; 
            }
        }
        

        #endregion

        #region Events
        /// <summary>
        /// Event that handles the change of page inside a Grid
        /// </summary>
        public event EventHandler<PageIndexChangedArgs> PageIndexChanged;
        /// <summary>
        /// Launch excel files exportation
        /// </summary>
        public event EventHandler LaunchExcelExportation;
        /// <summary>
        /// Export release notes to excel
        /// </summary>
        public event EventHandler LaunchExportReleaseNotes;
        #endregion

        #region Private Methods

        /// <summary>
        ///Export to excel all release notes
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteExportReleaseNotes(object parameter)
        {
            if (LaunchExportReleaseNotes != null)
                LaunchExportReleaseNotes(this, EventArgs.Empty);
        }

        /// <summary>
        /// Execute export command
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteExportCommand(object parameter)
        {
            if (LaunchExcelExportation != null)
                LaunchExcelExportation(this, EventArgs.Empty);
        }
        /// <summary>
        /// Set the page number
        /// </summary>
        private void SetPageNumber()
        {
            if (TotalPages != 0 && PageIndex != 0)
                PageNumber = string.Format(Strings.PageOf, PageIndex, TotalPages);
            else
                PageNumber = "";
        }

        /// <summary>
        /// Set the initial values for the user control
        /// </summary>
        private void SetInitialValues()
        {
            if (PageSize == 0)
                    return;

            uint totalPagesInt = Convert.ToUInt32(Math.Abs(TotalRecords/PageSize));
            double totalPages = Convert.ToDouble(TotalRecords)/Convert.ToDouble(PageSize);

            if (totalPages > totalPagesInt)
                TotalPages = totalPagesInt + 1;
            else
                TotalPages = totalPagesInt;

            if (TotalRecords != 0)
            {
                SetPageNumber();
            }
        }

        /// <summary>
        /// Notify the container that the page has changed
        /// </summary>
        private void OnPageIndexChanged()
        {
            EventHandler<PageIndexChangedArgs> eventHandler = PageIndexChanged;
            if (eventHandler != null && PageIndex != 0)
            {
                eventHandler(PageIndex, new PageIndexChangedArgs(PageIndex));
            }
        }

        /// <summary>
        /// Enable/Disable the navigation buttons based on the Total Records
        /// </summary>
        private void EnableButtons()
        {
            // If there are records then enabling buttons for pagination
            if (_totalRecords != 0)
                IsEnabled = true;
            else
                IsEnabled = false;

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reset the user control
        /// </summary>
        public void Reset()
        {
            TotalPages = 0;
            TotalRecords = 0;
            _pageIndex = 1;
            OnPropertyChanged("PageIndex");
        }
        #endregion

        #region ImportExportCommands
        /// <summary>
        /// Enable/Disable the import/export button commands
        /// </summary>
        /// <param name="parameter">The parameter is not used.</param>
        /// <returns>true for enabled, false for disabled</returns>
        public bool CanExecuteImportExport(object parameter)
        {
            if (Data == null)
                return false;
            return true;
        }

        
        #endregion

        #region PrivateExportExcelMethods

        #endregion

        #region PagingCommands
        /// <summary>
        /// Enable/Disable the paging commands using the RelayCommand
        /// </summary>
        /// <param name="parameter">int set to each button in the xaml</param>
        public void ExecutePaging(object parameter)
        {
            switch (Convert.ToInt16(parameter,CultureInfo.CurrentCulture))
            {
                case 1:
                    PageIndex = 1;
                    break;
                case 2:
                    PageIndex -= 1;
                    break;
                case 3:
                    PageIndex += 1;
                    break;
                case 4:
                    PageIndex = TotalPages;
                    break;
            }
        }
        
        #endregion
    }
}
