using System.Windows;
using System.Windows.Media;

namespace Mantin.Controls.Wpf.Notification
{
    /// <summary>
    /// Interaction logic for BaloonWindow.xaml
    /// </summary>
    public partial class BaloonWindow : Window
    {
        public BaloonWindow(string caption,
            double maxHeight = 0,
            double maxWidth = 0,
            bool autoWidth = false,
            string title = null)
        {
            InitializeComponent();

            LinearGradientBrush brush;
            switch (BalloonType.Help)
            {
                case BalloonType.Help:
                    imageType.Source = Properties.Resources.help20.ToBitmapImage();
                    //imageType.Source = Properties.Resources.help.ToBitmapImage();
                    brush = FindResource("HelpGradient") as LinearGradientBrush;
                    break;

                case BalloonType.Information:
                    imageType.Source = Properties.Resources.Information.ToBitmapImage();
                    brush = FindResource("InfoGradient") as LinearGradientBrush;
                    break;

                default:
                    imageType.Source = Properties.Resources.Warning.ToBitmapImage();
                    brush = FindResource("WarningGradient") as LinearGradientBrush;
                    break;
            }
            borderBalloon.SetValue(BackgroundProperty, brush);

            if (autoWidth)
            {
                SizeToContent = SizeToContent.WidthAndHeight;
            }

            textBlockCaption.Text = caption;

            if (maxHeight > 0)
            {
                scrollViewerCaption.MaxHeight = maxHeight;
            }

            if (maxWidth > 0)
            {
                MaxWidth = maxWidth;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                textBlockTitle.Text = title;
            }
            else
            {
                textBlockTitle.Visibility = Visibility.Collapsed;
                lineTitle.Visibility = Visibility.Collapsed;
            }
        }

        private void imageClose_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void DoubleAnimation_Completed(object sender, System.EventArgs e)
        {
            if (!IsMouseOver)
            {
                Close();
            }
        }

        private void DoubleAnimationCompleted(object sender, System.EventArgs e)
        {
            if (!IsMouseOver)
            {
                Close();
            }
        }
    }
}
