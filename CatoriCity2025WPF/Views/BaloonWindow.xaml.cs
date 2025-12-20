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
                string title = null)
        {
            InitializeComponent();

            LinearGradientBrush brush;
            brush = FindResource("HelpGradient") as LinearGradientBrush;
            borderBalloon.SetValue(BackgroundProperty, brush);

            textBlockCaption.Text = caption;

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
