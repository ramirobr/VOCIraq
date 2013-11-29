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

namespace Cotecna.Voc.Silverlight
{
    public partial class SecurityPaperList : UserControl
    {
        public SecurityPaperList()
        {
            InitializeComponent();
            Loaded += SecurityPaperListLoaded;
        }

        /// <summary>
        /// Is executed when the is fully loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecurityPaperListLoaded(object sender, RoutedEventArgs e)
        {
            var context = DataContext as SecurityPaperListViewModel;
            context.ShowHideExportButton += ContextShowHideExportButton;
        }

        /// <summary>
        /// show or hide export to excel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextShowHideExportButton(object sender, ContextEditionEventArgs<Visibility> e)
        {
            paginator.CanExportExcel = e.Context;
        }
    }
}
