using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for BaloonControl.xaml
    /// </summary>
    public partial class BaloonControl : UserControl
    {
        public event EventHandler<BaloonLocationInfoArgument>? MoveBaloon;

        public BaloonControl()
        {
            InitializeComponent();
            LinearGradientBrush brush;
            brush = FindResource("HelpGradient") as LinearGradientBrush;
            borderBalloon.SetValue(BackgroundProperty, brush);

            WeakReferenceMessenger.Default.Register<BaloonMesssage>(this, (r, m) =>
            {
                textBlockCaption.Text = m.Caption;
                textBlockTitle.Text = m.Title;
                SetPosition(m);
                ShowControl();
            });
           
        }

        private void SetPosition(BaloonMesssage message)
        {
            double left = 0;
            double top = 0;
            double baloonWidth = Width;
            double baloonHeight = Height;
            left = message.Left;    
            top = message.Top;

            BaloonLocationInfoArgument args = new BaloonLocationInfoArgument();
 
            switch (message.Location)
            {
                case BaloonLocationEnum.LeftTop:
                    PathPointRightTop.Visibility = Visibility.Collapsed;
                    PathPointRightBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftTop.Visibility = Visibility.Visible;
                    left  = left- baloonWidth;
                    break;
                case BaloonLocationEnum.RightTop:
                    PathPointRightTop.Visibility = Visibility.Collapsed;
                    PathPointRightBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftBottom.Visibility = Visibility.Visible;
                    PathPointLeftTop.Visibility = Visibility.Collapsed;
                    left = left + 25;
                    top = top - Height;
                    break;
                case BaloonLocationEnum.LeftBottom:
                    PathPointRightTop.Visibility = Visibility.Visible;
                    PathPointRightBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftTop.Visibility = Visibility.Collapsed;
                    left = left - baloonWidth;
                    top = top + 25;
                    break;
                case BaloonLocationEnum.RightBottom:
                    PathPointRightTop.Visibility = Visibility.Collapsed;
                    PathPointRightBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftTop.Visibility = Visibility.Visible;
                    left = left + 25;
                    break;
                case BaloonLocationEnum.Center:
                    PathPointRightTop.Visibility = Visibility.Collapsed;
                    PathPointRightBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftBottom.Visibility = Visibility.Collapsed;
                    PathPointLeftTop.Visibility = Visibility.Collapsed;
                    double mainWidth = GlobalStuff.MainViewWidth;
                    double mainHeight = GlobalStuff.MainViewHeight;
                    double mainwidthhalf = mainWidth / 2;
                    double mainheighthalf = mainHeight / 2;
                    double baloonLeft = mainwidthhalf - (baloonWidth / 2);
                    double baloonTop = mainheighthalf - (baloonHeight / 2);
                    args.Left = baloonLeft;
                    args.Top = baloonTop;
                    break;
                default:
                    break;
            }
            args.Left = left;
            args.Top = top;
            MoveBaloon(this,args);
        }

 
        private void imageClose_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HideControl(); ;
        }

        private void HideControl()
        {
            this.Visibility = Visibility.Collapsed;
        }
        private void ShowControl()
        {
            this.Visibility = Visibility.Visible;

        }
        private void DoubleAnimation_Completed(object sender, System.EventArgs e)
        {
            HideControl(); ;

        }

        private void DoubleAnimationCompleted(object sender, System.EventArgs e)
        {
            HideControl(); ;

        }
    }
}
