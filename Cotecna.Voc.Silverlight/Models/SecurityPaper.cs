using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cotecna.Voc.Business
{
    public partial class SecurityPaper
    {
        private bool _checked;

        /// <summary>
        /// Get or set Checked
        /// </summary>		
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked == value) return;
                _checked = value;
                RaisePropertyChanged("Checked");
            }
        }
    }
}
