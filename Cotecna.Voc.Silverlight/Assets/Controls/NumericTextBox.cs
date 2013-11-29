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

namespace Cotecna.Voc.Silverlight
{
   public class NumericTextBox: TextBox
    {
        static bool _altGrIsPressed;

        public NumericTextBox()
            :base()
        {
            this.KeyUp += Numclient_KeyUp;
            this.KeyDown += Numclient_KeyDown;
            this.LostFocus += NumericTextBox_LostFocus;
        }

        /// <summary>
        /// Lost Focus Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = "0";
            }
        }

        /// <summary>
        /// Override OnApplyTemplate method
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        void Numclient_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Alt)
            {
                _altGrIsPressed = false;
            }
        }

        void Numclient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Alt)
            {
                _altGrIsPressed = true;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift || _altGrIsPressed == true)
            {
                e.Handled = true;
            }

            if (e.Handled == false && (e.Key < Key.D0 || e.Key > Key.D9))
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if (e.Key != Key.Back && e.Key != Key.Tab && e.Key != Key.Delete)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

    }

}
