using Cotecna.Voc.Business;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Class for management the security papers being selected from the Expander Control
    /// </summary>
    public class CheckableSecurityPaper : ViewModel
    {
        private SecurityPaper _model;
        private bool _isSelected;

        public CheckableSecurityPaper(SecurityPaper securityPaper, bool isSelected)
        {
            _isSelected = isSelected;
            _model = securityPaper;
        }

        /// <summary>
        /// Gets or Sets the security papers selected on a Release Note
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public SecurityPaper SPaper
        {
            get
            {
                return _model;
            }
        }
    }
}
