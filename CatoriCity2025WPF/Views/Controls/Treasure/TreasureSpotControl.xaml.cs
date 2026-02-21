namespace CatoriCity2025WPF.Views.Controls.Treasure
{
    /// <summary>
    /// Interaction logic for TreasureSpotControl.xaml
    /// </summary>
    public partial class TreasureSpotControl : UserControl
    {
        public TreasureSpotControl()
        {
            InitializeComponent();
        }

        private void UC_Loaded(object sender, RoutedEventArgs e)
        {
            TreasureSpotImage.Source = UIUtility.GetImageControl(System.IO.Path.Combine(GlobalStuff.ImageFolder, "Fields", "DirtSpot.png"), 50, 50, 0).Source;

        }
    }
}
