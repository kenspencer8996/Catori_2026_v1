using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.DragDrop;
using System.Diagnostics;
using System.Windows.Media.Effects;
namespace CatoriCity2025WPF.Views.Controls.Treasure
{
    /// <summary>
    /// Interaction logic for TreasureSpotControl.xaml
    /// </summary>
    public partial class TreasureSpotControl : UserControl,IDropTarget
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

        private void UC_Loaded(object sender, RoutedEventArgs e)
        {
            TreasureSpotImage.Source = UIUtility.GetImageControl(System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Fields", "DirtSpot.png"), 50, 50, 0).Source;

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
                person.StartDiggingAnimation(totalSpotsCount); 
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
    }
}
