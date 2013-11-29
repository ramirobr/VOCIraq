using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Custom textbox to allow only decimals
    /// </summary>
    public class DecimalTextBox : TextBox, INotifyPropertyChanged
    {
        #region events
        /// <summary>
        /// Inform when a property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the number of decimal places
        /// </summary>
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); OnPropertyChanged("DecimalPlaces"); }
        }

        // Using a DependencyProperty as the backing store for DecimalPlaces.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(DecimalTextBox), new PropertyMetadata(2));

        /// <summary>
        /// Gets or sets MaxLengthDecimal
        /// </summary>
        public int MaxLenghDecimal
        {
            get { return (int)GetValue(MaxLenghDecimalProperty); }
            set 
            { 
                SetValue(MaxLenghDecimalProperty, value); 
                OnPropertyChanged("MaxLenghDecimal"); 
            }
        }

        // Using a DependencyProperty as the backing store for MaxLenghDecimal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLenghDecimalProperty =
            DependencyProperty.Register("MaxLenghDecimal", typeof(int), typeof(DecimalTextBox), new PropertyMetadata(5, OnMaxLenghDecimalChanged));

        private static void OnMaxLenghDecimalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DecimalTextBox decimalcontrol = sender as DecimalTextBox;
            decimalcontrol.MaxLength = int.Parse(e.NewValue.ToString()) + decimalcontrol.DecimalPlaces + 1;
        }

        #endregion

        #region constructor
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public DecimalTextBox()
        {
            this.LostFocus += DecimalTextBox_LostFocus;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Call PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region private methods
        /// <summary>
        /// Verify if exists decimals
        /// </summary>
        /// <returns></returns>
        private bool HasDecimal()
        {
            return this.Text.Contains(".");
        }

        /// <summary>
        /// Handle lost focus event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecimalTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = "0";
            }
        }
        
        /// <summary>
        /// Verify if the pressed key might be handled
        /// </summary>
        /// <returns></returns>
        private bool Handled()
        {
            bool handled = false;

            if (HasDecimal())
            {
                int indexOfDecimal = this.Text.IndexOf(".");

                if (this.SelectionStart > indexOfDecimal)
                {
                    int textLeght = this.Text.Length;
                    int dotIndex = this.Text.IndexOf(".");
                    int size = textLeght - dotIndex;
                    handled = size == DecimalPlaces + 1;    
                }

            }
            return handled;
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Hanle KeyDown event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Back:
                case Key.Home:
                case Key.End:
                case Key.Escape:
                case Key.Tab:
                case Key.Delete:
                    e.Handled = false;
                    break;
                case Key.Shift:
                case Key.CapsLock:
                case Key.Ctrl:
                case Key.Alt:
                    e.Handled = true;
                    break;
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    e.Handled = Handled();
                    break;
                case Key.Decimal:
                    {
                        e.Handled =  HasDecimal();
                        break;
                    }
                case Key.Unknown:
                    {
                        if (e.PlatformKeyCode == 190) // period
                        {
                            e.Handled =  HasDecimal();
                        }
                        else
                        {
                            e.Handled = true;
                        }
                        break;
                    }
                default:
                    e.Handled = true;
                    break;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Handle KeyUp event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Back:
                case Key.Home:
                case Key.End:
                case Key.Escape:
                case Key.Tab:
                case Key.Delete:
                    e.Handled = false;
                    break;
                case Key.Shift:
                case Key.CapsLock:
                case Key.Ctrl:
                case Key.Alt:
                    e.Handled = true;
                    break;
                default:
                    string _pattern = "[^0-9.]";
                    this.Text = Regex.Replace(this.Text, _pattern, "");
                    this.Select(this.Text.Length, 0);
                    break;

            }
            base.OnKeyUp(e);
        }
        #endregion

    }
}
