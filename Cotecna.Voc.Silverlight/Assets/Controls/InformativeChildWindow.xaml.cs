using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Cotecna.Voc.Silverlight.Assets.Resources;

namespace Cotecna.Voc.Silverlight
{
    /// <summary>
    /// Informative child window
    /// </summary>
    public partial class InformativeChildWindow : ChildWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformativeChildWindow"/> class.
        /// </summary>
        public InformativeChildWindow()
        {
            InitializeComponent();

            if (DesignerProperties.IsInDesignTool)
            {
                Message = "This is an example of waht is the message";
            }
        }

        /// <summary>
        /// Style images
        /// </summary>
        internal enum StyleImages
        {
            /// <summary>
            /// None item
            /// </summary>
            None,
            /// <summary>
            /// Warning item
            /// </summary>
            Warning,
            /// <summary>
            /// Warning 2 item
            /// </summary>
            Warning2,
            /// <summary>
            /// Error item
            /// </summary>
            Error,
            /// <summary>
            /// Question item
            /// </summary>
            Question,
            /// <summary>
            /// Information item
            /// </summary>
            Information,
            /// <summary>
            /// Excel item
            /// </summary>
            Excel
        }

        /// <summary>
        /// Button selections
        /// </summary>
        internal enum ButtonSelections
        {
            /// <summary>
            /// None item
            /// </summary>
            None,
            /// <summary>
            /// Yes - no item
            /// </summary>
            YesNo,
            /// <summary>
            /// Ok - cancel item
            /// </summary>
            OkCancel,
            /// <summary>
            /// Only ok item
            /// </summary>
            OnlyOk,
            /// <summary>
            /// Continue - update - cancel item
            /// </summary>
            ContinueUpdateCancel,
        }

        /// <summary>
        /// Gets or sets message
        /// </summary>
        public string Message
        {
            get
            {
                return txtMessage.Text;
            }
            set
            {
                txtMessage.Text = value;
            }
        }

        private StyleImages _image;
        /// <summary>
        /// Gets or sets image
        /// </summary>
        internal StyleImages Image
        {
            get { return _image; }
            set
            {
                _image = value;
                if (_image == StyleImages.None)
                    imagePlaceholder.Visibility = Visibility.Collapsed;
                else
                {
                    string styleString = "";
                    switch (_image)
                    {
                        case StyleImages.Warning:
                            styleString = "WarningImage";
                            break;
                        case StyleImages.Warning2:
                            styleString = "WarningImage2";
                            break;
                        case StyleImages.Error:
                            styleString = "ErrorImage";
                            break;
                        case StyleImages.Question:
                            styleString = "QuestionImage";
                            break;
                        case StyleImages.Information:
                            styleString = "InformationImage";
                            break;
                        case StyleImages.Excel:
                            styleString = "ExcelImage";
                            break;

                        default:
                            styleString = "InformationImage";
                            break;
                    }
                    imageContentControl.Style = App.Current.Resources[styleString] as Style;
                }
            }
        }

        private ButtonSelections _buttonSelection;
        /// <summary>
        /// Gets or sets button selection
        /// </summary>
        internal ButtonSelections ButtonSelection
        {
            get { return _buttonSelection; }
            set
            {
                _buttonSelection = value;
                switch (_buttonSelection)
                {
                    case ButtonSelections.None:
                        OKButton.Visibility = Visibility.Collapsed;
                        CancelButton.Visibility = Visibility.Collapsed;
                        UpdateButton.Visibility = Visibility.Collapsed;
                        break;
                    case ButtonSelections.YesNo:
                        OKButton.Content = Strings.Yes;
                        CancelButton.Content = Strings.No;
                        UpdateButton.Visibility = Visibility.Collapsed;
                        OKButton.Click += OKButton_Click;
                        CancelButton.Click += CancelButton_Click;
                        break;
                    case ButtonSelections.OkCancel:
                        OKButton.Content = Strings.Ok;
                        CancelButton.Content = Strings.Cancel;
                        UpdateButton.Visibility = Visibility.Collapsed;
                        OKButton.Click += OKButton_Click;
                        CancelButton.Click += CancelButton_Click;
                        break;
                    case ButtonSelections.OnlyOk:
                        OKButton.Content = Strings.Ok;
                        CancelButton.Visibility = Visibility.Collapsed;
                        UpdateButton.Visibility = Visibility.Collapsed;
                        OKButton.Click += OKButton_Click;
                        break;
                    case ButtonSelections.ContinueUpdateCancel:
                        OKButton.Content = Strings.Continue;
                        UpdateButton.Content = Strings.Update;
                        CancelButton.Content = Strings.Cancel;
                        OKButton.Click += OKButton_Click;
                        UpdateButton.Click += Update_Click;
                        CancelButton.Click += CancelButton_Click;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// When update clicked
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        void Update_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// When ok clicked
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// When Cancel clicked
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}

