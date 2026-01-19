namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for CardboardBoxUC.xaml
    /// </summary>
    public partial class CardboardBoxUC : UserControl
    {
        double w = 300;
        double h = 75;
        public CardboardBoxUC()
        {
            InitializeComponent();
            string imagepath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "DroneShipping", "BoxShippingOpen.png");
            ImageSourceOpen = UIUtility.GetImageControl(imagepath,h,w,3010);
            imagepath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "DroneShipping", "BoxShippingCLosing3.png");
            ImageSourceOpen = UIUtility.GetImageControl(imagepath, h, w, 3010);
            imagepath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "DroneShipping", "BoxShippingShipping2.png");
            ImageSourcePartialOpen = UIUtility.GetImageControl(imagepath, h, w, 3010);
        }
        private Image ImageSourceOpen;

        private Image ImageSourceClosed;

        private Image ImageSourcePartialOpen;

        public void ShowOpen()
        {
            CardboardBoxImage.Source = ImageSourceOpen.Source;
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
