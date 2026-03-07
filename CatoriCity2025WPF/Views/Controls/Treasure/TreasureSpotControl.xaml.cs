using CatoriCity2025WPF.Objects.Arguments;

namespace CatoriCity2025WPF.Views.Controls.Treasure
{
    /// <summary>
    /// Interaction logic for TreasureSpotControl.xaml
    /// </summary>
    public partial class TreasureSpotControl : UserControl
    {
        public event EventHandler<StartDiggingArgs> StartDiggingEvent;
        public int SpotIndex { get; set; }
        public List<string> DirtSpots { get; set; }

        string RawSpot = "";
        public TreasureSpotControl()
        {
            InitializeComponent();
            LoadDirtSpots();
        }

        private void LoadDirtSpots()
        {
            DirtSpots = new List<string>();
            string path = "";
            RawSpot = System.IO.Path.Combine(CityScapeGlobal.ImageFolder, "Fields", "DirtSpot.png");

            path = System.IO.Path.Combine(CityScapeGlobal.ImageFolder, "Fields", "DirtHole1.png");
            DirtSpots.Add(path);
            path = System.IO.Path.Combine(CityScapeGlobal.ImageFolder, "Fields", "DirtHole2.png");
            DirtSpots.Add(path);
            path = System.IO.Path.Combine(CityScapeGlobal.ImageFolder, "Fields", "DirtHole3.png");
            DirtSpots.Add(path);
            path = System.IO.Path.Combine(CityScapeGlobal.ImageFolder, "Fields", "DirtHole4.png");
            DirtSpots.Add(path);

            SpotIndex = 1;
        }
        public void SetHover(bool isHovering)
        {
            GlowBorder.BorderBrush = isHovering ? Brushes.Yellow : Brushes.White;
            GlowBorder.BorderThickness = isHovering ? new Thickness(3) : new Thickness(1);
        }

        public void SetSpotIndex()
        {
            if (SpotIndex >= 0 && SpotIndex < DirtSpots.Count)
            {
                TreasureSpotImage.Source = UIUtility.GetImageControl(DirtSpots[SpotIndex], 50, 50, 0).Source;
            }
            if (SpotIndex == DirtSpots.Count - 1)
                SpotIndex = -1;
            else
            {
                SpotIndex++;
            }
        }
        internal void StartDigging()
        {
            if (StartDiggingEvent != null)
            {
                StartDiggingArgs args = new StartDiggingArgs();
                StartDiggingEvent.Invoke(this, args);
            }
        }

        private void UC_Loaded(object sender, RoutedEventArgs e)
        {
            TreasureSpotImage.Source = UIUtility.GetImageControl(System.IO.Path.Combine(CityScapeGlobal.ImageFolder, "Fields", "DirtSpot.png"), 50, 50, 0).Source;

        }

        internal void DiggerCycleComplete()
        {
            SetSpotIndex();
        }
    }
}
