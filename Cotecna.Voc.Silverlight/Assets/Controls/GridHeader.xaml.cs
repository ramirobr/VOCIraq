using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Infragistics.Documents.Excel;


namespace Cotecna.Voc.Silverlight.Assets
{
    /// <summary>
    /// Grid header used for pagination, exporting features
    /// </summary>
    public partial class GridHeader : UserControl
    {

        #region GridData property for DataGrid
        public static readonly DependencyProperty GridDataProperty =
        DependencyProperty.Register("GridData", typeof(DataGrid), typeof(GridHeader)
          , new PropertyMetadata(null, GridDataChanged));

        /// <summary>
        /// Gets or sets datagrid control required to be paginated, exported, etc
        /// </summary>
        public DataGrid GridData
        {
            get { return (DataGrid)GetValue(GridDataProperty); }
            set { SetValue(GridDataProperty, value); }
        }

        /// <summary>
        /// Set the property (dependency object) created
        /// </summary>
        /// <param name="sender">this object</param>
        /// <param name="args">Has the new value for griddata property</param>
        private static void GridDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GridHeader header = sender as GridHeader;
            if (header != null)
            {
                header.GridData = (DataGrid)args.NewValue;
            }
        }
        #endregion

        #region GridXam property for XamGrid of infragistics

        public static readonly DependencyProperty GridXamProperty =
          DependencyProperty.Register("GridXam", typeof(Infragistics.Controls.Grids.XamGrid), typeof(GridHeader)
            , new PropertyMetadata(null, GridXamChanged));

        /// <summary>
        /// Gets or sets infragistics Grid which will be paginated, export, etc
        /// </summary>
        public Infragistics.Controls.Grids.XamGrid GridXam
        {
            get { return (Infragistics.Controls.Grids.XamGrid)GetValue(GridXamProperty); }
            set { SetValue(GridXamProperty, value); }
        }

        /// <summary>
        /// Set the property (dependency object) created
        /// </summary>
        /// <param name="sender">this object</param>
        /// <param name="args">Has the new value for gridxam property</param>
        private static void GridXamChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GridHeader header = sender as GridHeader;
            if (header != null)
            {
                header.GridXam = (Infragistics.Controls.Grids.XamGrid)args.NewValue;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GridHeader"/> class.
        /// </summary>
        public GridHeader()
        {
            //By default display paging controls
            CanPaging = Visibility.Visible;

            InitializeComponent();
        }


        #region CanExportExcel
        /// <summary>
        /// Gets or sets visibility of export excel button
        /// </summary>
        public Visibility CanExportExcel
        {
            get { return (Visibility)GetValue(CanExportExcelProperty); }
            set { SetValue(CanExportExcelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanExportRN.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanExportExcelProperty =
            DependencyProperty.Register("CanExportExcel", typeof(Visibility), typeof(GridHeader), new PropertyMetadata(Visibility.Visible));
        #endregion

        /// <summary>
        /// Gets or sets visibility for paging section buttons
        /// </summary>
        public Visibility CanPaging { get; set; }

        #region CanExportRN
        /// <summary>
        /// Gets or sets visiblity for export release note button
        /// </summary>
        public Visibility CanExportRN
        {
            get { return (Visibility)GetValue(CanExportRNProperty); }
            set { SetValue(CanExportRNProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanExportRN.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanExportRNProperty =
            DependencyProperty.Register("CanExportRN", typeof(Visibility), typeof(GridHeader), new PropertyMetadata(Visibility.Visible));
        #endregion

    }
}
