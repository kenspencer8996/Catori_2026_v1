using CatoriApp.Objects.Arguments;
using CatoriApp.Objects.DragDrop;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using System.Windows.Media.Effects;
namespace CatoriApp.Views.Controls.Treasure
{
    /// <summary>
    /// Interaction logic for TreasureSpotControl.xaml
    /// </summary>
    public partial class TreasureSpotControl : UserControl,IDropTarget
    {
        public event EventHandler<StartDiggingArgs> StartDiggingEvent;
        public int SpotIndex { get; set; }
        public List<string> DirtSpots { get; set; }
        public decimal Funds { get; set; } = 0;
        string RawSpot = "";
        public bool HasTreasure { get; set; }= true;
        public Canvas _hostCanvas;
        Point _locOnParent;
        public TreasureSpotControl(Canvas hostCanvas)
        {
            InitializeComponent();
            _hostCanvas = hostCanvas;
            if (GlobalAllApps.showDebugInfo)
                DebugLabel.Visibility = Visibility.Visible;
            else
                DebugLabel.Visibility = Visibility.Collapsed;
            LoadDirtSpots();

            Random rnd = new Random();
            double min = 0.01;
            double max = 0.9;

            // Generate random number in range [min, max)
            double randomValue = min + (rnd.NextDouble() * (max - min));
            HasTreasure = IsTreasure(randomValue);

            Funds = HasTreasure ? GetTreasureAmount(randomValue) : 0;
        }
        private void UC_Loaded(object sender, RoutedEventArgs e)
        {
            TreasureSpotImage.Source = UIUtility.GetImageControl(System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Fields", "DirtSpot.png"), 50, 50, 0).Source;
        }
        private void LoadDirtSpots()
        {
            DirtSpots = new List<string>();
            string path = "";
            RawSpot = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Fields", "DirtSpot.png");

            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Fields", "DirtHole1.png");
            DirtSpots.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Fields", "DirtHole2.png");
            DirtSpots.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Fields", "DirtHole3.png");
            DirtSpots.Add(path);
            path = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Fields", "DirtHole4.png");
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

       

        internal void DiggerCycleComplete()
        {
            SetSpotIndex();
        }

        public bool IDraggable(IDraggable element)
        {
            return true;
        }

        public void OnDrop(IDraggable element)
        {
            if (element != null)
            {
                PersonControl person = element as PersonControl;
                int totalSpotsCount = DirtSpots.Count;
                double thisLeft = Canvas.GetLeft(this);
                double thisTop = Canvas.GetTop(this);
                double personLeft = thisLeft + (this.Width / 4);
                double personTop = thisTop - 110;
                Canvas.SetLeft(person, personLeft);
                Canvas.SetTop(person, personTop);
                person.StartDiggingAsync();

                TreasureStepArgs args = new TreasureStepArgs();
                args.ClearList = true;
                args.Name = this.Name;
                args.TreasureStep = TreasureStepEnum.WalkToTreasureSpot;
                WeakReferenceMessenger.Default.Send(args);
              }
        }
        public void HighlightOn()
        {
            // Example glow
            this.Effect = new DropShadowEffect
            {
                Color = Colors.Gold,
                BlurRadius = 25,
                ShadowDepth = 0,
                Opacity = 0.8
            };
        }

        public void HighlightOff()
        {
            this.Effect = null;
        }

       

        public bool CanDrop(IDraggable element)
        {
            return true;
        }

        public Point GetSnapPoint(IDraggable dragged)
        {
            FrameworkElement root = this;

            // Walk up until we hit the Canvas child
            while (root.Parent is FrameworkElement feParent && !(feParent is Canvas))
                root = feParent;

            var feDragged = (FrameworkElement)dragged.Visual;

            double x = Canvas.GetLeft(root) + (root.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(root) + (root.ActualHeight - feDragged.ActualHeight) / 2;
            cLogger.Log($"TreasureSpotControl LEFT={Canvas.GetLeft(this)} TOP={Canvas.GetTop(this)}");
            cLogger.Log($"Parent LEFT={Canvas.GetLeft((FrameworkElement)this.Parent)} TOP={Canvas.GetTop((FrameworkElement)this.Parent)}");

            return new Point(x, y);

          
        }
        public void ShowPositon(Point thisloc)
        { 
            _locOnParent = thisloc;
            string debugInfo = $"loc: X={thisloc.X.ToString("F4")} Y={thisloc.Y.ToString("F4")}";
            DebugLabel.Content = debugInfo;

        }
        private void MainLayoutTreasureSpot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowPositon(e.GetPosition(_hostCanvas));
        }

        private void GlowBorder_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void DebugLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText($"TreasureSpotControl: {DebugLabel.Content}");

        }

        bool IsTreasure(double spotQuality)
        {
            var rand = new Random();

            // Clamp between 0 and 1
            spotQuality = Math.Max(0, Math.Min(1, spotQuality));

            return rand.NextDouble() < spotQuality;
        }

        int GetTreasureAmount(double quality)
        {
            Random _rand = new Random();
            int baseAmount = _rand.Next(5, 20);

            double multiplier = 1 + (quality * 2); // up to 3x

            return (int)(baseAmount * multiplier);
        }
    }
}

