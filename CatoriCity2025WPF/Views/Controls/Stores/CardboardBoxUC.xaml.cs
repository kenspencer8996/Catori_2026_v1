using CatoriApp.Objects.Arguments;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CatoriApp.Views.Controls
{
    /// <summary>
    /// Interaction logic for CardboardBoxUC.xaml
    /// </summary>
    public partial class CardboardBoxUC : UserControl
    {
        public event EventHandler<BoxOpenedArg> BoxOpenFinished;
        double w = 300;
        double h = 75;
        DispatcherTimer openBoxTimer;
        double heightfinal = 76;
        int i = 1;
        public CardboardBoxUC()
        {
            InitializeComponent();
            string imagepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "DroneShipping", "BoxShippingOpen.png");
            ImageSourceOpen = UIUtility.GetImageControl(imagepath,h,w,3010);
            imagepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "DroneShipping", "BoxShippingCLosing3.png");
            ImageSourceOpen = UIUtility.GetImageControl(imagepath, h, w, 3010);
            imagepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "DroneShipping", "BoxShippingShipping2.png");
            ImageSourcePartialOpen = UIUtility.GetImageControl(imagepath, h, w, 3010);

            openBoxTimer = new DispatcherTimer();
            openBoxTimer.Tick += new EventHandler(openBoxTimer_Tick);
            openBoxTimer.Interval = new TimeSpan(0, 0, 0,0,200);

            CardboardBoxImage.Source = ImageSourcePartialOpen.Source;
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(2);
                MainBorder.BorderBrush = Brushes.Red;
            }
        }

        private void openBoxTimer_Tick(object? sender, EventArgs e)
        {
            Height = i;
            if (i > heightfinal)
            {
                openBoxTimer.Stop();
                i = 0;
                BoxOpenFinished?.Invoke(this, new BoxOpenedArg());
            }
            i++;
        }
        private Image ImageSourceOpen;

        private Image ImageSourceClosed;

        private Image ImageSourcePartialOpen;

        public void ShowOpen()
        {
            openBoxTimer.Start();

        }
        public void ShowPartialOpen()
        {
            CardboardBoxImage.Source = ImageSourcePartialOpen.Source;
        }
        public void ShowCLosed()
        {
            CardboardBoxImage.Source = ImageSourceClosed.Source;

        }
    }
}

