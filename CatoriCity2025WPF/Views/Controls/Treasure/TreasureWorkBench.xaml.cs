using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Objects.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace CatoriCity2025WPF.Views.Controls.Treasure
{
    /// <summary>
    /// Interaction logic for TreasureWorkBench.xaml
    /// </summary>
    public partial class TreasureWorkBench : UserControl,IDropTarget
    {
        List<string> TreasureImages = new List<string>();
        List<string> ChestImages = new List<string>();
        string closedChest;
        string openChest1;
        string openChest2;
        string openChestFullOpen;
        double currentValue;
        public TreasureWorkBench()
        {
            InitializeComponent();

            ChestImage.Opacity = 0.0;
            ValuablesImage.Opacity = 0.0;
            string basePath = GlobalAllApps.ImageFolder;
            ChestImages.Add(System.IO.Path.Combine(basePath, "Treasure\\CHestClosed.png"));
            ChestImages.Add(System.IO.Path.Combine(basePath, "Treasure\\CHestOpen1.png"));
            ChestImages.Add(System.IO.Path.Combine(basePath, "Treasure\\CHestOpen2.png"));
            ChestImages.Add(System.IO.Path.Combine(basePath, "Treasure\\CHestOpen3.png")    );
            TreasureImages.Add(System.IO.Path.Combine(basePath, "Treasure\\BagOfrocks.png"));
            TreasureImages.Add(System.IO.Path.Combine(basePath, "Treasure\\BagOfSticks.png"));
            TreasureImages.Add(System.IO.Path.Combine(basePath, "Treasure\\BagOfValubleTrinkets.png"));
            TreasureImages.Add(System.IO.Path.Combine(basePath, "Treasure\\BagOfValuabeStones.png"));
            TreasureImages.Add(System.IO.Path.Combine(basePath, "Treasure\\CashStackWIhtBand.png"));

            closedChest = System.IO.Path.Combine(basePath, "Treasure\\CHestClosed.png");
            openChest1 = System.IO.Path.Combine(basePath, "Treasure\\CHestOpen1.png");
            openChest2 = System.IO.Path.Combine(basePath, "Treasure\\CHestOpen2.png");
            openChestFullOpen = System.IO.Path.Combine(basePath, "Treasure\\CHestOpen3.png");
            SetValuableImage();
            currentValue = GetTreasureValue();
            ValuablesImage.ToolTip = currentValue.ToString();
        }

        private void SetValuableImage()
        {
            Random rnd = new Random();
            int imagenumber = rnd.Next(TreasureImages.Count-1);
            string imagepath = TreasureImages[imagenumber];
            ValuablesImage.Source = UIUtility.GetImageControl(imagepath, ChestImage.Width, ChestImage.Height, 1).Source;
        }

        public async void OpenChest()
        { 
            ChestImage.Source = UIUtility.GetImageControl(openChest1, ChestImage.Width, ChestImage.Height, 1).Source;
            ChestImage.Opacity = 1.0;
            await Task.Delay(500); // Wait  without blocking
            ChestImage.Source = UIUtility.GetImageControl(openChest2, ChestImage.Width, ChestImage.Height, 1).Source;
            await Task.Delay(500); // Wait  without blocking
            ChestImage.Source = UIUtility.GetImageControl(openChestFullOpen, ChestImage.Width, ChestImage.Height, 1).Source;
            await Task.Delay(500); // Wait  without blocking
            ShowValuables();
        }
        private void ShowValuables()
        {
            // Create a DoubleAnimation for opacity
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0, // Start hidden
                To = 1.0, // End fully visible
                Duration = new Duration(TimeSpan.FromSeconds(5)), // seconds duration
                AutoReverse = false // No reverse animation
            };

            // Apply the animation to a button's Opacity property
            ValuablesImage.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
        public double GetTreasureValue()
        {
            double value = 0;
            string filename = GetBagImageFilePath();
            filename = System.IO.Path.GetFileName(filename); // Get just the file name
            cLogger.Log("Getting treasure value for: " + filename);
            switch (filename)
            {
                case "BagOfrocks.png":
                    value = GetRandomDouble(1, 10);
                    break;
                case "BagOfSticks.png":
                    value = GetRandomDouble(5, 300);
                    break;
                case "BagOfValubleTrinkets.png":
                    value = GetRandomDouble(1, 10);
                    break;
                case "BagOfValuabeStones.png":
                    value = GetRandomDouble(1, 10);
                    break;
                case "CashStackWIhtBand.png":
                    value = GetRandomDouble(1, 10);
                    break;
                default:
                    break;

            }
            return value;
        }
        public double GetRandomDouble(double minValue, double maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentException("minValue must be less than maxValue.");

            Random random = new Random(); // For repeated calls, reuse the same instance
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
        private string GetBagImageFilePath()
        {
            
            if (ValuablesImage?.Source is BitmapImage bitmap && bitmap.UriSource != null)
            {
                if (bitmap.UriSource.IsFile)
                {
                    return bitmap.UriSource.LocalPath; // Full file path
                }
                return bitmap.UriSource.ToString(); // Could be a pack URI
            }
            return null;
        }
        
       

        private void SetupTreasureChest()
        {
            OpenChest(); 
        }
        public int TableWidth { get { return (int)TableImage.ActualWidth; } }
        public int TableHeight { get { return (int)TableImage.ActualHeight; } }
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

        private void ATMMenu_Click(object sender, RoutedEventArgs e)
        {
            cLogger.Log("ATM menu clicked.");
            FundsDetailView fundsDetailView = new FundsDetailView((decimal)currentValue);
            fundsDetailView.Show();
        }

        private void DiscardMenu_Click(object sender, RoutedEventArgs e)
        {
            cLogger.Log("Discard menu clicked.");
            ValuablesImage.Opacity = 0.0;
        }

        private void AddToMyCashMenu_Click(object sender, RoutedEventArgs e)
        {
            cLogger.Log("Add to my cash menu clicked.");
            PersonValuablesMessage message = new PersonValuablesMessage(currentValue);
            WeakReferenceMessenger.Default.Send(message);
        }

        public Point GetSnapPoint(UIElement dragged)
        {
            var feDragged = (FrameworkElement)dragged;
            double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
            return new Point(x, y);
        }

        public bool CanDrop(IDraggable element)
        {
            return true;
        }

        public void OnDrop(IDraggable element)
        {
            try
            {
                PersonControl person = element as PersonControl;
                if (person.isCarryingChest)
                {
                    person.ShowPersonStandingNoChest();
                    SetupTreasureChest();
                    person.isCarryingChest = false;
                }
            }
            catch (Exception ex)
            {
                cLogger.Log("ExceptionL:" + ex.Message);
            }
        }

        public Point GetSnapPoint(IDraggable dragged)
        {
            var feDragged = (FrameworkElement)dragged;

            if (feDragged != null)
            {
                double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
                double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
                return new Point(x, y);
            }
            else
            {
                return new Point(0, 0);
            }
        }
    }
}
